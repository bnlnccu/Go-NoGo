using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 反應時間紀錄器：記錄每次反應的時間（毫秒），並提供統計功能。
/// 心理學實驗的核心工具——用來測量玩家的認知反應速度。
///
/// 使用方式一（自動計時）：
///   rtRecorder.StartTiming();    // 刺激出現時呼叫
///   float ms = rtRecorder.StopTimingMs();  // 玩家反應時呼叫
///
/// 使用方式二（手動記錄）：
///   rtRecorder.RecordTime(reactionTimeMs);  // 直接記錄一筆
/// </summary>
public class ReactionTimeRecorder : MonoBehaviour
{
    private List<float> records = new List<float>();
    private float timingStartTime;
    private bool isTiming;

    /// <summary>已記錄的筆數</summary>
    public int RecordCount => records.Count;

    /// <summary>
    /// 開始計時（刺激出現時呼叫）
    /// </summary>
    public void StartTiming()
    {
        timingStartTime = Time.time;
        isTiming = true;
    }

    /// <summary>
    /// 停止計時並自動記錄（玩家反應時呼叫）。
    /// 回傳反應時間（毫秒）。
    /// </summary>
    public float StopTimingMs()
    {
        if (!isTiming) return 0f;

        isTiming = false;
        float ms = (Time.time - timingStartTime) * 1000f;
        records.Add(ms);
        return ms;
    }

    /// <summary>
    /// 直接記錄一筆反應時間（毫秒）。
    /// 適合在其他腳本已經算好反應時間的情況下使用。
    /// </summary>
    public void RecordTime(float ms)
    {
        records.Add(ms);
    }

    /// <summary>取得平均反應時間（毫秒）</summary>
    public float GetAverageMs()
    {
        if (records.Count == 0) return 0f;

        float total = 0f;
        foreach (float rt in records)
            total += rt;
        return total / records.Count;
    }

    /// <summary>取得所有反應時間紀錄</summary>
    public List<float> GetAllRecords()
    {
        return new List<float>(records);
    }

    /// <summary>清除所有紀錄</summary>
    public void ClearRecords()
    {
        records.Clear();
        isTiming = false;
    }
}
