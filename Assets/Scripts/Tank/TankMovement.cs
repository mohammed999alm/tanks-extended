using Mirror;
using UnityEngine;

public class TankMovement : NetworkBehaviour
{
    public int m_PlayerNumber = 1;         
    public float m_Speed = 12f;            
    public float m_TurnSpeed = 180f;       
    public AudioSource m_MovementAudio;    
    public AudioClip m_EngineIdling;       
    public AudioClip m_EngineDriving;      
    public float m_PitchRange = 0.2f;

    private string m_MovementAxisName = "Vertical";
    private string m_TurnAxisName = "Horizontal";
    private Rigidbody m_Rigidbody;
    private GameObject m_TankRenderers;         
    private GameObject m_TankCanvas;         

    private float m_MovementInputValue;    
    private float m_TurnInputValue;        
    private float m_OriginalPitch;         
    private const string m_PlayerPrefsVolumeKey = "Volume";

    private GameObject m_Camera = null;

    public void SetCamera()
    {
        if(isLocalPlayer)
        {
            GameObject[] cams = GameObject.FindGameObjectsWithTag("CameraRig");
            if(cams.Length > 0)
            {
                m_Camera = cams[0];
            }
            transform.position = new Vector3(
                Random.Range(-30.0f, 30.0f),
                0.0f,
                Random.Range(-30.0f, 30.0f)
            );
        }
        m_TankRenderers.SetActive(true);
        m_TankCanvas.SetActive(true);
    }

    private void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_TankRenderers = transform.GetChild(0).gameObject;
        m_TankCanvas = transform.GetChild(1).gameObject;
    }


    private void OnEnable ()
    {
        m_Rigidbody.isKinematic = false;
        m_MovementInputValue = 0f;
        m_TurnInputValue = 0f;
    }


    private void OnDisable ()
    {
        m_Rigidbody.isKinematic = true;
    }


    private void Start()
    {
        m_OriginalPitch = m_MovementAudio.pitch;
        if(!m_MovementAudio || !PlayerPrefs.HasKey(m_PlayerPrefsVolumeKey)){
            return;
        }
        m_MovementAudio.volume = PlayerPrefs.GetFloat(m_PlayerPrefsVolumeKey)/100f;
    }

    private void Update()
    {
        if(isLocalPlayer){
            // Store the player's input and make sure the audio for the engine is playing.
            m_MovementInputValue = Input.GetAxis(m_MovementAxisName);
            m_TurnInputValue = Input.GetAxis(m_TurnAxisName);
            if(m_Camera)
            {
                m_Camera.transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    transform.position.z
                );
            }
        }
        
        EngineAudio();
    }


    private void EngineAudio()
    {
        // Play the correct audio clip based on whether or not the tank is moving and what audio is currently playing.
        if (Mathf.Abs(m_MovementInputValue) < 0.1f && Mathf.Abs(m_TurnInputValue) < 0.1f) {
            if (m_MovementAudio.clip == m_EngineDriving) {
                m_MovementAudio.clip = m_EngineIdling;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
        else {
            if (m_MovementAudio.clip == m_EngineIdling) {
                m_MovementAudio.clip = m_EngineDriving;
                m_MovementAudio.pitch = Random.Range(m_OriginalPitch - m_PitchRange, m_OriginalPitch + m_PitchRange);
                m_MovementAudio.Play();
            }
        }
    }


    private void FixedUpdate()
    {
        // Move and turn the tank.
        Move();
        Turn();
    }

    private void Move()
    {
        // Adjust the position of the tank based on the player's input.
        Vector3 movement = transform.forward * m_MovementInputValue * m_Speed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        // Adjust the rotation of the tank based on the player's input.
        float turn = m_TurnInputValue * m_TurnSpeed * Time.deltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
    }
}