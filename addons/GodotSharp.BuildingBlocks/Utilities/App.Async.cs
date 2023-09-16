//using AsyncAwaitBestPractices;
//using Godot;

//using CT = System.Threading.CancellationToken;
//using CTS = System.Threading.CancellationTokenSource;

//namespace GodotSharp.BuildingBlocks
//{
//    public static partial class App
//    {
//        public static void RunAsync(Action action)
//            => Task.Run(action).SafeFireAndForget();

//        public static CTS RunAsync(Action<CT> action)
//        {
//            var cts = new CTS();
//            Task.Run(() => action(cts.Token)).SafeFireAndForget();
//            return cts;
//        }

//        public static void CallDeferred(Action action)
//            => Callable.From(action).CallDeferred();

//        public static bool IsOnMainThread()
//            => OS.GetThreadCallerId() == OS.GetMainThreadId();

//        public static void RunOnMainThread(Action action)
//        {
//            if (IsOnMainThread()) { action(); return; }

//            var waitHandle = new ManualResetEvent(false);
//            CallDeferred(() => Try(action, waitHandle.Set));
//            waitHandle.WaitOne();
//        }

//        public static T RunOnMainThread<T>(Func<T> action)
//        {
//            if (IsOnMainThread()) return action();

//            T result = default;
//            var waitHandle = new ManualResetEvent(false);
//            CallDeferred(() => result = Try(action, waitHandle.Set));
//            waitHandle.WaitOne();
//            return result;
//        }

//        public static void RunParallel(params Action[] actions)
//            => Parallel.ForEach(actions, action => action());

//        public static void RunParallel<T>(Action<T> action, params T[] args)
//            => Parallel.ForEach(args, action);
//    }
//}
