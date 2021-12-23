// vis2k: GUILayout instead of spacey += ...; removed Update hotkeys to avoid
// confusion if someone accidentally presses one.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace Mirror
{
    /// <summary>Shows NetworkManager controls in a GUI at runtime.</summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/NetworkManagerHUD")]
    [RequireComponent(typeof(NetworkManager))]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-manager-hud")]
    public class NetworkManagerHUD : MonoBehaviour
    {
        NetworkManager manager;

        public bool useLobbyGUI = false;
        public GameObject lobbyGUI;
        public GameObject pauseGUI; // for exiting as server/client
        public GameObject optionsGUI; // in-game options, for now using same options panel as main menu
        public string username;

        private GameObject hostJoinGUI;
        private TMP_InputField userInput;
        private GameObject connectingGUI;

        private bool runOnce = true;

        private GameObject outerCamera;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }

        void Start() {
            SetHUD();
        }

        void SetHUD()
        {
            // used on new scene loads, for lobby is assigned in editor
            pauseGUI = GameObject.Find("Canvas").transform.Find("PauseMenu").gameObject;
            optionsGUI = GameObject.Find("Canvas").transform.Find("OptionsMenu").gameObject;

            if (useLobbyGUI) {
                hostJoinGUI = lobbyGUI.transform.Find("HostOrJoinLobby").gameObject;
                connectingGUI = lobbyGUI.transform.Find("ConnectingMenu").gameObject;
            }

            outerCamera = GameObject.Find("/OuterCamera");
            username = PlayerPrefs.GetString("username", "");

            if (useLobbyGUI) {
                userInput = hostJoinGUI.transform.Find("UsernameInputField").GetComponent<TMP_InputField>();
                userInput.text = username;
                HostJoinListeners();
                ConnectListeners();
            }

            PauseListeners();
            OptionsListeners();
        }
        
        void Update() {
            if (NetworkClient.isConnected) {
                if (runOnce) {
                    if (useLobbyGUI) {
                        lobbyGUI.SetActive(false);
                        connectingGUI.SetActive(false);
                        hostJoinGUI.SetActive(true);

                        PlayerPrefs.SetString("username", userInput.text);
                        username = userInput.text;

                    }
                    runOnce = false;
                }
            } else {
                if (!runOnce) {
                    if (useLobbyGUI) {
                        lobbyGUI.SetActive(true);
                        pauseGUI.SetActive(false);
                        outerCamera.SetActive(true);
                    } else {
                        Debug.Log("Disconnected from host, returning to lobby...");
                        transform.Find("CTFManager").GetComponent<CTFManager>().EndCTF();
                        SetHUD();
                    }
                    runOnce = true;
                }
            }
        }

        void HostJoinListeners() 
        {
            Button hostBtn = hostJoinGUI.transform
                .Find("HostField")
                .Find("Button").GetComponent<Button>();

            Button joinBtn = hostJoinGUI.transform
                .Find("JoinField")
                .Find("Button").GetComponent<Button>();

            var inputServerAddr = hostJoinGUI.transform
                .Find("JoinField")
                .Find("InputServerIP").GetComponent<TMP_InputField>();

            Button backBtn = hostJoinGUI.transform
                .Find("BackButton").GetComponent<Button>();
            
            
            hostBtn.onClick.AddListener(() => {
                manager.StartHost();
                lobbyGUI.SetActive(false);
            });

            joinBtn.onClick.AddListener(() => {
                manager.StartClient();
                manager.networkAddress = inputServerAddr.text;
                hostJoinGUI.SetActive(false);
                connectingGUI.SetActive(true);
            });

            backBtn.onClick.AddListener(() => {
                SceneManager.LoadScene("Menu");
                Destroy(gameObject);
            });
        }
        void ConnectListeners() 
        {
            Button cancelBtn = connectingGUI.transform
                .Find("CancelButton").GetComponent<Button>();
            
            cancelBtn.onClick.AddListener(() => {
                manager.StopClient();
                hostJoinGUI.SetActive(true);
                connectingGUI.SetActive(false);
            });
        }

        void PauseListeners()
        {

            Button optionsBtn = pauseGUI.transform
                .Find("OptionsButton").GetComponent<Button>();

            Button exitMenuBtn = pauseGUI.transform
                .Find("ExitToMenu").GetComponent<Button>();
            
            Button exitDesktopBtn = pauseGUI.transform
                .Find("ExitGame").GetComponent<Button>();
            
            optionsBtn.onClick.AddListener(() => {
                pauseGUI.SetActive(false);
                optionsGUI.SetActive(true);
            });

            exitMenuBtn.onClick.AddListener(() => {
                // stop host if host mode
                if (NetworkServer.active && NetworkClient.isConnected)
                {
                    manager.StopHost();
                }
                // stop client if client-only
                else if (NetworkClient.isConnected)
                {
                    manager.StopClient();
                }

                Debug.Log("exit?");
                if (useLobbyGUI) {
                    outerCamera.SetActive(true);

                    pauseGUI.SetActive(false);
                    lobbyGUI.SetActive(true);
                } else {
                    transform.Find("CTFManager").GetComponent<CTFManager>().EndCTF();
                    Destroy(gameObject);
                }
            });

            exitDesktopBtn.onClick.AddListener(() => {
                Debug.Log("quitting game from pause menu...");
                Application.Quit();
            });
        }

        void OptionsListeners()
        {
           Button backBtn = optionsGUI.transform.Find("BackButton").GetComponent<Button>(); 
           backBtn.onClick.AddListener(() => {
               optionsGUI.SetActive(false);
               pauseGUI.SetActive(true);
           });
        }
    }
}
