#if TOOLS
using Godot;

namespace GodotSharp.BuildingBlocks
{
    public enum BodyType { None, Static, Rigid, Area, Character, Animatable, Vehicle, Wheel }
    public enum ShapeType { None, Convex, MultiConvex, SimpleConvex, Trimesh, Box, Sphere, Cylinder, CylinderX, CylinderZ, Capsule, CapsuleX, CapsuleZ }
    public enum FrontFace { X, Z, NegX, NegZ }

    [Tool]
    public partial class PhysicsImportOptions : EditorScenePostImportPlugin
    {
        public PackedScene Root { get; private set; }

        public BodyType BodyType { get; private set; }
        public ShapeType ShapeType { get; private set; }
        public FrontFace FrontFace { get; private set; }

        public float MassMultiplier { get; private set; }
        public uint MaxMultiConvexShapes { get; private set; }

        public bool CreateConfig { get; private set; }
        public bool ImportOriginal { get; private set; }

        public override void _GetImportOptions(string path)
        {
            AddImportOption("PhysicsImport/Root", new PackedScene());

            this.AddEnumImportOption("PhysicsImport/BodyType", BodyType);
            this.AddEnumImportOption("PhysicsImport/ShapeType", ShapeType);
            this.AddEnumImportOption("PhysicsImport/FrontFace", FrontFace.Z);

            AddImportOption("PhysicsImport/MassMultiplier", 1.0f);
            AddImportOption("PhysicsImport/MaxMultiConvexShapes", 32u);

            AddImportOption("PhysicsImport/InitOptions/CreateConfig", false);
            AddImportOption("PhysicsImport/InitOptions/ImportOriginal", false);
        }

        public override void _PreProcess(Node scene)
        {
            Root = (PackedScene)GetOptionValue("PhysicsImport/Root");

            BodyType = this.GetEnumOptionValue<BodyType>("PhysicsImport/BodyType");
            ShapeType = this.GetEnumOptionValue<ShapeType>("PhysicsImport/ShapeType");
            FrontFace = this.GetEnumOptionValue<FrontFace>("PhysicsImport/FrontFace");

            MassMultiplier = (float)GetOptionValue("PhysicsImport/MassMultiplier");
            MaxMultiConvexShapes = (uint)GetOptionValue("PhysicsImport/MaxMultiConvexShapes");

            CreateConfig = (bool)GetOptionValue("PhysicsImport/InitOptions/CreateConfig");
            ImportOriginal = (bool)GetOptionValue("PhysicsImport/InitOptions/ImportOriginal");
        }

        public override void _PostProcess(Node scene)
        {
            Root = default;

            BodyType = default;
            ShapeType = default;
            FrontFace = default;

            MassMultiplier = default;
            MaxMultiConvexShapes = default;

            CreateConfig = default;
            ImportOriginal = default;
        }
    }
}
#endif
