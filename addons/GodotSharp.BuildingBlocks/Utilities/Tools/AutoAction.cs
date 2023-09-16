using Godot;

namespace GodotSharp.BuildingBlocks
{
    public partial class AutoAction(Action action = null) : GodotObject
    {
        public Action Action { get; set; } = action;
        public bool Triggered { get; private set; }

        public void Run()
        {
            if (Triggered) return;
            if (Action is null) return;

            Triggered = true;
            this.CallDeferred(() =>
            {
                Triggered = false;
                Action?.Invoke();
            });
        }
    }

    //public class AutoResetRefreshTimer
    //{
    //    private double time;
    //    private bool trigger;

    //    public float Time { get; set; } = 500;

    //    public void Set() => trigger = true;
    //    public bool Elapsed(double delta)
    //    {
    //        if (!trigger) return false;
    //        if ((time += delta) < Time) return false;

    //        time = 0;
    //        trigger = false;
    //        return true;
    //    }
    //}

    //public class ManualResetRefreshTimer(bool triggered = false)
    //{
    //    private double time;
    //    private bool trigger;

    //    public float Time { get; set; } = 500;

    //    public bool Get() => triggered;
    //    public void Set() => trigger = true;

    //    public bool Elapsed(double delta)
    //        => triggered || (triggered = trigger && (time += delta) >= Time);

    //    public void Reset()
    //    {
    //        time = 0;
    //        trigger = false;
    //        triggered = false;
    //    }
    //}
}
