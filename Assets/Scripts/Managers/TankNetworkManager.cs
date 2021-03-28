using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class TankNetworkManager : NetworkManager
{
  public override void OnStartServer()
  {
    Debug.Log("server up");
  }

  public override void OnStopServer()
  {
    Debug.Log("server down");
  }

  public override void OnClientConnect(NetworkConnection conn)
  {
    Debug.Log("client conn");
  }

  
}
