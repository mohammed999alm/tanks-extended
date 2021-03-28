using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class TankHealth : NetworkBehaviour
{
    public float m_StartingHealth = 100f;          
    public Slider m_Slider;                        
    public Image m_FillImage;                      
    public Color m_FullHealthColor = Color.green;  
    public Color m_ZeroHealthColor = Color.red;    
    public GameObject m_ExplosionPrefab;
    private MultiplayerGameManager m_GameManager;

    public bool m_MoneyMode = false;
    
    private AudioSource m_ExplosionAudio;          
    private ParticleSystem m_ExplosionParticles;  

    [SyncVar(hook = nameof(OnHealthChange))] 
    private float m_CurrentHealth;

    [SyncVar]
    public bool m_Dead;

    public void SetGameManager()
    {
        m_GameManager = GameObject.FindWithTag("GameController").GetComponent<MultiplayerGameManager>();
    }

    private void OnEnable()
    {
        m_CurrentHealth = m_StartingHealth;
        m_Dead = false;

        SetHealthUI();
    }

    [Command]
    private void TakeDamageServer(float amount){
        m_CurrentHealth -= amount;
    }

    public void TakeDamage(float amount)
    {
        if(m_MoneyMode) return;
        if(isLocalPlayer){
            TakeDamageServer(amount);
        }
    }


    private void SetHealthUI()
    {
        // Adjust the value and colour of the slider.
        m_Slider.value = m_CurrentHealth;
        m_FillImage.color = Color.Lerp(m_ZeroHealthColor, m_FullHealthColor, m_CurrentHealth / m_StartingHealth);
    }

    void OnHealthChange(float oldHealth, float newHealth){
        SetHealthUI();
        if (m_CurrentHealth <= 0 && !m_Dead) {
            OnDeath();
        }
    }

    [ClientRpc]
    private void ClientOnDeath(){
        m_ExplosionParticles = Instantiate(m_ExplosionPrefab).GetComponent<ParticleSystem>();
        m_ExplosionParticles.transform.position = transform.position;
        m_ExplosionAudio = m_ExplosionParticles.GetComponent<AudioSource>();
        m_ExplosionAudio.Play();
        m_ExplosionParticles.Play();
        if(isLocalPlayer)
        {
            m_GameManager.OnPlayerLose();
        }
        else
        {
            m_Dead = true;
            bool lastAlive = m_GameManager.GetNumAlivePlayers() == 1;
            Debug.Log(m_GameManager.GetNumAlivePlayers());
            if(lastAlive)
            {
                m_GameManager.OnPlayerWin();
            }
        }
        gameObject.GetComponent<TankLabel>().DestroyName();
        gameObject.GetComponent<TankShooting>().enabled = false;
        gameObject.GetComponent<TankMovement>().enabled = false;
        gameObject.SetActive(false);
    }

    [Command]
    private void ServerOnDeath()
    {
        ClientOnDeath();
    }

    private void OnDeath()
    {
        if(isLocalPlayer)
        {
            m_Dead = true;
            ServerOnDeath();

        }
    }
}