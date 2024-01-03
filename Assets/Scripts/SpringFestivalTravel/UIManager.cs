using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI speedText; //速度UI
    public TextMeshProUGUI distanceText; //路程UI

    public GameObject gameOverUI;
    public TextMeshProUGUI endSpeedText; //速度UI
    public TextMeshProUGUI endDistanceText; //路程UI
    public Button btn_Restart;//重新开始游戏
    void Update()
    {
        ShowSpeed();
        ShowDistance();
    }

    private void OnEnable()
    {
        GameManager._instance.OnGameOver += HandleGameOver;
        btn_Restart.onClick.AddListener(GameManager._instance.StartGame);
    }
    private void OnDisable()
    {
        GameManager._instance.OnGameOver -= HandleGameOver;
    }

    private void HandleGameOver(float distance, float speed)
    {
        gameOverUI.SetActive(true);
        endDistanceText.text = "速度: " + speed.ToString("F1") + "m/s";
        endSpeedText.text = "行驶距离: " + distance.ToString("F1") + "m";        
    }

    private void ShowSpeed()
    {
        float speed = PlayerController._instance.GetSpeedByScaled;
        speedText.text = speed.ToString("F1") + " m/s"; // 将速度格式化为字符串，并添加单位
    }
    private void ShowDistance()
    {
        float distance = PlayerController._instance.Distance;
        distanceText.text = distance.ToString("F1") + " m"; // 将速度格式化为字符串，并添加单位
    }

}
