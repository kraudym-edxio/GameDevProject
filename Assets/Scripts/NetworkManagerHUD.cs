// vis2k: GUILayout instead of spacey += ...; removed Update hotkeys to avoid
// confusion if someone accidentally presses one.
using UnityEngine;
using UnityEngine.UI;
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

        private GameObject hostJoinGUI;
        private GameObject connectingGUI;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();

            hostJoinGUI = lobbyGUI.transform.Find("HostOrJoinLobby").gameObject;
            connectingGUI = lobbyGUI.transform.Find("ConnectingMenu").gameObject;
        }

        void Start()
        {
            HostJoinListeners();
            ConnectListeners();
            PauseListeners();
        }
        
        void Update() {
            if (NetworkClient.isConnected) {
                lobbyGUI.SetActive(false);
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
        }
        void ConnectListeners() 
        {
            Button cancelBtn = connectingGUI.transform
                .Find("CancelButton").GetComponent<Button>();
            
            cancelBtn.onClick.AddListener(() => {
                manager.StopClient();
            });
        }

        void PauseListeners()
        {

            Button exitBtn = pauseGUI.transform
                .Find("ExitToMenu").GetComponent<Button>();
            

            exitBtn.onClick.AddListener(() => {
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
        }
    }
}
