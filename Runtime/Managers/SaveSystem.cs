// =====================================================================
// SaveSystem — 簡易存讀檔（基於 PlayerPrefs）
//
// 使用方式：
//   SaveSystem.SetInt("score", 100);
//   int s = SaveSystem.GetInt("score");   // 回傳 100
//   SaveSystem.SetString("playerName", "Gino");
//   SaveSystem.DeleteAll();               // 清除所有存檔
//
// 注意：PlayerPrefs 適合 Prototype 驗證用，正式遊戲請改用 JSON 存檔
// =====================================================================

using UnityEngine;

namespace Gino.PrototypeKit
{
    public static class SaveSystem
    {
        public static void SetInt(string key, int value) => PlayerPrefs.SetInt(key, value);
        public static int GetInt(string key, int defaultValue = 0) => PlayerPrefs.GetInt(key, defaultValue);

        public static void SetFloat(string key, float value) => PlayerPrefs.SetFloat(key, value);
        public static float GetFloat(string key, float defaultValue = 0f) => PlayerPrefs.GetFloat(key, defaultValue);

        public static void SetString(string key, string value) => PlayerPrefs.SetString(key, value);
        public static string GetString(string key, string defaultValue = "") => PlayerPrefs.GetString(key, defaultValue);

        public static void SetBool(string key, bool value) => PlayerPrefs.SetInt(key, value ? 1 : 0);
        public static bool GetBool(string key, bool defaultValue = false) => PlayerPrefs.GetInt(key, defaultValue ? 1 : 0) == 1;

        public static bool HasKey(string key) => PlayerPrefs.HasKey(key);
        public static void DeleteKey(string key) => PlayerPrefs.DeleteKey(key);
        public static void DeleteAll() => PlayerPrefs.DeleteAll();
        public static void Save() => PlayerPrefs.Save();
    }
}
