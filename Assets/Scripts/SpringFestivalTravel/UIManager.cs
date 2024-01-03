using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI speedText; //�ٶ�UI
    public TextMeshProUGUI distanceText; //·��UI

    public GameObject gameOverUI;
    public TextMeshProUGUI endSpeedText; //�ٶ�UI
    public TextMeshProUGUI endDistanceText; //·��UI
    public Button btn_Restart;//���¿�ʼ��Ϸ
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
        endDistanceText.text = "�ٶ�: " + speed.ToString("F1") + "m/s";
        endSpeedText.text = "��ʻ����: " + distance.ToString("F1") + "m";        
    }

    private void ShowSpeed()
    {
        float speed = PlayerController._instance.GetSpeedByScaled;
        speedText.text = speed.ToString("F1") + " m/s"; // ���ٶȸ�ʽ��Ϊ�ַ���������ӵ�λ
    }
    private void ShowDistance()
    {
        float distance = PlayerController._instance.Distance;
        distanceText.text = distance.ToString("F1") + " m"; // ���ٶȸ�ʽ��Ϊ�ַ���������ӵ�λ
    }

}
