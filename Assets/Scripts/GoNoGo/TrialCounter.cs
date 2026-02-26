using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 試驗計數器：追蹤目前第幾回合、總共幾回合。
/// 學生可呼叫 NextTrial() 推進回合，並讀取 HasRemaining 判斷是否結束。
/// </summary>
public class TrialCounter : MonoBehaviour
{
    [Header("===== 試驗設定 =====")]

    [Tooltip("顯示試驗進度的 Text 元件")]
    [SerializeField] private Text trialText;

    [Tooltip("總共要進行幾次試驗")]
    [Range(5, 200)]
    [SerializeField] private int totalTrials = 30;

    [Tooltip("顯示格式，{0}=目前回合，{1}=總回合")]
    [SerializeField] private string displayFormat = "第 {0} / {1} 隻";

    /// <summary>目前第幾回合</summary>
    public int CurrentTrial { get; private set; }

    /// <summary>總回合數（可在遊戲開始前修改）</summary>
    public int TotalTrials
    {
        get => totalTrials;
        set => totalTrials = value;
    }

    /// <summary>是否還有剩餘回合</summary>
    public bool HasRemaining => CurrentTrial < totalTrials;

    /// <summary>推進到下一回合</summary>
    public void NextTrial()
    {
        CurrentTrial++;
        UpdateUI();
    }

    /// <summary>重置為第 0 回合</summary>
    public void ResetTrials()
    {
        CurrentTrial = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (trialText != null)
            trialText.text = string.Format(displayFormat, CurrentTrial, totalTrials);
    }
}
