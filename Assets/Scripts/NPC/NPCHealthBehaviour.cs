using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NPCHealthBehaviour : NetworkBehaviour
{
    public float m_StartingHealth = 10f;
    
    [SyncVar]
    private float m_CurrentHealth;

    void Awake()
    {
        m_CurrentHealth = m_StartingHealth;
    }

    private void TakeDamageServer(float amount){
        m_CurrentHealth -= amount;
    }

    public void TakeDamage(float amount)
    {
        TakeDamageServer(amount);
    }


    void Update()
    {
        if(m_CurrentHealth <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
