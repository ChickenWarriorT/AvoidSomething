using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class RoadGroup : MonoBehaviour
{
    public List<GameObject> vehicles;
    public List<Vector2> roadPositions;
    public List<Vector3> gridPositions;

    public int numberOfRoad;
    public float maxVerticalOffset;


    private float roadWidth;
    private float roadLength;
    private float minVehicleSpacing = 10f;
    private int vehicleNum;

    private float destroyYAxis;

    public void Init(int numRoads, float width, float length, GameObject vehiclePrefab, int num)
    {
         numberOfRoad = numRoads;
        roadWidth = width;
        roadLength = length;
        vehicles = new List<GameObject>();
        roadPositions = new List<Vector2>();
        vehicleNum = num;
        CalculateRoadPositions();
        CalculateGridPositions();
        GenerateVehicleOnRandomRoad(vehiclePrefab, vehicleNum);
        destroyYAxis = RoadManager._instance.boundary.GetComponent<SpriteRenderer>().bounds.min.y - length;
    }
    private void FixedUpdate()
    {
        CheckBoundary();
    }
    private void CalculateGridPositions()
    {
        gridPositions = new List<Vector3>();
        // 计算每条道路的格子位置
        float roadStartY = transform.position.y - roadLength / 2;
        float roadEndY = roadStartY + roadLength;
        Debug.Log(roadStartY);
        for (int i = 0; i < numberOfRoad; i++)
        {

            for (float y = roadStartY; y < roadEndY; y += minVehicleSpacing)
            {
                gridPositions.Add(new Vector3(roadPositions[i].x, y, 0));
            }
        }
    }
    public void CalculateRoadPositions()
    {
        float totalWidth = roadWidth * numberOfRoad;
        Vector3 centerPosition = transform.position;
        Vector3 startPosition = new Vector3(centerPosition.x - totalWidth / 2 + roadWidth / 2, centerPosition.y, centerPosition.z);
        for (int i = 0; i < numberOfRoad; i++)
        {
            Vector3 roadPosition = startPosition + new Vector3(i * roadWidth, 0, 0);
            roadPositions.Add(roadPosition);
        }
    }
    public void GenerateVehicleOnRandomRoad(GameObject vehiclePrefab, int num)
    {
        for (int i = 0; i < num; i++)
        {
            if (gridPositions.Count == 0)
            {
                break;
            }
            int index = UnityEngine.Random.Range(0, gridPositions.Count);
            Vector3 position = gridPositions[index];
            position.y += UnityEngine.Random.Range(-maxVerticalOffset, maxVerticalOffset); // 加上随机的上下偏移


            GameObject newVehicle = Instantiate(vehiclePrefab, position, Quaternion.identity);
            vehicles.Add(newVehicle);

            gridPositions.RemoveAt(index); // 移除已经使用的位置
        }
    }

    private void CheckBoundary()
    {
        if (transform.position.y < destroyYAxis)
        {
            Destroy(gameObject);
            RoadManager._instance.roadGroupsList.Remove(this);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow; // 设置格子颜色，例如黄色
        foreach (var gridPosition in gridPositions)
        {
            float gizmoSize = 0.5f; // 设置格子大小
            Gizmos.DrawCube(gridPosition, new Vector3(gizmoSize, gizmoSize, gizmoSize));
        }
    }
}


