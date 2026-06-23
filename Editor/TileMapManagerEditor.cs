using UnityEditor;
using UnityEngine;

namespace Gino.ForgeAssetPack.Editor
{
    [CustomEditor(typeof(TileMapRenderer))]
    public class TileMapManagerEditor : UnityEditor.Editor
    {
        int  _selectedTile;
        bool _isPainting;

        static readonly string[] TileNames =
        {
            "空地",     "路徑右上", "路徑左曲", "角落樹叢",
            "樹叢左下", "密集樹叢", "路徑右側", "山樹右下",
            "斜路左",   "斜路+樹",  "山脈中",   "山脈右密",
            "森林稀",   "森林中",   "森林密",   "森林極密"
        };

        void OnEnable() => AutoAssignTilesheet();

        void AutoAssignTilesheet()
        {
            serializedObject.Update();
            var prop = serializedObject.FindProperty("_tilesheet");
            if (prop != null && prop.objectReferenceValue == null)
            {
                var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(
                    "Packages/com.ginotu.forge-asset-pack/Textures/Tiles/forest_tileset.png");
                if (tex != null)
                {
                    prop.objectReferenceValue = tex;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mapData"),
                new GUIContent("Map Data"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_tilesheet"),
                new GUIContent("Tilesheet"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("tileSize"),
                new GUIContent("Tile Size"));
            serializedObject.ApplyModifiedProperties();

            var renderer  = (TileMapRenderer)target;
            var tilesheet = serializedObject.FindProperty("_tilesheet").objectReferenceValue as Texture2D;
            var mapData   = renderer.mapData;

            if (mapData == null)
            {
                EditorGUILayout.HelpBox(
                    "請先建立 Map Data：右鍵 Assets → Create → Forge → TileMap Data，再拖入此欄位",
                    MessageType.Info);
                return;
            }

            if (mapData.tiles == null || mapData.tiles.Length != mapData.width * mapData.height)
            {
                Undo.RecordObject(mapData, "Init TileMap");
                mapData.tiles = new int[mapData.width * mapData.height];
                EditorUtility.SetDirty(mapData);
            }

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("地圖大小", EditorStyles.boldLabel);

            EditorGUI.BeginChangeCheck();
            int newW = EditorGUILayout.IntSlider("寬度", mapData.width,  2, 30);
            int newH = EditorGUILayout.IntSlider("高度", mapData.height, 2, 30);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(mapData, "Resize TileMap");
                mapData.Resize(newW, newH);
                EditorUtility.SetDirty(mapData);
                renderer.Render();
            }

            if (tilesheet == null) return;

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("選擇圖格", EditorStyles.boldLabel);
            DrawPalette(tilesheet);

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField(
                $"地圖繪製  {mapData.width} × {mapData.height}（點擊或拖曳塗抹）",
                EditorStyles.boldLabel);
            DrawMapGrid(renderer, mapData, tilesheet);

            EditorGUILayout.Space(8);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("重新渲染"))
                renderer.Render();
            if (GUILayout.Button("清空地圖"))
            {
                if (EditorUtility.DisplayDialog("清空地圖",
                        "確定要將所有格子重設為圖格 #0（空地）嗎？", "確定", "取消"))
                {
                    Undo.RecordObject(mapData, "Clear TileMap");
                    System.Array.Clear(mapData.tiles, 0, mapData.tiles.Length);
                    EditorUtility.SetDirty(mapData);
                    renderer.Render();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        // ── Palette ──────────────────────────────────────────────────────────

        void DrawPalette(Texture2D tilesheet)
        {
            float viewW  = EditorGUIUtility.currentViewWidth - 20f;
            int   cellPx = Mathf.Min(60, (int)(viewW / 4f));
            float totalH = cellPx * 4;

            var pal = GUILayoutUtility.GetRect(cellPx * 4, totalH + 18f);
            pal.height = totalH;

            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(pal, new Color(0.18f, 0.18f, 0.18f));

            for (int r = 0; r < 4; r++)
            for (int c = 0; c < 4; c++)
            {
                int idx      = r * 4 + c;
                var cellRect = new Rect(pal.x + c * cellPx, pal.y + r * cellPx, cellPx, cellPx);

                if (Event.current.type == EventType.Repaint)
                {
                    GUI.DrawTextureWithTexCoords(cellRect, tilesheet,
                        new Rect(c / 4f, (3 - r) / 4f, 0.25f, 0.25f));

                    if (_selectedTile == idx)
                        DrawBorder(cellRect, Color.yellow, 2);
                    else
                        DrawBorder(cellRect, new Color(0, 0, 0, 0.4f), 1);
                }

                if (Event.current.type == EventType.MouseDown &&
                    cellRect.Contains(Event.current.mousePosition))
                {
                    _selectedTile = idx;
                    Event.current.Use();
                    Repaint();
                }
            }

            string name = _selectedTile < TileNames.Length ? TileNames[_selectedTile] : "?";
            EditorGUI.LabelField(
                new Rect(pal.x, pal.yMax + 2f, pal.width, 16f),
                $"已選：#{_selectedTile}  {name}", EditorStyles.miniLabel);
        }

        // ── Map grid ─────────────────────────────────────────────────────────

        void DrawMapGrid(TileMapRenderer renderer, TileMapData mapData, Texture2D tilesheet)
        {
            float viewW  = EditorGUIUtility.currentViewWidth - 20f;
            int   cellPx = Mathf.Max(14, Mathf.Min(40, (int)(viewW / mapData.width)));
            float gridW  = mapData.width  * cellPx;
            float gridH  = mapData.height * cellPx;

            var grid = GUILayoutUtility.GetRect(gridW, gridH);

            if (Event.current.type == EventType.Repaint)
                EditorGUI.DrawRect(grid, new Color(0.12f, 0.12f, 0.12f));

            bool dirty = false;

            for (int y = 0; y < mapData.height; y++)
            for (int x = 0; x < mapData.width;  x++)
            {
                int idx      = mapData.GetTile(x, y);
                var cellRect = new Rect(grid.x + x * cellPx, grid.y + y * cellPx, cellPx, cellPx);

                if (Event.current.type == EventType.Repaint)
                {
                    int tc = idx % 4, tr = idx / 4;
                    GUI.DrawTextureWithTexCoords(cellRect, tilesheet,
                        new Rect(tc / 4f, (3 - tr) / 4f, 0.25f, 0.25f));
                    EditorGUI.DrawRect(new Rect(cellRect.xMax - 1, cellRect.y, 1, cellRect.height),
                        new Color(0, 0, 0, 0.3f));
                    EditorGUI.DrawRect(new Rect(cellRect.x, cellRect.yMax - 1, cellRect.width, 1),
                        new Color(0, 0, 0, 0.3f));
                }

                bool inCell = cellRect.Contains(Event.current.mousePosition);

                if (inCell && Event.current.type == EventType.MouseDown)
                {
                    _isPainting = true;
                    if (mapData.GetTile(x, y) != _selectedTile)
                    {
                        Undo.RecordObject(mapData, "Paint TileMap");
                        mapData.SetTile(x, y, _selectedTile);
                        dirty = true;
                    }
                    Event.current.Use();
                }
                else if (inCell && _isPainting && Event.current.type == EventType.MouseDrag)
                {
                    if (mapData.GetTile(x, y) != _selectedTile)
                    {
                        Undo.RecordObject(mapData, "Paint TileMap");
                        mapData.SetTile(x, y, _selectedTile);
                        dirty = true;
                    }
                    Event.current.Use();
                }
            }

            if (Event.current.type == EventType.MouseUp && _isPainting)
            {
                _isPainting = false;
                Event.current.Use();
            }

            if (dirty)
            {
                EditorUtility.SetDirty(mapData);
                renderer.Render();
                Repaint();
            }
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        static void DrawBorder(Rect r, Color c, int thickness)
        {
            EditorGUI.DrawRect(new Rect(r.x,          r.y,          r.width,    thickness), c);
            EditorGUI.DrawRect(new Rect(r.x,          r.yMax - thickness, r.width, thickness), c);
            EditorGUI.DrawRect(new Rect(r.x,          r.y,          thickness,  r.height),  c);
            EditorGUI.DrawRect(new Rect(r.xMax - thickness, r.y,   thickness,  r.height),  c);
        }
    }
}
