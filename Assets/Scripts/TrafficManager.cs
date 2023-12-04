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
    public GameObject gridPrefab; // �������ø��ӵ�Prefab
    public GameObject[] carPrefabs; // ����Ԥ����
    public Vector3[,] gridPositions; // ��ά�������洢���ӵ�����λ��
    [Header("����������")]
    public int rows = 4; // ����
    public int columns = 3; // ����

    [Header("�ȴ�ʱ������")]
    public float waitTimeAfterPlayerMove = 0.5f; // ����ƶ���ĵȴ�ʱ��
    public float waitTimeAfterCarsMove = 0.5f; // ���������ƶ���ĵȴ�ʱ��

    [Header("������ʼ���������ɵĸ���")]
    public int excludedRows = 5;

    [SerializeField]
    private List<GameObject> cars = new List<GameObject>(); // ������б�
    public HashSet<Vector2Int> occupiedCells = new HashSet<Vector2Int>();//�ѱ�ռ�ø���
    private float cellWidth;
    [Header("���Ӹ߶�")]
    [SerializeField]
    private float cellHeight;

    [Header("�¼�")]
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

        // ��ʼ������λ������
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
        //����ƶ������������ӳ��ƶ�
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

    //��ʼ����������
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
        // �����ƶ����г���
        for (int i = cars.Count - 1; i >= 0; i--)
        {
            cars[i].transform.Translate(0, -cellHeight, 0, Space.World);
            // ��������ƶ�����Ļ�ײ��⣬��ɾ����
            if (cars[i].transform.position.y < -Camera.main.orthographicSize - cellHeight)
            {
                Destroy(cars[i]);
                cars.RemoveAt(i);
            }
        }
    }

    //���и����Զ������������
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

    //��������
    private void SpawnCars(int numOfCars)
    {

        List<Vector2Int> allCoordinates = GetAllCellsCoordinate(excludedRows);
        for (int i = 0; i < numOfCars; i++)
        {
            if (allCoordinates.Count == 0) return;

            int randomIndex = UnityEngine.Random.Range(0, allCoordinates.Count);
            var selectedCell = allCoordinates[randomIndex];

            //���������ռ��λ����
            if (!occupiedCells.Contains(selectedCell))
            {
                //����������ɵ�λ��
                Vector3 spawnPosition = gridPositions[selectedCell.x, selectedCell.y];

                // ʵ��������Ԥ����
                GameObject car = Instantiate(carPrefabs[UnityEngine.Random.Range(0, carPrefabs.Length)], spawnPosition, Quaternion.identity);

                SpriteUtils.AdjustSizeToFitCell(car, cellWidth, cellHeight);

                cars.Add(car);
                occupiedCells.Add(selectedCell);
            }
        }
    }



    //�����ռ��λ��
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

            // ʵ����ÿ����
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
    #region Я��

    //
    private IEnumerator HandleOtherCarsMovement()
    {
        // ��ֹ����ƶ�������б�Ҫ��
        playerTurn = false;
        // ���磬��������PlayerController��һ����־����ֹ�ƶ�

        // �ȴ�����ƶ���ĵȴ�ʱ��
        yield return new WaitForSeconds(waitTimeAfterPlayerMove);

        // �ƶ���������
        MoveCars();

        // �ٴεȴ����Ը����������ƶ�����ʱ��
        yield return new WaitForSeconds(waitTimeAfterCarsMove);

        // ��������ٴβ���
        playerTurn = true;
    }
    #endregion

    #region Scene ������ʾ
#if UNITY_EDITOR
    // ��Unity�༭����Scene��ͼ�л��Ƹ���
    void OnDrawGizmos()
    {
        UpdateScreenAndCellSize();

        InitializeGridPositions();


        // ���¼�����Ļ��Ⱥ͸߶�����Ӧʵ�ʵ�����������
        float totalGridHeight = cellHeight * rows;
        float totalGridWidth = cellWidth * columns;

        // ��������½���ʼ��
        Vector3 bottomLeft = new Vector3(-screenWidth / 2, -screenHeight / 2, 0);

        Gizmos.color = Color.green;

        // ������
        for (int i = 0; i <= rows; i++)
        {
            Vector3 startPos = bottomLeft + new Vector3(0, cellHeight * i, 0);
            Vector3 endPos = startPos + new Vector3(totalGridWidth, 0, 0);
            Gizmos.DrawLine(startPos, endPos);
        }

        // ������
        for (int j = 0; j <= columns; j++)
        {
            Vector3 startPos = bottomLeft + new Vector3(cellWidth * j, 0, 0);
            Vector3 endPos = startPos + new Vector3(0, totalGridHeight, 0);
            Gizmos.DrawLine(startPos, endPos);
        }



        // �������б��
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 8;
        labelStyle.normal.textColor = Color.white;

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                Vector3 labelPosition = new Vector3(bottomLeft.x + cellWidth * j + cellWidth / 2, bottomLeft.y + cellHeight * i + cellHeight / 2, 0);
                labelPosition.x -= 0.5f;
                Handles.Label(labelPosition, $"{i}��{j}", labelStyle);
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
