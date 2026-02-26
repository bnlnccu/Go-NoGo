using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

/// <summary>
/// 倒數計時器：在畫面中央顯示 3、2、1 倒數，結束後通知遊戲開始。
/// 學生可呼叫 StartCountdown() 開始倒數，並傳入結束時要執行的回呼。
/// </summary>
public class CountdownTimer : MonoBehaviour
{
    [Header("===== 倒數設定 =====")]

    [Tooltip("顯示倒數數字的 Text 元件")]
    [SerializeField] private Text countdownText;

    [Tooltip("從幾開始倒數")]
    [Range(1, 10)]
    [SerializeField] private int countFrom = 3;

    [Tooltip("每個數字停留幾秒")]
    [Range(0.5f, 3f)]
    [SerializeField] private float secondsPerCount = 1f;

    [Tooltip("是否使用縮放動畫")]
    [SerializeField] private bool useScaleAnimation = true;

    /// <summary>是否正在倒數中</summary>
    public bool IsCountingDown { get; private set; }

    private Coroutine countdownCoroutine;

    /// <summary>
    /// 開始倒數。倒數結束後會呼叫 onFinished。
    /// </summary>
    /// <param name="onFinished">倒數結束時執行的動作</param>
    public void StartCountdown(Action onFinished)
    {
        StopCountdown();
        countdownCoroutine = StartCoroutine(CountdownRoutine(onFinished));
    }

    /// <summary>中斷倒數</summary>
    public void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
        IsCountingDown = false;

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    private IEnumerator CountdownRoutine(Action onFinished)
    {
        IsCountingDown = true;

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(true);

            for (int i = countFrom; i >= 1; i--)
            {
                countdownText.text = i.ToString();

                if (useScaleAnimation)
                {
                    // 從 1.5 倍縮小到 1 倍的動畫
                    countdownText.transform.localScale = Vector3.one * 1.5f;
                    float timer = 0f;
                    while (timer < secondsPerCount)
                    {
                        timer += Time.deltaTime;
                        float t = Mathf.Clamp01(timer / secondsPerCount);
                        countdownText.transform.localScale = Vector3.Lerp(
                            Vector3.one * 1.5f, Vector3.one, t);
                        yield return null;
                    }
                }
                else
                {
                    yield return new WaitForSeconds(secondsPerCount);
                }
            }

            countdownText.gameObject.SetActive(false);
        }
        else
        {
            // 沒有 Text 元件時，單純等待
            yield return new WaitForSeconds(countFrom * secondsPerCount);
        }

        IsCountingDown = false;
        countdownCoroutine = null;
        onFinished?.Invoke();
    }
}
