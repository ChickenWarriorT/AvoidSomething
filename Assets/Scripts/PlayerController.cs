using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float cellWidth;
    private float cellHeight;

    private int currentRow = 0; // 初始行位置
    private int currentColumn = 0; // 初始列位置
    private TrafficManager trafficManager;

    void Start()
    {
        trafficManager = TrafficManager._instance;
        currentColumn = trafficManager.columns / 2;
        // 根据格子大小调整玩家车辆的尺寸
        SpriteUtils.AdjustSizeToFitCell(this.gameObject, trafficManager.CellWidth, trafficManager.CellHeight);
    }

    void Update()
    {
        // 检测玩家的输入并移动车辆
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
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveDown();
        }
    }

    private void MoveLeft()
    {
        if (currentColumn > 0)
        {
            currentColumn--;
            MoveToGridPosition();
        }
    }

    private void MoveRight()
    {
        if (currentColumn < trafficManager.columns - 1)
        {
            currentColumn++;
            MoveToGridPosition();
        }
    }

    private void MoveUp()
    {
        if (currentRow < trafficManager.rows - 1)
        {
            currentRow++;
            MoveToGridPosition();
        }
    }

    private void MoveDown()
    {
        if (currentRow > 0)
        {
            currentRow--;
            MoveToGridPosition();
        }
    }

    private void MoveToGridPosition()
    {
        // 获取目标格子的位置
        Vector3 targetPosition = trafficManager.gridPositions[currentRow, currentColumn];
        //立即移动到目标为止
        transform.position = targetPosition;
    }
}
