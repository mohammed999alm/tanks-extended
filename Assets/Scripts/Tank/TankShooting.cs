using Mirror;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class TankShooting : NetworkBehaviour
{
    public GameObject m_Shell;            
    public Transform m_FireTransform;    
    public Slider m_AimSlider;           
    public AudioSource m_ShootingAudio;  
    public AudioClip m_ChargingClip;     
    public AudioClip m_FireClip;         
    public float m_MinLaunchForce = 15f; 
    public float m_MaxLaunchForce = 30f; 
    public float m_MaxChargeTime = 0.75f;
    public int m_ShootTimes = 1;
    public float m_ShootInterval = 0.1f;
    public int m_ShootCost = 0;
    public AudioClip m_ErrorClip;  
    public TankCash m_TankCash;       

    public string m_FireButton = "Fire";         
    private float m_CurrentLaunchForce;  
    private float m_ChargeSpeed;         
    private bool m_Fired;   
    private bool m_CanShoot = true;   

    void Awake()
    {
        m_TankCash = gameObject.GetComponent<TankCash>();
    }

    private void OnEnable()
    {
        m_CurrentLaunchForce = m_MinLaunchForce;
        m_AimSlider.value = m_MinLaunchForce;
    }


    private void Start()
    {
        m_ChargeSpeed = (m_MaxLaunchForce - m_MinLaunchForce) / m_MaxChargeTime;
    }  

    private void Update()
    {
        // Track the current state of the fire button and make decisions based on the current launch force.

        if(isLocalPlayer){
            m_AimSlider.value = m_MinLaunchForce;

            if (m_CurrentLaunchForce >= m_MaxLaunchForce && !m_Fired) {
                m_CurrentLaunchForce = m_MaxLaunchForce;
                TryToFire();
            }
            else if (Input.GetButtonDown(m_FireButton)) {
                m_Fired = false;
                m_CurrentLaunchForce = m_MinLaunchForce;

                m_ShootingAudio.clip = m_ChargingClip;
                m_ShootingAudio.Play();
            }
            else if (Input.GetButton(m_FireButton) && !m_Fired) {
                m_CurrentLaunchForce += m_ChargeSpeed * Time.deltaTime;
                m_AimSlider.value = m_CurrentLaunchForce;
            }
            else if (Input.GetButtonUp(m_FireButton) && !m_Fired) {
                TryToFire();
            }
        }

    }

    [Command]
    private void FireServer(){
        FireRPC();
    }

    [ClientRpc]
    private void FireRPC(){
        GameObject shellInstance = Instantiate(m_Shell, m_FireTransform.position, m_FireTransform.rotation);
        shellInstance.GetComponent<Rigidbody>().velocity = m_CurrentLaunchForce * m_FireTransform.forward;
    }

    private void FireLocal(){
        if(isLocalPlayer){
            // Instantiate and launch the shell.
            m_Fired = true;

            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();

            m_CurrentLaunchForce = m_MinLaunchForce; 
        }
    }

    IEnumerator ResetFireCooldown() {
        yield return new WaitForSeconds(0.15f);
        m_CanShoot = true;
    }

    private void TryToFire(){
        m_Fired = true;
        if(m_CanShoot){
            if(m_TankCash.m_CurrentCash < m_ShootCost)
            {
                m_ShootingAudio.clip = m_ErrorClip;
                m_ShootingAudio.Play();
            }
            else
            {
                m_TankCash.AddCash(-m_ShootCost, false);
                StartCoroutine(InvokeMethod(Fire, m_ShootInterval, m_ShootTimes));
            }
            m_CanShoot = false;
            StartCoroutine(ResetFireCooldown());
        }
    }
    public IEnumerator InvokeMethod(Action method, float interval, int invokeCount)
    {
        for (int i = 0; i < invokeCount; i++)
        {
            method();
            yield return new WaitForSeconds(m_ShootInterval);
        }
    }

    private void Fire()
    {
        FireLocal();
        FireServer();
    }
}