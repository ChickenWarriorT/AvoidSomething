using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI speedText; //速度UI
    public TextMeshProUGUI distanceText; //路程UI
    private float scaledSpeed = 100;
    void Update()
    {
        ShowSpeed();
        ShowDistance();
    }

    private void ShowSpeed()
    {
        float speed = PlayerController._instance.speed / PlayerController._instance.scaleSpeed;
        speedText.text = speed.ToString() + " m/s"; // 将速度格式化为字符串，并添加单位
    }
    private void ShowDistance()
    {
        float distance = PlayerController._instance.Distance;
        distanceText.text = distance.ToString("F1") + " m"; // 将速度格式化为字符串，并添加单位
    }
}
