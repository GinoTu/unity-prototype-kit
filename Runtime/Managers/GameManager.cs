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

        public GameState CurrentState { get; private set; } = GameState.Playing;

        private void Awake()
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

        public bool IsPlaying => CurrentState == GameState.Playing;
    }
}
