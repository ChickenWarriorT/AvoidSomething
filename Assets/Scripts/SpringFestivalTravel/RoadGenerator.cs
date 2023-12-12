using UnityEngine;

public class RoadGenerator : MonoBehaviour
{
    public GameObject roadPrefab; // �������ó�����Prefab
    public GameObject boundary; // ����װ���г�����AreaPrefab
    public int numberOfLanes; // ��ϣ���ĳ�����
    public float spaceBetweenLanes; // ����֮��Ŀ�϶���

    void Start()
    {
        // ��ȡAreaPrefab�Ŀ��
        float areaWidth = boundary.GetComponent<SpriteRenderer>().bounds.size.x;
        float areaLength = boundary.GetComponent<SpriteRenderer>().bounds.size.y;


        // ����ÿ�������Ŀ��
        float laneWidth = CalculateLaneWidth(areaWidth, numberOfLanes, spaceBetweenLanes);


        // ���ɳ���
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
            // ���㵱ǰ������λ��
            Vector3 lanePosition = startPosition + new Vector3(space * i + laneWidth / 2, 0, 0);

            // ʵ����RoadPrefab
            GameObject lane = Instantiate(roadPrefab, lanePosition, Quaternion.identity, boundary.transform);
            lane.transform.localScale = new Vector3(laneWidth, landLength, 1); // ���賵���ĸ߶Ȳ���
        }
    }
}
