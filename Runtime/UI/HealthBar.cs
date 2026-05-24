// =====================================================================
// HealthBar — Slider 型血條
//
// 使用方式：
//   1. 拖入 HealthBar Prefab 到 Canvas 下
//   2. 從其他腳本呼叫：healthBar.SetHealth(currentHP, maxHP)
//   3. 也可用事件驅動：healthBar.Init(maxHP) 然後 healthBar.TakeDamage(10)
//
// 可調參數（Inspector）：
//   slider         — 指定 Unity UI Slider 元件
//   fillColor      — 血條顏色（預設紅色）
//   showText       — 是否顯示數字（例如 "80/100"）
// =====================================================================

using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Gino.PrototypeKit
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Image fillImage;
        [SerializeField] private Color fillColor = Color.red;
        [SerializeField] private bool showText = false;
        [SerializeField] private TMP_Text hpText;

        private float _maxHp;

        private void Awake()
        {
            if (fillImage != null) fillImage.color = fillColor;
        }

        public void Init(float maxHp)
        {
            _maxHp = maxHp;
            SetHealth(maxHp, maxHp);
        }

        public void SetHealth(float current, float max)
        {
            _maxHp = max;
            if (slider != null) slider.value = Mathf.Clamp01(current / max);
            UpdateText(current, max);
        }

        public void TakeDamage(float amount)
        {
            if (slider == null) return;
            float current = slider.value * _maxHp - amount;
            SetHealth(Mathf.Max(current, 0), _maxHp);
        }

        private void UpdateText(float current, float max)
        {
            if (!showText || hpText == null) return;
            hpText.text = $"{Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
    }
}
