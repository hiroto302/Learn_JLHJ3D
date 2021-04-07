using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEnding : MonoBehaviour
{
    public float fadeDuration = 1.0f;
    public float displayImageDuration = 1.0f;
    public GameObject player;
    public CanvasGroup exitBackgroundImageCanvasGroup;
    bool m_IsPlayerExit; //初期値 false
    float m_Timer;       //初期値 0

    void OnTriggerEnter (Collider other)
    {
        if (other.gameObject == player)
        {
            m_IsPlayerExit = true;
        }
    }
    void Update ()
    {
        if (m_IsPlayerExit)
        {
            EndLevel();
        }
    }

    void EndLevel ()
    {
        m_Timer += Time.deltaTime;
        exitBackgroundImageCanvasGroup.alpha = m_Timer / fadeDuration;
        if (m_Timer > fadeDuration + displayImageDuration)
        {
            Application.Quit();
        }
    }

}


