using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public int columns;
    private int rows;
    [SerializeField]
    private float cellSize;
    public GameObject boundary;
    public int numberOfLine;
    public GameObject linePrefabA;
    public GameObject linePrefabB;

    

    private void OnDrawGizmos()
    {
        Vector2 size = boundary.GetComponent<SpriteRenderer>().bounds.size;
        Vector2 position = boundary.transform.position;
        cellSize = size.x / columns;
        rows = Mathf.RoundToInt(size.y / cellSize);

        Gizmos.color = Color.red;

        // ���ƴ�ֱ�ߣ��У�
        for (int i = 0; i <= columns; i++)
        {
            float xPosition = position.x + i * cellSize - size.x / 2;
            Gizmos.DrawLine(
                new Vector2(xPosition, position.y - size.y / 2),
                new Vector2(xPosition, position.y + size.y / 2));
        }

        // ����ˮƽ�ߣ��У�
        for (int j = 0; j <= rows; j++)
        {
            float yPosition = position.y - size.y / 2 + j * cellSize;
            if (j == rows && size.y % cellSize != 0) // �����һ�����⴦��
            {
                break;
            }
            Gizmos.DrawLine(
                new Vector2(position.x - size.x / 2, yPosition),
                new Vector2(position.x + size.x / 2, yPosition));
        }
    }
}
