using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public GameObject roadPrefab; // 用来放置车道的Prefab
    public GameObject boundary; // 用来装所有车道的AreaPrefab
    public int numberOfLanes; // 您希望的车道数
    public float spaceBetweenLanes; // 车道之间的空隙宽度

    void Start()
    {
        // 获取AreaPrefab的宽度
        float areaWidth = boundary.GetComponent<SpriteRenderer>().bounds.size.x;
        float areaLength = boundary.GetComponent<SpriteRenderer>().bounds.size.y;


        // 计算每个车道的宽度
        float laneWidth = CalculateLaneWidth(areaWidth, numberOfLanes, spaceBetweenLanes);


        // 生成车道
        GenerateLanes(laneWidth, areaLength);
    }

    float CalculateLaneWidth(float totalWidth, int numLanes, float spaceBetween)
    {
        return (totalWidth - (numLanes - 1) * spaceBetween) / numLanes;
    }

    void GenerateLanes(float laneWidth, float landLength)
    {
        Vector3 startPosition = boundary.transform.position - new Vector3(boundary.GetComponent<SpriteRenderer>().bounds.size.x / 2, 0, 0);
        float space = laneWidth + spaceBetweenLanes;

        for (int i = 0; i < numberOfLanes; i++)
        {
            // 计算当前车道的位置
            Vector3 lanePosition = startPosition + new Vector3(space * i + laneWidth / 2, 0, 0);

            // 实例化RoadPrefab
            GameObject lane = Instantiate(roadPrefab, lanePosition, Quaternion.identity, boundary.transform);
            lane.transform.localScale = new Vector3(laneWidth, landLength, 1); // 假设车道的高度不变
        }
    }
}
