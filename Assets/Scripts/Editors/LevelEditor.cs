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
        // 获取需要的SerializedProperty
        carPrefabs = serializedObject.FindProperty("carPrefabs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // 显示默认的inspector属性
        DrawDefaultInspector();

        // 保存按钮
        if (GUILayout.Button("Save Level"))
        {
            SaveLevel();
        }

        serializedObject.ApplyModifiedProperties();

        // 添加生成网格的按钮
        TrafficManager manager = (TrafficManager)target;
        if (GUILayout.Button("Generate Grid Prefabs"))
        {
            GenerateGridPrefabs(manager);
        }

        // 添加清除网格的按钮
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

                // 设置Prefab实例的名字，包含其网格坐标
                prefabInstance.name = $"Grid({i},{j})";

                // 调整Prefab实例的大小以匹配格子的大小
                AdjustPrefabSize(prefabInstance, manager.CellWidth, manager.CellHeight);
            }
        }
    }
    private void AdjustPrefabSize(GameObject prefab, float cellWidth, float cellHeight)
    {
        // 假设原Prefab的大小为1x1单位
        Vector3 scale = new Vector3(cellWidth, cellHeight, 1);
        prefab.transform.localScale = scale;
    }

    private void ClearGridPrefabs(TrafficManager manager)
    {
        // 注意：这个方法假设所有gridPrefab实例都是该对象的直接子对象
        int childCount = manager.transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            var child = manager.transform.GetChild(i);
            if (child.gameObject.CompareTag("Grid")) // 确保只销毁gridPrefab实例
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
                e.Use(); // 防止事件进一步传播
            }
        }
    }

    private Vector2Int WorldToGrid(GameObject gridPrefab, TrafficManager manager)
    {
        // 假设每个gridPrefab的名字或其他属性包含了它的网格坐标
        // 例如：gridPrefab的名字可能是 "Grid(2,3)"

        string[] parts = gridPrefab.name.Trim().Split(new char[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length >= 3)
        {
            int x, y;
            if (int.TryParse(parts[1], out x) && int.TryParse(parts[2], out y))
            {
                return new Vector2Int(x, y);
            }
        }

        return new Vector2Int(-1, -1); // 返回一个无效的坐标，如果解析失败
    }

    private void PlaceCar(TrafficManager manager, Vector2Int gridPos)
    {
        manager.InitializeGridPositions();
        Debug.Log(gridPos);
        // 实现在网格位置放置车辆的逻辑
        // 这可以是在Scene视图中绘制一个标记，或者实际实例化一个车辆预览
        // 以下是一个基本的实例化逻辑
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
        // 实现保存关卡的逻辑
        // 这里你需要收集所有的车辆数据并保存
        TrafficManager manager = (TrafficManager)target;
        // 示例：manager.SaveLevelData(收集的关卡数据, "保存路径");
    }
}
