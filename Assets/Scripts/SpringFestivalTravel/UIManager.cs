using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI speedText; // 在编辑器中将此变量指向您的 UI Text 组件

    void Update()
    {
        float speed = PlayerController._instance.speed;
        speedText.text = speed.ToString() + " m/s"; // 将速度格式化为字符串，并添加单位
    }
}
