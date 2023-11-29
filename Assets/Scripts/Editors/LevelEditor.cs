using System;
using UnityEditor;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

[CustomEditor(typeof(TrafficManager))]
public class TrafficManagerEditor : Editor
{
    SerializedProperty carPrefabs;

    void OnEnable()
    {
        // ��ȡ��Ҫ��SerializedProperty
        carPrefabs = serializedObject.FindProperty("carPrefabs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // ��ʾĬ�ϵ�inspector����
        DrawDefaultInspector();

        // ���水ť
        if (GUILayout.Button("Save Level"))
        {
            SaveLevel();
        }

        serializedObject.ApplyModifiedProperties();

        // �����������İ�ť
        TrafficManager manager = (TrafficManager)target;
        if (GUILayout.Button("Generate Grid Prefabs"))
        {
            GenerateGridPrefabs(manager);
        }

        // ����������İ�ť
        if (GUILayout.Button("Clear Grid Prefabs"))
        {
            ClearGridPrefabs(manager);
        }
    }



    private void GenerateGridPrefabs(TrafficManager manager)
    {
        if (manager.gridPrefab == null)
        {
            Debug.LogError("Grid prefab is not assigned!");
            return;
        }

        for (int i = 0; i < manager.rows; i++)
        {
            for (int j = 0; j < manager.columns; j++)
            {
                Vector3 spawnPosition = manager.gridPositions[i, j];
                GameObject prefabInstance = PrefabUtility.InstantiatePrefab(manager.gridPrefab, manager.transform) as GameObject;
                prefabInstance.transform.position = spawnPosition;

                // ����Prefabʵ�������֣���������������
                prefabInstance.name = $"Grid({i},{j})";

                // ����Prefabʵ���Ĵ�С��ƥ����ӵĴ�С
                AdjustPrefabSize(prefabInstance, manager.CellWidth, manager.CellHeight);
            }
        }
    }
    private void AdjustPrefabSize(GameObject prefab, float cellWidth, float cellHeight)
    {
        // ����ԭPrefab�Ĵ�СΪ1x1��λ
        Vector3 scale = new Vector3(cellWidth, cellHeight, 1);
        prefab.transform.localScale = scale;
    }

    private void ClearGridPrefabs(TrafficManager manager)
    {
        // ע�⣺���������������gridPrefabʵ�����Ǹö����ֱ���Ӷ���
        int childCount = manager.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            var child = manager.transform.GetChild(i);
            if (child.gameObject.CompareTag("Grid")) // ȷ��ֻ����gridPrefabʵ��
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }
    void OnSceneGUI()
    {

        TrafficManager manager = (TrafficManager)target;

        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Debug.Log("hit");
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Grid"))
            {
                Vector2Int gridPos = WorldToGrid(hit.collider.gameObject, manager);
                
                PlaceCar(manager, gridPos);
                e.Use(); // ��ֹ�¼���һ������
            }
        }
    }

    private Vector2Int WorldToGrid(GameObject gridPrefab, TrafficManager manager)
    {
        // ����ÿ��gridPrefab�����ֻ��������԰�����������������
        // ���磺gridPrefab�����ֿ����� "Grid(2,3)"

        string[] parts = gridPrefab.name.Trim().Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 3)
        {
            int x, y;
            if (int.TryParse(parts[1], out x) && int.TryParse(parts[2], out y))
            {
                return new Vector2Int(x, y);
            }
        }

        return new Vector2Int(-1, -1); // ����һ����Ч�����꣬�������ʧ��
    }

    private void PlaceCar(TrafficManager manager, Vector2Int gridPos)
    {
        manager.InitializeGridPositions();
        Debug.Log(gridPos);
        // ʵ��������λ�÷��ó������߼�
        // ���������Scene��ͼ�л���һ����ǣ�����ʵ��ʵ����һ������Ԥ��
        // ������һ��������ʵ�����߼�
        if (carPrefabs.arraySize > 0)
        {
            Debug.Log(manager.gridPositions.Length);
            Vector3 spawnPosition = manager.gridPositions[gridPos.x, gridPos.y];
            GameObject carPrefab = carPrefabs.GetArrayElementAtIndex(0).objectReferenceValue as GameObject;
            PrefabUtility.InstantiatePrefab(carPrefab, manager.transform);
            carPrefab.transform.position = spawnPosition;
        }
    }

    private void SaveLevel()
    {
        // ʵ�ֱ���ؿ����߼�
        // ��������Ҫ�ռ����еĳ������ݲ�����
        TrafficManager manager = (TrafficManager)target;
        // ʾ����manager.SaveLevelData(�ռ��Ĺؿ�����, "����·��");
    }
}
