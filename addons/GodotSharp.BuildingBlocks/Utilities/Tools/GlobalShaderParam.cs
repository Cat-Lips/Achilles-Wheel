using System.Runtime.CompilerServices;
using Godot;
using Godot.Collections;
using static Godot.RenderingServer;

namespace GodotSharp.BuildingBlocks
{
    public class GlobalShaderParam<[MustBeVariant] T>(T defaultValue = default, [CallerFilePath] string caller = null)
    {
        public T Value { get; private set; } = defaultValue;
        public StringName ParamName { get; } = Path.GetFileNameWithoutExtension(caller).ToSnakeCase();

        public void Add(string shaderType, T defaultValue = default)
        {
            ProjectSettings.SetSetting($"shader_globals/{ParamName}", new Dictionary()
            {
                { "type", shaderType },
                { "value", Variant.From(defaultValue) }
            });
        }

        public void Remove()
            => ProjectSettings.SetSetting($"shader_globals/{ParamName}", default);

        public T GetDefault()
            => ProjectSettings.GetSetting($"shader_globals/{ParamName}").AsGodotDictionary()["value"].As<T>();

        public bool IsSet()
            => ProjectSettings.HasSetting($"shader_globals/{ParamName}");

        public void Set(in T value)
        {
            if (!Equals(Value, value))
                GlobalShaderParameterSet(ParamName, Variant.From(Value = value));
        }
    }
}
