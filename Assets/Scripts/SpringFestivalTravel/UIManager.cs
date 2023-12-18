using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI speedText; //�ٶ�UI
    public TextMeshProUGUI distanceText; //·��UI
    private float scaledSpeed = 100;
    void Update()
    {
        ShowSpeed();
        ShowDistance();
    }

    private void ShowSpeed()
    {
        float speed = PlayerController._instance.speed / PlayerController._instance.scaleSpeed;
        speedText.text = speed.ToString() + " m/s"; // ���ٶȸ�ʽ��Ϊ�ַ���������ӵ�λ
    }
    private void ShowDistance()
    {
        float distance = PlayerController._instance.Distance;
        distanceText.text = distance.ToString("F1") + " m"; // ���ٶȸ�ʽ��Ϊ�ַ���������ӵ�λ
    }
}
