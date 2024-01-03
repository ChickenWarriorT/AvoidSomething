using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController _instance;
    PlayerInputController inputControler;
    private Rigidbody2D rb;
    public float speedIncreaseDistance = 100f; // ÿ500��λ������ٶ�����
    public float speedIncreaseAmount = 1000f;    // ÿ������10��λ�ٶ�

    private float nextSpeedIncreaseThreshold;  // ��һ���ٶ����ӵ���ֵ

    private Vector2 inputDirection;
    public float speed;
    public float scaleSpeed = 100;
    private float distance;
    public float Distance { get => distance; }
    public float GetSpeedByScaled { get => speed / scaleSpeed; }

    private void Awake()
    {
        _instance = this;
        inputControler = new PlayerInputController();
        rb = GetComponent<Rigidbody2D>();

        nextSpeedIncreaseThreshold = speedIncreaseDistance;
    }

    private void OnEnable()
    {
        inputControler.Enable();
    }

    private void OnDisable()
    {
        inputControler.Disable();
    }

    private void Update()
    {
        inputDirection = inputControler.GamePlay.Move.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        Move();

        distance += GetSpeedByScaled * Time.deltaTime;
        CheckForSpeedIncrease();
    }

    private void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, 0);
    }
    private void CheckForSpeedIncrease()
    {
        if (distance >= nextSpeedIncreaseThreshold)
        {
            speed += speedIncreaseAmount;            // �����ٶ�
            nextSpeedIncreaseThreshold += speedIncreaseDistance; // ������һ����ֵ
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Vehicle"))  // ��ײ��������ʱ
        {
            GameManager._instance.GameEnd(distance, GetSpeedByScaled);
        }
    }

}
