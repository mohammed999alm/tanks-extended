using TMPro;
using UnityEngine.UI;
using UnityEngine;
using Mirror;

public class MainLobbyControl : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TMP_InputField m_NameInput = null;
    [SerializeField] private TMP_InputField m_IPInput = null;
    [SerializeField] private Slider m_VolumeControl = null;
    [SerializeField] private Button m_HostButton = null;
    [SerializeField] private Button m_JoinButton = null;
    [SerializeField] private LobbyNetworkManager m_NetworkManager = null;
    [SerializeField] private GameObject m_MainMenuCanvas = null;

    public static string m_DisplayName { get; private set; }
    public static string m_IP { get; private set; }

    public static float m_Volume { get; private set; }
    private const string m_PlayerPrefsNameKey = "PlayerName";
    private const string m_PlayerPrefsIPKey = "IP";
    private const string m_PlayerPrefsVolumeKey = "Volume";
    private bool m_JoinValidIP = false;
    private bool m_JoinValidName = false;
    
    // UI Part
    void Start()
    {
        SetupInputField();
        SetupIPField();
        SetupVolumeField();
    }

    private void SetupInputField()
    {
        if(!PlayerPrefs.HasKey(m_PlayerPrefsNameKey)){
            return;
        }

        string name = PlayerPrefs.GetString(m_PlayerPrefsNameKey);
        m_NameInput.text = name;
        SetPlayerName(name);
    }

    public void SetPlayerName(string name)
    {
        bool validity = !string.IsNullOrWhiteSpace(name);
        m_HostButton.interactable = validity;
        m_JoinValidName = validity;
        m_JoinButton.interactable = m_JoinValidIP && m_JoinValidName;
    }

    public void SavePlayerName()
    {
        m_DisplayName = m_NameInput.text;
        PlayerPrefs.SetString(m_PlayerPrefsNameKey, m_DisplayName);
    }

    private void SetupIPField()
    {
        if(!PlayerPrefs.HasKey(m_PlayerPrefsIPKey)){
            return;
        }

        string ip = PlayerPrefs.GetString(m_PlayerPrefsIPKey);
        m_IPInput.text = ip;
        SetIP(ip);
    }

    public void SetIP(string ip)
    {
        bool validity = !string.IsNullOrWhiteSpace(ip);
        m_JoinValidIP = validity;
        m_JoinButton.interactable = m_JoinValidIP && m_JoinValidName;
    }

    public void SaveIP()
    {
        m_IP = m_IPInput.text;
        PlayerPrefs.SetString(m_PlayerPrefsIPKey, m_IP);
    }

    void SetupVolumeField()
    {
        if(!PlayerPrefs.HasKey(m_PlayerPrefsVolumeKey)){
            return;
        }

        float value = PlayerPrefs.GetFloat(m_PlayerPrefsVolumeKey);
        m_VolumeControl.value = value;
    }

    public void SaveVolume(){
        m_Volume = m_VolumeControl.value;
        PlayerPrefs.SetFloat(m_PlayerPrefsVolumeKey, m_Volume);
    }

    // Network Part

    public void HostLobby(){
        m_NetworkManager.StartHost();
    }

    private void OnEnable(){
        LobbyNetworkManager.OnClientConnected += HandleClientConnected;
        LobbyNetworkManager.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable(){
        LobbyNetworkManager.OnClientConnected -= HandleClientConnected;
        LobbyNetworkManager.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby(){
        string ip = m_IP;
        m_NetworkManager.StartClient();
        m_JoinButton.interactable = false;
        Debug.Log("connecting");
    }

    private void HandleClientConnected(){
        m_JoinButton.interactable = false;
        m_MainMenuCanvas.SetActive(false);
        Debug.Log("connected");
        gameObject.SetActive(false);
    }

    private void HandleClientDisconnected(){
        m_JoinButton.interactable = true;
        Debug.Log("disconnected");
    }

    
}
