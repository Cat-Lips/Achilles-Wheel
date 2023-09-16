using Godot;

namespace GodotSharp.BuildingBlocks
{
    public static class SetterExtensions
    {
        public static bool Set<T, TValue>(this T src, ref TValue field, TValue value, params Action[] onChanged) where T : GodotObject => src._Set(ref field, value, onChanged: onChanged);
        public static bool Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, params Action[] onChanged) where T : GodotObject => src._Set(ref field, value, notify, onChanged: onChanged);
        public static bool OnSet<T, TValue>(this T src, ref TValue field, TValue value, Action onSet, params Action[] onChanged) where T : GodotObject => src._Set(ref field, value, onSet: [onSet], onChanged: onChanged);
        public static bool OnSet<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action onSet, params Action[] onChanged) where T : GodotObject => src._Set(ref field, value, notify, onSet: [onSet], onChanged: onChanged);

        public static bool OnSet<T, TValue>(this T src, ref TValue field, TValue value, params Action<TValue>[] onSet) where T : GodotObject => src._Set(ref field, value, onSetOld: onSet);
        public static bool OnSet<T, TValue>(this T src, ref TValue field, TValue value, params Action<TValue, TValue>[] onSet) where T : GodotObject => src._Set(ref field, value, onSetOldNew: onSet);
        public static bool OnSet<T, TValue>(this T src, ref TValue field, TValue value, bool notify, params Action<TValue>[] onSet) where T : GodotObject => src._Set(ref field, value, notify, onSetOld: onSet);
        public static bool OnSet<T, TValue>(this T src, ref TValue field, TValue value, bool notify, params Action<TValue, TValue>[] onSet) where T : GodotObject => src._Set(ref field, value, notify, onSetOldNew: onSet);

        public static bool Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue> onSet, params Action[] onChanged) where T : GodotObject => src._Set(ref field, value, onSetOld: [onSet], onChanged: onChanged);
        public static bool Set<T, TValue>(this T src, ref TValue field, TValue value, Action<TValue, TValue> onSet, params Action[] onChanged) where T : GodotObject => src._Set(ref field, value, onSetOldNew: [onSet], onChanged: onChanged);
        public static bool Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue> onSet, params Action[] onChanged) where T : GodotObject => src._Set(ref field, value, notify, onSetOld: [onSet], onChanged: onChanged);
        public static bool Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify, Action<TValue, TValue> onSet, params Action[] onChanged) where T : GodotObject => src._Set(ref field, value, notify, onSetOldNew: [onSet], onChanged: onChanged);

        private static bool _Set<T, TValue>(this T src, ref TValue field, TValue value, bool notify = false,
            Action[] onSet = null,
            Action[] onChanged = null,
            Action<TValue>[] onSetOld = null,
            Action<TValue, TValue>[] onSetOldNew = null) where T : GodotObject
        {
            if (Equals(field, value)) return false;

            var srcRes = src as Resource;

            Disconnect(field);
            var old = field;
            field = value;
            Connect(field);

            OnSet();
            OnChanged();
            NotifyEditor();

            return true;

            void OnSet()
            {
                onSet?.ForEach(x => x?.Invoke());
                onSetOld?.ForEach(x => x?.Invoke(old));
                onSetOldNew?.ForEach(x => x?.Invoke(old, value));
            }

            void OnChanged()
            {
                onChanged?.ForEach(x => x?.Invoke());
                srcRes?.EmitSignal(Resource.SignalName.Changed);
            }

            void NotifyEditor()
            {
                if (notify)
                    src.NotifyPropertyListChanged();
            }

            void Connect(TValue value)
            {
                if (value is Resource res) Connect(res);
                else if (value is Resource[] resArray) ConnectArray(resArray);

                void Connect(Resource res)
                {
                    if (res is null) return;
                    res.Connect(Resource.SignalName.Changed, Callable.From(OnChanged));
                }

                void ConnectArray(Resource[] resArray)
                {
                    if (resArray is null) return;
                    resArray.ForEach(Connect);
                }
            }

            void Disconnect(TValue value)
            {
                if (value is Resource res) Disconnect(res);
                else if (value is Resource[] resArray) DisconnectArray(resArray);

                void Disconnect(Resource res)
                {
                    if (res is null) return;
                    if (res.IsConnected(Resource.SignalName.Changed, Callable.From(OnChanged)))
                        res.Disconnect(Resource.SignalName.Changed, Callable.From(OnChanged));
                }

                void DisconnectArray(Resource[] resArray)
                {
                    if (resArray is null) return;
                    resArray.ForEach(Disconnect);
                }
            }
        }
    }
}
