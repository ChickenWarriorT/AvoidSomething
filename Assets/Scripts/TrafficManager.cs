using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public static TrafficManager _instance;
    public GameObject[] carPrefabs; // ����Ԥ����
    public Vector3[,] gridPositions; // ��ά�������洢���ӵ�����λ��
    [Header("����")]
    public int rows = 4; // ����
    [Header("����")]
    public int columns = 3; // ����

    private List<GameObject> cars = new List<GameObject>(); // ������б�
    [Header("���ӿ��")]
    [SerializeField]
    private float cellWidth;
    [Header("���Ӹ߶�")]
    [SerializeField]
    private float cellHeight;

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

    }

    private void InitializeGridPositions()
    {
        screenHeight = Camera.main.orthographicSize * 2f;
        screenWidth = screenHeight * Camera.main.aspect;

        cellWidth = screenWidth / columns;

        // ���������Ұ���и߼�������
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
        // �����ҵ�����
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveCars();
            SpawnCar();
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

    private void SpawnCar()
    {
        // ���ѡ��һ���������ɳ���
        int column = Random.Range(0, columns);
        // ���㳵��������λ��
        Vector3 spawnPosition = new Vector3(column * cellWidth - Camera.main.aspect * Camera.main.orthographicSize + cellWidth / 2, Camera.main.orthographicSize + cellHeight / 2, 0);

        // ʵ��������Ԥ����
        GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], spawnPosition, Quaternion.identity);

        SpriteUtils.AdjustSizeToFitCell(car, cellWidth, cellHeight);

        cars.Add(car);
    }



#if UNITY_EDITOR
    // ��Unity�༭����Scene��ͼ�л��Ƹ���
    void OnDrawGizmos()
    {
        // ���¼�����Ļ��Ⱥ͸߶�����Ӧʵ�ʵ�����������
        float totalGridHeight = cellHeight * rows;
        float totalGridWidth = cellWidth * columns;

        // ��������½���ʼ��
        Vector3 bottomLeft = new Vector3(-totalGridWidth / 2, -totalGridHeight / 2, 0);

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

        if (gridPositions == null)
        {
            InitializeGridPositions();
        }

        // �������б��
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
