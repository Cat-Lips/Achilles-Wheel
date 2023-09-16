#if TOOLS
using Godot;

namespace GodotSharp.BuildingBlocks
{
    [Tool]
    public partial class PhysicsImport : EditorScenePostImport
    {
        private static PhysicsImportOptions Options { get; } = new();
        public static void Add(EditorPlugin source) => source.AddScenePostImportPlugin(Options);
        public static void Remove(EditorPlugin source) => source.RemoveScenePostImportPlugin(Options);

        private static readonly Dictionary<string, PackedScene> DefaultRoot = new()
        {
            { "Vehicle", App.LoadScene<Vehicle>() },
        };

        public override GodotObject _PostImport(Node scene)
        {
            if (Options.ImportOriginal)
                return scene;

            LoadConfig(out var config);
            CreateRoot(out var root, out var offset, out var ret);
            if (root is null) return scene;

            AddShapes(out var mass);
            SetMass(root, GetMass(root) + mass);

            SaveConfig();
            return ret;

            #region Import

            void CreateRoot(out Node3D root, out Transform3D offset, out Node3D ret)
            {
                ret = GetRootOverride()?.InstantiateOrNull<Node3D>();
                var name = GetOption(scene, "Name", (string)scene.Name);
                root = GetBody(GetEnumOption(scene, "BodyType", Options.BodyType));
                offset = GetOffset(GetEnumOption(scene, "FrontFace", Options.FrontFace));

                if (ret is null && root is null) return;
                if (ret is null && root is not null) { root.Name = name; ret = root; return; }
                if (ret is not null && root is null) { ret.Name = name; root = ret; return; }
                if (ret.GetType().IsAssignableFrom(root.GetType())) { ret.Name = name; root = ret; return; }

                ret.Name = name;
                root.Name = name;
                ret.OwnChild(root);

                PackedScene GetRootOverride()
                {
                    return Valid(Options.Root) ?? Valid(GetDefaultRoot());

                    PackedScene Valid(PackedScene scene)
                        => scene?.CanInstantiate() ?? false ? scene : null;

                    PackedScene GetDefaultRoot()
                    {
                        return GetDefaultRoot(GetSourceFile());

                        PackedScene GetDefaultRoot(string hint)
                            => DefaultRoot.FirstOrDefault(x => hint.Contains(x.Key)).Value;
                    }
                }
            }

            void AddShapes(out float mass)
            {
                var myMass = 0f;
                scene.RecurseChildren<MeshInstance3D>().ForEach(AddShapes);
                mass = myMass;

                void AddShapes(MeshInstance3D mesh)
                {
                    var xform = mesh.GetLocalRootTransform();
                    var body = CreateBody(xform, out var bodyMass);

                    CreateShapes();

                    myMass += bodyMass;
                    SetMass(body, bodyMass);

                    Node3D CreateBody(in Transform3D xform, out float bodyMass)
                    {
                        var name = GetOption(mesh, "Name", (string)mesh.Name);
                        var body = GetBody(GetEnumOption(mesh, "BodyType", BodyType.None));
                        if (body is null) { bodyMass = 0; return root; }

                        body.Name = name;
                        root.OwnChild(body);
                        body.SetLocalRootTransform(BodyOffset(xform));

                        bodyMass = GetMass(body);
                        return body;

                        Transform3D BodyOffset(in Transform3D xform)
                        {
                            if (body is VehicleWheel3D wheel)
                            {
                                wheel.WheelRadius = mesh.GetAabb().Size.Y * .5f;
                                return offset * xform * offset.AffineInverse();
                            }

                            return offset * xform;
                        }
                    }

                    void CreateShapes()
                    {
                        var shapes = GetShapes(mesh);

                        if (shapes.Length is 0 or 1)
                            AddShape(mesh.Copy(), shapes.SingleOrDefault());
                        else
                            shapes.ForEach(x => AddShape(mesh.Clip(x), x, true));

                        CollisionShape3D[] GetShapes(MeshInstance3D mesh)
                        {
                            return GetShapes(GetEnumOption(mesh, "ShapeType", Options.ShapeType));

                            CollisionShape3D[] GetShapes(ShapeType shapeType)
                            {
                                return shapeType switch
                                {
                                    ShapeType.None => [],
                                    ShapeType.MultiConvex => GetMultiConvexShapes(),
                                    _ => [GetSingleCollisionShape()],
                                };

                                CollisionShape3D[] GetMultiConvexShapes()
                                {
                                    mesh.CreateMultipleConvexCollisions(Settings());
                                    var temp = mesh.GetChildren<StaticBody3D>().Single();
                                    var shapes = temp.GetChildren<CollisionShape3D>().ForAll(temp.RemoveChild).ToArray();
                                    SetOption(mesh, "MaxMultiConvexShapes", (uint)shapes.Length, Options.MaxMultiConvexShapes);
                                    mesh.RemoveChild(temp, free: true);
                                    return shapes;

                                    MeshConvexDecompositionSettings Settings() => new()
                                    {
                                        MaxConcavity = 0.001f,
                                        MaxConvexHulls = GetOption(mesh, "MaxMultiConvexShapes", Options.MaxMultiConvexShapes),
                                    };
                                }

                                CollisionShape3D GetSingleCollisionShape()
                                {
                                    var (shape, rotation) = GetShapeWithRotation();
                                    return new() { Shape = shape, Rotation = rotation };

                                    (Shape3D Shape, Vector3 Rotation) GetShapeWithRotation() => shapeType switch
                                    {
                                        ShapeType.Convex => (mesh.Mesh.CreateConvexShape(), Vector3.Zero),
                                        ShapeType.Trimesh => (mesh.Mesh.CreateTrimeshShape(), Vector3.Zero),
                                        ShapeType.SimpleConvex => (mesh.Mesh.CreateConvexShape(simplify: true), Vector3.Zero),

                                        ShapeType.Box => (mesh.Mesh.CreateBoxShape(), Vector3.Zero),
                                        ShapeType.Sphere => (mesh.Mesh.CreateSphereShape(), Vector3.Zero),

                                        ShapeType.Capsule => (mesh.Mesh.CreateCapsuleShape(), Vector3.Zero),
                                        ShapeType.CapsuleX => mesh.Mesh.CreateCapsuleShape(Vector3.Axis.X),
                                        ShapeType.CapsuleZ => mesh.Mesh.CreateCapsuleShape(Vector3.Axis.Z),

                                        ShapeType.Cylinder => (mesh.Mesh.CreateCylinderShape(), Vector3.Zero),
                                        ShapeType.CylinderX => mesh.Mesh.CreateCylinderShape(Vector3.Axis.X),
                                        ShapeType.CylinderZ => mesh.Mesh.CreateCylinderShape(Vector3.Axis.Z),

                                        _ => throw new NotImplementedException(),
                                    };
                                }
                            }
                        }

                        void AddShape(GeometryInstance3D mesh, CollisionShape3D shape, bool multi = false)
                        {
                            var name = GetOption(mesh, "Name", (string)mesh.Name);
                            var mass = mesh.GetAabb().Volume * GetOption(mesh, "MassMultiplier", Options.MassMultiplier);

                            if (shape is null)
                            {
                                mesh.Name = name;
                                body.OwnChild(mesh);
                                mesh.SetMeta("Mass", mass);
                                mesh.SetLocalRootTransform(offset * xform);
                            }
                            else
                            {
                                var meshName = name.TrimSuffix("1");
                                var shapeName = multi ? $"{name}1" : meshName;

                                shape.Name = shapeName;
                                body.OwnChild(shape);
                                shape.SetMeta("Mass", mass);
                                shape.SetLocalRootTransform(offset * xform * shape.Transform);

                                mesh.Name = meshName;
                                shape.OwnChild(mesh);
                                mesh.SetMeta("Mass", mass);
                                mesh.SetLocalRootTransform(offset * xform);
                            }

                            bodyMass += mass;
                        }
                    }
                }
            }

            #endregion

            #region Config

            string FilePath()
            {
                var source = GetSourceFile();
                var baseDir = source.GetBaseDir();
                var fileName = source.GetFile();
                return baseDir.PathJoin($".physics.{fileName}.physics");
            }

            string NodePath(Node node)
                => node is null || node == scene ? $"{scene.Name}" : $"{scene.Name}.{node.Name}";

            void LoadConfig(out ConfigFile config)
            {
                config = new ConfigFile();
                config.Load(FilePath());
            }

            void SaveConfig()
                => config.Save(FilePath());

            T GetOption<[MustBeVariant] T>(Node node, string option, T @default)
            {
                var path = NodePath(node);
                var value = config.GetValue(path, option, @default);
                SetOption(node, option, value, @default);
                return value;
            }

            void SetOption<[MustBeVariant] T>(Node node, string option, T value, T @default)
            {
                SaveValue(NodePath(node));

                void SaveValue(string path)
                {
                    if (Options.CreateConfig || !Equals(value, @default))
                        config.SetValue(path, option, value);
                    else
                        config.ResetValue(path, option);
                }
            }

            T GetEnumOption<[MustBeVariant] T>(Node node, string option, T @default) where T : struct, Enum
            {
                var path = NodePath(node);
                var value = config.GetEnumValue(path, option, @default);
                SetEnumOption(node, option, value, @default);
                return value;
            }

            void SetEnumOption<[MustBeVariant] T>(Node node, string option, T value, T @default) where T : struct, Enum
            {
                SaveEnumValue(NodePath(node));

                void SaveEnumValue(string path)
                {
                    if (Options.CreateConfig || !Equals(value, @default))
                        config.SetEnumValue(path, option, value);
                    else
                        config.ResetValue(path, option);
                }
            }

            #endregion

            #region Utils

            static float GetMass(Node3D node)
            {
                return node is RigidBody3D body ? body.Mass
                    : (float)node.GetMeta("Mass", @default: 0);
            }

            static void SetMass(Node3D node, float mass)
            {
                if (node is RigidBody3D body)
                    body.Mass = mass;
                else
                    node.SetMeta("Mass", mass);
            }

            static Node3D GetBody(BodyType bodyType) => bodyType switch
            {
                BodyType.None => null,
                BodyType.Area => new Area3D(),
                BodyType.Rigid => new RigidBody3D(),
                BodyType.Static => new StaticBody3D(),
                BodyType.Vehicle => new VehicleBody3D(),
                BodyType.Character => new CharacterBody3D(),
                BodyType.Animatable => new AnimatableBody3D(),
                BodyType.Wheel => new VehicleWheel3D(),
                _ => throw new NotImplementedException(),
            };

            static Transform3D GetOffset(FrontFace frontFace)
            {
                return Transform3D.Identity.Rotated(Vector3.Up, Angle());

                float Angle() => frontFace switch
                {
                    FrontFace.Z => 0,
                    FrontFace.NegZ => Mathf.Pi,
                    FrontFace.X => Const.HalfPi,
                    FrontFace.NegX => -Const.HalfPi,
                    _ => throw new NotImplementedException(),
                };
            }

            #endregion
        }
    }
}
#endif
