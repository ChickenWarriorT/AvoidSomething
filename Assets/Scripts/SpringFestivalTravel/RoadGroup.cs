using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class RoadGroup : MonoBehaviour
{
    public GameObject roadGroupObject;
    public int numberOfRoad;
    public List<GameObject> vehicles;
    private float roadWidth;
    private float roadLength;
    public List<Vector2> roadPositions;
    private float minVehicleSpacing = 2f;
    private int vehicleNum;

    private float destroyYAxis;
    public void Init(GameObject obj, int numRoads, float width, float length, GameObject vehiclePrefab, int num)
    {
        roadGroupObject = obj;
        roadGroupObject = obj;
        numberOfRoad = numRoads;
        roadWidth = width;
        roadLength = length;
        vehicles = new List<GameObject>();
        roadPositions = new List<Vector2>();
        vehicleNum = num;
        CalculateRoadPositions();
        GenerateVehicleOnRandomRoad(vehiclePrefab, vehicleNum, RoadManager._instance.vehiclesContainer.transform);
        destroyYAxis = RoadManager._instance.boundary.GetComponent<SpriteRenderer>().bounds.min.y - length;
    }
    private void FixedUpdate()
    {
        CheckBoundary();
    }
    public RoadGroup(GameObject obj, int numRoads, float width, float length, GameObject vehiclePrefab, int num)
    {
        roadGroupObject = obj;
        roadGroupObject = obj;
        numberOfRoad = numRoads;
        roadWidth = width;
        roadLength = length;
        vehicles = new List<GameObject>();
        roadPositions = new List<Vector2>();
        vehicleNum = num;
        CalculateRoadPositions();
        GenerateVehicleOnRandomRoad(vehiclePrefab, vehicleNum, RoadManager._instance.vehiclesContainer.transform);

    }

    public void CalculateRoadPositions()
    {
        float totalWidth = roadWidth * numberOfRoad;
        Vector3 centerPosition = roadGroupObject.transform.position;
        Vector3 startPosition = new Vector3(centerPosition.x - totalWidth / 2 + roadWidth, centerPosition.y, centerPosition.z);
        for (int i = 0; i < numberOfRoad; i++)
        {
            Vector3 roadPosition = startPosition + new Vector3(i * roadWidth, 0, 0);
            roadPositions.Add(roadPosition);
        }
    }
    public void GenerateVehicleOnRandomRoad(GameObject vehiclePrefab, int num, Transform vehiclesContainer)
    {
        for (int i = 0; i < num; i++)
        {

            int roadIndex = UnityEngine.Random.Range(0, roadPositions.Count);
            Vector3 roadPosition = roadPositions[roadIndex];
            Vector3 worldRoadPosition = roadGroupObject.transform.TransformPoint(roadPosition);

            // 计算一个合适的位置，避免与其他车辆重叠
            Vector3 vehiclePosition = CalculateVehiclePosition(worldRoadPosition);

            GameObject newVehicle = GameObject.Instantiate(vehiclePrefab, vehiclePosition, Quaternion.identity, vehiclesContainer);
            vehicles.Add(newVehicle);
        }
    }
    private Vector3 CalculateVehiclePosition(Vector3 roadPosition)
    {
        int maxAttempts = 50;
        int attempts = 0;
        Vector3 position;
        bool positionFound;

        do
        {
            positionFound = true;
            float yPos = roadPosition.y + UnityEngine.Random.Range(-roadWidth / 2, roadWidth / 2);
            position = new Vector3(roadPosition.x, yPos, roadPosition.z);

            // 检查与现有车辆的距离
            foreach (var vehicle in vehicles)
            {
                if (Vector3.Distance(vehicle.transform.position, position) < minVehicleSpacing)
                {
                    positionFound = false;
                    break;
                }
            }
            attempts++;
            if (attempts > maxAttempts)
            {
                return roadPosition;
            }
        } while (!positionFound);

        return position;
    }
    private void CheckBoundary()
    {
        if (transform.position.y < destroyYAxis)
        {
            Destroy(gameObject);
            RoadManager._instance.roadGroupsList.Remove(this);
        }
    }
}


