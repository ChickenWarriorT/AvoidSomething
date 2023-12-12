using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject roadPrefab; // 用来放置车道的Prefab
    public GameObject boundary; // 用来装所有车道的AreaPrefab
    public int numberOfRoads; // 您希望的车道数
    public int numberOfStartGroups;
    public int numberOfGroups;
    public float spaceBetweenRoads; // 车道之间的空隙宽度

    public Transform roadGroupsTransform;
    public List<GameObject> roads;
    public List<GameObject> roadGroups;

    void Start()
    {
        // 获取AreaPrefab的宽度
        float areaWidth = boundary.GetComponent<SpriteRenderer>().bounds.size.x;
        float areaLength = boundary.GetComponent<SpriteRenderer>().bounds.size.y;


        // 计算每个车道的宽度
        float laneWidth = CalculateRoadWidth(areaWidth, numberOfRoads, spaceBetweenRoads);


        // 生成车道组
        GenerateRoadGroups(laneWidth, areaLength, numberOfStartGroups);
    }

    float CalculateRoadWidth(float totalWidth, int numRoads, float spaceBetween)
    {
        return (totalWidth - (numRoads - 1) * spaceBetween) / numRoads;
    }

    void GenerateRoadGroups(float roadWidth, float roadLength, int numberOfGroups)
    {
        for (int i = 0; i < numberOfGroups; i++)
        {
            GameObject roadGroup = new GameObject("RoadGroup_" + i);
            roadGroup.transform.parent = roadGroupsTransform;
            roadGroup.AddComponent<Rigidbody2D>().gravityScale = 0;

            roadGroups.Add(roadGroup);
            GenerateLanes(roadWidth, roadLength, roadGroup.transform);
        }
    }
    void GenerateLanes(float roadWidth, float roadLength, Transform parent)
    {
        Vector3 startPosition = boundary.transform.position - new Vector3(boundary.GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        float space = roadWidth + spaceBetweenRoads;

        for (int i = 0; i < numberOfRoads; i++)
        {
            // 计算当前车道的位置
            Vector3 lanePosition = startPosition + new Vector3(space * i + roadWidth / 2, 0, 0);

            // 实例化RoadPrefab
            GameObject road = Instantiate(roadPrefab, lanePosition, Quaternion.identity, parent);
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
