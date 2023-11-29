using UnityEditor;
using UnityEngine;

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
                // ת���㵽��������
                Vector2Int gridPos = WorldToGrid(point, manager);
                // �����λ�÷��ó���
                Debug.Log(gridPos);
                PlaceCar(manager, gridPos);
                e.Use(); // ��ֹ�¼���һ������
            }
        }
    }

    private Vector2Int WorldToGrid(Vector3 worldPosition, TrafficManager manager)
    {
        // ʵ���������굽���������ת��
        // ��Ҫ�����������ϵͳ�����ⲿ���߼�
        // ������һ��ʾ��ʵ��
        Vector3 localPos = manager.transform.InverseTransformPoint(worldPosition);
        int x = Mathf.FloorToInt(localPos.x / manager.CellWidth);
        int y = Mathf.FloorToInt(localPos.y / manager.CellHeight);
        return new Vector2Int(x, y);
    }

    private void PlaceCar(TrafficManager manager, Vector2Int gridPos)
    {
        // ʵ��������λ�÷��ó������߼�
        // ���������Scene��ͼ�л���һ����ǣ�����ʵ��ʵ����һ������Ԥ��
        // ������һ��������ʵ�����߼�
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
        // ʵ�ֱ���ؿ����߼�
        // ��������Ҫ�ռ����еĳ������ݲ�����
        TrafficManager manager = (TrafficManager)target;
        // ʾ����manager.SaveLevelData(�ռ��Ĺؿ�����, "����·��");
    }
}
