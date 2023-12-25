using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject roadPrefab; // 用来放置车道的Prefab
    public GameObject boundary; // 用来装所有车道的AreaPrefab
    public GameObject solidLinePrefab;
    public GameObject dashedLinePrefab;

    
    public int initNumberOfRoads; // 您希望的车道数
    public int numberOfStartGroups;
    public int numberOfGroups;
    //public float lineWidth; //线的宽度
    public float groupVerticalOffset;
    public Transform roadGroupsTransform;
    public List<GameObject> roads;
    public List<GameObject> roadGroups;
    private float roadWidth;
    private float roadLength;
    private float roadGroupWidth;

    private float distanceForNewGroup = 0;
    private float distanceGap = 50f;
    void Start()
    {
        // 获取AreaPrefab的宽度
        roadGroupWidth = boundary.GetComponent<SpriteRenderer>().bounds.size.x;
        float roadGroupLength = boundary.GetComponent<SpriteRenderer>().bounds.size.y;
        Debug.Log(roadGroupLength);
        roadLength = roadGroupLength;

        // 计算每个车道的宽度
        roadWidth = CalculateRoadWidth(roadGroupWidth, initNumberOfRoads);

        // 初始生成车道组
        GenerateRoadGroups(roadWidth, roadLength, numberOfStartGroups, initNumberOfRoads);
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
            GenerateRoadGroups(roadWidth, roadLength, 1, Random.Range(1, initNumberOfRoads + 1));
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

    //计算道路宽度
    float CalculateRoadWidth(float totalWidth, int numRoads)
    {
        return totalWidth / numRoads;
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
            GenerateLines(roadWidth, roadLength, numberOfRoad, roadGroup.transform);
            roadGroup.transform.position = new Vector3(roadGroup.transform.position.x, yPos + i * (roadLength + groupVerticalOffset), 0);
        }
    }

    void GenerateLines(float roadWidth, float roadLength, int numberOfRoad, Transform group)
    {

        // 计算第一条线的起始位置
        Vector3 startPosition = boundary.transform.position
                                - new Vector3(boundary.GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        Debug.Log("Generating lines, Number of Roads: " + numberOfRoad);
        for (int i = 0; i <= numberOfRoad; i++)
        {
            GameObject linePrefab = (i == 0 || i == numberOfRoad) ? solidLinePrefab : dashedLinePrefab;
            if (linePrefab == null)
            {
                Debug.LogError("Line prefab is null for index: " + i);
                continue;
            }
            GameObject line = Instantiate(linePrefab, group);

            float scaleY = roadLength / line.GetComponent<SpriteRenderer>().sprite.bounds.size.y;

            // 计算当前分割线的位置
            Vector3 linePosition = startPosition + new Vector3(i * roadWidth, 0, 0);
            line.transform.localPosition = linePosition;

            // 应用缩放
            line.transform.localScale = new Vector3(line.transform.localScale.x, scaleY, 1);
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
