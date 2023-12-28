using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private float destroyYAxis;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        
    }

    private void Start()
    {
        destroyYAxis = RoadManager._instance.boundary.GetComponent<SpriteRenderer>().bounds.min.y- this.GetComponent<SpriteRenderer>().bounds.size.y;
        Debug.Log(destroyYAxis);
    }
    private void FixedUpdate()
    {
        Move();
        CheckBoundary();
    }

    private void Move()
    {
        var playerSpeed = PlayerController._instance.speed;
        rb.velocity = Vector2.up * (speed - playerSpeed) * Time.deltaTime;
    }
    private void CheckBoundary()
    {
        if (transform.position.y < destroyYAxis)
        {
            Destroy(gameObject);
        }
    }
}
