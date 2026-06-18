// =====================================================================
// TopDownController — 俯視角八方向移動控制器
//
// 使用方式：
//   1. 把這個腳本（或對應 Prefab）掛到角色 GameObject
//   2. 確保角色有 Rigidbody2D（Gravity Scale = 0）和 Collider2D
//   3. 呼叫 EnableInput(false) 可以暫時停用玩家輸入
//
// 可調參數（Inspector）：
//   moveSpeed     — 移動速度（預設 5）
//   useSmoothMove — 開啟加速/減速平滑（預設 false，Prototype 用 false 最直接）
// =====================================================================

using System.Collections.Generic;
using UnityEngine;

namespace Gino.PrototypeKit
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class TopDownController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private bool useSmoothMove = false;
        [SerializeField] private float smoothTime = 0.1f;

        private Rigidbody2D _rb;
        private Vector2 _keyInput;
        private Vector2 _smoothVelocity;
        private bool _inputEnabled = true;

        private readonly HashSet<Vector2> _heldDirections = new();

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
        }

        private void Update()
        {
            if (!_inputEnabled) return;
            _keyInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        }

        private void FixedUpdate()
        {
            var input = CombinedInput();
            if (useSmoothMove)
                _rb.linearVelocity = Vector2.SmoothDamp(_rb.linearVelocity, input * moveSpeed, ref _smoothVelocity, smoothTime);
            else
                _rb.linearVelocity = input * moveSpeed;
        }

        // ── Virtual button API (called by VirtualButton.cs) ──────────────────
        public void PressVirtualButton(Vector2 dir)   => _heldDirections.Add(dir);
        public void ReleaseVirtualButton(Vector2 dir) => _heldDirections.Remove(dir);

        public void EnableInput(bool enabled) => _inputEnabled = enabled;
        public Vector2 MoveDirection => CombinedInput();

        Vector2 CombinedInput()
        {
            var v = _keyInput;
            foreach (var dir in _heldDirections) v += dir;
            return v.normalized;
        }
    }
}
