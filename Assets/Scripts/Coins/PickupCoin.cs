using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PickupCoin : NetworkBehaviour
{
    public int m_CoinValue = 1;
    
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag == "Player")
        {
            var tankCollider = collider.gameObject.GetComponent<TankCash>();
            var npcCollider = collider.gameObject.GetComponent<NPCCash>();
            if(tankCollider){
                tankCollider.AddCash(m_CoinValue, true);
                gameObject.SetActive(false);
            }
            else if(npcCollider){
                npcCollider.AddSpawnerCash(m_CoinValue);
                gameObject.SetActive(false);
            }
        }
    }
}
