//using Godot;

//namespace GodotSharp.BuildingBlocks
//{
//    [Tool]
//    public partial class ComputeShader : Node
//    {
//        [Export] private RDShaderFile ShaderFile { get; set; }
//        [Export] private bool UseGlobalDevice { get; set; } = true;
//        [Export] private Vector3I WorkGroups { get; set; } = Vector3I.One;
//        [Export] private Godot.Collections.Dictionary<string, Variant> Uniforms { get; set; } = new();

//        public ComputeShader()
//        {
//            RenderingDevice rd = null;
//            Rid ridUniformSet = default;
//            Rid ridPipeline = default;
//            Rid ridShader = default;

//            TreeEntered += OnTreeEntered;
//            Ready += Execute;
//            TreeExiting += OnTreeExiting;

//            void OnTreeEntered()
//            {
//                rd = UseGlobalDevice
//                    ? RenderingServer.GetRenderingDevice()
//                    : RenderingServer.CreateLocalRenderingDevice();
//                ridShader = rd.ShaderCreateFromSpirV(ShaderFile.GetSpirV());
//                ridPipeline = rd.ComputePipelineCreate(ridShader);

//                //foreach (var kvp in Uniforms)
//                //{

//                //}

//                //ridUniformSet = rd.uniform

//                var cl = rd.ComputeListBegin();
//                rd.ComputeListBindComputePipeline(cl, ridPipeline);
//                rd.ComputeListBindUniformSet(cl, ridUniformSet, 0);
//                rd.ComputeListDispatch(cl, (uint)WorkGroups.X, (uint)WorkGroups.Y, (uint)WorkGroups.Z);
//                rd.ComputeListEnd();
//            }

//            void Execute()
//            {
//                //rd.Submit();
//                //this.RunAsync(() =>
//                //{
//                //    rd.Sync();
//                //    return () => { };
//                //});
//            }

//            void OnTreeExiting()
//            {
//                rd.FreeRid(ridShader);
//                rd.FreeRid(ridPipeline);
//                rd.FreeRid(ridUniformSet);

//                if (!UseGlobalDevice)
//                    rd.Free();
//            }
//        }
//    }
//}
