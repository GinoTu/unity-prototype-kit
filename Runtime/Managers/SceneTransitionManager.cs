// =====================================================================
// SceneTransitionManager — 場景切換（含淡入淡出）
//
// 使用方式：
//   1. 把 SceneTransitionManager Prefab 拖入場景
//   2. Prefab 需包含一個全螢幕黑色 UI Image（指定到 fadeImage 欄位）
//   3. 呼叫：SceneTransitionManager.Instance.LoadScene("SceneName")
//   4. 重新載入當前場景：SceneTransitionManager.Instance.ReloadCurrentScene()
//
// 可調參數（Inspector）：
//   fadeDuration — 淡入/淡出秒數（預設 0.3）
//   fadeImage    — 負責遮黑的 UI Image 元件
// =====================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Gino.ForgeAssetPack
{
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [SerializeField] private Image fadeImage;
        [SerializeField] private float fadeDuration = 0.3f;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (fadeImage != null)
                fadeImage.color = new Color(0, 0, 0, 0);
        }

        public void LoadScene(string sceneName) => StartCoroutine(Transition(sceneName));
        public void ReloadCurrentScene() => LoadScene(SceneManager.GetActiveScene().name);

        private IEnumerator Transition(string sceneName)
        {
            yield return StartCoroutine(Fade(1f));
            SceneManager.LoadScene(sceneName);
            yield return StartCoroutine(Fade(0f));
        }

        private IEnumerator Fade(float targetAlpha)
        {
            if (fadeImage == null) yield break;

            float startAlpha = fadeImage.color.a;
            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                float a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
                fadeImage.color = new Color(0, 0, 0, a);
                yield return null;
            }
            fadeImage.color = new Color(0, 0, 0, targetAlpha);
        }
    }
}
