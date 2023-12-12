using System.Collections.Generic;
using UnityEngine;

public class RoadManager : MonoBehaviour
{
    public GameObject roadPrefab; // �������ó�����Prefab
    public GameObject boundary; // ����װ���г�����AreaPrefab
    public int numberOfRoads; // ��ϣ���ĳ�����
    public int numberOfStartGroups;
    public int numberOfGroups;
    public float spaceBetweenRoads; // ����֮��Ŀ�϶���

    public Transform roadGroupsTransform;
    public List<GameObject> roads;
    public List<GameObject> roadGroups;

    void Start()
    {
        // ��ȡAreaPrefab�Ŀ��
        float areaWidth = boundary.GetComponent<SpriteRenderer>().bounds.size.x;
        float areaLength = boundary.GetComponent<SpriteRenderer>().bounds.size.y;


        // ����ÿ�������Ŀ��
        float laneWidth = CalculateRoadWidth(areaWidth, numberOfRoads, spaceBetweenRoads);


        // ���ɳ�����
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
            // ���㵱ǰ������λ��
            Vector3 lanePosition = startPosition + new Vector3(space * i + roadWidth / 2, 0, 0);

            // ʵ����RoadPrefab
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
            rb.velocity = Vector2.down * playerSpeed * Time.deltaTime; // ʹ���������ƶ����ٶȵ�������ٶ�
        }
    }
}
