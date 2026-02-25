using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// Go/No-Go 打地鼠遊戲主控制器。
/// 管理遊戲流程、計分、反應時間記錄與結算。
/// 所有可調參數皆透過 Inspector 面板暴露，學生不需修改任何程式碼。
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

    [Tooltip("總共會出現幾隻地鼠（試驗次數）。\n" +
             "認知意義：試驗次數影響數據的統計信度。")]
    [Range(10, 100)]
    [SerializeField] private int totalTrials = 30;

    [Header("===== 地鼠外觀（可拖拽替換圖片）=====")]

    [Tooltip("Go 地鼠的圖片（應該敲的地鼠）。\n留空則使用預設棕色圓形。")]
    [SerializeField] private Sprite goMoleSprite;

    [Tooltip("No-Go 地鼠的圖片（不應該敲的地鼠）。\n留空則使用預設紅色圓形。")]
    [SerializeField] private Sprite noGoMoleSprite;

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

    // ========== UI 參考（場景中連接）==========

    [Header("===== UI 參考（請勿修改）=====")]

    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject resultPanel;

    [SerializeField] private Text scoreText;
    [SerializeField] private Text trialText;

    [SerializeField] private Text resultTotalScoreText;
    [SerializeField] private Text resultGoAccuracyText;
    [SerializeField] private Text resultNoGoAccuracyText;
    [SerializeField] private Text resultAvgRTText;

    [SerializeField] private Button startButton;
    [SerializeField] private Button restartButton;

    [SerializeField] private MoleSpawner moleSpawner;

    // ========== 內部狀態 ==========

    private int currentScore;
    private int currentTrial;
    private int goTrials;
    private int goHits;
    private int goMisses;
    private int noGoTrials;
    private int noGoCorrectInhibitions;
    private int noGoFails;
    private List<float> goReactionTimes = new List<float>();
    private bool gameRunning;

    // ========== 公開屬性（供其他腳本讀取）==========

    public float TargetDisplayDuration => targetDisplayDuration;
    public float NoGoRatio => noGoRatio;
    public float SpawnInterval => spawnInterval;
    public int TotalTrials => totalTrials;
    public Sprite GoMoleSprite => goMoleSprite;
    public Sprite NoGoMoleSprite => noGoMoleSprite;
    public bool IsGameRunning => gameRunning;
    public bool HasTrialsRemaining => currentTrial < totalTrials;

    // ========== 生命週期 ==========

    private void Start()
    {
        // 生成預設 Sprite（如果沒有指定）
        if (goMoleSprite == null)
            goMoleSprite = CreateDefaultSprite(new Color(0.6f, 0.4f, 0.2f));
        if (noGoMoleSprite == null)
            noGoMoleSprite = CreateDefaultSprite(new Color(0.85f, 0.2f, 0.2f));

        // 初始化 UI
        ShowPanel(PanelType.Start);

        // 綁定按鈕事件
        if (startButton != null)
            startButton.onClick.AddListener(StartGame);
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);

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
        currentScore = 0;
        currentTrial = 0;
        goTrials = 0;
        goHits = 0;
        goMisses = 0;
        noGoTrials = 0;
        noGoCorrectInhibitions = 0;
        noGoFails = 0;
        goReactionTimes.Clear();
        gameRunning = true;

        UpdateScoreUI();
        UpdateTrialUI();
        ShowPanel(PanelType.Game);

        // 開始生成地鼠
        if (moleSpawner != null)
            moleSpawner.StartSpawning();
    }

    public void RestartGame()
    {
        StartGame();
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
        currentTrial++;
        if (isNoGo)
            noGoTrials++;
        else
            goTrials++;

        UpdateTrialUI();

        // 如果已達到總試驗數，停止生成新的（等待當前地鼠消失後結算）
        if (currentTrial >= totalTrials)
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
        currentScore += goHitScore;
        goReactionTimes.Add(reactionTimeMs);
        UpdateScoreUI();
    }

    /// <summary>Go 地鼠超時未被點擊（漏敲）</summary>
    public void OnMoleMissed()
    {
        goMisses++;
        currentScore = Mathf.Max(0, currentScore - goMissPenalty);
        UpdateScoreUI();
    }

    /// <summary>玩家錯誤敲擊了 No-Go 地鼠（衝動錯誤）</summary>
    public void OnNoGoClicked(float reactionTimeMs)
    {
        noGoFails++;
        currentScore = Mathf.Max(0, currentScore - noGoFailPenalty);
        UpdateScoreUI();
    }

    /// <summary>No-Go 地鼠正確地未被點擊（成功抑制）</summary>
    public void OnNoGoCorrectInhibition()
    {
        noGoCorrectInhibitions++;
        currentScore += noGoInhibitScore;
        UpdateScoreUI();
    }

    // ========== UI 更新 ==========

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "分數：" + currentScore;
    }

    private void UpdateTrialUI()
    {
        if (trialText != null)
            trialText.text = string.Format("第 {0} / {1} 隻", currentTrial, totalTrials);
    }

    private void ShowResults()
    {
        float goAccuracy = goTrials > 0 ? (float)goHits / goTrials * 100f : 0f;
        float noGoAccuracy = noGoTrials > 0 ? (float)noGoCorrectInhibitions / noGoTrials * 100f : 0f;

        float avgRT = 0f;
        if (goReactionTimes.Count > 0)
        {
            float total = 0f;
            foreach (float rt in goReactionTimes)
                total += rt;
            avgRT = total / goReactionTimes.Count;
        }

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
}
