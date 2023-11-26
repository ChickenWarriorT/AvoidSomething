using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public GameObject[] carPrefabs; // 车辆预制体
    public Transform[] spawnPoints; // 生成点
    public float moveDistance; // 每次移动的距离
    private List<GameObject> cars = new List<GameObject>(); // 活动车辆列表

    void Update()
    {
        // 检测玩家的输入
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.UpArrow))// 用空格键代替玩家的点击操作
        {
            MoveCars();
            SpawnCar();
        }
    }

    private void MoveCars()
    {
        foreach (var car in cars)
        {
            car.transform.Translate(0, -moveDistance, 0); // 向下移动所有车辆
        }
    }

    private void SpawnCar()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length); // 随机选择一个生成点
        GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], spawnPoints[spawnIndex].position, Quaternion.identity);
        cars.Add(car);
    }
}
