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

        public GameObject lobbyGUI;
        public GameObject pauseGUI; // for exiting as server/client
        public string username;

        private GameObject hostJoinGUI;
        private TMP_InputField userInput;
        private GameObject connectingGUI;

        private bool runOnce = true;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();

            hostJoinGUI = lobbyGUI.transform.Find("HostOrJoinLobby").gameObject;
            connectingGUI = lobbyGUI.transform.Find("ConnectingMenu").gameObject;
        }

        void Start()
        {
            username = PlayerPrefs.GetString("username", "");

            userInput = hostJoinGUI.transform.Find("UsernameInputField").GetComponent<TMP_InputField>();

            userInput.text = username;

            HostJoinListeners();
            ConnectListeners();
            PauseListeners();
        }
        
        void Update() {
            try {
                if (NetworkClient.isConnected) {
                    if (runOnce) {
                        lobbyGUI.SetActive(false);
                        connectingGUI.SetActive(false);
                        hostJoinGUI.SetActive(true);

                        PlayerPrefs.SetString("username", userInput.text);
                        username = userInput.text;

                        runOnce = false;
                    }
                } else {
                    if (!runOnce) {
                        lobbyGUI.SetActive(true);
                        runOnce = true;
                    }
                }
            } catch {

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
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex-1);
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

            Button exitMenuBtn = pauseGUI.transform
                .Find("ExitToMenu").GetComponent<Button>();
            
            Button exitDesktopBtn = pauseGUI.transform
                .Find("ExitGame").GetComponent<Button>();
            

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

                pauseGUI.SetActive(false);
                lobbyGUI.SetActive(true);
            });

            exitDesktopBtn.onClick.AddListener(() => {
                Debug.Log("quitting game from pause menu...");
                Application.Quit();
            });
        }
    }
}
