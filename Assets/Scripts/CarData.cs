using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarData
{
    public Vector2Int gridPosition;
    public int carPrefabIndex;

    public CarData(Vector2Int gridPos, int prefabIndex)
    {
        gridPosition = gridPos;
        carPrefabIndex = prefabIndex;
    }
}

