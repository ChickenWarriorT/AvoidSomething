using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public static RoadManager _instance;

    public GameObject roadPrefab; // �������ó�����Prefab
    public GameObject boundary; // ����װ���г�����AreaPrefab
    public GameObject solidLinePrefab;
    public GameObject dashedLinePrefab;
    public GameObject vehiclePrefab;

    public Transform vehiclesContainer;

    public int initNumberOfRoads; // ��ϣ���ĳ�����
    public int numberOfStartGroups;
    public int numberOfGroups;
    //public float lineWidth; //�ߵĿ��
    public float groupVerticalOffset;
    public Transform roadGroupsTransform;
    public List<GameObject> roads;
    public List<RoadGroup> roadGroupsList = new List<RoadGroup>();
    private float roadWidth;
    private float roadLength;
    private float roadGroupWidth;

    private float distanceForNewGroup = 0;
    private float distanceGap = 50f;
    private void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        // ��ȡAreaPrefab�Ŀ��
        roadGroupWidth = boundary.GetComponent<SpriteRenderer>().bounds.size.x;
        float roadGroupLength = boundary.GetComponent<SpriteRenderer>().bounds.size.y;
        Debug.Log(roadGroupLength);
        roadLength = roadGroupLength;

        // ����ÿ�������Ŀ��
        roadWidth = CalculateRoadWidth(roadGroupWidth, initNumberOfRoads);

        // ��ʼ���ɳ�����
        GenerateRoadGroups(roadWidth, roadLength, numberOfStartGroups, initNumberOfRoads,3);
        distanceForNewGroup = distanceGap;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateRoadGroups(roadWidth, roadLength, 1, Random.Range(2, 5),10);
        }
        if (CanGenerateNewGroup())
        {
            distanceForNewGroup += distanceGap;
            GenerateRoadGroups(roadWidth, roadLength, 1, Random.Range(2, initNumberOfRoads + 1),10);
        }
    }

    bool CanGenerateNewGroup()
    {
        if (PlayerController._instance.Distance >= distanceForNewGroup)
        {
            return true;
        }
        return false;
    }

    //�����·���
    float CalculateRoadWidth(float totalWidth, int numRoads)
    {
        return totalWidth / numRoads;
    }

    /// <summary>
    /// ��·������
    /// </summary>
    /// <param name="roadWidth">��·���</param>
    /// <param name="roadLength">��·����</param>
    /// <param name="numberOfNewGroups">��·������</param>
    /// <param name="numberOfRoad">��·����</param>
    private void GenerateRoadGroups(float roadWidth, float roadLength, int numberOfNewGroups, int numberOfRoad, int numOfVehicle)
    {
        float yPos = CalculateNextGroupYPosition();
        for (int i = 0; i < numberOfNewGroups; i++)
        {
            GameObject roadGroupPrefab = new GameObject("RoadGroup");
            roadGroupPrefab.transform.parent = roadGroupsTransform;
            roadGroupPrefab.AddComponent<Rigidbody2D>().gravityScale = 0;
            RoadGroup roadGroup = roadGroupPrefab.AddComponent<RoadGroup>();
            roadGroup.Init(roadGroupPrefab, numberOfRoad, roadWidth, roadLength, vehiclePrefab, numOfVehicle);
            
           // RoadGroup newRoadGroup = new RoadGroup(roadGroupPrefab, numberOfRoad, roadWidth, roadLength, vehiclePrefab, numOfVehicle);
            roadGroupsList.Add(roadGroup);
            GenerateLines(roadWidth, roadLength, numberOfRoad, roadGroup.transform);
            roadGroup.transform.position = new Vector3(roadGroup.transform.position.x, yPos + i * (roadLength + groupVerticalOffset), 0);
        }
    }

    /// <summary>
    /// �������е�·��
    /// </summary>
    /// <param name="roadWidth"></param>
    /// <param name="roadLength"></param>
    /// <param name="numberOfRoad"></param>
    /// <param name="group"></param>
    void GenerateLines(float roadWidth, float roadLength, int numberOfRoad, Transform group)
    {
        float totalWidth = roadWidth * numberOfRoad;
        // ������ʼλ�õ�ƫ�ƣ���ʹ���������
        Vector3 centerPosition = boundary.transform.position;
        Vector3 startPosition = new Vector3(centerPosition.x - totalWidth / 2, centerPosition.y, centerPosition.z);
        for (int i = 0; i <= numberOfRoad; i++)
        {
            GameObject linePrefab = (i == 0 || i == numberOfRoad) ? solidLinePrefab : dashedLinePrefab;
            if (linePrefab == null)
            {
                continue;
            }
            GameObject line = Instantiate(linePrefab, group);

            float scaleY = roadLength / line.GetComponent<SpriteRenderer>().sprite.bounds.size.y;

            // ���㵱ǰ�ָ��ߵ�λ��
            Vector3 linePosition = startPosition + new Vector3(i * roadWidth, 0, 0);
            line.transform.localPosition = linePosition;

            // Ӧ������
            line.transform.localScale = new Vector3(line.transform.localScale.x, scaleY, 1);
        }
    }

    private void FixedUpdate()
    {
        RoadGroupsMover();
    }

    //�ƶ�����RoadGroups
    private void RoadGroupsMover()
    {
        foreach (var roadGroup in roadGroupsList)
        {
            var rb = roadGroup.roadGroupObject.GetComponent<Rigidbody2D>();
            var playerSpeed = PlayerController._instance.speed;
            rb.velocity = Vector2.down * playerSpeed * Time.deltaTime; // ʹ���������ƶ����ٶȵ�������ٶ�
        }
    }

    //������һ��RoadGroup��Y��λ��
    private float CalculateNextGroupYPosition()
    {
        if (roadGroupsList.Count > 0)
        {
            // ��ȡ���һ�� roadGroup ��λ��
            RoadGroup lastGroup = roadGroupsList[roadGroupsList.Count - 1];
            GameObject roadGroupObject = lastGroup.roadGroupObject;
            return roadGroupObject.transform.position.y + roadLength + groupVerticalOffset;
        }
        return 0;
    }


}
