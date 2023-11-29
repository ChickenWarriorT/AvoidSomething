using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TrafficManager : MonoBehaviour
{
    public static TrafficManager _instance;
    public GameObject gridPrefab; // 用于引用格子的Prefab
    public GameObject[] carPrefabs; // 车辆预制体
    public Vector3[,] gridPositions; // 二维数组来存储格子的中心位置
    [Header("行数")]
    public int rows = 4; // 行数
    [Header("列数")]
    public int columns = 3; // 列数
    [Header("车辆初始化不会生成的格子")]
    public int excludedRows = 5;

    private List<GameObject> cars = new List<GameObject>(); // 活动车辆列表
    private HashSet<Tuple<int,int>> occupiedCells = new HashSet<Tuple<int, int>>();//已被占用格子
    private float cellWidth;
    [Header("格子高度")]
    [SerializeField]
    private float cellHeight;

    public int numOfCarsToSpawn = 10;
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
        SpawnCars(numOfCarsToSpawn);
    }

    public void InitializeGridPositions()
    {
        if (gridPositions == null)
        {
            screenHeight = Camera.main.orthographicSize * 2f;
            screenWidth = screenHeight * Camera.main.aspect;

            cellWidth = screenWidth / columns;

            gridPositions = new Vector3[rows, columns];

            Vector3 bottomLeft = new Vector3(-screenWidth / 2, -screenHeight / 2, 0);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    float xPosition = bottomLeft.x + cellWidth * j + cellWidth / 2;
                    float yPosition = bottomLeft.y + cellHeight * i + cellHeight / 2;
                    gridPositions[i, j] = new Vector3(xPosition, yPosition, 0);
                }
            }
        }
    }


    void Update()
    {
        // 检测玩家的输入
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCars();
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

    //所有格子自定义坐标的数组
    private List<Tuple<int, int>> GetAllCellsCoordinate(int excludedRows)
    {
        List<Tuple<int, int>> allCoordinates = new List<Tuple<int, int>>();
        for (int i = excludedRows; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                allCoordinates.Add(new Tuple<int, int>(i, j));
            }
        }
        return allCoordinates;
    }

    //车辆生成
    private void SpawnCars(int numOfCars)
    {

        List<Tuple<int, int>> allCoordinates = GetAllCellsCoordinate(excludedRows);
        for (int i = 0; i < numOfCars; i++)
        {
            if (allCoordinates.Count == 0) return;

            int randomIndex = UnityEngine.Random.Range(0, allCoordinates.Count);
            Tuple<int, int> selectedCell = allCoordinates[randomIndex];

            //如果不在已占用位置中
            if (!occupiedCells.Contains(selectedCell))
            {
                //车辆随机生成的位置
                Vector3 spawnPosition = gridPositions[selectedCell.Item1, selectedCell.Item2];

                // 实例化车辆预制体
                GameObject car = Instantiate(carPrefabs[UnityEngine.Random.Range(0, carPrefabs.Length)], spawnPosition, Quaternion.identity);

                SpriteUtils.AdjustSizeToFitCell(car, cellWidth, cellHeight);

                cars.Add(car);
                occupiedCells.Add(selectedCell);
            }
        }
    }

    //清除已占用位置
    private void ClearOccupiedPositions()
    {
        occupiedCells.Clear();
    }



#if UNITY_EDITOR
    // 在Unity编辑器的Scene视图中绘制格子
    void OnDrawGizmos()
    {
        UpdateScreenAndCellSize();

        InitializeGridPositions();
        
        
        // 重新计算屏幕宽度和高度以适应实际的行数和列数
        float totalGridHeight = cellHeight * rows;
        float totalGridWidth = cellWidth * columns;

        // 网格的左下角起始点
        Vector3 bottomLeft = new Vector3(-screenWidth / 2, -screenHeight / 2, 0);

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



        // 绘制行列编号
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 8;
        labelStyle.normal.textColor = Color.white;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 labelPosition = new Vector3(bottomLeft.x + cellWidth * j + cellWidth / 2, bottomLeft.y + cellHeight * i + cellHeight / 2, 0);
                labelPosition.x -= 0.5f;
                Handles.Label(labelPosition, $"{i}，{j}", labelStyle);
            }
        }

    }
    private void UpdateScreenAndCellSize()
    {
        screenHeight = Camera.main.orthographicSize * 2f;
        screenWidth = screenHeight * Camera.main.aspect;
        cellWidth = screenWidth / columns;
    }

#endif
}
