using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 地鼠生成器：負責隨機選擇洞口、決定 Go/NoGo 類型、按間隔持續生成。
/// 由 GoNoGoManager 啟動和停止。
/// </summary>
public class MoleSpawner : MonoBehaviour
{
    [Header("洞口設定")]
    [Tooltip("場景中所有洞口的參考（自動或手動指定）")]
    [SerializeField] private List<MoleHole> moleHoles = new List<MoleHole>();

    private GoNoGoManager manager;
    private Coroutine spawnCoroutine;

    /// <summary>
    /// 初始化生成器
    /// </summary>
    public void Initialize(GoNoGoManager mgr)
    {
        manager = mgr;
    }

    /// <summary>
    /// 自動搜尋場景中所有 MoleHole
    /// </summary>
    public void AutoFindHoles()
    {
        if (moleHoles == null || moleHoles.Count == 0)
        {
            MoleHole[] found = FindObjectsOfType<MoleHole>();
            moleHoles = new List<MoleHole>(found);
        }
    }

    /// <summary>
    /// 開始生成地鼠
    /// </summary>
    public void StartSpawning()
    {
        StopSpawning();
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// 停止生成
    /// </summary>
    public void StopSpawning()
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnRoutine()
    {
        while (manager.IsGameRunning)
        {
            SpawnOneMole();
            yield return new WaitForSeconds(manager.SpawnInterval);
        }
    }

    private void SpawnOneMole()
    {
        if (!manager.IsGameRunning) return;
        if (!manager.HasTrialsRemaining) return;

        // 找到所有空閒洞口
        List<MoleHole> available = new List<MoleHole>();
        foreach (var hole in moleHoles)
        {
            if (!hole.IsOccupied)
                available.Add(hole);
        }

        if (available.Count == 0) return;

        // 隨機選一個洞口
        MoleHole selectedHole = available[Random.Range(0, available.Count)];

        // 決定是 Go 還是 No-Go
        bool isNoGo = Random.value < manager.NoGoRatio;

        // 選擇對應的 Sprite
        Sprite moleSprite = isNoGo ? manager.NoGoMoleSprite : manager.GoMoleSprite;

        // 生成地鼠
        selectedHole.ShowMole(moleSprite, isNoGo, manager.TargetDisplayDuration, manager);

        // 通知 Manager 消耗一個 trial
        manager.OnTrialStarted(isNoGo);
    }
}
