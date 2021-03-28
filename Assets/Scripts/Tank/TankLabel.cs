using Mirror;
using UnityEngine;

public class TankLabel : NetworkBehaviour
{
    [SyncVar]
    public string m_PlayerName;
    private GameObject m_Label;

    private TextMesh m_Text;
    private Rigidbody m_Body;
    public void OnGameStart()
    {
        m_Body = gameObject.GetComponent<Rigidbody>();
        m_Label = new GameObject("label");   
        m_Text = m_Label.AddComponent<TextMesh>();
        m_Text.text = m_PlayerName;
        m_Text.color = new Color(1.0f, 1.0f, 1.0f);
        m_Text.fontStyle = FontStyle.Bold;
        m_Text.alignment = TextAlignment.Center;
        m_Text.anchor = TextAnchor.MiddleCenter;
        m_Text.characterSize = 0.090f;
        m_Text.fontSize = 60;
    }

    // Update is called once per frame
    void Update()
    {
        if(m_Text)
        {
            m_Text.text = m_PlayerName;
        }
        if(m_Label)
        {
            m_Label.transform.rotation = Camera.main.transform.rotation;
            m_Label.transform.position = m_Body.position + Vector3.up * 3f;  
        }
    }

    public void DestroyName(){
        m_Label.SetActive(false);
    }
}
