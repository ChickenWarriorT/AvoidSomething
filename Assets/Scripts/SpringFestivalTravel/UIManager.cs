using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI speedText; //速度UI
    public TextMeshProUGUI distanceText; //路程UI

    void Update()
    {
        float speed = PlayerController._instance.speed;
        speedText.text = speed.ToString() + " m/s"; // 将速度格式化为字符串，并添加单位
    }
}
