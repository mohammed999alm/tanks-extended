using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    AudioSource m_bgm = null;

    private const string m_PlayerPrefsVolumeKey = "Volume";
    void Start()
    {
        if(!m_bgm || !PlayerPrefs.HasKey(m_PlayerPrefsVolumeKey)){
            return;
        }
        m_bgm.volume = PlayerPrefs.GetFloat(m_PlayerPrefsVolumeKey)/100f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
