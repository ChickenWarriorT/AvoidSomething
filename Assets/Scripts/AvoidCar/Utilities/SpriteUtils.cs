using UnityEngine;

namespace AvoidCar.Common
{
    public static class SpriteUtils
    {
        // 调整GameObject的尺寸以适应指定的格子大小
        public static void AdjustSizeToFitCell(GameObject gameObject, float cellWidth, float cellHeight)
        {
            SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                float spriteWidth = spriteRenderer.sprite.bounds.size.x;
                float spriteHeight = spriteRenderer.sprite.bounds.size.y;

                // 计算缩放比例
                float widthScale = cellWidth / spriteWidth;
                float heightScale = cellHeight / spriteHeight;
                float minScale = Mathf.Min(widthScale, heightScale);

                // 设置缩放比例
                gameObject.transform.localScale = new Vector3(minScale, minScale, 1f);
            }
        }
    }
}
