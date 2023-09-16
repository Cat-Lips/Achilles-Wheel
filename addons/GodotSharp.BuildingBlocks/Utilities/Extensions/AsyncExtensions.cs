using AsyncAwaitBestPractices;
using Godot;
using CT = System.Threading.CancellationToken;
using CTS = System.Threading.CancellationTokenSource;

namespace GodotSharp.BuildingBlocks
{
    public static class AsyncExtensions
    {
        public static void CallDeferred(this GodotObject source, Action action, Action<bool> onComplete = null)
        {
            Callable.From(() =>
            {
                var yes = GodotObject.IsInstanceValid(source);
                if (yes) action();
                onComplete?.Invoke(yes);
            }).CallDeferred();
        }

        public static void CallDeferred(this GodotObject source, CT ct, Action action, Action<bool> onComplete = null)
        {
            Callable.From(() =>
            {
                var yes = GodotObject.IsInstanceValid(source) && !ct.IsCancellationRequested;
                if (yes) action();
                onComplete?.Invoke(yes);
            }).CallDeferred();
        }

        public static void RunAsync(this GodotObject _, Action action)
            => Task.Run(action).SafeFireAndForget();

        public static CTS RunAsync(this GodotObject _, Action<CT> action)
        {
            var cts = new CTS();
            Task.Run(() => action(cts.Token)).SafeFireAndForget();
            return cts;
        }

        //private static readonly Dictionary<(GodotObject, string), AsyncAction> AsyncActions = new();
        //public static CTS RunAsync(this GodotObject source, Func<CT, Action> action, [CallerArgumentExpression(nameof(action))] string key = null)
        //{
        //    var sourceKey = (source, key);
        //    if (!AsyncActions.TryGetValue(sourceKey, out var asyncAction))
        //    {
        //        AsyncActions.Add(sourceKey, asyncAction = new(source, ct =>
        //        {
        //            var onComplete = action(ct);

        //            return () =>
        //            {
        //                AsyncActions.Remove(sourceKey);
        //                onComplete?.Invoke();
        //            };
        //        }));
        //    }

        //    return asyncAction.Run();
        //}

        //public static CTS RunAsync(this GodotObject source, Func<Action> action, [CallerArgumentExpression(nameof(action))] string key = null)
        //    => source.RunAsync(_ => action(), key);

        //public static CTS RunAsync(this GodotObject source, Action<CT> action, [CallerArgumentExpression(nameof(action))] string key = null)
        //    => source.RunAsync(ct => { action(ct); return null; }, key);

        //public static CTS RunAsync(this GodotObject source, Action action, [CallerArgumentExpression(nameof(action))] string key = null)
        //    => source.RunAsync(_ => { action(); return null; }, key);

        //private class AsyncAction(GodotObject gd, Func<CT, Action> action)
        //{
        //    private CTS cts = new();
        //    private readonly GodotObject gd = gd;
        //    private readonly Func<CT, Action> action = action;

        //    public CTS Run()
        //    {
        //        cts.Cancel();

        //        return cts = App.RunAsync(ct =>
        //        {
        //            var onComplete = action(ct);
        //            gd.CallDeferred(ct, () => onComplete?.Invoke());
        //        });
        //    }
        //}
    }
}
