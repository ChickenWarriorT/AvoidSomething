using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    //��Ϸ����ί��
    public Action<float, float> OnGameOver;

    private void Awake()
    {
        _instance = this;
    }

    public void GameEnd(float distance,float speed)
    {
        if (OnGameOver != null)
        {
            OnGameOver?.Invoke(distance, speed);
            Time.timeScale = 0; // ֹͣ��Ϸʱ��
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1; // ������Ϸʱ��
    }

}
