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
    void Start()
    {
        // ��ȡAreaPrefab�Ŀ��
        float areaWidth = boundary.GetComponent<SpriteRenderer>().bounds.size.x;
        float areaLength = boundary.GetComponent<SpriteRenderer>().bounds.size.y;
        roadLength = areaLength;

        // ����ÿ�������Ŀ��
        roadWidth = CalculateRoadWidth(areaWidth, initNumberOfRoads, spaceBetweenRoads);

        
        // ��ʼ���ɳ�����
        GenerateRoadGroups(roadWidth, roadLength, numberOfStartGroups);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            GenerateRoadGroup(roadWidth, roadLength, 1);
        }
    }

    float CalculateRoadWidth(float totalWidth, int numRoads, float spaceBetween)
    {
        return (totalWidth - (numRoads - 1) * spaceBetween) / numRoads;
    }

    //����һ����·��
    GameObject GenerateRoadGroup(float roadWidth, float roadLength, int numberOfRoad)
    {
       
        GameObject roadGroup = new GameObject("RoadGroup");
        roadGroup.transform.parent = roadGroupsTransform;
        roadGroup.AddComponent<Rigidbody2D>().gravityScale = 0;

        roadGroups.Add(roadGroup);
        GenerateLanes(roadWidth, roadLength, numberOfRoad, roadGroup.transform);
        return roadGroup;
    }

    void GenerateRoadGroups(float roadWidth, float roadLength, int numberOfGroups)
    {
        for (int i = 0; i < numberOfGroups; i++)
        {
            GameObject roadGroup = GenerateRoadGroup(roadWidth, roadLength, Random.Range(1, 5));
            float yPos = i * (roadLength + groupVerticalOffset);
            roadGroup.transform.position = new Vector3(roadGroup.transform.position.x, yPos);
        }
    }
    void GenerateLanes(float roadWidth, float roadLength,int numberOfRoad, Transform group)
    {
        Vector3 startPosition = boundary.transform.position - new Vector3(boundary.GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        float space = roadWidth + spaceBetweenRoads;

        for (int i = 0; i < numberOfRoad; i++)
        {
            // ���㵱ǰ������λ��
            Vector3 lanePosition = startPosition + new Vector3(space * i + roadWidth / 2, group.position.y, 0);

            // ʵ����RoadPrefab
            GameObject road = Instantiate(roadPrefab, lanePosition, Quaternion.identity, group);
            //float scaleFactorX = 1 / boundary.transform.localScale.x; 
            //float scaleFactorY = 1 / boundary.transform.localScale.y; 
            //road.transform.localScale = new Vector3(roadWidth * scaleFactorX, roadLength * scaleFactorY, 1);
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
}
