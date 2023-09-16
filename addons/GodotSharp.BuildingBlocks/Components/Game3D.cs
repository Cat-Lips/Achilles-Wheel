using Godot;

namespace GodotSharp.BuildingBlocks
{
    [SceneTree]
    public partial class Game3D : Node
    {
        protected virtual void AddPlayer(int playerId) { }
        protected virtual void RemovePlayer(int playerId) { }

        [GodotOverride]
        private void OnReady()
        {
            InitialiseMenu();
            InitialiseNetwork();
            ParseCommandLineArgs();
            ShowQuickHelpOnStartup();

            void InitialiseMenu()
            {
                MainMenu.Initialise(Network);
                MainMenu.Initialise(QuickHelp);
            }

            void InitialiseNetwork()
            {
                Network.Initialise<PlayerProfile>(InitLocalPlayer, OnPlayerAdded, OnPlayerRemoved);

                void InitLocalPlayer(PlayerProfile pp)
                {
                    pp.PlayerName = MainMenu.GetPlayerName();
                    pp.PlayerColor = MainMenu.GetPlayerColor();
                }

                void OnPlayerAdded(PlayerProfile pp)
                {
                    pp.OnReady(() =>
                    {
                        var playerId = pp.GetMultiplayerAuthority();
                        PlayerList.AddPlayer(playerId, pp.PlayerName, pp.PlayerColor);

                        if (this.IsMultiplayerServer())
                            AddPlayer(playerId);
                    });
                }

                void OnPlayerRemoved(PlayerProfile pp)
                {
                    var playerId = pp.GetMultiplayerAuthority();
                    PlayerList.RemovePlayer(playerId);

                    if (this.IsMultiplayerServer())
                        RemovePlayer(playerId);
                }
            }

            void ParseCommandLineArgs()
            {
                var args = OS.GetCmdlineArgs().Select(x => x.ToLower()).ToArray();
                if (args.Contains("--server")) InvokeStartServer();
                else if (args.Contains("--client")) InvokeCreateClient();

                void InvokeStartServer()
                    => this.CallDeferred(() => MainMenu.StartServer.EmitSignal("pressed"));

                void InvokeCreateClient()
                    => this.CallDeferred(() => MainMenu.CreateClient.EmitSignal("pressed"));
            }

            void ShowQuickHelpOnStartup()
            {
                const string ShowOnStartup = "ShowOnStartup";
                var settings = new Settings<QuickHelp>();

                if (settings.TryGet(ShowOnStartup, true))
                {
                    QuickHelp.Show();
                    settings.Set(ShowOnStartup, false);
                }
            }
        }

        public override partial void _Ready();
    }
}
