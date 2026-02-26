using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Go/No-Go 打地鼠遊戲主控制器。
/// 管理遊戲流程與結算，計分、倒數、計次、反應時間則委託給各功能元件。
///
/// 學生可在 Inspector 調整遊戲參數，並透過功能元件提供的 API 來擴充遊戲邏輯。
/// </summary>
public class GoNoGoManager : MonoBehaviour
{
    // ========== 遊戲參數（學生可在 Inspector 調整）==========

    [Header("===== 遊戲參數（可自由調整）=====")]

    [Tooltip("地鼠出現後停留多久（秒）。\n" +
             "認知意義：時間越短，玩家需要更快做出反應，增加時間壓力。")]
    [Range(0.5f, 5.0f)]
    [SerializeField] private float targetDisplayDuration = 1.5f;

    [Tooltip("No-Go 地鼠出現的比例（0.0~1.0）。\n" +
             "認知意義：比例越高，Go 反應的慣性越弱，抑制控制的難度越低。\n" +
             "經典設定通常為 0.2~0.3（少量 No-Go），此時抑制最困難。")]
    [Range(0.1f, 0.8f)]
    [SerializeField] private float noGoRatio = 0.3f;

    [Tooltip("兩隻地鼠出現的間隔時間（秒）。\n" +
             "認知意義：間隔越短，認知負荷越高。")]
    [Range(0.3f, 3.0f)]
    [SerializeField] private float spawnInterval = 1.0f;

    [Header("===== 地鼠外觀（可拖拽替換圖片）=====")]

    [Tooltip("Go 地鼠的圖片（應該敲的地鼠）。\n留空則使用預設棕色圓形。")]
    [SerializeField] private Sprite goMoleSprite;

    [Tooltip("No-Go 地鼠的圖片（不應該敲的地鼠）。\n留空則使用預設紅色圓形。")]
    [SerializeField] private Sprite noGoMoleSprite;

    [Tooltip("Go 地鼠被正確敲中後顯示的圖片。\n留空則使用預設暈眩圖。")]
    [SerializeField] private Sprite goMoleHitSprite;

    [Tooltip("No-Go 地鼠被錯誤敲擊後顯示的圖片。\n留空則使用預設生氣圖。")]
    [SerializeField] private Sprite noGoMoleHitSprite;

    [Header("===== 計分設定 =====")]

    [Tooltip("正確敲擊 Go 地鼠的得分")]
    [Range(1, 20)]
    [SerializeField] private int goHitScore = 10;

    [Tooltip("成功抑制（沒敲）No-Go 地鼠的得分")]
    [Range(1, 20)]
    [SerializeField] private int noGoInhibitScore = 5;

    [Tooltip("錯誤敲擊 No-Go 地鼠的扣分")]
    [Range(1, 20)]
    [SerializeField] private int noGoFailPenalty = 10;

    [Tooltip("漏敲 Go 地鼠的扣分")]
    [Range(0, 10)]
    [SerializeField] private int goMissPenalty = 5;

    // ========== 功能元件（拖入場景中的元件）==========

    [Header("===== 功能元件（自動連接）=====")]

    [Tooltip("計分管理器")]
    [SerializeField] private ScoreManager scoreManager;

    [Tooltip("倒數計時器")]
    [SerializeField] private CountdownTimer countdownTimer;

    [Tooltip("試驗計數器")]
    [SerializeField] private TrialCounter trialCounter;

    [Tooltip("反應時間紀錄器")]
    [SerializeField] private ReactionTimeRecorder rtRecorder;

    // ========== UI 參考（場景中連接）==========

    [Header("===== UI 參考（請勿修改）=====")]

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject resultPanel;

    [SerializeField] private Text resultTotalScoreText;
    [SerializeField] private Text resultGoAccuracyText;
    [SerializeField] private Text resultNoGoAccuracyText;
    [SerializeField] private Text resultAvgRTText;

    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButtonStart;
    [SerializeField] private Button quitButtonResult;

    [SerializeField] private MoleSpawner moleSpawner;

    // ========== 內部狀態 ==========

    private int goTrials;
    private int goHits;
    private int noGoTrials;
    private int noGoCorrectInhibitions;
    private bool gameRunning;

    // ========== 公開屬性（供其他腳本讀取）==========

    public float TargetDisplayDuration => targetDisplayDuration;
    public float NoGoRatio => noGoRatio;
    public float SpawnInterval => spawnInterval;
    public Sprite GoMoleSprite => goMoleSprite;
    public Sprite NoGoMoleSprite => noGoMoleSprite;
    public Sprite GoMoleHitSprite => goMoleHitSprite;
    public Sprite NoGoMoleHitSprite => noGoMoleHitSprite;
    public bool IsGameRunning => gameRunning;

    /// <summary>是否還有剩餘回合（委託給 TrialCounter）</summary>
    public bool HasTrialsRemaining => trialCounter != null ? trialCounter.HasRemaining : true;

    // ========== 生命週期 ==========

    private void Start()
    {
        // 生成預設 Sprite（如果沒有指定）
        if (goMoleSprite == null)
            goMoleSprite = CreateDefaultSprite(new Color(0.6f, 0.4f, 0.2f));
        if (noGoMoleSprite == null)
            noGoMoleSprite = CreateDefaultSprite(new Color(0.85f, 0.2f, 0.2f));
        if (goMoleHitSprite == null)
            goMoleHitSprite = CreateDefaultHitSprite(new Color(0.5f, 0.35f, 0.15f), false);
        if (noGoMoleHitSprite == null)
            noGoMoleHitSprite = CreateDefaultHitSprite(new Color(0.7f, 0.15f, 0.15f), true);

        // 初始化 UI
        ShowPanel(PanelType.Start);

        // 綁定按鈕事件
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (quitButtonStart != null)
            quitButtonStart.onClick.AddListener(QuitGame);
        if (quitButtonResult != null)
            quitButtonResult.onClick.AddListener(QuitGame);

        // 初始化生成器
        if (moleSpawner != null)
        {
            moleSpawner.Initialize(this);
            moleSpawner.AutoFindHoles();
        }
    }

    // ========== 遊戲流程 ==========

    public void StartGame()
    {
        // 重置所有數據
        goTrials = 0;
        goHits = 0;
        noGoTrials = 0;
        noGoCorrectInhibitions = 0;

        // 重置功能元件
        if (scoreManager != null) scoreManager.ResetScore();
        if (trialCounter != null) trialCounter.ResetTrials();
        if (rtRecorder != null) rtRecorder.ClearRecords();

        ShowPanel(PanelType.Game);

        // 使用倒數計時器開始倒數
        if (countdownTimer != null)
        {
            countdownTimer.StartCountdown(OnCountdownFinished);
        }
        else
        {
            // 沒有倒數計時器，直接開始
            OnCountdownFinished();
        }
    }

    private void OnCountdownFinished()
    {
        gameRunning = true;
        if (moleSpawner != null)
            moleSpawner.StartSpawning();
    }

    public void RestartGame()
    {
        StartGame();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void EndGame()
    {
        gameRunning = false;

        if (moleSpawner != null)
            moleSpawner.StopSpawning();

        ShowResults();
        ShowPanel(PanelType.Result);
    }

    // ========== 地鼠事件回調 ==========

    /// <summary>當一個新 trial 開始</summary>
    public void OnTrialStarted(bool isNoGo)
    {
        if (trialCounter != null)
            trialCounter.NextTrial();

        if (isNoGo)
            noGoTrials++;
        else
            goTrials++;

        // 如果已達到總試驗數，停止生成新的（等待當前地鼠消失後結算）
        if (trialCounter != null && !trialCounter.HasRemaining)
        {
            if (moleSpawner != null)
                moleSpawner.StopSpawning();

            // 延遲結束，等最後一隻地鼠結束
            Invoke(nameof(EndGame), targetDisplayDuration + 0.5f);
        }
    }

    /// <summary>玩家正確敲擊了 Go 地鼠</summary>
    public void OnGoClicked(float reactionTimeMs)
    {
        goHits++;
        if (scoreManager != null) scoreManager.AddScore(goHitScore);
        if (rtRecorder != null) rtRecorder.RecordTime(reactionTimeMs);
    }

    /// <summary>Go 地鼠超時未被點擊（漏敲）</summary>
    public void OnMoleMissed()
    {
        if (scoreManager != null) scoreManager.SubtractScore(goMissPenalty);
    }

    /// <summary>玩家錯誤敲擊了 No-Go 地鼠（衝動錯誤）</summary>
    public void OnNoGoClicked(float reactionTimeMs)
    {
        if (scoreManager != null) scoreManager.SubtractScore(noGoFailPenalty);
    }

    /// <summary>No-Go 地鼠正確地未被點擊（成功抑制）</summary>
    public void OnNoGoCorrectInhibition()
    {
        noGoCorrectInhibitions++;
        if (scoreManager != null) scoreManager.AddScore(noGoInhibitScore);
    }

    // ========== UI 更新 ==========

    private void ShowResults()
    {
        float goAccuracy = goTrials > 0 ? (float)goHits / goTrials * 100f : 0f;
        float noGoAccuracy = noGoTrials > 0 ? (float)noGoCorrectInhibitions / noGoTrials * 100f : 0f;

        float avgRT = rtRecorder != null ? rtRecorder.GetAverageMs() : 0f;
        int currentScore = scoreManager != null ? scoreManager.CurrentScore : 0;

        if (resultTotalScoreText != null)
            resultTotalScoreText.text = "總分：" + currentScore;
        if (resultGoAccuracyText != null)
            resultGoAccuracyText.text = string.Format("Go 正確率：{0:F1}% ({1}/{2})", goAccuracy, goHits, goTrials);
        if (resultNoGoAccuracyText != null)
            resultNoGoAccuracyText.text = string.Format("No-Go 正確抑制率：{0:F1}% ({1}/{2})", noGoAccuracy, noGoCorrectInhibitions, noGoTrials);
        if (resultAvgRTText != null)
            resultAvgRTText.text = string.Format("平均反應時間：{0:F0} ms", avgRT);
    }

    private enum PanelType { Start, Game, Result }

    private void ShowPanel(PanelType type)
    {
        if (startPanel != null) startPanel.SetActive(type == PanelType.Start);
        if (gamePanel != null) gamePanel.SetActive(type == PanelType.Game);
        if (resultPanel != null) resultPanel.SetActive(type == PanelType.Result);
    }

    // ========== 工具方法 ==========

    private Sprite CreateDefaultSprite(Color color)
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float center = size / 2f;
        float radius = size / 2f - 1;
        Color transparent = new Color(0, 0, 0, 0);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = x - center;
                float dy = y - center;
                if (dx * dx + dy * dy <= radius * radius)
                {
                    tex.SetPixel(x, y, color);
                }
                else
                {
                    tex.SetPixel(x, y, transparent);
                }
            }
        }

        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
    }

    /// <summary>
    /// 生成被打中的預設 Sprite（扁橢圓 + 標記，與一般地鼠有明顯視覺差異）
    /// </summary>
    private Sprite CreateDefaultHitSprite(Color color, bool isAngry)
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float cx = size / 2f;
        float cy = size / 2f;
        Color transparent = new Color(0, 0, 0, 0);

        // 扁橢圓（被打扁的效果）
        float rx = size / 2f - 1;
        float ry = size / 4f;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dx = (x - cx) / rx;
                float dy = (y - cy) / ry;
                if (dx * dx + dy * dy <= 1f)
                {
                    tex.SetPixel(x, y, color);
                }
                else
                {
                    tex.SetPixel(x, y, transparent);
                }
            }
        }

        // 畫 X 標記（暈眩眼）或 ! 標記（生氣）
        Color markColor = isAngry ? Color.yellow : Color.white;
        int markSize = size / 6;
        for (int i = -markSize; i <= markSize; i++)
        {
            for (int t = -1; t <= 1; t++)
            {
                // 左眼 X
                int lx = (int)(cx - size * 0.18f) + i;
                int ly1 = (int)(cy + size * 0.05f) + i + t;
                int ly2 = (int)(cy + size * 0.05f) - i + t;
                if (lx >= 0 && lx < size && ly1 >= 0 && ly1 < size)
                    tex.SetPixel(lx, ly1, markColor);
                if (lx >= 0 && lx < size && ly2 >= 0 && ly2 < size)
                    tex.SetPixel(lx, ly2, markColor);

                // 右眼 X
                int rxx = (int)(cx + size * 0.18f) + i;
                int ry1 = (int)(cy + size * 0.05f) + i + t;
                int ry2 = (int)(cy + size * 0.05f) - i + t;
                if (rxx >= 0 && rxx < size && ry1 >= 0 && ry1 < size)
                    tex.SetPixel(rxx, ry1, markColor);
                if (rxx >= 0 && rxx < size && ry2 >= 0 && ry2 < size)
                    tex.SetPixel(rxx, ry2, markColor);
            }
        }

        tex.Apply();
        tex.filterMode = FilterMode.Bilinear;
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), 100);
    }
}
