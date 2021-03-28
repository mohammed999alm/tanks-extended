using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneStartManager : MonoBehaviour
{
    [SerializeField]
    private GameObject m_LevelArt1 = null;

    [SerializeField]
    private GameObject m_LevelArt2 = null;

    [SerializeField]
    private Text m_ModeText = null;

    [SerializeField]
    private Text m_InstructionText = null;
    private int defaultMapNum = 1;
    private int defaultGameNum = 1;
    private const string m_PlayerPrefsMapNumKey = "MapNum";
    private const string m_PlayerPrefsGameNumKey = "GameNum";
    public MultiplayerGameManager m_GameManager;

    void Start()
    {
        int mapNum = PlayerPrefs.GetInt(m_PlayerPrefsMapNumKey, defaultMapNum);
        int gameNum = PlayerPrefs.GetInt(m_PlayerPrefsGameNumKey, defaultGameNum);

        if(mapNum==1)
        {
            m_LevelArt1.SetActive(true);
            m_LevelArt2.SetActive(false);
        }
        else
        {
            m_LevelArt1.SetActive(false);
            m_LevelArt2.SetActive(true);
        }

        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject tank in tanks)
        {
            tank.GetComponent<TankMovement>().SetCamera();
            tank.GetComponent<TankHealth>().SetGameManager();
            tank.GetComponent<TankLabel>().OnGameStart();
            tank.GetComponent<TankCash>().SetCashUI();
            tank.GetComponent<TankCash>().SetGameManager();
        }

        if(gameNum==1)
        {
            m_ModeText.text = "BATTLE ROYALE: Be the Last Survivor!";
            m_InstructionText.text = "MOVE: WASD,  FIRE: [SPACE]\nSPECIAL ($2): P,  ULTIMATE ($10): O,  ROBOT ($5): K,  COIN DRONE ($3): L";
        }
        else
        {
            m_ModeText.text = "MONEY RUSH: Collect the Most Money in 60s!";
            m_InstructionText.text = "MOVE: WASD,  FIRE: [SPACE]\nYOU CAN'T DAMAGE YOUR ENEMIES!";

            foreach (GameObject tank in tanks)
            {
                tank.GetComponent<TankHealth>().m_MoneyMode = true;
                tank.GetComponent<TankNPCController>().enabled = false;
                var shootings = tank.GetComponents<TankShooting>();
                
                foreach(var shooting in shootings)
                {
                    if(shooting.m_FireButton != "Fire")
                    {
                        shooting.enabled = false;
                    }
                }
            }

            m_GameManager.StartMoneyRush();
        }
    }
}
