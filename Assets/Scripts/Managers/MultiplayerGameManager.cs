using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class MultiplayerGameManager : MonoBehaviour
{
    public Text m_MessageText;
    public Text m_TimerText;
    private List<GameObject> m_Players = new List<GameObject>();

    void Start()
    {
        m_MessageText.text = string.Empty;
        GameObject[] tanks = GameObject.FindGameObjectsWithTag("Player");
        foreach(var tank in tanks)
        {
            m_Players.Add(tank);
        }
    }

    public int GetNumAlivePlayers()
    {
        int total = 0;
        foreach(var tank in m_Players) {
            if(tank.activeInHierarchy && !tank.GetComponent<TankHealth>().m_Dead)
            {
                total += 1;
            }
        }
        return total;
    }

    public void OnPlayerWin()
    {
        m_MessageText.text = "You Win!";
        StartCoroutine(ShowCredits());
    }

    public void OnPlayerLose()
    {
        m_MessageText.text = "You Lose!";
        StartCoroutine(ShowCredits());
    }

    private void DestroyAllTanks()
    {
        foreach(var tank in m_Players) {
            if(tank.activeInHierarchy)
            {
                tank.GetComponent<TankLabel>().DestroyName();
                tank.GetComponent<TankShooting>().enabled = false;
                tank.GetComponent<TankMovement>().enabled = false;
                tank.SetActive(false);
            }
        }
    }

    IEnumerator ShowCredits()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Credits", LoadSceneMode.Additive);
        // DestroyAllTanks();
    }

    IEnumerator TimerCountdown()
    {
        for(int i=60; i>=0; i--)
        {
            m_TimerText.text = $"{i}";
            yield return new WaitForSeconds(1f);
        }

        foreach(var tank in m_Players) {
            if(tank.activeInHierarchy)
            {
                tank.GetComponent<TankCash>().HandleMoneyRushEnd();
                tank.GetComponent<TankLabel>().DestroyName();
                tank.GetComponent<TankShooting>().enabled = false;
                tank.GetComponent<TankMovement>().enabled = false;
                tank.SetActive(false);
            }
        }

    }

    public void StartMoneyRush()
    {
        Debug.Log("Started");
        StartCoroutine(TimerCountdown());
    }

    public bool IsMoneyRushWinner(GameObject player)
    {
        int maxCash = 0;
        foreach(var tank in m_Players) {
            var tankCash = tank.GetComponent<TankCash>();
            maxCash = Math.Max(maxCash, tankCash.m_CurrentCash);
        }

        var playerCash = player.GetComponent<TankCash>();
        return playerCash.m_CurrentCash == maxCash;
    }
}
