using Godot;
using FileAccess = Godot.FileAccess;

namespace GodotSharp.BuildingBlocks
{
    [SceneTree]
    public partial class QuickHelp : Root
    {
        [Export(PropertyHint.File, "*.txt"), Notify]
        public string TextFile { get => _textFile.Get(); set => _textFile.Set(value); }

        [GodotOverride]
        private void OnReady()
        {
            InitText();
            InitContent();

            void InitText()
            {
                LoadText();
                TextFileChanged += LoadText;

                void LoadText()
                {
                    Content.Text = FileAccess.FileExists(TextFile)
                        ? FileAccess.GetFileAsString(TextFile)
                        : $"QuickHelpFileNotFound: {TextFile}";
                }
            }

            void InitContent()
            {
                Content.MetaClicked += OpenUrl;

                void OpenUrl(Variant link)
                    => OS.ShellOpen(link.AsString());
            }
        }

        public override partial void _Ready();
    }
}
