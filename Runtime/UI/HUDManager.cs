// =====================================================================
// HUDManager — 統一管理遊戲 HUD 的顯示/隱藏
//
// 使用方式：
//   1. 把 HUDManager Prefab 拖入 Canvas 下
//   2. 指定各個 UI 群組到 Inspector 欄位
//   3. 呼叫：HUDManager.Instance.SetHUDVisible(false) 可整體隱藏
//   4. HUDManager 自動監聽 GameManager.OnStateChanged，暫停時隱藏 HUD
//
// 可調參數（Inspector）：
//   healthBar    — HealthBar 元件的引用
//   scoreText    — 分數文字（TMP_Text）
//   timerText    — 計時器文字（TMP_Text）
//   autoHideOnPause — 暫停時是否自動隱藏（預設 true）
// =====================================================================

using UnityEngine;
using TMPro;

namespace Gino.ForgeAssetPack
{
    public class HUDManager : MonoBehaviour
    {
        public static HUDManager Instance { get; private set; }

        [SerializeField] private HealthBar healthBar;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text timerText;
        [SerializeField] private bool autoHideOnPause = true;

        private int _score;
        private float _timer;
        private bool _timerRunning;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        private void OnEnable() => GameManager.OnStateChanged += HandleStateChange;
        private void OnDisable() => GameManager.OnStateChanged -= HandleStateChange;

        private void Update()
        {
            if (!_timerRunning) return;
            _timer += Time.deltaTime;
            if (timerText != null) timerText.text = FormatTime(_timer);
        }

        public void SetHUDVisible(bool visible) => gameObject.SetActive(visible);

        public void SetScore(int score)
        {
            _score = score;
            if (scoreText != null) scoreText.text = $"Score: {score}";
        }

        public void AddScore(int amount) => SetScore(_score + amount);

        public void StartTimer() => _timerRunning = true;
        public void StopTimer() => _timerRunning = false;
        public void ResetTimer() { _timer = 0f; if (timerText != null) timerText.text = "00:00"; }

        public HealthBar HealthBar => healthBar;

        private void HandleStateChange(GameState state)
        {
            if (!autoHideOnPause) return;
            if (state == GameState.Paused) SetHUDVisible(false);
            else if (state == GameState.Playing) SetHUDVisible(true);
        }

        private static string FormatTime(float seconds)
        {
            int m = (int)(seconds / 60);
            int s = (int)(seconds % 60);
            return $"{m:00}:{s:00}";
        }
    }
}
