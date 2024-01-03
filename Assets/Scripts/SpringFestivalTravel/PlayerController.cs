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
    public float speedIncreaseDistance = 100f; // 每500单位距离后速度增加
    public float speedIncreaseAmount = 1000f;    // 每次增加10单位速度

    private float nextSpeedIncreaseThreshold;  // 下一个速度增加的阈值

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
            speed += speedIncreaseAmount;            // 增加速度
            nextSpeedIncreaseThreshold += speedIncreaseDistance; // 更新下一个阈值
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Vehicle"))  // 碰撞其他车辆时
        {
            GameManager._instance.GameEnd(distance, GetSpeedByScaled);
        }
    }

}
