using UnityEngine;

namespace Gino.ForgeAssetPack
{
    [CreateAssetMenu(fileName = "TileMapData", menuName = "Forge/TileMap Data")]
    public class TileMapData : ScriptableObject
    {
        public int width  = 10;
        public int height = 8;
        public int[] tiles;

        void OnEnable()
        {
            if (tiles == null || tiles.Length != width * height)
                tiles = new int[width * height];
        }

        public int GetTile(int x, int y)
        {
            if (tiles == null || x < 0 || x >= width || y < 0 || y >= height) return 0;
            return tiles[y * width + x];
        }

        public void SetTile(int x, int y, int tileIndex)
        {
            if (tiles == null || x < 0 || x >= width || y < 0 || y >= height) return;
            tiles[y * width + x] = tileIndex;
        }

        public void Resize(int newW, int newH)
        {
            var old  = tiles ?? new int[0];
            var next = new int[newW * newH];
            int minW = Mathf.Min(width, newW);
            int minH = Mathf.Min(height, newH);
            for (int y = 0; y < minH; y++)
            for (int x = 0; x < minW;  x++)
            {
                int oldIdx = y * width + x;
                if (oldIdx < old.Length)
                    next[y * newW + x] = old[oldIdx];
            }
            width  = newW;
            height = newH;
            tiles  = next;
        }
    }
}
