using System.Runtime.CompilerServices;
using FastEnumUtility;
using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class ErrorExtensions
    {
        public static bool Ok(this Error err) => err is Error.Ok;
        public static bool NotOk(this Error err) => err is not Error.Ok;

        public static string Str(this Error err) => FastEnum.ToString(err).Capitalize();

        public static void Throw(this Error err, string title, string msg, params object[] data)
        {
            if (err.Ok()) return;
            var message = $"{title} ({err.Str()}): {msg}{FormatData()}";
            GD.PrintErr(message);
            throw new Exception(message);

            string FormatData()
                => data.IsNullOrEmpty() ? null : $" [{string.Join(", ", data)}]";
        }

        public static void ThrowOnErr(this Error err, [CallerArgumentExpression(nameof(err))] string expr = null, [CallerFilePath] string file = null, [CallerMemberName] string caller = null, [CallerLineNumber] int line = default)
            => err.Throw($"{Path.GetFileNameWithoutExtension(file)}.{caller}.{line}", expr);
    }
}
