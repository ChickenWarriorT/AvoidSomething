using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class TrafficManager : MonoBehaviour
{
    public static TrafficManager _instance;
    public GameObject gridPrefab; // �������ø��ӵ�Prefab
    public GameObject[] carPrefabs; // ����Ԥ����
    public Vector3[,] gridPositions; // ��ά�������洢���ӵ�����λ��
    [Header("����")]
    public int rows = 4; // ����
    [Header("����")]
    public int columns = 3; // ����
    [Header("������ʼ���������ɵĸ���")]
    public int excludedRows = 5;

    private List<GameObject> cars = new List<GameObject>(); // ������б�
    private HashSet<Tuple<int,int>> occupiedCells = new HashSet<Tuple<int, int>>();//�ѱ�ռ�ø���
    private float cellWidth;
    [Header("���Ӹ߶�")]
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

        // ��ʼ������λ������
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
        // �����ҵ�����
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCars();
        }
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

    //��������
    private void SpawnCars(int numOfCars)
    {

        List<Tuple<int, int>> allCoordinates = GetAllCellsCoordinate(excludedRows);
        for (int i = 0; i < numOfCars; i++)
        {
            if (allCoordinates.Count == 0) return;

            int randomIndex = UnityEngine.Random.Range(0, allCoordinates.Count);
            Tuple<int, int> selectedCell = allCoordinates[randomIndex];

            //���������ռ��λ����
            if (!occupiedCells.Contains(selectedCell))
            {
                //����������ɵ�λ��
                Vector3 spawnPosition = gridPositions[selectedCell.Item1, selectedCell.Item2];

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
}
