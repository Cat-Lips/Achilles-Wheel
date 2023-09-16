#if TOOLS
using Godot;
using Godot.Collections;

namespace GodotSharp.BuildingBlocks
{
    public static partial class Editor
    {
        public static bool Hide(Dictionary source, StringName property)
            => SetUsage(source, property, PropertyUsageFlags.Internal);

        public static bool Hide(Dictionary source, StringName property, bool @if)
            => Show(source, property, !@if);

        public static bool Show(Dictionary source, StringName property, bool @if)
        {
            if (source["name"].AsStringName() == property && !@if)
            {
                source["usage"] = (int)(source["usage"].As<PropertyUsageFlags>() & ~PropertyUsageFlags.Editor);
                return true;
            }

            return false;
        }

        public static bool SetReadOnly(Dictionary source, StringName property)
            => SetUsage(source, property, PropertyUsageFlags.ReadOnly, PropertyUsageFlags.NoInstanceState);

        public static bool SetDisplayOnly(Dictionary source, StringName property)
            => SetUsage(source, property, PropertyUsageFlags.Editor, PropertyUsageFlags.NoInstanceState);

        private static bool SetUsage(Dictionary source, StringName property, params PropertyUsageFlags[] usage)
        {
            if (source["name"].AsStringName() == property)
            {
                source["usage"] = (int)usage.Aggregate((x, y) => x | y);
                return true;
            }

            return false;
        }

        public static Dictionary AddReadOnly(StringName name, Variant.Type type)
            => Add(name, type, PropertyUsageFlags.ReadOnly, PropertyUsageFlags.NoInstanceState);

        private static Dictionary Add(StringName name, Variant.Type type, params PropertyUsageFlags[] usage) => new()
        {
            { "name", name },
            { "type", (int)type },
            { "usage", (int)usage.Aggregate((x, y) => x | y) }
        };

        private static Dictionary Add(StringName name, Variant.Type type, PropertyHint hint, string hintString, params PropertyUsageFlags[] usage)
        {
            var cfg = Add(name, type, usage);
            cfg.Add("hint", (int)hint);
            cfg.Add("hint_string", hintString);
            return cfg;
        }
    }
}
#endif
