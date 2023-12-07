using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        var playerSpeed = PlayerController._instance.speed;
        rb.velocity = Vector2.up * (speed - playerSpeed) * Time.deltaTime;
    }
}
