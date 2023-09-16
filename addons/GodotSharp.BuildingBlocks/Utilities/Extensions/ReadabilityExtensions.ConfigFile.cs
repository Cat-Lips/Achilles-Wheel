using FastEnumUtility;
using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static partial class WheelExtensions
    {
        public static T GetValue<[MustBeVariant] T>(this ConfigFile source, string section, string key, T @default = default)
            => source.GetValue(section, key, Variant.From(@default)).As<T>();

        public static T GetEnumValue<[MustBeVariant] T>(this ConfigFile source, string section, string key, T @default = default) where T : struct, Enum
        {
            var dflt = FastEnum.GetName(@default);
            var value = (string)source.GetValue(section, key, dflt);
            if (FastEnum.TryParse(value, out T ret))
            {
                //GD.Print($"{typeof(T).Name} in {section}.{key} is {value}");
                return ret;
            }

            GD.PushError($"Unknown {typeof(T).Name} in {section}.{key} [Received: {value}, Returning: {dflt}, Expected: {string.Join("|", FastEnum.GetNames<T>())}]");
            return @default;
        }

        public static void SetValue<[MustBeVariant] T>(this ConfigFile source, string section, string key, T value)
            => source.SetValue(section, key, Variant.From(value));

        public static void SetEnumValue<[MustBeVariant] T>(this ConfigFile source, string section, string key, T value) where T : struct, Enum
            => source.SetValue(section, key, FastEnum.GetName(value));

        public static void ResetValue(this ConfigFile source, string section, string key)
            => source.SetValue(section, key, new Variant());
    }
}
