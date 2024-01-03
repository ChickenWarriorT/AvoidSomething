using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager _instance;
    //游戏结束委托
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
            Time.timeScale = 0; // 停止游戏时间
        }
    }
    public void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Time.timeScale = 1; // 重置游戏时间
    }

}
