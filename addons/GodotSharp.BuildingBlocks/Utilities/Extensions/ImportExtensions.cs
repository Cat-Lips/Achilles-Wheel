#if TOOLS
using FastEnumUtility;
using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class ImportExtensions
    {
        public static void AddEnumImportOption<[MustBeVariant] T>(this EditorScenePostImportPlugin source, string name, T @default = default) where T : struct, Enum
            => source.AddImportOptionAdvanced(Variant.Type.String, name, FastEnum.GetName(@default), PropertyHint.Enum, string.Join(",", FastEnum.GetNames<T>()));

        public static void AddResourceImportOption<[MustBeVariant] T>(this EditorScenePostImportPlugin source, string name, T @default = default) where T : Resource
            => source.AddImportOptionAdvanced(Variant.Type.Object, name, @default, PropertyHint.ResourceType, typeof(T).Name);

        public static T GetEnumOptionValue<[MustBeVariant] T>(this EditorScenePostImportPlugin source, string name) where T : struct, Enum
            => FastEnum.Parse<T>((string)source.GetOptionValue(name));

        public static T GetResourceOptionValue<[MustBeVariant] T>(this EditorScenePostImportPlugin source, string name) where T : Resource
            => (T)source.GetOptionValue(name);

        public static Transform3D GetLocalRootTransform(this Node3D node)
        {
            var parent = node.GetParent<Node3D>();
            var result = node.TopLevel || parent is null ? node.Transform
                : (parent.GetLocalRootTransform() * node.Transform);
            return result;
        }

        public static void SetLocalRootTransform(this Node3D node, in Transform3D transform)
        {
            var parent = node.GetParent<Node3D>();
            node.Transform = parent is null ? transform
                : parent.GetLocalRootTransform().AffineInverse() * transform;
        }

        public static BoxShape3D CreateBoxShape(this Mesh mesh)
            => new() { Size = mesh.GetAabb().Size };

        public static SphereShape3D CreateSphereShape(this Mesh mesh)
            => new() { Radius = mesh.GetAabb().GetLongestAxisSize() * .5f };

        public static CylinderShape3D CreateCylinderShape(this Mesh mesh) => mesh.CreateCylinderShape(Vector3.Axis.Y).Shape;
        public static (CylinderShape3D Shape, Vector3 Rotation) CreateCylinderShape(this Mesh mesh, Vector3.Axis axis)
        {
            var (radius, height, rotation) = mesh.GetAabb().GetRadialDimensions(axis);
            return (new() { Radius = radius, Height = height }, rotation);
        }

        public static CapsuleShape3D CreateCapsuleShape(this Mesh mesh) => mesh.CreateCapsuleShape(Vector3.Axis.Y).Shape;
        public static (CapsuleShape3D Shape, Vector3 Rotation) CreateCapsuleShape(this Mesh mesh, Vector3.Axis axis)
        {
            var (radius, height, rotation) = mesh.GetAabb().GetRadialDimensions(axis);
            return (new() { Radius = radius, Height = height }, rotation);
        }

        public static GeometryInstance3D Clip(this MeshInstance3D source, CollisionShape3D shape)
        {
            var copy = source.Copy();
            return copy; // TODO
            //var csgRoot = new CsgCombiner3D { Name = source.Name };

            //csgRoot.OwnChild(new CsgMesh3D { Name = "Mesh", Mesh = source.Mesh });
            //csgRoot.OwnChild(new CsgMesh3D { Name = "Shape", Mesh = shape.Shape.GetDebugMesh(), Operation = OperationEnum.Intersection });

            //return csgRoot;
        }

        private static (float Radius, float Height, Vector3 Rotation) GetRadialDimensions(this in Aabb bb, Vector3.Axis axis)
        {
            return axis switch
            {
                Vector3.Axis.X => (Math.Max(bb.Size.Y, bb.Size.Z) * .5f, bb.Size.X, new(0, 0, Const.HalfPi)),
                Vector3.Axis.Y => (Math.Max(bb.Size.X, bb.Size.Z) * .5f, bb.Size.Y, Vector3.Zero),
                Vector3.Axis.Z => (Math.Max(bb.Size.X, bb.Size.Y) * .5f, bb.Size.Z, new(Const.HalfPi, 0, 0)),
                _ => throw new NotImplementedException(),
            };
        }
    }
}
#endif
