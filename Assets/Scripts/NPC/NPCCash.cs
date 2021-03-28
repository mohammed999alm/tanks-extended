using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCCash : MonoBehaviour
{
    public TankCash m_SpawnerCash;     
    public void SetSpawner(TankCash spawner)
    {
        m_SpawnerCash = spawner;
    }

    public void AddSpawnerCash(int val)
    {
        m_SpawnerCash.AddCash(val, true);
    }
}
