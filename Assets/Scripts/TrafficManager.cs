using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TrafficManager : MonoBehaviour
{
    public static TrafficManager _instance;
    public GameObject gridPrefab; // 用于引用格子的Prefab
    public GameObject[] carPrefabs; // 车辆预制体
    public Vector3[,] gridPositions; // 二维数组来存储格子的中心位置
    [Header("行数和列数")]
    public int rows = 4; // 行数
    public int columns = 3; // 列数

    [Header("等待时间设置")]
    public float waitTimeAfterPlayerMove = 0.5f; // 玩家移动后的等待时间
    public float waitTimeAfterCarsMove = 0.5f; // 其他车辆移动后的等待时间

    [Header("车辆初始化不会生成的格子")]
    public int excludedRows = 5;

    [SerializeField]
    private List<GameObject> cars = new List<GameObject>(); // 活动车辆列表
    public HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();//已被占用格子
    private float cellWidth;
    [Header("格子高度")]
    [SerializeField]
    private float cellHeight;

    [Header("事件")]
    public CarEventSO playerMovedEvent;

    public bool playerTurn = true;
    public int numOfCarsToSpawn = 10;
    private float screenHeight;
    private float screenWidth;
    public float CellWidth { get => cellWidth; }
    public float CellHeight { get => cellHeight; }

    public bool isRandomSpawnCar;

    public List<CarData> carsData = new List<CarData>();
    public string levelDataPath;

    void Awake()
    {
        _instance = this;

        // 初始化格子位置数组
        InitializeGridPositions();
    }

    private void OnEnable()
    {
        playerMovedEvent.OnEventRaised += OnPlayerMoved;
    }
    private void OnDisable()
    {
        playerMovedEvent.OnEventRaised -= OnPlayerMoved;
    }

    private void OnPlayerMoved()
    {
        //玩家移动后，其他车辆延迟移动
        StartCoroutine(HandleOtherCarsMovement());
    }

    private void Start()
    {
        if (isRandomSpawnCar)
            SpawnCars(numOfCarsToSpawn);
        else
            SpawnCarsByLevelData();
    }

    private void SpawnCarsByLevelData()
    {
        LoadLevelData(levelDataPath);
    }

    //初始化格子坐标
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
    private List<Vector2Int> GetAllCellsCoordinate(int excludedRows)
    {
        List<Vector2Int> allCoordinates = new List<Vector2Int>();
        for (int i = excludedRows; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                allCoordinates.Add(new Vector2Int(i, j));
            }
        }
        return allCoordinates;
    }

    //车辆生成
    private void SpawnCars(int numOfCars)
    {

        List<Vector2Int> allCoordinates = GetAllCellsCoordinate(excludedRows);
        for (int i = 0; i < numOfCars; i++)
        {
            if (allCoordinates.Count == 0) return;

            int randomIndex = UnityEngine.Random.Range(0, allCoordinates.Count);
            var selectedCell = allCoordinates[randomIndex];

            //如果不在已占用位置中
            if (!occupiedCells.Contains(selectedCell))
            {
                //车辆随机生成的位置
                Vector3 spawnPosition = gridPositions[selectedCell.x, selectedCell.y];

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

    public void SaveLevelData(string path)
    {
        string jsonData = JsonUtility.ToJson(new Serialization<CarData>(carsData));
        System.IO.File.WriteAllText(path, jsonData);
    }
    public void LoadLevelData(string path)
    {
        if (System.IO.File.Exists(path))
        {
            string jsonData = System.IO.File.ReadAllText(path);
            carsData = JsonUtility.FromJson<Serialization<CarData>>(jsonData).ToList();

            // 实例化每辆车
            foreach (var carData in carsData)
            {
                Vector3 spawnPosition = gridPositions[carData.gridPosition.x, carData.gridPosition.y];
                GameObject carPrefab = carPrefabs[carData.carPrefabIndex];
                Instantiate(carPrefab, spawnPosition, Quaternion.identity, this.transform);

                SpriteUtils.AdjustSizeToFitCell(carPrefab, cellWidth, cellHeight);

                cars.Add(carPrefab);
                occupiedCells.Add(new Vector2Int(carData.gridPosition.x, carData.gridPosition.y));
            }
        }
    }
    #region 携程

    //
    private IEnumerator HandleOtherCarsMovement()
    {
        // 禁止玩家移动（如果有必要）
        playerTurn = false;
        // 例如，可以设置PlayerController的一个标志来禁止移动

        // 等待玩家移动后的等待时间
        yield return new WaitForSeconds(waitTimeAfterPlayerMove);

        // 移动其他车辆
        MoveCars();

        // 再次等待，以给其他车辆移动留出时间
        yield return new WaitForSeconds(waitTimeAfterCarsMove);

        // 允许玩家再次操作
        playerTurn = true;
    }
    #endregion

    #region Scene 格子显示
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
    #endregion
}
