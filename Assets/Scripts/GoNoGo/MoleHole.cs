using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 洞口元件：管理單個洞口的狀態（是否有地鼠佔用）。
/// 掛在每個洞口的 GameObject 上。
/// </summary>
public class MoleHole : MonoBehaviour
{
    [Header("元件參考")]
    [Tooltip("洞口圖片")]
    [SerializeField] private Image holeImage;

    [Tooltip("地鼠圖片（子物件）")]
    [SerializeField] private Image moleImage;

    /// <summary>此洞口是否正被地鼠佔用</summary>
    public bool IsOccupied { get; private set; }

    /// <summary>目前在此洞口的 Mole 元件</summary>
    public Mole CurrentMole { get; private set; }

    private void Awake()
    {
        if (moleImage != null)
            moleImage.gameObject.SetActive(false);
    }

    /// <summary>
    /// 在此洞口顯示一隻地鼠
    /// </summary>
    public void ShowMole(Sprite moleSprite, bool isNoGo, float displayDuration, GoNoGoManager manager)
    {
        if (IsOccupied) return;

        IsOccupied = true;

        if (moleImage != null)
        {
            moleImage.sprite = moleSprite;
            moleImage.gameObject.SetActive(true);

            // 取得或新增 Mole 元件
            CurrentMole = moleImage.GetComponent<Mole>();
            if (CurrentMole == null)
                CurrentMole = moleImage.gameObject.AddComponent<Mole>();

            CurrentMole.Initialize(this, isNoGo, displayDuration, manager);
        }
    }

    /// <summary>
    /// 清除此洞口的地鼠
    /// </summary>
    public void ClearMole()
    {
        IsOccupied = false;
        CurrentMole = null;

        if (moleImage != null)
            moleImage.gameObject.SetActive(false);
    }
}
