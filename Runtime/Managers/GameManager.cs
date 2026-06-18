// =====================================================================
// GameManager — 遊戲狀態管理單例
//
// 使用方式：
//   1. 把 GameManager Prefab 拖入場景（只放一個）
//   2. 從其他腳本呼叫靜態方法：
//      GameManager.Instance.SetState(GameState.Paused)
//      GameManager.OnStateChanged += MyHandler;
//
// 狀態列表：
//   Playing | Paused | GameOver | Victory
// =====================================================================

using System;
using UnityEngine;

namespace Gino.PrototypeKit
{
    public enum GameState { Playing, Paused, GameOver, Victory }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }
        public static event Action<GameState> OnStateChanged;
        public static event Action<int> OnScoreChanged;

        public GameState CurrentState { get; private set; } = GameState.Playing;
        public int Score { get; private set; }

        protected virtual void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void SetState(GameState newState)
        {
            if (CurrentState == newState) return;
            CurrentState = newState;

            Time.timeScale = newState == GameState.Paused ? 0f : 1f;
            OnStateChanged?.Invoke(newState);
        }

        public void TogglePause()
        {
            SetState(CurrentState == GameState.Paused ? GameState.Playing : GameState.Paused);
        }

        // ── forge-build 相容方法（virtual 允許子類別 override）────────────────

        public virtual void WinGame()  => SetState(GameState.Victory);
        public virtual void GameOver() => SetState(GameState.GameOver);

        public virtual void AddScore(int amount)
        {
            Score += amount;
            OnScoreChanged?.Invoke(Score);
        }

        public virtual void ResetScore()
        {
            Score = 0;
            OnScoreChanged?.Invoke(Score);
        }

        public bool IsPlaying => CurrentState == GameState.Playing;

#if UNITY_EDITOR
        private void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F1)) AddScore(10);
            if (UnityEngine.Input.GetKeyDown(KeyCode.F2)) WinGame();
            if (UnityEngine.Input.GetKeyDown(KeyCode.F3)) GameOver();
        }
#endif
    }
}
