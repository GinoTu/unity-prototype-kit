# Gino Prototype Kit

快速起原型用的 Unity 6 UPM 套件，涵蓋俯視角 / 2D 平台 / 策略模擬三種遊戲類型的常用元件。

---

## 安裝方式

### 從 GitHub 安裝（推薦）

1. 開啟 Unity 專案
2. 上方選單 → **Window → Package Manager**
3. 左上角 **+** → **Add package from git URL**
4. 貼入：
   ```
   https://github.com/GinoTu/unity-prototype-kit.git#0.1.0
   ```
5. 點 **Add** → 安裝完成

### 安裝 Demo Scene（選用）

Package Manager → 找到 **Gino Prototype Kit** → 點開 **Samples** → Install **Demo**

---

## 模組列表

### 🎮 Player Controllers（`Runtime/Player/`）

| 腳本 | 用途 | 掛載對象 |
|:--|:--|:--|
| `TopDownController` | 俯視角八方向移動 | 角色 GameObject |
| `PlatformerController` | 2D 橫向平台跳躍 | 角色 GameObject |

**快速起手（TopDown）：**
```
1. 建一個空 GameObject，加 Rigidbody2D（Gravity Scale = 0）+ CircleCollider2D
2. 掛上 TopDownController
3. Play → WASD 移動
```

**快速起手（Platformer）：**
```
1. 建一個空 GameObject，加 Rigidbody2D（Gravity Scale = 3）+ BoxCollider2D
2. 掛上 PlatformerController
3. 在 groundLayerMask 欄位設定你的地板 Layer
4. Play → A/D 移動，Space 跳躍
```

---

### 🧩 Game Managers（`Runtime/Managers/`）

| 腳本 | 用途 | 取用方式 |
|:--|:--|:--|
| `GameManager` | 遊戲狀態管理（Playing / Paused / GameOver / Victory） | `GameManager.Instance.SetState(GameState.Paused)` |
| `SceneTransitionManager` | 場景切換 + 淡入淡出 | `SceneTransitionManager.Instance.LoadScene("SceneName")` |
| `SaveSystem` | 用 PlayerPrefs 存讀數據 | `SaveSystem.SetInt("score", 100)` |

**監聽遊戲狀態變化：**
```csharp
GameManager.OnStateChanged += (state) => {
    if (state == GameState.GameOver) ShowGameOverUI();
};
```

**存讀檔範例：**
```csharp
SaveSystem.SetInt("highScore", 9999);
int best = SaveSystem.GetInt("highScore"); // 9999
```

---

### 📺 UI Components（`Runtime/UI/`）

| 腳本 | 用途 | 關鍵方法 |
|:--|:--|:--|
| `HealthBar` | Slider 型血條 | `SetHealth(current, max)` |
| `DialogBox` | 逐字對話框 | `ShowDialog(string[])` |
| `HUDManager` | 統一管理 HUD 顯示 | `AddScore(10)` / `StartTimer()` |

**血條範例：**
```csharp
[SerializeField] private HealthBar healthBar;

void Start() => healthBar.Init(100f);
void OnHit() => healthBar.TakeDamage(20f);
```

**對話框範例：**
```csharp
dialogBox.ShowDialog(new string[] {
    "歡迎來到這個世界！",
    "記得存檔再走。"
});
DialogBox.OnDialogEnd += () => { /* 對話結束後的邏輯 */ };
```

---

## 最小可跑 Prototype 組合

```
場景階層：
├── GameManager (Prefab)
├── SceneTransitionManager (Prefab)
├── Player (掛 TopDownController)
└── Canvas
    └── HUDManager (Prefab)
        ├── HealthBar (Prefab)
        ├── ScoreText (TMP_Text)
        └── TimerText (TMP_Text)
```

---

## 已知限制

- `SaveSystem` 用 PlayerPrefs，資料存在本機，不適合正式遊戲上線
- `SceneTransitionManager` 需要 Canvas + 一個全螢幕 Image 才能做淡入淡出
- `HealthBar` 依賴 TextMeshPro（Unity 6 內建）

---

## Changelog

### 0.1.0 (2026-05-24)
- 初始版本
- TopDownController、PlatformerController
- GameManager、SceneTransitionManager、SaveSystem
- HealthBar、DialogBox、HUDManager
