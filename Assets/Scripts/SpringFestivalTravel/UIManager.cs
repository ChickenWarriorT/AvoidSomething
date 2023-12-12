using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI speedText; // �ڱ༭���н��˱���ָ������ UI Text ���

    void Update()
    {
        float speed = PlayerController._instance.speed;
        speedText.text = speed.ToString() + " m/s"; // ���ٶȸ�ʽ��Ϊ�ַ���������ӵ�λ
    }
}
