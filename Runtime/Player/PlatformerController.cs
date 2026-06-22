// =====================================================================
// PlatformerController — 2D 橫向平台跳躍控制器
//
// 使用方式：
//   1. 把這個腳本（或對應 Prefab）掛到角色 GameObject
//   2. 確保角色有 Rigidbody2D（Gravity Scale 建議 3-5）和 Collider2D
//   3. 設定 groundLayerMask，指定哪些 Layer 算地面
//   4. 角色站在地面上按 Jump（預設 Space / 手把 South Button）即可跳躍
//
// 可調參數（Inspector）：
//   moveSpeed       — 水平移動速度（預設 5）
//   jumpForce       — 跳躍初速度（預設 12）
//   groundCheckRadius — 腳底偵測半徑（預設 0.2）
//   coyoteTime      — 離地後仍可跳躍的寬限秒數（預設 0.15，設 0 關閉）
//   groundLayerMask — 指定地面 Layer
// =====================================================================

using UnityEngine;

namespace Gino.ForgeAssetPack
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlatformerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float jumpForce = 12f;

        [Header("Ground Check")]
        [SerializeField] private float groundCheckRadius = 0.2f;
        [SerializeField] private LayerMask groundLayerMask;
        [SerializeField] private Transform groundCheckPoint;

        [Header("Feel")]
        [SerializeField] private float coyoteTime = 0.15f;

        private Rigidbody2D _rb;
        private bool _isGrounded;
        private float _coyoteTimer;
        private bool _inputEnabled = true;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.freezeRotation = true;

            // 如果沒設 groundCheckPoint，用自身位置
            if (groundCheckPoint == null) groundCheckPoint = transform;
        }

        private void Update()
        {
            CheckGround();

            if (_coyoteTimer > 0f) _coyoteTimer -= Time.deltaTime;

            if (!_inputEnabled) return;

            float h = Input.GetAxisRaw("Horizontal");
            _rb.linearVelocity = new Vector2(h * moveSpeed, _rb.linearVelocity.y);

            if (Input.GetButtonDown("Jump") && _coyoteTimer > 0f)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
                _coyoteTimer = 0f;
            }
        }

        private void CheckGround()
        {
            _isGrounded = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayerMask);
            if (_isGrounded) _coyoteTimer = coyoteTime;
        }

        public void EnableInput(bool enabled) => _inputEnabled = enabled;
        public bool IsGrounded => _isGrounded;

        private void OnDrawGizmosSelected()
        {
            if (groundCheckPoint == null) return;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(groundCheckPoint.position, groundCheckRadius);
        }
    }
}
