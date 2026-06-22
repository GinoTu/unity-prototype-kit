using System.Collections.Generic;
using UnityEngine;

namespace Gino.ForgeAssetPack
{
    [RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer))]
    public class DirectionalCharacterController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;

        [Header("Directional Sprites")]
        public Sprite spriteUp;
        public Sprite spriteDown;
        public Sprite spriteLeft;
        public Sprite spriteRight;

        private Rigidbody2D _rb;
        private SpriteRenderer _sr;

        // Keyboard input
        private Vector2 _keyInput;

        // Virtual button input (each active button adds its direction)
        private readonly HashSet<Vector2> _heldDirections = new();

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _sr = GetComponent<SpriteRenderer>();
        }

        void Update()
        {
            _keyInput = new Vector2(
                Input.GetAxisRaw("Horizontal"),
                Input.GetAxisRaw("Vertical")
            ).normalized;
        }

        void FixedUpdate()
        {
            var input = CombinedInput();
            _rb.linearVelocity = input * moveSpeed;
            if (input != Vector2.zero) UpdateSprite(input);
        }

        // ── Virtual button API (called by VirtualButton.cs) ─────────────────

        public void PressVirtualButton(Vector2 dir)   => _heldDirections.Add(dir);
        public void ReleaseVirtualButton(Vector2 dir) => _heldDirections.Remove(dir);

        // ── Helpers ──────────────────────────────────────────────────────────

        Vector2 CombinedInput()
        {
            var v = _keyInput;
            foreach (var dir in _heldDirections) v += dir;
            return v.normalized;
        }

        void UpdateSprite(Vector2 dir)
        {
            Sprite next;
            if (Mathf.Abs(dir.x) >= Mathf.Abs(dir.y))
                next = dir.x > 0 ? spriteRight : spriteLeft;
            else
                next = dir.y > 0 ? spriteUp : spriteDown;

            if (next != null) _sr.sprite = next;
        }
    }
}
