using Godot;

namespace GodotSharp.BuildingBlocks
{
    [SceneTree]
    public partial class MainMenu : Root
    {
        private readonly Settings<MainMenu> settings = new();

        private static readonly Color WarnColor = Colors.Yellow;
        private static readonly Color ErrorColor = Colors.Red;
        private static readonly Color SuccessColor = Colors.Green;

        public MainMenu()
            => LogStart();

        public void Initialise(Network network)
        {
            ShowNetworkMenus();

            InitActions();
            InitDisplay();

            void ShowNetworkMenus()
                => ClientMenu.Visible = ServerMenu.Visible = true;

            void InitActions()
            {
                this.StartServer.Pressed += StartServer;
                this.CreateClient.Pressed += CreateClient;
                this.StopServer.Pressed += StopServer;
                this.CloseClient.Pressed += CloseClient;

                void StartServer()
                    => network.StartServer(ServerPort.Value, SetServerStatus);

                void CreateClient()
                    => network.CreateClient(ConnectAddress.Text, ConnectPort.Value, SetClientStatus);

                void StopServer()
                    => network.StopServer();

                void CloseClient()
                    => network.CloseClient();
            }

            void InitDisplay()
            {
                InitNetworkMenus();
                network.NetworkStateChanged += InitNetworkMenus;

                var windowTitle = GetWindow().Title;
                network.NetworkStateChanged += SetWindowTitle;

                void InitNetworkMenus()
                {
                    switch (network.NetworkState)
                    {
                        case NetworkState.ClientConnecting:
                            ServerMenu.Visible = false;

                            CreateClient.Visible = false;
                            CloseClient.Visible = true;
                            ConnectAddress.Editable = false;
                            ConnectPort.Editable = false;

                            PlayerName.Editable = false;
                            PlayerColor.Enabled(false);
                            GameOptions.Enabled(false);

                            break;

                        case NetworkState.ServerStarting:
                            ClientMenu.Visible = false;

                            StartServer.Visible = false;
                            StopServer.Visible = true;
                            ServerPort.Editable = false;

                            PlayerName.Editable = false;
                            PlayerColor.Enabled(false);
                            GameOptions.Enabled(false);

                            break;

                        case NetworkState.None:
                            ClientMenu.Visible = true;
                            ServerMenu.Visible = true;

                            CreateClient.Visible = true;
                            CloseClient.Visible = false;
                            ConnectAddress.Editable = true;
                            ConnectPort.Editable = true;

                            StartServer.Visible = true;
                            StopServer.Visible = false;
                            ServerPort.Editable = true;

                            SetServerStatus(StatusType.Info, "(not running)");
                            SetClientStatus(StatusType.Info, "(not connected)");

                            PlayerName.Editable = true;
                            PlayerColor.Enabled(true);
                            GameOptions.Enabled(true);

                            break;
                    }
                }

                void SetWindowTitle()
                {
                    GetWindow().Title = $"{windowTitle}{NetworkStateSuffix()}";

                    string NetworkStateSuffix() => network.NetworkState switch
                    {
                        NetworkState.ServerStarted => " - SERVER",
                        NetworkState.ClientConnected => " - CLIENT",
                        _ => "",
                    };
                }
            }

            void SetServerStatus(StatusType status, string message)
                => SetStatus(ServerStatus, status, message);

            void SetClientStatus(StatusType status, string message)
                => SetStatus(ClientStatus, status, message);

            static void SetStatus(Label statusLabel, StatusType status, string message)
            {
                SetStatus(statusLabel, message, StatusColor());

                Color? StatusColor() => status switch
                {
                    StatusType.Warn => WarnColor,
                    StatusType.Error => ErrorColor,
                    StatusType.Success => SuccessColor,
                    _ => null,
                };

                static void SetStatus(Label statusLabel, string message, Color? color = null)
                {
                    statusLabel.Text = message;

                    if (color is null)
                        statusLabel.ResetFontColor();
                    else
                        statusLabel.SetFontColor(color.Value);
                }
            }
        }

        public void Initialise(params Control[] gameOptions)
        {
            ShowGameOptions();
            InitGameOptions();

            void ShowGameOptions()
                => GameOptionsSep.Visible = GameOptions.Visible = true;

            void InitGameOptions()
            {
                var popup = GameOptions.GetPopup();

                popup.Clear();
                popup.IndexPressed += idx => gameOptions[idx].Show();
                gameOptions.ForEach(x => popup.AddItem(x.Name.ToString().Capitalize()));
            }
        }

        public event Action<string> PlayerNameChanged;
        public event Action<Color> PlayerColorChanged;
        public string GetPlayerName() => PlayerName.Text;
        public Color GetPlayerColor() => PlayerColor.Color;

        [GodotOverride]
        private void OnReady()
        {
            InitGameMenu();
            InitPlayerMenu();
            InitNetworkMenus();

            Quit.Pressed += QuitGame;
            PreSortChildren += ResetSize;
            GetWindow().CloseRequested += OnWindowClosing;

            void InitGameMenu()
                => GameOptionsSep.Visible = GameOptions.Visible = false;

            void InitPlayerMenu()
            {
                PlayerName.Text = GetPlayerName();
                PlayerColor.Color = GetPlayerColor();

                PlayerName.FocusExited += SetPlayerName;
                PlayerColor.ColorChanged += SetPlayerColor;

                string GetPlayerName()
                    => settings.TryGet(PlayerMenu, PlayerName, DefaultPlayerName());

                Color GetPlayerColor()
                    => settings.TryGet(PlayerMenu, PlayerColor, Random.Shared.Next(DefaultColors));

                string DefaultPlayerName()
                    => System.Environment.UserName;

                void SetPlayerName()
                {
                    ValidatePlayerName();
                    SetPlayerName();

                    void ValidatePlayerName()
                    {
                        if (string.IsNullOrWhiteSpace(PlayerName.Text))
                            PlayerName.Text = DefaultPlayerName();
                    }

                    void SetPlayerName()
                    {
                        settings.Set(PlayerMenu, PlayerName, PlayerName.Text);
                        PlayerNameChanged?.Invoke(GetPlayerName());
                    }
                }

                void SetPlayerColor(Color color)
                {
                    settings.Set(PlayerMenu, PlayerColor, color);
                    PlayerColorChanged?.Invoke(GetPlayerColor());

                }
            }

            void InitNetworkMenus()
            {
                ClientMenu.Visible = ServerMenu.Visible = false;
                ServerAddress.Text = Network.GetLocalAddress();
            }

            void QuitGame()
            {
                GetWindow().PropagateNotification((int)NotificationWMCloseRequest);
                GetTree().Quit();
                LogEnd();
            }

            void OnWindowClosing()
                => Input.MouseMode = default;
        }

        public override partial void _Ready();

        //[Conditional("TOOLS")]
        private static void LogStart()
        {
            if (!Engine.IsEditorHint())
                Log.Debug(">>> GAME START <<<");
        }

        //[Conditional("TOOLS")]
        private static void LogEnd()
        {
            if (!Engine.IsEditorHint())
                Log.Debug(">>> GAME EXIT <<<");
        }

        private static readonly Color[] DefaultColors =
        {
            new(1, 0, 0), // Red
            new(0, 1, 0), // Green
            new(0, 0, 1), // Blue
            new(1, 1, 0), // Yellow (red+green)
            new(0, 1, 1), // Cyan (green+blue)
            new(1, 0, 1), // Purple (red+blue)
        };
    }
}
