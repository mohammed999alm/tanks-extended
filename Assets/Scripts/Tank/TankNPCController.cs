using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TankNPCController : NetworkBehaviour
{
    private TankCash m_TankCash;
    public GameObject m_Robot;
    public GameObject m_CoinDrone;
    public int m_RobotCost = 5;
    public int m_CoinDroneCost = 5;

    public AudioSource m_ErrorAudio;
    private const string m_SpawnButton1 = "Spawn1";
    private const string m_SpawnButton2 = "Spawn2";
    private bool m_CanSpawn1 = true;
    private bool m_CanSpawn2 = true;
    void Awake()
    {
        m_TankCash = gameObject.GetComponent<TankCash>();
    }

    IEnumerator ResetSpawn1Cooldown() {
        yield return new WaitForSeconds(1f);
        m_CanSpawn1 = true;
    }

    IEnumerator ResetSpawn2Cooldown() {
        yield return new WaitForSeconds(1f);
        m_CanSpawn2 = true;
    }

    private void TryToSpawn1(){
        if(m_CanSpawn1){
            if(m_TankCash.m_CurrentCash < m_RobotCost)
            {
                m_ErrorAudio.Play();
            }
            else
            {
                m_TankCash.AddCash(-m_RobotCost, false);
                SpawnRobotServer();
            }
            m_CanSpawn1 = false;
            StartCoroutine(ResetSpawn1Cooldown());
        }
    }

    private void TryToSpawn2(){
        if(m_CanSpawn2){
            if(m_TankCash.m_CurrentCash < m_CoinDroneCost)
            {
                m_ErrorAudio.Play();
            }
            else
            {
                m_TankCash.AddCash(-m_CoinDroneCost, false);
                SpawnCoinDroneServer();
            }
            m_CanSpawn2 = false;
            StartCoroutine(ResetSpawn2Cooldown());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isLocalPlayer)
        {
            if (Input.GetButton(m_SpawnButton1))
            {
                TryToSpawn1();
            }
            if (Input.GetButton(m_SpawnButton2))
            {
                TryToSpawn2();
            }
        }
    }

    [Command]
    private void SpawnRobotServer(){
        GameObject robot = Instantiate(m_Robot, transform.position + transform.forward, transform.rotation);
        robot.GetComponent<RobotBehaviour>().ExcludeSpawner(gameObject);
        NetworkServer.Spawn(robot);
        SpawnRobotRPC(robot);
    }

    [ClientRpc]
    private void SpawnRobotRPC(GameObject robot){
        robot.GetComponent<RobotBehaviour>().ExcludeSpawner(gameObject);
        robot.GetComponent<NPCCash>().SetSpawner(gameObject.GetComponent<TankCash>());
    }

    [Command]
    private void SpawnCoinDroneServer(){
        GameObject drone = Instantiate(m_CoinDrone, transform.position + transform.forward, transform.rotation);
        NetworkServer.Spawn(drone);
        drone.GetComponent<CoinDroneBehaviour>().SetSpawner(gameObject);
        SpawnCoinDroneRPC(drone);
    }

    [ClientRpc]
    private void SpawnCoinDroneRPC(GameObject drone){
        drone.GetComponent<CoinDroneBehaviour>().SetSpawner(gameObject);
        drone.GetComponent<NPCCash>().SetSpawner(gameObject.GetComponent<TankCash>());
    }
}
