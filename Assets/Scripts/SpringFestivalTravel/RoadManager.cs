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

    private float distanceForNewGroup = 0;
    private float distanceGap = 50f;
    void Start()
    {
        // 获取AreaPrefab的宽度
        float areaWidth = boundary.GetComponent<SpriteRenderer>().bounds.size.x;
        float areaLength = boundary.GetComponent<SpriteRenderer>().bounds.size.y;
        roadLength = areaLength;

        // 计算每个车道的宽度
        roadWidth = CalculateRoadWidth(areaWidth, initNumberOfRoads, spaceBetweenRoads);

        // 初始生成车道组
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

    //计算道路宽度
    float CalculateRoadWidth(float totalWidth, int numRoads, float spaceBetween)
    {
        return (totalWidth - (numRoads - 1) * spaceBetween) / numRoads;
    }

    /// <summary>
    /// 道路组生成
    /// </summary>
    /// <param name="roadWidth">道路宽度</param>
    /// <param name="roadLength">道路长度</param>
    /// <param name="numberOfNewGroups">道路组数量</param>
    /// <param name="numberOfRoad">道路数量</param>
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
        // 计算所有道路的总宽度（包括空隙）
        float totalRoadsWidth = numberOfRoad * roadWidth + (numberOfRoad - 1) * spaceBetweenRoads;
        // 计算第一条道路的起始位置，使道路组居中
        float startOffsetX = (boundary.GetComponent<SpriteRenderer>().bounds.size.x - totalRoadsWidth) / 2;
        Vector3 startPosition = new Vector3(startOffsetX, 0, 0) + boundary.transform.position
                                - new Vector3(boundary.GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        float space = roadWidth + spaceBetweenRoads;

        for (int i = 0; i < numberOfRoad; i++)
        {
            // 计算当前车道的位置
            Vector3 lanePosition = startPosition + new Vector3(space * i + roadWidth / 2, group.position.y, 0);

            // 实例化RoadPrefab
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
            rb.velocity = Vector2.down * playerSpeed * Time.deltaTime; // 使车道组下移动，速度等于玩家速度
        }
    }

    private float CalculateNextGroupYPosition()
    {
        if (roadGroups.Count > 0)
        {
            // 获取最后一个 roadGroup 的位置
            GameObject lastGroup = roadGroups[roadGroups.Count - 1];
            return lastGroup.transform.position.y + roadLength + groupVerticalOffset;
        }
        return 0;
    }
}
