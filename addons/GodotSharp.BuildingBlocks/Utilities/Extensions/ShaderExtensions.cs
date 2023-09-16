//using System.Diagnostics;
//using System.Reflection;
//using Godot;

//// https://www.youtube.com/watch?v=5CKvGYqagyI

//using WorkGroups = (uint x, uint y, uint z);

//namespace GodotSharp.BuildingBlocks
//{
//    public static class ShaderExtensions
//    {
//        private static string GetShaderPath(this GodotObject source)
//        {
//            return GetScriptPath().Replace(".cs", ".glsl");

//            string GetScriptPath()
//                => source.GetType().GetCustomAttribute<ScriptPathAttribute>(false).Path;
//        }

//        public static void RunComputeShader(this GodotObject source, uint byteCount, Action<byte[]> output) => source.RunComputeShader((1, 1, 1), new byte[byteCount], output);
//        public static void RunComputeShader(this GodotObject source, WorkGroups wg, uint byteCount, Action<byte[]> output) => source.RunComputeShader(wg, new byte[byteCount], output);
//        public static void RunComputeShader(this GodotObject source, byte[] input, Action<byte[]> output) => source.RunComputeShader((1, 1, 1), input, output);
//        public static void RunComputeShader(this GodotObject source, WorkGroups wg, byte[] input, Action<byte[]> output)
//        {
//            var rd = RenderingServer.CreateLocalRenderingDevice();
//            if (rd is null) return; // headless

//            CreateShader(out var ridShader);
//            CreatePipeline(out var ridPipeline);
//            CreateStorage(out var ridStorage, out var ridUniforms);
//            CreateComputeList();
//            RunComputeList();

//            void CreateShader(out Rid ridShader)
//            {
//                var glsl = GD.Load<RDShaderFile>(source.GetShaderPath());
//                ridShader = rd.ShaderCreateFromSpirV(glsl.GetSpirV());
//            }

//            void CreatePipeline(out Rid ridPipeline)
//                => ridPipeline = rd.ComputePipelineCreate(ridShader);

//            void CreateStorage(out Rid ridStorage, out Rid ridUniforms)
//            {
//                ridStorage = rd.StorageBufferCreate((uint)input.Length, input);

//                var uniform = new RDUniform { UniformType = RenderingDevice.UniformType.StorageBuffer };
//                Debug.Assert(uniform.Binding is 0);
//                uniform.AddId(ridStorage);

//                ridUniforms = rd.UniformSetCreate(new() { uniform }, ridShader, 0);
//            }

//            void CreateComputeList()
//            {
//                var cl = rd.ComputeListBegin();
//                rd.ComputeListBindComputePipeline(cl, ridPipeline);
//                rd.ComputeListBindUniformSet(cl, ridUniforms, 0);
//                rd.ComputeListDispatch(cl, wg.x, wg.y, wg.z);
//                rd.ComputeListEnd();
//            }

//            void RunComputeList()
//            {
//                rd.Submit();
//                App.RunAsync(() =>
//                {
//                    rd.Sync();
//                    App.CallDeferred(() =>
//                    {
//                        output(rd.BufferGetData(ridStorage));
//                        rd.FreeRid(ridPipeline);
//                        rd.FreeRid(ridUniforms);
//                        rd.FreeRid(ridStorage);
//                        rd.FreeRid(ridShader);
//                        rd.Free();
//                    });
//                });
//            }
//        }
//    }
//}
