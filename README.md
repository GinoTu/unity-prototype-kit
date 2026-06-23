# Forge Asset Pack

快速起原型用的 Unity 6 UPM 套件。涵蓋俯視角 / 方向性 2D 兩種角色控制器、遊戲狀態管理、虛擬搖桿 UI、手繪美術素材，以及與 `/forge-build` 自動化工具整合的 Editor Helper。

---

## 安裝方式

### 從 GitHub 安裝（推薦）

1. 開啟 Unity 專案
2. 上方選單 → **Window → Package Manager**
3. 左上角 **+** → **Add package from git URL**
4. 貼入：
   ```
   https://github.com/GinoTu/forge-asset-pack.git#0.3.1
   ```
5. 點 **Add** → 安裝完成

安裝後點選單 **Forge > Verify Setup** 確認環境就緒（Kit 已裝、Tags 存在、TMP 已導入）。

---

## 模組列表

### 🎮 Player Controllers（`Runtime/Player/`）

| 腳本 | 用途 | 掛載對象 |
|:--|:--|:--|
| `TopDownController` | 俯視角八方向移動，支援鍵盤與虛擬搖桿 | 角色 GameObject |
| `DirectionalCharacterController` | 方向性移動 + 自動切換四方向 Sprite | 角色 GameObject |
| `PlatformerController` | 2D 橫向平台跳躍 | 角色 GameObject |

**TopDown 快速起手：**
```
1. 建空 GameObject，加 Rigidbody2D（Gravity Scale = 0）+ CircleCollider2D
2. 掛上 TopDownController
3. Play → WASD 移動
```

**DirectionalCharacter 快速起手：**
```
1. 執行選單 Forge Asset Pack > Create Directional Character Prefab
2. 把生成的 Prefab 拖進場景
3. Play → WASD 移動，Sprite 自動切換方向
```

---

### 🧩 Game Managers（`Runtime/Managers/`）

| 腳本 | 用途 | 取用方式 |
|:--|:--|:--|
| `GameManager` | 遊戲狀態（Playing / Paused / GameOver / Victory）、分數 | `GameManager.Instance.AddScore(10)` |
| `SceneTransitionManager` | 場景切換 + 淡入淡出 | `SceneTransitionManager.Instance.LoadScene("SceneName")` |
| `SaveSystem` | PlayerPrefs 存讀 | `SaveSystem.SetInt("score", 100)` |

`GameManager` 所有核心方法都是 `virtual`，可直接繼承擴充：
```csharp
public class MyGameManager : GameManager
{
    public override void WinGame()
    {
        base.WinGame();
        ShowWinEffect();
    }
}
```

監聽狀態與分數變化：
```csharp
GameManager.OnStateChanged += state => { if (state == GameState.GameOver) ShowUI(); };
GameManager.OnScoreChanged += score => scoreText.text = score.ToString();
```

---

### 📱 UI Components（`Runtime/UI/`）

| 腳本 | 用途 | 說明 |
|:--|:--|:--|
| `VirtualButton` | 螢幕虛擬方向鍵 | 掛在 Image 上，自動連接 TopDownController / DirectionalCharacterController |
| `HealthBar` | Slider 型血條 | `SetHealth(current, max)` |
| `DialogBox` | 逐字對話框 | `ShowDialog(string[])` |
| `HUDManager` | 統一管理 HUD | `AddScore(10)` / `StartTimer()` |

**虛擬方向鍵設定：**
```
1. 建一個 Image GameObject
2. 掛上 VirtualButton
3. 設定 direction（Vector2.up / down / left / right）
4. 場景裡有 TopDownController 或 DirectionalCharacterController 時自動連接
```

---

### 🎨 手繪素材（`Textures/`）

| 分類 | 檔案 |
|:--|:--|
| Characters | character_front / back / left / right / topdown |
| Enemies | enemy |
| Items | bomb / coin / star / blue_element / green_element / red_element / yellow_element |
| Tiles | forest_tileset（4×4 手繪森林格子集，16 種地形）|
| UI | button_up / button_down / button_left / button_right / bar / bar外框 |
| Weapons | 刀 / 法杖 |

執行 **Forge Asset Pack > Create Prefabs from Sprites** 可一鍵生成所有素材的 Prefab。

---

### 🗺️ TileMap System（`Runtime/TileMap/`）

| 腳本 | 用途 |
|:--|:--|
| `TileMapData` | ScriptableObject，儲存地圖寬高與格子索引陣列 |
| `TileMapRenderer` | 掛在 GameObject 上，自動把地圖渲染為 SpriteRenderer 子物件 |

**快速起手：**
```
1. 右鍵 Assets → Create → Forge → TileMap Data，建立地圖資料
2. 在場景建空 GameObject，掛上 TileMapRenderer
3. 把 TileMapData 拖進 Map Data 欄位
4. Inspector 出現 4×4 圖格調色盤 → 點選圖格，點擊地圖格子塗抹
5. 用寬度/高度 Slider 調整地圖大小
```

---

### 🔧 Editor Helpers（`Editor/`）（與 `/forge-build` 整合）

`ForgeEditorHelpers` 提供 Editor 腳本可直接呼叫的工具方法：

| 方法 | 說明 |
|:--|:--|
| `AddCamera(orthoSize, bg)` | 建立 2D Camera（SolidColor clearFlags） |
| `AddCanvasAndES(refW, refH)` | 建立 ScreenSpaceOverlay Canvas + EventSystem |
| `MakeButton(...)` | 從 GDD Anchor 表格建立 TMP Button |
| `MakeLabel(...)` | 建立 TMP 文字標籤 |
| `MakePanel(...)` | 建立容器 Panel |
| `MakeDirectionalPad(...)` | 一鍵生成四方向虛擬搖桿 |
| `ResolveSprite(entity)` | 依實體名稱（"player" / "enemy" 等）回傳 Kit Sprite |
| `FindTypeBySimpleName(name)` | 跨 Namespace 尋找 Type（解決反射限制） |
| `FindButtonByLabel(label)` | 用 TMP 文字尋找場景中的 Button |
| `VerifySetup()` | 選單觸發版（**Forge > Verify Setup**），失敗時顯示 Dialog |
| `VerifySetupChecks()` | 回傳 `bool`，供 Editor 腳本在開頭 abort-on-fail |

**在生成的 Editor 腳本中使用：**
```csharp
using Gino.ForgeAssetPack.Editor;

public static class MySceneCreator
{
    public static void Build()
    {
        if (!ForgeEditorHelpers.VerifySetupChecks()) return;
        var cam = ForgeEditorHelpers.AddCamera(5f);
        var (canvas, es) = ForgeEditorHelpers.AddCanvasAndES(1080f, 1920f);
        // ...
    }
}
```

---

## 選單總覽

| 選單路徑 | 功能 |
|:--|:--|
| **Forge > Verify Setup** | 環境預檢（Kit / Tags / TMP） |
| **Forge Asset Pack > Create Prefabs from Sprites** | 所有素材一鍵生成 Prefab |
| **Forge Asset Pack > Create Directional Character Prefab** | 四方向角色 Prefab |
| **Forge Asset Pack > Create TopDown Character Prefab** | 俯視角角色 Prefab |

---

## 已知限制

- `SaveSystem` 用 PlayerPrefs，不適合正式遊戲上線
- `SceneTransitionManager` 需要 Canvas + 全螢幕 Image 才能淡入淡出
- `HealthBar` / TMP 元件依賴 TextMeshPro（Unity 6 內建，需先導入 Resources）

---

## Changelog

### 0.3.1 (2026-06-23)
- **新增** `TileMapData`（ScriptableObject）＋ `TileMapRenderer`（ExecuteAlways）：完整的手繪格子地圖系統
- **新增** `TileMapManagerEditor`：可視化 4×4 圖格調色盤 + 點擊/拖曳塗抹地圖 + 地圖大小 Slider
- **新增** `Textures/Tiles/forest_tileset.png`：16 種手繪森林地形格子集
- **移除** 舊版 grass.png / water.png

### 0.3.0 (2026-06-22)
- **新增** `DirectionalCharacterController`：四方向 Sprite 切換 + 鍵盤 / 虛擬按鍵雙輸入
- **新增** `VirtualButton`：觸控螢幕虛擬方向鍵，自動連接兩種 Controller
- **更新** `GameManager`：WinGame / GameOver / AddScore / ResetScore 改為 `virtual`，支援繼承擴充
- **更新** `TopDownController`：加入虛擬按鍵 HashSet 輸入 + `CombinedInput` / `MoveDirection`
- **新增** `ForgeEditorHelpers`：完整 Editor Helper 庫，含 `VerifySetupChecks()` 環境驗證
- **新增** `PrefabCreatorEditor`：選單一鍵從素材生成 Prefab
- **新增** 23 張手繪素材（Characters / Enemies / Items / Tiles / UI / Weapons）
- **新增** 所有檔案的 `.meta`（修正 UPM 安裝時資產被 ignore 的問題）

### 0.1.0 (2026-05-24)
- 初始版本：TopDownController、PlatformerController、GameManager、SceneTransitionManager、SaveSystem、HealthBar、DialogBox、HUDManager
