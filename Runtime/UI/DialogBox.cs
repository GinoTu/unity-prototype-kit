// =====================================================================
// DialogBox — 逐字顯示對話框
//
// 使用方式：
//   dialogBox.ShowDialog(new string[] { "你好！", "這是第二句。" });
//   按任意鍵（或呼叫 dialogBox.Next()）推進到下一句
//   對話結束後觸發 OnDialogEnd 事件
//
// 可調參數（Inspector）：
//   textComponent  — 顯示文字的 TMP_Text
//   charDelay      — 每個字出現的間隔（秒，預設 0.03）
//   panel          — 對話框的根 GameObject（顯示/隱藏用）
// =====================================================================

using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace Gino.PrototypeKit
{
    public class DialogBox : MonoBehaviour
    {
        public static event Action OnDialogEnd;

        [SerializeField] private TMP_Text textComponent;
        [SerializeField] private float charDelay = 0.03f;
        [SerializeField] private GameObject panel;

        private string[] _lines;
        private int _currentLine;
        private bool _isTyping;
        private Coroutine _typingCoroutine;

        public bool IsActive { get; private set; }

        public void ShowDialog(string[] lines)
        {
            _lines = lines;
            _currentLine = 0;
            IsActive = true;
            if (panel != null) panel.SetActive(true);
            ShowLine(_currentLine);
        }

        public void Next()
        {
            if (!IsActive) return;

            if (_isTyping)
            {
                // 跳過打字動畫，直接顯示全文
                if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
                textComponent.text = _lines[_currentLine];
                _isTyping = false;
                return;
            }

            _currentLine++;
            if (_currentLine >= _lines.Length)
            {
                Close();
                return;
            }
            ShowLine(_currentLine);
        }

        private void Update()
        {
            if (IsActive && Input.GetMouseButtonDown(0)) Next();
        }

        private void ShowLine(int index)
        {
            if (_typingCoroutine != null) StopCoroutine(_typingCoroutine);
            _typingCoroutine = StartCoroutine(TypeLine(_lines[index]));
        }

        private IEnumerator TypeLine(string line)
        {
            _isTyping = true;
            textComponent.text = "";
            foreach (char c in line)
            {
                textComponent.text += c;
                yield return new WaitForSeconds(charDelay);
            }
            _isTyping = false;
        }

        private void Close()
        {
            IsActive = false;
            if (panel != null) panel.SetActive(false);
            OnDialogEnd?.Invoke();
        }
    }
}
