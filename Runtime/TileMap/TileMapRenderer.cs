using UnityEngine;

namespace Gino.ForgeAssetPack
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    public class TileMapRenderer : MonoBehaviour
    {
        public TileMapData mapData;
        [SerializeField] Texture2D _tilesheet;
        public float tileSize = 1f;

        Sprite[] _sprites;

#if UNITY_EDITOR
        void Reset()
        {
            _tilesheet = UnityEditor.AssetDatabase.LoadAssetAtPath<Texture2D>(
                "Packages/com.ginotu.forge-asset-pack/Textures/Tiles/forest_tileset.png");
        }
#endif

        void OnEnable()  => Render();
        void OnDisable() => ClearCells();
        void OnDestroy() => ClearCells();

        void OnValidate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                UnityEditor.EditorApplication.delayCall += () => { if (this) Render(); };
#endif
        }

        public void Render()
        {
            ClearCells();
            if (mapData == null || _tilesheet == null) return;
            EnsureTiles();
            SliceSprites();

            float ox = -(mapData.width  * tileSize) * 0.5f + tileSize * 0.5f;
            float oy =  (mapData.height * tileSize) * 0.5f - tileSize * 0.5f;

            for (int y = 0; y < mapData.height; y++)
            for (int x = 0; x < mapData.width;  x++)
            {
                int idx   = mapData.GetTile(x, y);
                var cellGO = new GameObject($"tile_{x}_{y}");
                cellGO.hideFlags = HideFlags.DontSave | HideFlags.NotEditable;
                cellGO.transform.SetParent(transform, false);
                cellGO.transform.localPosition = new Vector3(
                    ox + x * tileSize,
                    oy - y * tileSize, 0f);
                var sr = cellGO.AddComponent<SpriteRenderer>();
                if (_sprites != null && idx >= 0 && idx < _sprites.Length)
                    sr.sprite = _sprites[idx];
            }
        }

        void ClearCells()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                var child = transform.GetChild(i).gameObject;
                if ((child.hideFlags & HideFlags.DontSave) != 0)
                    DestroyImmediate(child);
            }
        }

        void EnsureTiles()
        {
            if (mapData.tiles == null || mapData.tiles.Length != mapData.width * mapData.height)
                mapData.tiles = new int[mapData.width * mapData.height];
        }

        void SliceSprites()
        {
            if (_tilesheet == null) { _sprites = null; return; }
            int cw = _tilesheet.width  / 4;
            int ch = _tilesheet.height / 4;
            _sprites = new Sprite[16];
            for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
                _sprites[r * 4 + c] = Sprite.Create(
                    _tilesheet,
                    new Rect(c * cw, (3 - r) * ch, cw, ch),
                    new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
