using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;
using System;

public class NetworkRoomPlayerLobby : NetworkBehaviour
{
  [Header("UI")]
  [SerializeField] private GameObject m_LobbyUI = null;
  [SerializeField] private Button m_StartGameButton = null;
  [SerializeField] private TMP_Dropdown m_MapSelect = null;
  [SerializeField] private TMP_Dropdown m_ModeSelect = null;
  [SerializeField] private Text[] m_PlayerNameTexts = new Text[4];

  [SyncVar(hook = nameof(HandleMapNumChanged))]
  [SerializeField] private int m_mapNum = 1;

  [SyncVar(hook = nameof(HandleGameNumChanged))]
  [SerializeField] private int m_gameNum = 1;
  private const string m_PlayerPrefsMapNumKey = "MapNum";
  private const string m_PlayerPrefsGameNumKey = "GameNum";

  [SyncVar(hook = nameof(HandleDisplayNameChanged))]
  public string m_DisplayName = "Loading ...";

  private bool m_LocalIsLeader;
  public bool m_IsLeader
  {
    set
    {
      m_LocalIsLeader = value;
      m_StartGameButton.gameObject.SetActive(value);
      m_MapSelect.gameObject.SetActive(value);
      m_ModeSelect.gameObject.SetActive(value);
    }
  }

  private LobbyNetworkManager m_Room;
  private LobbyNetworkManager Room
  {
    get
    {
      if(m_Room != null){ return m_Room; }
      return m_Room = NetworkManager.singleton as LobbyNetworkManager;
    }
  }

  [Command]
  private void CmdSetDisplayName(string displayName)
  {
    m_DisplayName = displayName;
  }

  [Command]
  public void CmdStartGame()
  {
    if(Room.m_RoomPlayers[0].connectionToClient != connectionToClient) { return; }
    Room.StartGame();
  }

  public void StartGame()
  {
    PlayerPrefs.SetInt(m_PlayerPrefsGameNumKey, m_gameNum);
    PlayerPrefs.SetInt(m_PlayerPrefsMapNumKey, m_mapNum);
    CmdStartGame();
  }

  public override void OnStartAuthority()
  {
    CmdSetDisplayName(MainLobbyControl.m_DisplayName);
    m_LobbyUI.SetActive(true);
  }

  public override void OnStartClient()
  {
    Room.m_RoomPlayers.Add(this);
    UpdateDisplay();
    Room.NotifyPlayersOfReadyState();
    if(m_LocalIsLeader)
    {
      InvokeRepeating(nameof(BroadcastConfig), 0, 0.25f);
    }
  }

  [Command]
  private void BroadcastConfig()
  {
    BroadcastConfigRpcRpc(m_mapNum, m_gameNum);
  }

  [ClientRpc]
  private void BroadcastConfigRpcRpc(int MapNum, int GameNum)
  {
    m_mapNum = MapNum;
    PlayerPrefs.SetInt(m_PlayerPrefsMapNumKey, MapNum);
    m_gameNum = GameNum;
    PlayerPrefs.SetInt(m_PlayerPrefsGameNumKey, GameNum);
  }


  public override void OnStopClient()
  {
    Room.m_RoomPlayers.Remove(this);
    UpdateDisplay();
  }

  public void MapSelection(Int32 val)
  {
    m_mapNum = val+1;
  }

  public void GameModeSelection(Int32 val)
  {
    m_gameNum = val+1;
  }

  private void UpdateDisplay()
  {
    if(!hasAuthority)
    {
      foreach(var player in Room.m_RoomPlayers)
      {
        if(player.hasAuthority)
        {
          player.UpdateDisplay();
          break;
        }
      }
      return;
    }

    for(int i=0; i<m_PlayerNameTexts.Length; i++)
    {
      m_PlayerNameTexts[i].text = "Waiting for player...";
    }

    for(int i=0; i<Room.m_RoomPlayers.Count; i++)
    {
      m_PlayerNameTexts[i].text = Room.m_RoomPlayers[i].m_DisplayName;
    }

  }

  public void HandleReadyToStart(bool ready)
  {
    if(!m_LocalIsLeader){ return; }
    m_StartGameButton.interactable = ready;
  }

  public void HandleDisplayNameChanged(string oldValue, string newValue)
  {
    UpdateDisplay();
  }

  public void HandleMapNumChanged(int oldValue, int newValue)
  {
    PlayerPrefs.SetInt(m_PlayerPrefsMapNumKey, m_mapNum);
  }

  public void HandleGameNumChanged(int oldValue, int newValue)
  {
    PlayerPrefs.SetInt(m_PlayerPrefsGameNumKey, m_mapNum);
  }
}
