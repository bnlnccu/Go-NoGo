# Go/No-Go 打地鼠 — 課堂教學指引

---

## 一、不寫程式就能調整的部分（Inspector 參數）

在 Hierarchy 面板中選取 **GoNoGoManager** 物件，Inspector 裡會看到以下可調整的參數。
滑鼠停留在參數名稱上會顯示 Tooltip 說明。

### GoNoGoManager（遊戲主控制器）

| 參數 | 預設值 | 範圍 | 認知心理學意義 |
|------|--------|------|---------------|
| Target Display Duration | 1.5 秒 | 0.5 ~ 5.0 | 地鼠停留時間。越短 → 時間壓力越大 |
| No-Go Ratio | 0.3 | 0.1 ~ 0.8 | No-Go 出現比例。經典設定 0.2~0.3，此時抑制最困難 |
| Spawn Interval | 1.0 秒 | 0.3 ~ 3.0 | 出現間隔。越短 → 認知負荷越高 |
| Go Hit Score | 10 | 1 ~ 20 | 正確敲擊得分 |
| No-Go Inhibit Score | 5 | 1 ~ 20 | 成功忍住得分 |
| No-Go Fail Penalty | 10 | 1 ~ 20 | 錯誤敲擊扣分 |
| Go Miss Penalty | 5 | 0 ~ 10 | 漏敲扣分 |

地鼠外觀有 4 個 Sprite 欄位可以直接拖入圖片替換（留空則用預設圖案）：

| 欄位 | 用途 |
|------|------|
| Go Mole Sprite | Go 地鼠外觀（應該敲的） |
| No-Go Mole Sprite | No-Go 地鼠外觀（不應該敲的） |
| Go Mole Hit Sprite | Go 地鼠被敲中後的樣子 |
| No-Go Mole Hit Sprite | No-Go 地鼠被誤敲後的樣子 |

### CountdownTimer（倒數計時器）

| 參數 | 預設值 | 說明 |
|------|--------|------|
| Count From | 3 | 從幾開始倒數（1~10） |
| Seconds Per Count | 1.0 | 每個數字停留幾秒（0.5~3.0） |
| Use Scale Animation | true | 是否顯示數字縮放動畫 |

### TrialCounter（試驗計數器）

| 參數 | 預設值 | 說明 |
|------|--------|------|
| Total Trials | 30 | 總共出現幾隻地鼠（5~200） |
| Display Format | `第 {0} / {1} 隻` | `{0}` = 目前回合，`{1}` = 總回合 |

### ScoreManager（計分管理器）

| 參數 | 預設值 | 說明 |
|------|--------|------|
| Display Format | `分數：{0}` | `{0}` = 目前分數 |

---

## 二、寫程式要改的檔案

學生只需要修改一個檔案：

```
Assets/Scripts/GoNoGo/GoNoGoManager.cs
```

這個檔案是遊戲的「規則中心」，裡面有 4 個事件方法，**每一個對應一種玩家行為**。
學生修改這些方法的內容，就能改變遊戲規則。

### 學生可以改的 4 個方法

打開 `GoNoGoManager.cs`，搜尋以下方法：

#### 1. `OnGoClicked` — 玩家正確敲擊 Go 地鼠時觸發

```csharp
// GoNoGoManager.cs 第 242 行附近
public void OnGoClicked(float reactionTimeMs)
{
    goHits++;
    scoreManager.AddScore(goHitScore);        // 加分
    rtRecorder.RecordTime(reactionTimeMs);    // 記錄反應時間
}
```

`reactionTimeMs` 是玩家從看到地鼠到點擊的時間（毫秒）。

#### 2. `OnMoleMissed` — Go 地鼠超時沒被敲到時觸發

```csharp
// GoNoGoManager.cs 第 250 行附近
public void OnMoleMissed()
{
    scoreManager.SubtractScore(goMissPenalty);   // 扣分
}
```

#### 3. `OnNoGoClicked` — 玩家錯誤敲擊 No-Go 地鼠時觸發

```csharp
// GoNoGoManager.cs 第 256 行附近
public void OnNoGoClicked(float reactionTimeMs)
{
    scoreManager.SubtractScore(noGoFailPenalty);  // 扣分
}
```

#### 4. `OnNoGoCorrectInhibition` — 玩家成功忍住沒敲 No-Go 地鼠時觸發

```csharp
// GoNoGoManager.cs 第 262 行附近
public void OnNoGoCorrectInhibition()
{
    noGoCorrectInhibitions++;
    scoreManager.AddScore(noGoInhibitScore);     // 加分
}
```

### 學生可以呼叫的工具

在上面 4 個方法裡面，學生可以使用以下工具：

#### ScoreManager（計分）

```csharp
scoreManager.AddScore(10);            // 加 10 分
scoreManager.SubtractScore(5);        // 扣 5 分（不會低於 0）
scoreManager.ResetScore();            // 歸零
int score = scoreManager.CurrentScore; // 讀取目前分數
```

#### ReactionTimeRecorder（反應時間）

```csharp
rtRecorder.RecordTime(reactionTimeMs);          // 記錄一筆反應時間
float avg = rtRecorder.GetAverageMs();          // 取得平均反應時間（毫秒）
List<float> all = rtRecorder.GetAllRecords();   // 取得全部紀錄
int count = rtRecorder.RecordCount;             // 已記錄幾筆
```

#### TrialCounter（回合）

```csharp
int current = trialCounter.CurrentTrial;   // 目前第幾回合
bool more = trialCounter.HasRemaining;     // 還有剩餘回合嗎
```

#### Debug.Log（在 Console 印出訊息）

```csharp
Debug.Log("這段文字會出現在 Unity 的 Console 面板");
Debug.Log("反應時間：" + reactionTimeMs + " ms");
```

---

## 三、改法範例

以下是幾個可以在課堂上示範的修改，全部都在 `GoNoGoManager.cs` 裡面改。

### 範例 A：反應超快額外加分

如果玩家在 200 毫秒內敲到 Go 地鼠，額外獎勵 5 分。

```csharp
public void OnGoClicked(float reactionTimeMs)
{
    goHits++;
    scoreManager.AddScore(goHitScore);
    rtRecorder.RecordTime(reactionTimeMs);

    // --- 學生新增的程式碼 ---
    if (reactionTimeMs < 200f)
    {
        scoreManager.AddScore(5);
        Debug.Log("超快反應！額外 +5 分");
    }
}
```

### 範例 B：連續正確加倍得分

連續正確敲擊 3 次以上，分數加倍。需要先在 class 裡面加一個計數變數。

```csharp
// 在 GoNoGoManager class 的最上面（private int goHits; 那幾行附近）加一行：
private int streak = 0;   // 連擊計數
```

然後修改方法：

```csharp
public void OnGoClicked(float reactionTimeMs)
{
    goHits++;
    rtRecorder.RecordTime(reactionTimeMs);

    // --- 學生修改的計分邏輯 ---
    streak++;
    if (streak >= 3)
    {
        scoreManager.AddScore(goHitScore * 2);   // 加倍！
        Debug.Log("連擊 " + streak + " 次！雙倍得分！");
    }
    else
    {
        scoreManager.AddScore(goHitScore);
    }
}

public void OnMoleMissed()
{
    scoreManager.SubtractScore(goMissPenalty);
    streak = 0;   // 漏敲就中斷連擊
}

public void OnNoGoClicked(float reactionTimeMs)
{
    scoreManager.SubtractScore(noGoFailPenalty);
    streak = 0;   // 誤敲也中斷連擊
}
```

### 範例 C：誤敲 No-Go 扣更多分（懲罰遞增）

每次誤敲 No-Go 地鼠，扣分越來越重。

```csharp
// 加一個變數：
private int noGoMistakeCount = 0;
```

```csharp
public void OnNoGoClicked(float reactionTimeMs)
{
    noGoMistakeCount++;
    int penalty = noGoFailPenalty * noGoMistakeCount;  // 第1次扣10，第2次扣20...
    scoreManager.SubtractScore(penalty);
    Debug.Log("第 " + noGoMistakeCount + " 次衝動錯誤！扣 " + penalty + " 分");
}
```

記得在 `StartGame()` 方法裡重置：

```csharp
public void StartGame()
{
    // ... 原本的重置程式碼 ...
    noGoMistakeCount = 0;   // 加這一行
}
```

---

## 四、課堂 Demo 建議流程

### 第一步：先玩一輪（5 分鐘）

1. 按 Play，用預設參數玩一輪完整遊戲
2. 讓學生看結算畫面（正確率、反應時間）

### 第二步：調 Inspector 參數（10 分鐘）

1. 把 `Target Display Duration` 調成 **0.5**，再玩一次 → 體驗時間壓力
2. 把 `No-Go Ratio` 調成 **0.7** → 體驗「Go 變少反而容易忍住」
3. 把 `Total Trials` 改成 **10** → 快速測試

每次調完讓學生討論：「為什麼難度變了？對應到什麼認知機制？」

### 第三步：現場改程式碼（15 分鐘）

1. 打開 `Assets/Scripts/GoNoGo/GoNoGoManager.cs`
2. 找到 `OnGoClicked` 方法
3. 現場加入「範例 A：反應超快額外加分」
4. 存檔，回 Unity 按 Play 測試
5. 讓學生觀察 Console 面板出現的 Debug.Log 訊息

### 第四步：學生自己改（課堂作業）

讓學生挑一個範例（或自己想一個規則），修改 `GoNoGoManager.cs` 裡的 4 個方法。

引導問題：

| 問題 | 提示 |
|------|------|
| 「怎麼讓衝動錯誤的代價越來越大？」 | 參考範例 C |
| 「怎麼獎勵反應特別快的玩家？」 | 參考範例 A，用 `reactionTimeMs` 判斷 |
| 「怎麼做連擊機制？」 | 參考範例 B，加一個 `streak` 變數 |
| 「怎麼在後半段讓遊戲變難？」 | 用 `trialCounter.CurrentTrial` 判斷回合數 |

---

## 五、專案檔案結構

```
Assets/Scripts/GoNoGo/
    GoNoGoManager.cs        ← 學生要修改的檔案（遊戲規則）
    ScoreManager.cs         ← 計分工具（學生呼叫，不需要改）
    CountdownTimer.cs       ← 倒數工具（學生呼叫，不需要改）
    TrialCounter.cs         ← 回合工具（學生呼叫，不需要改）
    ReactionTimeRecorder.cs ← 反應時間工具（學生呼叫，不需要改）
    MoleSpawner.cs          ← 地鼠生成（不需要動）
    MoleHole.cs             ← 地鼠洞口（不需要動）
    Mole.cs                 ← 地鼠行為（不需要動）
    Editor/
        SceneBuilder.cs     ← Tools > Go-No-Go > 建構遊戲場景
        SpriteGenerator.cs  ← Tools > Go-No-Go > 生成佔位素材
```

---

## 六、場景壞掉怎麼辦

1. 選單 **Tools > Go-No-Go > 生成佔位素材**
2. 選單 **Tools > Go-No-Go > 建構遊戲場景**
3. 按 **Play** 測試
