using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public GameObject[] carPrefabs; // ����Ԥ����
    public Transform[] spawnPoints; // ���ɵ�
    public float moveDistance; // ÿ���ƶ��ľ���
    private List<GameObject> cars = new List<GameObject>(); // ������б�

    void Update()
    {
        // �����ҵ�����
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.UpArrow))// �ÿո��������ҵĵ������
        {
            MoveCars();
            SpawnCar();
        }
    }

    private void MoveCars()
    {
        foreach (var car in cars)
        {
            car.transform.Translate(0, -moveDistance, 0); // �����ƶ����г���
        }
    }

    private void SpawnCar()
    {
        int spawnIndex = Random.Range(0, spawnPoints.Length); // ���ѡ��һ�����ɵ�
        GameObject car = Instantiate(carPrefabs[Random.Range(0, carPrefabs.Length)], spawnPoints[spawnIndex].position, Quaternion.identity);
        cars.Add(car);
    }
}
