using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RobotBehaviour : NetworkBehaviour
{
    public float m_TurnSpeed = 100f;
    public float m_MoveSpeed = 10f;
    public float m_LaunchForce = 15f;
    public float m_Proximity = 3f;
    public Transform m_FireTransform;    
    private Rigidbody m_Rigidbody;
    private Animator m_Animator;
    public AudioSource m_ShootingAudio;
    public AudioClip m_FireClip;
    public GameObject m_Shell;
    public GameObject m_Spawner;      
    public List<GameObject> m_Enemies = new List<GameObject>();
    
    [SyncVar]
    private bool m_CanShoot = true;

    [SyncVar]
    private bool m_Shooting = false;

    [SyncVar]
    private Vector3 m_TargetLocation = new Vector3(0f, 0f, 0f);

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
    }

    public void ExcludeSpawner(GameObject spawner)
    {
        m_Spawner = spawner;
        m_Enemies.Remove(m_Spawner);
    }

    void Start()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Player");
        foreach(var enemy in enemies)
        {
            m_Enemies.Add(enemy);
        }
        if(m_Spawner)
        {
            m_Enemies.Remove(m_Spawner);
        }
        m_Enemies.Remove(gameObject);
        InvokeRepeating(nameof(SetRandomTargetLocation), 0.0f, 5.0f);
    }

    void SetRandomTargetLocation()
    {
        m_TargetLocation = new Vector3(
            Random.Range(-30f, 30f),
            transform.position.y,
            Random.Range(-30f, 30f)
        );
    }

    void Update()
    {
        var enemy = getNearEnemy();
        if(enemy)
        {
            TryToFire();
            m_TargetLocation = enemy.transform.position;
            m_Shooting = true;
        }
        else {
            m_Shooting = false;
        }
    }

    private GameObject getNearEnemy()
    {
        foreach(var enemy in m_Enemies)
        {
            if(
                enemy.activeInHierarchy &&
                Vector3.Distance(transform.position, enemy.transform.position) < m_Proximity
            )
            {
                return enemy;
            }
        }
        return null;
    }

    void FixedUpdate()
    {
        if (Vector3.Distance(transform.position, m_TargetLocation) > 0.1f)
        {
            Turn();
            Move();
            m_Animator.SetBool("isWalking", true);
        }
        else {
            m_Animator.SetBool("isWalking", false);
        }

        m_Animator.SetBool("isShooting", m_Shooting);
    }

    private void Move()
    {
        Vector3 movement = transform.forward * m_MoveSpeed * Time.deltaTime;
        m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
    }

    private void Turn()
    {
        Vector3 targetDirection = m_TargetLocation - transform.position;
        float singleStep = m_TurnSpeed * Time.deltaTime;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, singleStep, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
    }

    private void FireLogic(){
        var position = m_FireTransform.position + transform.forward;
        position.y = 1.4f;
        GameObject shellInstance = Instantiate(m_Shell, position, m_FireTransform.rotation);
        shellInstance.GetComponent<Rigidbody>().velocity = m_LaunchForce * m_FireTransform.forward;
    }
    private void FireLocal(){
        if(isLocalPlayer){
            m_ShootingAudio.clip = m_FireClip;
            m_ShootingAudio.Play();
        }
    }

    private void Fire()
    {
        Debug.Log("Fire");
        FireLogic();
        FireLocal();
    }
    
    IEnumerator ResetFireCooldown() {
        yield return new WaitForSeconds(1f);
        m_CanShoot = true;
    }

    private void TryToFire(){
        if(m_CanShoot){
            Fire();
            m_CanShoot = false;
            StartCoroutine(ResetFireCooldown());
        }
    }


}
