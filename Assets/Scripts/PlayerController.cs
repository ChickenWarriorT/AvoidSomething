using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // 控制车辆移动速度
    public Transform[] lanes; // 定义车道位置
    private int currentLane = 1; // 初始车道位置

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveLeft();
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveRight();
        }
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveUp();
        }
    }

    private void MoveLeft()
    {
        if (currentLane > 0)
        {
            currentLane--;
            MoveToLane(currentLane);
        }
    }

    private void MoveRight()
    {
        if (currentLane < lanes.Length - 1)
        {
            currentLane++;
            MoveToLane(currentLane);
        }
    }

    private void MoveUp()
    {

        MoveToLane(currentLane);

    }

    private void MoveToLane(int laneIndex)
    {
        Vector3 targetPosition = lanes[laneIndex].position;
        transform.position = new Vector3(targetPosition.x, transform.position.y, 0);
    }
}
