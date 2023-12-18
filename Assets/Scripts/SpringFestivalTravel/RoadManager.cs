using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject roadPrefab; // �������ó�����Prefab
    public GameObject boundary; // ����װ���г�����AreaPrefab
    public int initNumberOfRoads; // ��ϣ���ĳ�����
    public int numberOfStartGroups;
    public int numberOfGroups;
    public float spaceBetweenRoads; // ����֮��Ŀ�϶���
    public float groupVerticalOffset;
    public Transform roadGroupsTransform;
    public List<GameObject> roads;
    public List<GameObject> roadGroups;
    private float roadWidth;
    private float roadLength;

    private float distanceForNewGroup = 0;
    private float distanceGap = 50f;
    void Start()
    {
        // ��ȡAreaPrefab�Ŀ��
        float areaWidth = boundary.GetComponent<SpriteRenderer>().bounds.size.x;
        float areaLength = boundary.GetComponent<SpriteRenderer>().bounds.size.y;
        roadLength = areaLength;

        // ����ÿ�������Ŀ��
        roadWidth = CalculateRoadWidth(areaWidth, initNumberOfRoads, spaceBetweenRoads);

        // ��ʼ���ɳ�����
        GenerateRoadGroups(roadWidth, roadLength, numberOfStartGroups, 6);
        distanceForNewGroup = distanceGap;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateRoadGroups(roadWidth, roadLength, 1, Random.Range(2, 5));
        }
        if (CanGenerateNewGroup())
        {
            distanceForNewGroup += distanceGap;
            GenerateRoadGroups(roadWidth, roadLength, 1, Random.Range(1, initNumberOfRoads+1));
        }
    }

    bool CanGenerateNewGroup()
    {
        
        if(PlayerController._instance.Distance>= distanceForNewGroup)
        {
            return true;
        }
        return false;
    }

    //�����·���
    float CalculateRoadWidth(float totalWidth, int numRoads, float spaceBetween)
    {
        return (totalWidth - (numRoads - 1) * spaceBetween) / numRoads;
    }

    /// <summary>
    /// ��·������
    /// </summary>
    /// <param name="roadWidth">��·���</param>
    /// <param name="roadLength">��·����</param>
    /// <param name="numberOfNewGroups">��·������</param>
    /// <param name="numberOfRoad">��·����</param>
    private void GenerateRoadGroups(float roadWidth, float roadLength, int numberOfNewGroups, int numberOfRoad)
    {
        float yPos = CalculateNextGroupYPosition();
        for (int i = 0; i < numberOfNewGroups; i++)
        {
            GameObject roadGroup = new GameObject("RoadGroup");
            roadGroup.transform.parent = roadGroupsTransform;
            roadGroup.AddComponent<Rigidbody2D>().gravityScale = 0;

            roadGroups.Add(roadGroup);
            GenerateLanes(roadWidth, roadLength, numberOfRoad, roadGroup.transform);
            roadGroup.transform.position = new Vector3(roadGroup.transform.position.x, yPos + i * (roadLength + groupVerticalOffset), 0);
        }
    }


    void GenerateLanes(float roadWidth, float roadLength, int numberOfRoad, Transform group)
    {
        // �������е�·���ܿ�ȣ�������϶��
        float totalRoadsWidth = numberOfRoad * roadWidth + (numberOfRoad - 1) * spaceBetweenRoads;
        // �����һ����·����ʼλ�ã�ʹ��·�����
        float startOffsetX = (boundary.GetComponent<SpriteRenderer>().bounds.size.x - totalRoadsWidth) / 2;
        Vector3 startPosition = new Vector3(startOffsetX, 0, 0) + boundary.transform.position
                                - new Vector3(boundary.GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        float space = roadWidth + spaceBetweenRoads;

        for (int i = 0; i < numberOfRoad; i++)
        {
            // ���㵱ǰ������λ��
            Vector3 lanePosition = startPosition + new Vector3(space * i + roadWidth / 2, group.position.y, 0);

            // ʵ����RoadPrefab
            GameObject road = Instantiate(roadPrefab, lanePosition, Quaternion.identity, group);

            road.transform.localScale = new Vector3(roadWidth, roadLength, 1);
            roads.Add(road);
        }
    }


    private void FixedUpdate()
    {
        RoadGroupsMover();
    }

    private void RoadGroupsMover()
    {
        foreach (var roadGroup in roadGroups)
        {
            var rb = roadGroup.GetComponent<Rigidbody2D>();
            var playerSpeed = PlayerController._instance.speed;
            rb.velocity = Vector2.down * playerSpeed * Time.deltaTime; // ʹ���������ƶ����ٶȵ�������ٶ�
        }
    }

    private float CalculateNextGroupYPosition()
    {
        if (roadGroups.Count > 0)
        {
            // ��ȡ���һ�� roadGroup ��λ��
            GameObject lastGroup = roadGroups[roadGroups.Count - 1];
            return lastGroup.transform.position.y + roadLength + groupVerticalOffset;
        }
        return 0;
    }
}
