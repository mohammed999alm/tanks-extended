using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PeriodicCoinSpawner : NetworkBehaviour
{
    public GameObject m_Coin;
    public bool m_StopSpawning = false;
    public float m_SpawnTime;
    public float m_SpawnDelay;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(SpawnObjectLocal), m_SpawnTime, m_SpawnDelay);   
    }

    private Vector3 GetRandomPos()
    {
        return new Vector3(
          Random.Range(-30f, 30f),
          0f,
          Random.Range(-30f, 30f)
        );
    }

    [Command]
    public void SpawnObjectServer()
    {
        GameObject coin = Instantiate(m_Coin, GetRandomPos(), transform.rotation);
        NetworkServer.Spawn(coin);
    }
    public void SpawnObjectLocal()
    {
        if(isLocalPlayer)
        {
            SpawnObjectServer();
        }
        if (m_StopSpawning)
        {
            CancelInvoke(nameof(SpawnObjectLocal));
        }
    }
}
