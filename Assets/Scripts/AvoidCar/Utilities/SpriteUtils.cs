using UnityEngine;

namespace AvoidCar.Common
{
    public static class SpriteUtils
    {
        // ����GameObject�ĳߴ�����Ӧָ���ĸ��Ӵ�С
        public static void AdjustSizeToFitCell(GameObject gameObject, float cellWidth, float cellHeight)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                float spriteWidth = spriteRenderer.sprite.bounds.size.x;
                float spriteHeight = spriteRenderer.sprite.bounds.size.y;

                // �������ű���
                float widthScale = cellWidth / spriteWidth;
                float heightScale = cellHeight / spriteHeight;
                float minScale = Mathf.Min(widthScale, heightScale);

                // �������ű���
                gameObject.transform.localScale = new Vector3(minScale, minScale, 1f);
            }
        }
    }
}
