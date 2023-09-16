using System.Diagnostics;
using Godot;
using static Godot.TextureRect;

namespace GodotSharp.BuildingBlocks
{
    [SceneTree]
    public partial class PlayerList : Root
    {
        [Export] private bool AvatarAsImage { get; set; }

        private readonly Dictionary<int, Control[]> content = new();

        public void AddPlayer(int playerId, string playerName, Color? color = null, Texture2D avatar = null)
        {
            Debug.Assert(playerId is not 0);

            Visible = true;

            var items = Items().ToArray();
            content.Add(playerId, items);
            items.ForEach(x => Content.AddChild(x));

            IEnumerable<Control> Items()
            {
                yield return Item(Player, playerName);
                yield return Sep(Sep1);
                yield return AvatarAsImage ? Image(avatar) : Item(Avatar);
                yield return Sep(Sep2);
                yield return Item(Score);

                Label Item(Label source, string content = "")
                {
                    var item = (Label)source.Duplicate();
                    item.SetFontColor(color);
                    item.Text = content;
                    return item;
                }

                TextureRect Image(Texture2D avatar) => new()
                {
                    Texture = avatar,
                    ExpandMode = ExpandModeEnum.IgnoreSize,
                    StretchMode = StretchModeEnum.KeepAspectCentered,
                };

                Separator Sep(Separator source)
                    => (Separator)source.Duplicate();
            }
        }

        public void RemovePlayer(int playerId)
        {
            Debug.Assert(playerId is not 0);

            content.Remove(playerId, out var items);
            items.ForEach(x => Content.RemoveChild(x, free: true));

            if (content.Count is 0)
                Visible = false;
        }

        public void SetPlayerName(int playerId, string playerName)
        {
            if (!TryGetItem(playerId, Column.Player, out var item)) return;
            item.Text = playerName;
        }

        public void SetPlayerColor(int playerId, Color? color)
        {
            if (!TryGetContent(playerId, out var items)) return;
            items.OfType<Label>().ForEach(x => x.SetFontColor(color));
        }

        public void SetPlayerAvatar(int playerId, string avatar)
        {
            if (AvatarAsImage) return;
            if (!TryGetItem(playerId, Column.Avatar, out var item)) return;
            item.Text = avatar;
        }

        public void SetPlayerAvatar(int playerId, Texture2D avatar)
        {
            if (!AvatarAsImage) return;
            if (!TryGetItem<TextureRect>(playerId, Column.Avatar, out var item)) return;
            item.Texture = avatar;
        }

        public void SetPlayerScore(int playerId, int score, string tooltip = null)
        {
            if (!TryGetItem(playerId, Column.Score, out var item)) return;
            item.Text = $"{score}";
            item.TooltipText = tooltip;
        }

        private bool TryGetItem(int playerId, int itemId, out Label item)
            => TryGetItem<Label>(playerId, itemId, out item);

        private bool TryGetItem<T>(int playerId, int itemId, out T item) where T : Control
        {
            item = default;
            if (!TryGetContent(playerId, out var items)) return false;
            item = items[itemId] as T;
            return item is not null;
        }

        private bool TryGetContent(int playerId, out Control[] items)
        {
            Debug.Assert(playerId is not 0);

            if (content.TryGetValue(playerId, out items))
                return true;

            Log.Warn($"Unknown PlayerId {playerId}");
            return false;
        }

        private static class Column
        {
            public const int Player = 0;
            public const int Sep1 = 1;
            public const int Avatar = 2;
            public const int Sep2 = 3;
            public const int Score = 4;
        }
    }
}
