using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float cellWidth;
    private float cellHeight;

    private int currentRow = 0; // ��ʼ��λ��
    private int currentColumn = 0; // ��ʼ��λ��
    private TrafficManager trafficManager;

    void Start()
    {
        trafficManager = TrafficManager._instance;
        currentColumn = trafficManager.columns / 2;
        // ���ݸ��Ӵ�С������ҳ����ĳߴ�
        SpriteUtils.AdjustSizeToFitCell(this.gameObject, trafficManager.CellWidth, trafficManager.CellHeight);
    }

    void Update()
    {
        // �����ҵ����벢�ƶ�����
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
        // ��ȡĿ����ӵ�λ��
        Vector3 targetPosition = trafficManager.gridPositions[currentRow, currentColumn];
        //�����ƶ���Ŀ��Ϊֹ
        transform.position = targetPosition;
    }
}
