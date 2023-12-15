using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject roadPrefab; // 用来放置车道的Prefab
    public GameObject boundary; // 用来装所有车道的AreaPrefab
    public int initNumberOfRoads; // 您希望的车道数
    public int numberOfStartGroups;
    public int numberOfGroups;
    public float spaceBetweenRoads; // 车道之间的空隙宽度
    public float groupVerticalOffset;
    public Transform roadGroupsTransform;
    public List<GameObject> roads;
    public List<GameObject> roadGroups;
    private float roadWidth;
    private float roadLength;
    void Start()
    {
        // 获取AreaPrefab的宽度
        float areaWidth = boundary.GetComponent<SpriteRenderer>().bounds.size.x;
        float areaLength = boundary.GetComponent<SpriteRenderer>().bounds.size.y;
        roadLength = areaLength;

        // 计算每个车道的宽度
        roadWidth = CalculateRoadWidth(areaWidth, initNumberOfRoads, spaceBetweenRoads);

        
        // 初始生成车道组
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

    //生成一个道路组
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
            // 计算当前车道的位置
            Vector3 lanePosition = startPosition + new Vector3(space * i + roadWidth / 2, group.position.y, 0);

            // 实例化RoadPrefab
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
            rb.velocity = Vector2.down * playerSpeed * Time.deltaTime; // 使车道组下移动，速度等于玩家速度
        }
    }
}
