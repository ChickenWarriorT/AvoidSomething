using UnityEditor;
using UnityEngine;

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
    }

    void OnSceneGUI()
    {
        TrafficManager manager = (TrafficManager)target;

        Event e = Event.current;
        if (e.type == EventType.MouseDown && e.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 point = hit.point;
                // 转换点到网格坐标
                Vector2Int gridPos = WorldToGrid(point, manager);
                // 在这个位置放置车辆
                Debug.Log(gridPos);
                PlaceCar(manager, gridPos);
                e.Use(); // 防止事件进一步传播
            }
        }
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition, TrafficManager manager)
    {
        // 实现世界坐标到网格坐标的转换
        // 需要根据你的网格系统调整这部分逻辑
        // 以下是一个示例实现
        Vector3 localPos = manager.transform.InverseTransformPoint(worldPosition);
        int x = Mathf.FloorToInt(localPos.x / manager.CellWidth);
        int y = Mathf.FloorToInt(localPos.y / manager.CellHeight);
        return new Vector2Int(x, y);
    }

    private void PlaceCar(TrafficManager manager, Vector2Int gridPos)
    {
        // 实现在网格位置放置车辆的逻辑
        // 这可以是在Scene视图中绘制一个标记，或者实际实例化一个车辆预览
        // 以下是一个基本的实例化逻辑
        if (carPrefabs.arraySize > 0)
        {
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
