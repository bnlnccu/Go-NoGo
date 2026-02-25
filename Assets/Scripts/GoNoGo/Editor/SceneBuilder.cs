using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// Editor 工具：一鍵建構 Go/No-Go 打地鼠遊戲場景。
/// 透過選單 Tools > Go-No-Go > 建構遊戲場景 執行。
/// </summary>
public static class SceneBuilder
{
    private static readonly Color BACKGROUND_GREEN = new Color(0.35f, 0.65f, 0.25f);
    private static readonly Color PANEL_BG = new Color(0.15f, 0.15f, 0.15f, 0.85f);
    private static readonly Color BUTTON_COLOR = new Color(0.2f, 0.6f, 0.3f);
    private static readonly Color BUTTON_RESULT = new Color(0.3f, 0.5f, 0.7f);
    private static readonly Color TEXT_WHITE = Color.white;
    private static readonly Color TEXT_YELLOW = new Color(1f, 0.95f, 0.4f);

    [MenuItem("Tools/Go-No-Go/建構遊戲場景")]
    public static void BuildScene()
    {
        // 建立全新場景（避免舊場景路徑問題）
        var newScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // ===== 基礎設定 =====
        // Camera
        var cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";
        var cam = cameraGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = BACKGROUND_GREEN;
        cam.orthographic = true;
        cameraGO.AddComponent<AudioListener>();
        cameraGO.transform.position = new Vector3(0, 0, -10);

        // Light
        var lightGO = new GameObject("Directional Light");
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        lightGO.transform.rotation = Quaternion.Euler(50, -30, 0);

        // EventSystem
        var eventSystemGO = new GameObject("EventSystem");
        eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<StandaloneInputModule>();

        // ===== Canvas =====
        var canvasGO = new GameObject("Canvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 0;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        // ===== Background Image =====
        var bgGO = CreateUIElement("BackgroundImage", canvasGO.transform);
        StretchFull(bgGO);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = BACKGROUND_GREEN;
        // 嘗試載入生成的背景 Sprite
        var bgSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/GoNoGo/Background.png");
        if (bgSprite != null) bgImg.sprite = bgSprite;

        // ===== Start Panel =====
        var startPanel = CreatePanel("StartPanel", canvasGO.transform, PANEL_BG);
        StretchFull(startPanel);

        // Start Panel 內容容器
        var startContent = CreateUIElement("StartContent", startPanel.transform);
        SetAnchored(startContent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(600, 400));
        var startLayout = startContent.AddComponent<VerticalLayoutGroup>();
        startLayout.spacing = 30;
        startLayout.childAlignment = TextAnchor.MiddleCenter;
        startLayout.childForceExpandWidth = true;
        startLayout.childForceExpandHeight = false;
        startLayout.childControlWidth = true;
        startLayout.childControlHeight = true;

        // 標題
        var titleText = CreateText("TitleText", startContent.transform, "Go / No-Go 打地鼠", 48, TEXT_YELLOW, TextAnchor.MiddleCenter);
        SetLayoutElement(titleText, 80);
        var titleTextComp = titleText.GetComponent<Text>();
        titleTextComp.fontStyle = FontStyle.Bold;

        // 說明
        var instrText = CreateText("InstructionText", startContent.transform,
            "規則說明：\n" +
            "• 看到【棕色地鼠】→ 快速點擊！（Go）\n" +
            "• 看到【紅色地鼠】→ 忍住不要點！（No-Go）\n\n" +
            "正確敲擊得分，錯誤敲擊扣分！",
            24, TEXT_WHITE, TextAnchor.MiddleCenter);
        SetLayoutElement(instrText, 180);

        // 按鈕列（水平並排）
        var startBtnRow = CreateUIElement("ButtonRow", startContent.transform);
        var startBtnRowLayout = startBtnRow.AddComponent<HorizontalLayoutGroup>();
        startBtnRowLayout.spacing = 20;
        startBtnRowLayout.childAlignment = TextAnchor.MiddleCenter;
        startBtnRowLayout.childForceExpandWidth = true;
        startBtnRowLayout.childForceExpandHeight = true;
        startBtnRowLayout.childControlWidth = true;
        startBtnRowLayout.childControlHeight = true;
        SetLayoutElement(startBtnRow, 60);

        // 開始按鈕
        var startBtn = CreateButton("StartButton", startBtnRow.transform, "開始遊戲", 32, BUTTON_COLOR);

        // 結束按鈕
        var quitBtnStart = CreateButton("QuitButtonStart", startBtnRow.transform, "結束遊戲", 32, new Color(0.6f, 0.25f, 0.25f));

        // ===== Game Panel =====
        var gamePanel = CreateUIElement("GamePanel", canvasGO.transform);
        StretchFull(gamePanel);

        // 頂部資訊列
        var topBar = CreateUIElement("TopBar", gamePanel.transform);
        SetAnchored(topBar, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -10), new Vector2(0, 60));
        topBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -40);
        var topBarLayout = topBar.AddComponent<HorizontalLayoutGroup>();
        topBarLayout.spacing = 50;
        topBarLayout.childAlignment = TextAnchor.MiddleCenter;
        topBarLayout.childForceExpandWidth = true;
        topBarLayout.childForceExpandHeight = true;
        topBarLayout.padding = new RectOffset(30, 30, 5, 5);

        // 背景色
        var topBarBg = topBar.AddComponent<Image>();
        topBarBg.color = new Color(0, 0, 0, 0.5f);

        // 分數文字
        var scoreText = CreateText("ScoreText", topBar.transform, "分數：0", 28, TEXT_WHITE, TextAnchor.MiddleLeft);
        // 試驗文字
        var trialText = CreateText("TrialText", topBar.transform, "第 0 / 30 隻", 28, TEXT_WHITE, TextAnchor.MiddleRight);

        // 地鼠網格區域
        var moleGridContainer = CreateUIElement("MoleGridContainer", gamePanel.transform);
        SetAnchored(moleGridContainer, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(720, 720));

        var gridLayout = moleGridContainer.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(200, 200);
        gridLayout.spacing = new Vector2(30, 30);
        gridLayout.startAxis = GridLayoutGroup.Axis.Horizontal;
        gridLayout.startCorner = GridLayoutGroup.Corner.UpperLeft;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = 3;

        // 載入素材
        var holeSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/GoNoGo/Hole.png");
        var goMoleSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/GoNoGo/GoMole.png");

        // 建立 3x3 = 9 個洞口
        for (int i = 0; i < 9; i++)
        {
            var holeGO = CreateUIElement("MoleHole_" + i, moleGridContainer.transform);
            var holeImg = holeGO.AddComponent<Image>();
            if (holeSprite != null) holeImg.sprite = holeSprite;
            holeImg.color = new Color(0.3f, 0.2f, 0.15f);
            holeImg.preserveAspect = true;

            // MoleHole 元件
            var moleHole = holeGO.AddComponent<MoleHole>();

            // 地鼠 Image（子物件，預設隱藏）
            var moleImgGO = CreateUIElement("MoleImage", holeGO.transform);
            StretchFull(moleImgGO);
            var moleImg = moleImgGO.AddComponent<Image>();
            if (goMoleSprite != null) moleImg.sprite = goMoleSprite;
            moleImg.preserveAspect = true;
            moleImg.raycastTarget = true;
            moleImgGO.SetActive(false);

            // 透過 SerializedObject 設定 MoleHole 的私有欄位
            var so = new SerializedObject(moleHole);
            so.FindProperty("holeImage").objectReferenceValue = holeImg;
            so.FindProperty("moleImage").objectReferenceValue = moleImg;
            so.ApplyModifiedProperties();
        }

        // ===== Result Panel =====
        var resultPanel = CreatePanel("ResultPanel", canvasGO.transform, PANEL_BG);
        StretchFull(resultPanel);
        resultPanel.SetActive(false);

        // Result 內容
        var resultContent = CreateUIElement("ResultContent", resultPanel.transform);
        SetAnchored(resultContent, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(700, 500));
        var resultLayout = resultContent.AddComponent<VerticalLayoutGroup>();
        resultLayout.spacing = 20;
        resultLayout.childAlignment = TextAnchor.MiddleCenter;
        resultLayout.childForceExpandWidth = true;
        resultLayout.childForceExpandHeight = false;
        resultLayout.childControlWidth = true;
        resultLayout.childControlHeight = true;

        // 結算標題
        var resultTitle = CreateText("ResultTitle", resultContent.transform, "遊戲結束！", 40, TEXT_YELLOW, TextAnchor.MiddleCenter);
        resultTitle.GetComponent<Text>().fontStyle = FontStyle.Bold;
        SetLayoutElement(resultTitle, 60);

        // 結果文字
        var totalScoreText = CreateText("TotalScoreText", resultContent.transform, "總分：0", 30, TEXT_WHITE, TextAnchor.MiddleCenter);
        SetLayoutElement(totalScoreText, 45);
        var goAccText = CreateText("GoAccuracyText", resultContent.transform, "Go 正確率：0%", 26, TEXT_WHITE, TextAnchor.MiddleCenter);
        SetLayoutElement(goAccText, 40);
        var noGoAccText = CreateText("NoGoAccuracyText", resultContent.transform, "No-Go 正確抑制率：0%", 26, TEXT_WHITE, TextAnchor.MiddleCenter);
        SetLayoutElement(noGoAccText, 40);
        var avgRTText = CreateText("AvgRTText", resultContent.transform, "平均反應時間：0 ms", 26, TEXT_WHITE, TextAnchor.MiddleCenter);
        SetLayoutElement(avgRTText, 40);

        // 間距
        var spacer = CreateUIElement("Spacer", resultContent.transform);
        SetLayoutElement(spacer, 20);

        // 按鈕列（水平並排）
        var resultBtnRow = CreateUIElement("ButtonRow", resultContent.transform);
        var resultBtnRowLayout = resultBtnRow.AddComponent<HorizontalLayoutGroup>();
        resultBtnRowLayout.spacing = 20;
        resultBtnRowLayout.childAlignment = TextAnchor.MiddleCenter;
        resultBtnRowLayout.childForceExpandWidth = true;
        resultBtnRowLayout.childForceExpandHeight = true;
        resultBtnRowLayout.childControlWidth = true;
        resultBtnRowLayout.childControlHeight = true;
        SetLayoutElement(resultBtnRow, 55);

        // 重新開始按鈕
        var restartBtn = CreateButton("RestartButton", resultBtnRow.transform, "再玩一次", 30, BUTTON_RESULT);

        // 結束按鈕
        var quitBtnResult = CreateButton("QuitButtonResult", resultBtnRow.transform, "結束遊戲", 30, new Color(0.6f, 0.25f, 0.25f));


        // ===== 建立管理器物件 =====
        var managerGO = new GameObject("GoNoGoManager");
        managerGO.AddComponent<GoNoGoManager>();
        var spawnerGO = new GameObject("MoleSpawner");
        spawnerGO.AddComponent<MoleSpawner>();

        var manager = managerGO.GetComponent<GoNoGoManager>();
        var spawner = spawnerGO.GetComponent<MoleSpawner>();


        var managerSO = new SerializedObject(manager);
        managerSO.FindProperty("startPanel").objectReferenceValue = startPanel;
        managerSO.FindProperty("gamePanel").objectReferenceValue = gamePanel;
        managerSO.FindProperty("resultPanel").objectReferenceValue = resultPanel;
        managerSO.FindProperty("scoreText").objectReferenceValue = scoreText.GetComponent<Text>();
        managerSO.FindProperty("trialText").objectReferenceValue = trialText.GetComponent<Text>();
        managerSO.FindProperty("resultTotalScoreText").objectReferenceValue = totalScoreText.GetComponent<Text>();
        managerSO.FindProperty("resultGoAccuracyText").objectReferenceValue = goAccText.GetComponent<Text>();
        managerSO.FindProperty("resultNoGoAccuracyText").objectReferenceValue = noGoAccText.GetComponent<Text>();
        managerSO.FindProperty("resultAvgRTText").objectReferenceValue = avgRTText.GetComponent<Text>();
        managerSO.FindProperty("startButton").objectReferenceValue = startBtn.GetComponent<Button>();
        managerSO.FindProperty("restartButton").objectReferenceValue = restartBtn.GetComponent<Button>();
        managerSO.FindProperty("quitButtonStart").objectReferenceValue = quitBtnStart.GetComponent<Button>();
        managerSO.FindProperty("quitButtonResult").objectReferenceValue = quitBtnResult.GetComponent<Button>();
        managerSO.FindProperty("moleSpawner").objectReferenceValue = spawner;

        // 設定預設 Sprite
        if (goMoleSprite != null)
            managerSO.FindProperty("goMoleSprite").objectReferenceValue = goMoleSprite;
        var noGoSprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Art/GoNoGo/NoGoMole.png");
        if (noGoSprite != null)
            managerSO.FindProperty("noGoMoleSprite").objectReferenceValue = noGoSprite;

        managerSO.ApplyModifiedProperties();

        // ===== 連接 MoleSpawner =====
        var spawnerSO = new SerializedObject(spawner);
        var holesProperty = spawnerSO.FindProperty("moleHoles");
        var holes = GameObject.FindObjectsOfType<MoleHole>();
        holesProperty.arraySize = holes.Length;
        for (int i = 0; i < holes.Length; i++)
        {
            holesProperty.GetArrayElementAtIndex(i).objectReferenceValue = holes[i];
        }
        spawnerSO.ApplyModifiedProperties();

        // 預設隱藏 GamePanel（顯示 StartPanel）
        gamePanel.SetActive(false);

        // 儲存場景到正確的路徑
        string scenePath = "Assets/Scenes/GoNoGo.unity";
        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), scenePath);

        Debug.Log("Go/No-Go 遊戲場景建構完成！場景已儲存至：" + scenePath);
    }

    // ===== 工具方法 =====

    private static GameObject CreateUIElement(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    private static GameObject CreatePanel(string name, Transform parent, Color bgColor)
    {
        var go = CreateUIElement(name, parent);
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        return go;
    }

    private static GameObject CreateText(string name, Transform parent, string content, int fontSize, Color color, TextAnchor alignment)
    {
        var go = CreateUIElement(name, parent);
        var text = go.AddComponent<Text>();
        text.text = content;
        text.fontSize = fontSize;
        text.color = color;
        text.alignment = alignment;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        if (text.font == null)
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;
        return go;
    }

    private static GameObject CreateButton(string name, Transform parent, string label, int fontSize, Color bgColor)
    {
        var go = CreateUIElement(name, parent);
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = bgColor * 1.2f;
        colors.pressedColor = bgColor * 0.8f;
        btn.colors = colors;

        // 按鈕文字
        var textGO = CreateText("Text", go.transform, label, fontSize, TEXT_WHITE, TextAnchor.MiddleCenter);
        StretchFull(textGO);

        return go;
    }

    private static void StretchFull(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
    }

    private static void SetAnchored(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = sizeDelta;
    }

    private static void SetLayoutElement(GameObject go, float preferredHeight)
    {
        var le = go.AddComponent<LayoutElement>();
        le.preferredHeight = preferredHeight;
    }
}
