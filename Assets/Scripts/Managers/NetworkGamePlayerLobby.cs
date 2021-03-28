using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Mirror;

public class NetworkGamePlayerLobby : NetworkBehaviour
{
  private string m_DisplayName = "Loading ...";
  private LobbyNetworkManager m_Room;
  private LobbyNetworkManager Room
  {
    get
    {
      if(m_Room != null){ return m_Room; }
      return m_Room = NetworkManager.singleton as LobbyNetworkManager;
    }
  }

  public override void OnStartClient()
  {
    DontDestroyOnLoad(gameObject);
    Room.m_GamePlayers.Add(this);
  }


  public override void OnStopClient()
  {
    Room.m_GamePlayers.Remove(this);
  }

  [Server]
  public void SetDisplayName(string displayName)
  {
    this.m_DisplayName = displayName;
    TankLabel label = gameObject.GetComponent<TankLabel>();
    label.m_PlayerName = displayName;
  }

}
