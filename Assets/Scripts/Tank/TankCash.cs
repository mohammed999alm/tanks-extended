using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class TankCash : NetworkBehaviour
{
    public int m_StartingCash = 0;
    [SyncVar(hook = nameof(OnCashChange))] 
    public int m_CurrentCash;
    public Text m_CashUI;
    public AudioSource m_CoinSound;
    private MultiplayerGameManager m_GameManager;

    private const string m_InitString = "CASH: $";

    public void SetGameManager()
    {
        m_GameManager = GameObject.FindWithTag("GameController").GetComponent<MultiplayerGameManager>();
    }

    void Awake()
    {
        m_CurrentCash = m_StartingCash;
    }

    public void SetCashUI()
    {
        if(isLocalPlayer)
        {
            GameObject cashUI = GameObject.FindGameObjectWithTag("CashUI");
            m_CashUI = cashUI.GetComponent<Text>();
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        if(m_CashUI)
        {
            m_CashUI.text = m_InitString + m_CurrentCash;
        }
    }

    [Command]
    private void AddCashServer(int val)
    {
        m_CurrentCash += val;
    }

    public void AddCash(int val, bool playSound)
    {
        if(isLocalPlayer)
        {
            AddCashServer(val);
            if(playSound)
            {
                m_CoinSound.Play();
            }
        }
    }

    void OnCashChange(int oldVal, int newVal){
        UpdateUI();
    }

    public void HandleMoneyRushEnd()
    {
        if(isLocalPlayer)
        {
            bool win = m_GameManager.IsMoneyRushWinner(gameObject);
            if(win)
            {
                m_GameManager.OnPlayerWin();
            }
            else
            {
                m_GameManager.OnPlayerLose();
            }
        }
        gameObject.GetComponent<TankLabel>().DestroyName();
        gameObject.GetComponent<TankShooting>().enabled = false;
        gameObject.GetComponent<TankMovement>().enabled = false;
        gameObject.SetActive(false);
    }
}
