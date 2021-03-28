using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CoinDroneBehaviour : NetworkBehaviour
{
    public float m_TurnSpeed = 100f;
    public float m_MoveSpeed = 10f;
    public float m_CoinProximity = 5f;
    public float m_PlayerProximity = 3f;
    private Rigidbody m_Rigidbody;

    public GameObject m_Spawner;     
    private Animator m_Animator;
    
    [SyncVar]
    private Vector3 m_TargetLocation = new Vector3(0f, 0f, 0f);

    public void SetSpawner(GameObject spawner)
    {
        m_Spawner = spawner;
    }

    void Awake()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponent<Animator>();
    }

    void SetTargetLocation()
    {
        var cashObjs = GameObject.FindGameObjectsWithTag("Cash");
        if(cashObjs.Length > 0)
        {
            var minDist = 100f;
            var minV3 = new Vector3(0f, 0f, 0f);
            foreach(var cashObj in cashObjs)
            {
                if(cashObj.activeInHierarchy)
                {
                    var curDist = Vector3.Distance(transform.position, cashObj.transform.position);
                    if(curDist < minDist)
                    {
                        minDist = curDist;
                        minV3 = cashObj.transform.position;
                    }
                }
            }
            if(minDist <= m_CoinProximity)
            {
                m_TargetLocation = minV3;
                return;
            }
        }

        if(m_Spawner)
        {
            var dirVec = Vector3.Normalize(m_Spawner.transform.position - transform.position);
            m_TargetLocation = m_Spawner.transform.position - (dirVec * m_PlayerProximity);
        }
        
    }

    void Update()
    {
        SetTargetLocation();
    }
    void FixedUpdate()
    {
        bool isWalking = false;
        if(Vector3.Distance(transform.position, m_TargetLocation) > 0.5f)
        {
            isWalking = true;
            Turn();
            Move();
        }
        m_Animator.SetBool("Walk_Anim", isWalking);
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
}
