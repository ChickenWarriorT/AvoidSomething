using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public static TrafficManager _instance;
    public GameObject[] carPrefabs; // 车辆预制体
    public Vector3[,] gridPositions; // 二维数组来存储格子的中心位置
    [Header("行数")]
    public int rows = 4; // 行数
    [Header("列数")]
    public int columns = 3; // 列数

    private List<GameObject> cars = new List<GameObject>(); // 活动车辆列表
    [Header("格子宽度")]
    [SerializeField]
    private float cellWidth;
    [Header("格子高度")]
    [SerializeField]
    private float cellHeight;

    private float screenHeight;
    private float screenWidth;
    public float CellWidth { get => cellWidth; }
    public float CellHeight { get => cellHeight; }

    void Awake()
    {
        _instance = this;

        // 初始化格子位置数组
        InitializeGridPositions();
    }
    private void Start()
    {

    }

    private void InitializeGridPositions()
    {
        screenHeight = Camera.main.orthographicSize * 2f;
        screenWidth = screenHeight * Camera.main.aspect;

        cellWidth = screenWidth / columns;

        // 根据相机视野和行高计算行数
        int calculatedRows = Mathf.CeilToInt(screenHeight / cellHeight);

        gridPositions = new Vector3[calculatedRows, columns];

        Vector3 bottomLeft = new Vector3(-screenWidth / 2, -screenHeight / 2, 0);

        for (int i = 0; i < calculatedRows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                float xPosition = bottomLeft.x + cellWidth * j + cellWidth / 2;
                float yPosition = bottomLeft.y + cellHeight * i + cellHeight / 2;
                gridPositions[i, j] = new Vector3(xPosition, yPosition, 0);
            }
        }
    }



    void Update()
    {
        // 检测玩家的输入
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCars();
            SpawnCar();
        }
    }

    private void MoveCars()
    {
        // 向下移动所有车辆
        for (int i = cars.Count - 1; i >= 0; i--)
        {
            cars[i].transform.Translate(0, -cellHeight, 0, Space.World);
            // 如果车辆移动到屏幕底部外，则删除它
            if (cars[i].transform.position.y < -Camera.main.orthographicSize - cellHeight)
            {
                Destroy(cars[i]);
                cars.RemoveAt(i);
            }
        }
    }

    private void SpawnCar()
    {
        // 随机选择一个列来生成车辆
        int column = Random.Range(0, columns);
        // 计算车辆的生成位置
        Vector3 spawnPosition = new Vector3(column * cellWidth - Camera.main.aspect * Camera.main.orthographicSize + cellWidth / 2, Camera.main.orthographicSize + cellHeight / 2, 0);

        // 实例化车辆预制体
        GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], spawnPosition, Quaternion.identity);

        SpriteUtils.AdjustSizeToFitCell(car, cellWidth, cellHeight);

        cars.Add(car);
    }



#if UNITY_EDITOR
    // 在Unity编辑器的Scene视图中绘制格子
    void OnDrawGizmos()
    {
        // 重新计算屏幕宽度和高度以适应实际的行数和列数
        float totalGridHeight = cellHeight * rows;
        float totalGridWidth = cellWidth * columns;

        // 网格的左下角起始点
        Vector3 bottomLeft = new Vector3(-totalGridWidth / 2, -totalGridHeight / 2, 0);

        Gizmos.color = Color.green;

        // 绘制行
        for (int i = 0; i <= rows; i++)
        {
            Vector3 startPos = bottomLeft + new Vector3(0, cellHeight * i, 0);
            Vector3 endPos = startPos + new Vector3(totalGridWidth, 0, 0);
            Gizmos.DrawLine(startPos, endPos);
        }

        // 绘制列
        for (int j = 0; j <= columns; j++)
        {
            Vector3 startPos = bottomLeft + new Vector3(cellWidth * j, 0, 0);
            Vector3 endPos = startPos + new Vector3(0, totalGridHeight, 0);
            Gizmos.DrawLine(startPos, endPos);
        }

        if (gridPositions == null)
        {
            InitializeGridPositions();
        }

        // 绘制行列编号
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 8;
        labelStyle.normal.textColor = Color.white;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 labelPosition = new Vector3(-totalGridWidth / 2 + cellWidth * j, -totalGridHeight / 2 + cellHeight * i, 0);
                labelPosition.x -= 0.5f;
                Handles.Label(labelPosition, $"{i} C{j}", labelStyle);
            }
        }
    }

#endif
}
