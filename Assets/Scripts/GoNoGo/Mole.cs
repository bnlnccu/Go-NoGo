using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 單隻地鼠的行為：冒出、停留、縮回、點擊偵測。
/// 動態掛在地鼠 Image 上，由 MoleHole.ShowMole() 初始化。
/// </summary>
public class Mole : MonoBehaviour, IPointerClickHandler
{
    private MoleHole parentHole;
    private GoNoGoManager manager;
    private bool isNoGo;
    private float displayDuration;
    private float spawnTime;
    private bool isActive;
    private bool wasClicked;

    // 冒出動畫相關
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private float animationTime = 0.15f;
    private float currentAnimTime;
    private bool isAnimatingIn;
    private bool isAnimatingOut;

    /// <summary>
    /// 初始化地鼠
    /// </summary>
    public void Initialize(MoleHole hole, bool noGo, float duration, GoNoGoManager mgr)
    {
        parentHole = hole;
        isNoGo = noGo;
        displayDuration = duration;
        manager = mgr;
        spawnTime = Time.time;
        isActive = true;
        wasClicked = false;

        rectTransform = GetComponent<RectTransform>();
        originalScale = Vector3.one;

        // 開始冒出動畫
        isAnimatingIn = true;
        isAnimatingOut = false;
        currentAnimTime = 0f;
        rectTransform.localScale = new Vector3(1f, 0f, 1f);
    }

    private void Update()
    {
        if (!isActive) return;

        // 冒出動畫
        if (isAnimatingIn)
        {
            currentAnimTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentAnimTime / animationTime);
            // 彈性效果
            float eased = 1f - Mathf.Pow(1f - t, 3f);
            rectTransform.localScale = new Vector3(1f, eased, 1f);

            if (t >= 1f)
            {
                isAnimatingIn = false;
                rectTransform.localScale = originalScale;
            }
            return;
        }

        // 縮回動畫
        if (isAnimatingOut)
        {
            currentAnimTime += Time.deltaTime;
            float t = Mathf.Clamp01(currentAnimTime / animationTime);
            float eased = 1f - t;
            rectTransform.localScale = new Vector3(1f, eased, 1f);

            if (t >= 1f)
            {
                FinishAndClear();
            }
            return;
        }

        // 停留計時 → 超時自動縮回（僅限未被點擊的情況）
        // 已被點擊的地鼠由 HitFeedbackCoroutine 控制縮回時機
        if (Time.time - spawnTime >= displayDuration && !wasClicked)
        {
            if (!isNoGo)
            {
                // Go 地鼠沒被敲 → miss
                manager.OnMoleMissed();
            }
            else
            {
                // No-Go 地鼠沒被點 → 正確抑制
                manager.OnNoGoCorrectInhibition();
            }
            StartHideAnimation();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isActive || wasClicked || isAnimatingOut) return;

        wasClicked = true;
        float reactionTime = (Time.time - spawnTime) * 1000f; // 轉換為毫秒

        if (isNoGo)
        {
            // 點了 No-Go 地鼠 → 錯誤（衝動錯誤）
            manager.OnNoGoClicked(reactionTime);
        }
        else
        {
            // 點了 Go 地鼠 → 正確
            manager.OnGoClicked(reactionTime);
        }

        // 視覺回饋：換圖 + 洞口閃爍 + 延遲縮回
        StartCoroutine(HitFeedbackCoroutine());
    }

    private IEnumerator HitFeedbackCoroutine()
    {
        // 換圖
        var image = GetComponent<Image>();
        if (image != null && manager != null)
        {
            Sprite hitSprite = isNoGo ? manager.NoGoMoleHitSprite : manager.GoMoleHitSprite;
            if (hitSprite != null)
                image.sprite = hitSprite;
        }

        // 洞口閃爍
        if (parentHole != null)
        {
            Color flashColor = isNoGo
                ? new Color(0.9f, 0.2f, 0.2f, 0.8f)   // 錯誤：紅色
                : new Color(0.2f, 0.9f, 0.3f, 0.8f);   // 正確：綠色
            parentHole.FlashColor(flashColor, 0.3f);
        }

        // 延遲 0.3 秒讓玩家看到回饋
        yield return new WaitForSeconds(0.3f);

        StartHideAnimation();
    }

    private void StartHideAnimation()
    {
        isAnimatingOut = true;
        currentAnimTime = 0f;
    }

    private void FinishAndClear()
    {
        isActive = false;
        rectTransform.localScale = originalScale;
        if (parentHole != null)
            parentHole.ClearMole();
    }
}
