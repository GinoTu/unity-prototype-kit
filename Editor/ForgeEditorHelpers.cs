using TMPro;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Gino.PrototypeKit.Editor
{
    /// <summary>
    /// Shared Editor helpers for forge-build generated scripts.
    /// Usage: add "using Gino.PrototypeKit.Editor;" to your SceneCreatorEditor / WireupEditor.
    /// </summary>
    public static class ForgeEditorHelpers
    {
        // ── Scene helpers ─────────────────────────────────────────────────────

        /// <summary>
        /// Add a Camera configured for 2D scenes (clearFlags = SolidColor, no Skybox).
        /// </summary>
        public static Camera AddCamera(float orthoSize = 5f, Color? bg = null)
        {
            var go  = new GameObject("Main Camera");
            go.tag  = "MainCamera";
            var cam = go.AddComponent<Camera>();
            cam.orthographic      = true;
            cam.orthographicSize  = orthoSize;
            cam.clearFlags        = CameraClearFlags.SolidColor;
            cam.backgroundColor   = bg ?? new Color(0.1f, 0.1f, 0.15f);
            cam.transform.position = new Vector3(0f, 0f, -10f);
            go.AddComponent<AudioListener>();
            return cam;
        }

        /// <summary>
        /// Add a full-screen ScreenSpaceOverlay Canvas + EventSystem.
        /// refW / refH come from GDD 一 (direct wording 1080×1920 portrait, 1920×1080 landscape).
        /// </summary>
        public static (Transform canvasTf, GameObject eventSystem) AddCanvasAndES(
            float refW = 1080f, float refH = 1920f)
        {
            var cGO = new GameObject("Canvas");
            var c   = cGO.AddComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;

            var sc = cGO.AddComponent<CanvasScaler>();
            sc.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            sc.referenceResolution = new Vector2(refW, refH);
            sc.matchWidthOrHeight  = 0.5f;

            cGO.AddComponent<GraphicRaycaster>();

            var esGO = new GameObject("EventSystem");
            esGO.AddComponent<EventSystem>();
            esGO.AddComponent<StandaloneInputModule>();
            return (cGO.transform, esGO);
        }

        // ── UI element builders ───────────────────────────────────────────────

        /// <summary>Create a UI Button from GDD 七 anchor table row.</summary>
        public static Button MakeButton(Transform parent, string label,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 anchoredPos, Vector2 size,
            Color? btnColor = null, Sprite sprite = null)
        {
            var go = new GameObject(label.Replace(" ", "") + "Button");
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin        = anchorMin;
            rt.anchorMax        = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta        = size;

            var img = go.AddComponent<Image>();
            if (sprite != null) img.sprite = sprite;
            else img.color = btnColor ?? new Color(0.25f, 0.5f, 1f);

            var btn = go.AddComponent<Button>();

            var tGO = new GameObject("Text");
            tGO.transform.SetParent(go.transform, false);
            var trt = tGO.AddComponent<RectTransform>();
            trt.anchorMin  = Vector2.zero;
            trt.anchorMax  = Vector2.one;
            trt.offsetMin  = trt.offsetMax = Vector2.zero;
            var tmp = tGO.AddComponent<TextMeshProUGUI>();
            tmp.text      = label;
            tmp.fontSize  = 48;
            tmp.color     = Color.white;
            tmp.alignment = TextAlignmentOptions.Center;

            return btn;
        }

        /// <summary>Create a TMP label from GDD 七 anchor table row.</summary>
        public static TextMeshProUGUI MakeLabel(Transform parent, string text,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 anchoredPos, Vector2 size,
            int fontSize = 60, Color? color = null)
        {
            var go = new GameObject(text.Replace(" ", "") + "Label");
            go.transform.SetParent(parent, false);

            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin        = anchorMin;
            rt.anchorMax        = anchorMax;
            rt.anchoredPosition = anchoredPos;
            rt.sizeDelta        = size;

            var tmp = go.AddComponent<TextMeshProUGUI>();
            tmp.text      = text;
            tmp.fontSize  = fontSize;
            tmp.color     = color ?? Color.white;
            tmp.alignment = TextAlignmentOptions.Center;
            return tmp;
        }

        /// <summary>Create a container panel from GDD 七 anchor table row.</summary>
        public static RectTransform MakePanel(Transform parent, string name,
            Vector2 anchorMin, Vector2 anchorMax,
            Vector2 offsetMin = default, Vector2 offsetMax = default)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin  = anchorMin;
            rt.anchorMax  = anchorMax;
            rt.offsetMin  = offsetMin;
            rt.offsetMax  = offsetMax;
            return rt;
        }

        // ── VirtualButton setup ───────────────────────────────────────────────

        /// <summary>
        /// Create four directional VirtualButton Image objects inside a parent transform.
        /// Used when GDD specifies on-screen directional controls.
        /// Requires Gino Prototype Kit installed (VirtualButton.cs must be compiled).
        /// </summary>
        public static void MakeDirectionalPad(Transform parent,
            Vector2 dpadAnchorMin, Vector2 dpadAnchorMax, Vector2 dpadAnchoredPos,
            float btnSize = 160f, float btnSpacing = 170f)
        {
            var dpadGO = new GameObject("DirectionalPad");
            dpadGO.transform.SetParent(parent, false);
            var dpadRt = dpadGO.AddComponent<RectTransform>();
            dpadRt.anchorMin        = dpadAnchorMin;
            dpadRt.anchorMax        = dpadAnchorMax;
            dpadRt.anchoredPosition = dpadAnchoredPos;
            dpadRt.sizeDelta        = new Vector2(btnSize + btnSpacing * 2f, btnSize + btnSpacing * 2f);

            var btnType = FindTypeBySimpleName("VirtualButton");

            (string name, Vector2 dir, Vector2 pos, string spriteName)[] btns =
            {
                ("Up",    Vector2.up,    new Vector2(0f,  btnSpacing), "button_up"),
                ("Down",  Vector2.down,  new Vector2(0f, -btnSpacing), "button_down"),
                ("Left",  Vector2.left,  new Vector2(-btnSpacing, 0f), "button_left"),
                ("Right", Vector2.right, new Vector2( btnSpacing, 0f), "button_right"),
            };

            foreach (var (btnName, dir, pos, spriteName) in btns)
            {
                var go = new GameObject(btnName + "Button");
                go.transform.SetParent(dpadGO.transform, false);
                var rt = go.AddComponent<RectTransform>();
                rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = pos;
                rt.sizeDelta        = new Vector2(btnSize, btnSize);

                var img = go.AddComponent<Image>();
                var kitSprite = AssetDatabase.LoadAssetAtPath<Sprite>(
                    $"Packages/com.ginotu.prototype-kit/Textures/UI/{spriteName}.png");
                if (kitSprite != null) img.sprite = kitSprite;

                if (btnType != null)
                {
                    var vb = go.AddComponent(btnType);
                    var so = new UnityEditor.SerializedObject(vb);
                    so.FindProperty("direction").vector2Value = dir;
                    so.ApplyModifiedProperties();
                }
            }
        }

        // ── Verify Setup ─────────────────────────────────────────────────────

        /// <summary>
        /// Run before /forge-build to confirm environment is ready.
        /// Menu: Forge > Verify Setup
        /// </summary>
        /// <summary>
        /// Menu entry — runs checks and shows a dialog on failure.
        /// For programmatic use (abort on failure), call VerifySetupChecks() instead.
        /// </summary>
        [MenuItem("Forge/Verify Setup")]
        public static void VerifySetup()
        {
            bool ok = VerifySetupChecks();
            if (!ok)
                EditorUtility.DisplayDialog(
                    "Forge Setup Failed",
                    "請查看 Console 視窗中的紅色 Error，依指示修正後再執行 /forge-build。",
                    "OK");
        }

        /// <summary>
        /// Returns true if all pre-conditions are met; false otherwise (errors logged).
        /// Call this from SetupEditor.Initialize() to abort early on bad environment.
        /// </summary>
        public static bool VerifySetupChecks()
        {
            int errors = 0;

            // Check 1: Kit in manifest.json
            const string manifestPath = "Packages/manifest.json";
            if (System.IO.File.Exists(manifestPath))
            {
                if (System.IO.File.ReadAllText(manifestPath).Contains("com.ginotu.prototype-kit"))
                    Debug.Log("[ForgeVerify] ✅ Kit 已安裝");
                else
                {
                    Debug.LogError(
                        "[ForgeVerify] ❌ Kit 未安裝：manifest.json 缺少 com.ginotu.prototype-kit\n" +
                        "修正：在 Packages/manifest.json 的 dependencies 加入：\n" +
                        "  \"com.ginotu.prototype-kit\": " +
                        "\"https://github.com/GinoTu/unity-prototype-kit.git#0.2.0\"");
                    errors++;
                }
            }

            // Check 2: Required Tags
            var tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");

            foreach (string tag in new[] { "Player", "Enemy", "Pickup" })
            {
                bool found = false;
                for (int i = 0; i < tagsProp.arraySize; i++)
                    if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag) { found = true; break; }

                if (!found)
                {
                    Debug.LogError(
                        $"[ForgeVerify] ❌ Tag 缺失：\"{tag}\"\n" +
                        "修正：執行 Tools/[GameName]/1. 初始化");
                    errors++;
                }
            }
            if (errors == 0) Debug.Log("[ForgeVerify] ✅ Tags 完整（Player / Enemy / Pickup）");

            // Check 3: TextMeshPro Settings asset (more reliable than directory check)
            if (AssetDatabase.FindAssets("t:TMP_Settings").Length > 0)
                Debug.Log("[ForgeVerify] ✅ TextMeshPro 已導入");
            else
            {
                Debug.LogError(
                    "[ForgeVerify] ❌ TextMeshPro Resources 未導入\n" +
                    "修正：執行 Tools/[GameName]/1. 初始化（會自動呼叫 TMP_PackageUtilities.ImportProjectResourcesMenu）");
                errors++;
            }

            if (errors == 0)
                Debug.Log("[ForgeVerify] ✅ 所有項目通過，環境就緒。");
            else
                Debug.LogError($"[ForgeVerify] ❌ 發現 {errors} 個問題，請修正後重試。");

            return errors == 0;
        }

        // ── Type search ───────────────────────────────────────────────────────

        /// <summary>
        /// Find a Type by simple class name (ignores namespace).
        /// Fixes the namespace issue when Kit scripts live in Gino.PrototypeKit.
        /// Prefer over asm.GetType(name) which requires the fully-qualified name.
        /// </summary>
        public static System.Type FindTypeBySimpleName(string typeName)
        {
            foreach (var asm in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                var t = asm.GetType(typeName);
                if (t != null) return t;

                foreach (var type in asm.GetTypes())
                    if (type.Name == typeName) return type;
            }
            return null;
        }

        /// <summary>
        /// Find a Button in the active scene by its TMP child text.
        /// More robust than finding by GameObject name.
        /// </summary>
        public static Button FindButtonByLabel(string label)
        {
            foreach (var btn in Object.FindObjectsByType<Button>(FindObjectsSortMode.None))
            {
                var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
                if (tmp != null && tmp.text == label) return btn;
            }
            Debug.LogWarning(
                $"[PrototypeKit] FindButtonByLabel: \"{label}\" not found. " +
                "Ensure MakeButton label matches exactly.");
            return null;
        }

        // ── Placeholder sprite ────────────────────────────────────────────────

        /// <summary>
        /// Resolve a sprite for a given entity type.
        /// Returns Kit sprite if installed, falls back to Unity built-in UISprite.
        /// entity: "player", "enemy", "ground", "collectible", "bullet"
        /// </summary>
        public static Sprite ResolveSprite(string entity)
        {
            string kitPath = entity.ToLower() switch
            {
                "player"      => "Packages/com.ginotu.prototype-kit/Textures/Characters/character_front.png",
                "enemy"       => "Packages/com.ginotu.prototype-kit/Textures/Enemies/enemy.png",
                "coin"        => "Packages/com.ginotu.prototype-kit/Textures/Items/coin.png",
                "star"        => "Packages/com.ginotu.prototype-kit/Textures/Items/star.png",
                "bomb"        => "Packages/com.ginotu.prototype-kit/Textures/Items/bomb.png",
                "grass"       => "Packages/com.ginotu.prototype-kit/Textures/Tiles/grass.png",
                _             => null
            };

            if (kitPath != null)
            {
                var s = AssetDatabase.LoadAssetAtPath<Sprite>(kitPath);
                if (s != null) return s;
            }

            return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        }
    }
}
