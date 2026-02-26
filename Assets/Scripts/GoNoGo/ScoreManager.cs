using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 計分管理器：負責加分、扣分、顯示分數。
/// 學生可直接呼叫 AddScore() / SubtractScore() 來操作分數。
/// </summary>
public class ScoreManager : MonoBehaviour
{
    [Header("===== 計分設定 =====")]

    [Tooltip("顯示分數的 Text 元件")]
    [SerializeField] private Text scoreText;

    [Tooltip("分數顯示格式，{0} 會被替換成目前分數")]
    [SerializeField] private string displayFormat = "分數：{0}";

    /// <summary>目前的分數</summary>
    public int CurrentScore { get; private set; }

    /// <summary>加分</summary>
    public void AddScore(int points)
    {
        CurrentScore += points;
        UpdateUI();
    }

    /// <summary>扣分（分數不會低於 0）</summary>
    public void SubtractScore(int points)
    {
        CurrentScore = Mathf.Max(0, CurrentScore - points);
        UpdateUI();
    }

    /// <summary>重置分數為 0</summary>
    public void ResetScore()
    {
        CurrentScore = 0;
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = string.Format(displayFormat, CurrentScore);
    }
}
