using Mirror;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyNetworkManager : NetworkManager
{
    [SerializeField] private int m_MinPlayers = 2;
    [Scene] [SerializeField] private string m_MenuScene = string.Empty;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayerLobby m_GamePlayerPrefab = null;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayerLobby m_RoomPlayerPrefab = null;
    
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;

    public List<NetworkRoomPlayerLobby> m_RoomPlayers { get; } = new List<NetworkRoomPlayerLobby>();
    public List<NetworkGamePlayerLobby> m_GamePlayers { get; } = new List<NetworkGamePlayerLobby>();

  public override void OnStartServer()
  {
      spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
  }

  public override void OnStartClient()
  {
      var spawnablePrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs");

      foreach(var prefab in spawnablePrefabs)
      {
        ClientScene.RegisterPrefab(prefab);
      }
  }

  public override void OnClientConnect(NetworkConnection conn)
  {
    base.OnClientConnect(conn);
    OnClientConnected?.Invoke();
  }

  public override void OnClientDisconnect(NetworkConnection conn)
  {
    base.OnClientDisconnect(conn);
    OnClientDisconnected?.Invoke();
  }

  public override void OnServerConnect(NetworkConnection conn)
  {
    if(numPlayers >= maxConnections){
        conn.Disconnect();
        return;
    }
    if(SceneManager.GetActiveScene().path != m_MenuScene){
        conn.Disconnect();
        return;
    }
  }

  public override void OnServerAddPlayer(NetworkConnection conn)
  {
    if(SceneManager.GetActiveScene().path == m_MenuScene){
      bool isLeader = m_RoomPlayers.Count == 0;
      NetworkRoomPlayerLobby roomPlayerInstance = Instantiate(m_RoomPlayerPrefab);
      roomPlayerInstance.m_IsLeader = isLeader;
      NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
    }
  }

  public override void OnServerDisconnect(NetworkConnection conn)
  {
    if(conn.identity != null){
      var player = conn.identity.GetComponent<NetworkRoomPlayerLobby>();
      m_RoomPlayers.Remove(player);
      NotifyPlayersOfReadyState();
    }
    base.OnServerDisconnect(conn);
  }

  public override void OnStopServer()
  {
    m_RoomPlayers.Clear();
  }

  public void NotifyPlayersOfReadyState()
  {
    foreach(var player in m_RoomPlayers)
    {
      player.HandleReadyToStart(IsReadyToStart());
    }
  }

  private bool IsReadyToStart()
  {
    return numPlayers >= m_MinPlayers;
  }

  public void StartGame(){
    if(SceneManager.GetActiveScene().path == m_MenuScene){
      if(IsReadyToStart())
      {
        ServerChangeScene("Game");
      }
    }
  }

  public override void ServerChangeScene(string mapName)
  {
    for(int i=m_RoomPlayers.Count-1; i>=0; i--)
    {
      var conn = m_RoomPlayers[i].connectionToClient;
      var gameplayerInstance = Instantiate(m_GamePlayerPrefab);
      gameplayerInstance.SetDisplayName(m_RoomPlayers[i].m_DisplayName);
      NetworkServer.Destroy(conn.identity.gameObject);
      NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject, true);
    }
    base.ServerChangeScene(mapName);
  }
}
