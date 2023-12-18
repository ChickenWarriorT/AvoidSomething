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
    private Vector2 inputDirection;
    public float speed;
    public float scaleSpeed = 100;
    private float distance;
    public float Distance { get => distance; }

    private void Awake()
    {
        _instance = this;
        inputControler = new PlayerInputController();
        rb = GetComponent<Rigidbody2D>();
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
        //speed += 1f;
        distance += speed / scaleSpeed * Time.deltaTime;
    }

    private void Move()
    {
        rb.velocity = new Vector2(inputDirection.x * speed * Time.deltaTime, 0);
    }
}
