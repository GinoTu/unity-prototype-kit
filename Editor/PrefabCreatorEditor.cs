using System.IO;
using UnityEditor;
using UnityEngine;

namespace Gino.ForgeAssetPack.Editor
{
    public static class PrefabCreatorEditor
    {
        private const string TexturesRoot = "Packages/com.ginotu.forge-asset-pack/Textures";
        private const string PrefabOutput  = "Assets/Prefabs/PrototypeKit";

        [MenuItem("Forge Asset Pack/Create Prefabs from Sprites")]
        public static void CreateAllPrefabs()
        {
            EnsureFolder("Assets/Prefabs", "PrototypeKit");

            string[] categories = { "Characters", "Enemies", "Items", "Tiles", "UI", "Weapons" };
            int total = 0;

            foreach (string cat in categories)
            {
                string folderPath = $"{TexturesRoot}/{cat}";
                string[] guids = AssetDatabase.FindAssets("t:Texture2D", new[] { folderPath });
                if (guids.Length == 0) continue;

                string outFolder = $"{PrefabOutput}/{cat}";
                EnsureFolder(PrefabOutput, cat);

                foreach (string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    SetSpriteImportMode(assetPath);

                    Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                    if (sprite == null) continue;

                    string prefabName = Path.GetFileNameWithoutExtension(assetPath);
                    BuildPrefab(sprite, cat, outFolder, prefabName);
                    total++;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[ForgeAssetPack] {total} prefabs created at {PrefabOutput}");
        }

        // ── Texture import ──────────────────────────────────────────────────

        static void SetSpriteImportMode(string assetPath)
        {
            var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
            if (importer == null) return;
            if (importer.textureType == TextureImporterType.Sprite) return;

            importer.textureType        = TextureImporterType.Sprite;
            importer.spriteImportMode   = SpriteImportMode.Single;
            importer.alphaIsTransparency = true;
            importer.filterMode         = FilterMode.Bilinear;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        }

        // ── Prefab builder ──────────────────────────────────────────────────

        static void BuildPrefab(Sprite sprite, string category, string outFolder, string name)
        {
            var go = new GameObject(name);

            switch (category)
            {
                case "Characters": SetupCharacter(go, sprite); break;
                case "Enemies":    SetupEnemy(go, sprite);     break;
                case "Items":      SetupItem(go, sprite);      break;
                case "Tiles":      SetupTile(go, sprite);      break;
                case "UI":         SetupUISprite(go, sprite);  break;
                case "Weapons":    SetupWeapon(go, sprite);    break;
            }

            string prefabPath = $"{outFolder}/{name}.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);
        }

        // ── Component setups ────────────────────────────────────────────────

        static void SetupCharacter(GameObject go, Sprite sprite)
        {
            go.tag = "Player";
            go.layer = LayerMask.NameToLayer("Default");

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale  = 0f;
            rb.freezeRotation = true;

            go.AddComponent<CircleCollider2D>();
        }

        static void SetupEnemy(GameObject go, Sprite sprite)
        {
            go.tag = "Enemy";

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.freezeRotation = true;

            go.AddComponent<CircleCollider2D>();
        }

        static void SetupItem(GameObject go, Sprite sprite)
        {
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            var col = go.AddComponent<CircleCollider2D>();
            col.isTrigger = true;
        }

        static void SetupTile(GameObject go, Sprite sprite)
        {
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            go.AddComponent<BoxCollider2D>();
        }

        static void SetupUISprite(GameObject go, Sprite sprite)
        {
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
        }

        static void SetupWeapon(GameObject go, Sprite sprite)
        {
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;
        }

        // ── Directional Character Prefab ────────────────────────────────────

        [MenuItem("Forge Asset Pack/Create Directional Character Prefab")]
        public static void CreateDirectionalCharacterPrefab()
        {
            // Ensure sprites are imported
            string charFolder = $"{TexturesRoot}/Characters";
            foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { charFolder }))
                SetSpriteImportMode(AssetDatabase.GUIDToAssetPath(guid));

            Sprite spriteFront = LoadCharacterSprite("character_front");
            Sprite spriteBack  = LoadCharacterSprite("character_back");
            Sprite spriteLeft  = LoadCharacterSprite("character_left");
            Sprite spriteRight = LoadCharacterSprite("character_right");

            if (spriteFront == null)
            {
                Debug.LogError("[ForgeAssetPack] character_front.png not found. Run 'Create Prefabs from Sprites' first to import textures.");
                return;
            }

            EnsureFolder("Assets/Prefabs", "PrototypeKit");

            var go = new GameObject("DirectionalCharacter");
            go.tag = "Player";

            // Sprite (facing front by default)
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spriteFront;

            // Physics
            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale   = 0f;
            rb.freezeRotation = true;
            go.AddComponent<CircleCollider2D>();

            // Controller
            var ctrl = go.AddComponent<DirectionalCharacterController>();
            ctrl.spriteUp    = spriteBack;
            ctrl.spriteDown  = spriteFront;
            ctrl.spriteLeft  = spriteLeft;
            ctrl.spriteRight = spriteRight;

            string prefabPath = $"{PrefabOutput}/DirectionalCharacter.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[ForgeAssetPack] DirectionalCharacter prefab created at {prefabPath}");
        }

        [MenuItem("Forge Asset Pack/Create TopDown Character Prefab")]
        public static void CreateTopDownCharacterPrefab()
        {
            string charFolder = $"{TexturesRoot}/Characters";
            foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { charFolder }))
                SetSpriteImportMode(AssetDatabase.GUIDToAssetPath(guid));

            Sprite spriteTopDown = LoadCharacterSprite("character_topdown");

            if (spriteTopDown == null)
            {
                Debug.LogError("[ForgeAssetPack] character_topdown.png not found.");
                return;
            }

            EnsureFolder("Assets/Prefabs", "PrototypeKit");

            var go = new GameObject("TopDownCharacter");
            go.tag = "Player";

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = spriteTopDown;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale   = 0f;
            rb.freezeRotation = true;
            go.AddComponent<CircleCollider2D>();

            go.AddComponent<TopDownController>();

            string prefabPath = $"{PrefabOutput}/TopDownCharacter.prefab";
            PrefabUtility.SaveAsPrefabAsset(go, prefabPath);
            Object.DestroyImmediate(go);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"[ForgeAssetPack] TopDownCharacter prefab created at {prefabPath}");
        }

        static Sprite LoadCharacterSprite(string filename)
        {
            string path = $"{TexturesRoot}/Characters/{filename}.png";
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }

        // ── Utilities ───────────────────────────────────────────────────────

        static void EnsureFolder(string parent, string child)
        {
            string full = $"{parent}/{child}";
            if (!AssetDatabase.IsValidFolder(full))
                AssetDatabase.CreateFolder(parent, child);
        }
    }
}
