# Go/No-Go 打地鼠遊戲 — 上機前環境準備

上課時我們會用 Unity 這套軟體來操作遊戲專案。
**請在上課前把以下軟體裝好、專案下載好**，這樣當天就能直接開始。

整個流程大約 30~60 分鐘，大部分時間是等下載。
建議找一個網路穩定的環境（學校 Wi-Fi 或家裡）來準備。

---

## 你的筆電需要符合這些條件

### Windows 筆電

- 作業系統：**Windows 10** 或 **Windows 11**
- 記憶體（RAM）：**8 GB 以上**（4 GB 會很卡）
- 硬碟剩餘空間：**至少 10 GB**

> 不確定自己的規格？按 `Ctrl + Shift + Esc` 打開工作管理員，點「效能」分頁可以看到記憶體大小。

### Mac 筆電

- 作業系統：**macOS Big Sur（11）** 或更新版本
- 記憶體（RAM）：**8 GB 以上**
- 硬碟剩餘空間：**至少 10 GB**

> 不確定自己的版本？點螢幕左上角的蘋果圖示 >「關於這台 Mac」就能看到。

---

## 第一步：註冊 Unity 帳號

使用 Unity 之前需要先有一個免費的 Unity 帳號。

1. 用瀏覽器打開：https://id.unity.com
2. 點擊 **Create a Unity ID**（建立 Unity 帳號）
3. 填寫以下資料：
   - **Email**：填你常用的 Email
   - **Password**：設定密碼（至少 8 個字，要包含大小寫和數字）
   - **Username**：取一個使用者名稱（英文，不能跟別人重複）
   - **Full Name**：填你的名字（中文英文都可以）
4. 勾選同意使用條款
5. 完成人機驗證（勾選「我不是機器人」或點圖片）
6. 點擊 **Create a Unity ID**
7. 去你的 **Email 信箱**收驗證信，點信裡面的確認連結

> 收到「Email confirmed」的畫面就代表帳號註冊完成了。如果找不到信，記得看垃圾郵件資料夾。

---

## 第二步：安裝 Unity Hub（管理工具）

Unity Hub 是一個「管理中心」，用來幫你安裝 Unity 編輯器和開啟專案。

1. 用瀏覽器打開：https://unity.com/download
2. 點擊畫面中間的 **Download Unity Hub** 按鈕
3. 下載完成後：
   - **Windows**：雙擊下載的 `.exe` 檔案，按照畫面一路點 **Next** > **Install** > **Finish**
   - **Mac**：雙擊下載的 `.dmg` 檔案，把 Unity Hub 的圖示**拖到右邊的 Applications 資料夾**
4. 安裝好之後，打開 Unity Hub

> 看到 Unity Hub 的主畫面就代表這一步完成了。

---

## 第三步：登入帳號 & 啟用免費授權

打開 Unity Hub 之後，需要登入你的帳號並啟用免費的 Personal 授權。

### 登入帳號

1. Unity Hub 會顯示登入畫面，點 **Sign in**（登入）
2. 瀏覽器會自動打開 Unity 的登入頁面
3. 輸入你在第一步註冊的 **Email** 和 **密碼**，點 **Sign in**
4. 登入成功後，瀏覽器會顯示「You can close this tab」，回到 Unity Hub

### 啟用 Personal 授權

登入後 Unity Hub 通常會自動啟用授權。如果沒有，或是畫面要求你選擇授權：

1. 點左邊側邊欄最下方的齒輪圖示（**Settings**，或稱 **Preferences**）
2. 點擊 **Licenses**（授權）
3. 點擊 **Add**（新增）或 **+ Add license**
4. 選擇 **Get a free personal license**（取得免費個人版授權）
5. 閱讀條款後，點擊 **Agree and get personal edition license**（同意並取得）

> 看到授權清單中出現 **Personal** 就代表啟用成功了。

---

## 第四步：安裝 Unity 編輯器

Unity Hub 裝好之後，還需要透過它來安裝「Unity 編輯器」，這才是真正用來做遊戲的軟體。

1. 打開 **Unity Hub**
2. 點左邊側邊欄的 **Installs**（安裝）
3. 點右上角的 **Install Editor**（安裝編輯器）
4. 畫面會列出可安裝的版本，找到 **2022.3.x LTS**
   - 「LTS」代表長期支援版本，旁邊通常會有綠色標籤
   - 小版本號不需要完全一樣（例如 2022.3.50 也可以）
5. 點擊該版本旁邊的 **Install** 按鈕
6. 接著會出現「模組選擇」畫面：
   - **Windows 使用者**：勾選 **Microsoft Visual Studio Community**（程式碼編輯器）
   - **Mac 使用者**：這裡**不用勾選任何東西**（Mac 的編輯器我們在下一步另外裝）
   - 其他選項（Android、iOS、WebGL 等等）**都不用勾**
7. 點擊 **Install** 開始下載安裝
8. 下載量大約 3~5 GB，耐心等待（進度條會顯示在畫面上）

> 安裝完成後，回到 Installs 頁面，看到 **2022.3.x** 出現在清單中就成功了。

### 如果找不到 2022.3.x 版本

1. 用瀏覽器打開：https://unity.com/releases/editor/archive
2. 點擊頁面上方的 **Unity 2022.x** 分頁
3. 找到 **2022.3.62f3**（或附近的版本）
4. 點擊該版本旁邊的 **Unity Hub** 按鈕
5. 瀏覽器會問你是否要打開 Unity Hub，點「允許」或「開啟」
6. Unity Hub 會自動開始安裝

---

## 第五步：安裝程式碼編輯器

上課時我們會打開程式碼來看和修改，需要一個「程式碼編輯器」。
它就像 Word 是用來寫文件的，程式碼編輯器是用來寫程式的。

### Windows 使用者：Visual Studio Community

如果你在第四步安裝 Unity 時有勾選 **Microsoft Visual Studio Community**，它應該已經裝好了。

確認方式：在電腦左下角的「開始」搜尋「**Visual Studio**」，如果找得到就代表已安裝。

如果沒有裝到，手動安裝：

1. 用瀏覽器打開：https://visualstudio.microsoft.com/zh-hant/free-developer-offers/
2. 在 **Visual Studio Community** 底下點擊「免費下載」
3. 下載完成後雙擊安裝檔
4. 安裝畫面會問你要裝哪些「工作負載」，勾選「**使用 Unity 的遊戲開發**」
5. 點擊「**安裝**」，等待完成

### Mac 使用者：Visual Studio Code（VS Code）

Mac 版的 Visual Studio 已經停止支援了，所以我們改用微軟的另一款免費編輯器 **VS Code**。

1. 用瀏覽器打開：https://code.visualstudio.com
2. 點擊 **Download for Mac** 按鈕
3. 下載完成後，打開 `.zip` 檔案，把 **Visual Studio Code** 拖到 **Applications**（應用程式）資料夾
4. 打開 **VS Code**
5. 安裝 C# 語言支援（這樣看程式碼才會有顏色標示）：
   - 點左邊側邊欄的方塊圖示（**Extensions**，延伸模組）
   - 在搜尋框輸入 **C#**
   - 找到 **C# Dev Kit**（由 Microsoft 發布的），點 **Install**
   - 再搜尋 **Unity**，找到 **Unity** 擴充套件（由 Microsoft 發布的），也點 **Install**

---

## 第六步：設定 Unity 使用你的程式碼編輯器

這一步是讓 Unity 知道「打開程式碼時要用哪個軟體」。

1. 打開 Unity Hub，開啟任意一個專案（或先做完第七步再回來做這步也可以）
2. 在 Unity 編輯器最上方的選單：
   - **Windows**：點 **Edit** > **Preferences**
   - **Mac**：點 **Unity** > **Settings**（或 **Preferences**）
3. 在左邊清單中點 **External Tools**（外部工具）
4. 找到 **External Script Editor**（外部程式碼編輯器）這個下拉選單
5. 選擇你安裝的編輯器：
   - **Windows**：選 **Microsoft Visual Studio Community 2022**
   - **Mac**：選 **Visual Studio Code**
6. 關閉 Preferences 視窗

> 設定好之後，在 Unity 裡面雙擊任何程式碼檔案，就會自動用你選的編輯器打開。

---

## 第七步：下載遊戲專案

1. 用瀏覽器打開：https://github.com/bnlnccu/Go-NoGo
2. 點擊綠色的 **<> Code** 按鈕
3. 在彈出的選單最下方，點擊 **Download ZIP**
4. 下載完成後，找到下載的 `Go-NoGo-main.zip` 檔案
5. **解壓縮**：
   - **Windows**：對 ZIP 檔按右鍵 >「**解壓縮全部**」>「**解壓縮**」
   - **Mac**：雙擊 ZIP 檔，會自動解壓縮
6. 解壓縮後會得到一個叫 **Go-NoGo-main** 的資料夾
7. 把這個資料夾移到你方便找到的地方（例如桌面或文件資料夾）

---

## 第八步：用 Unity 開啟專案

1. 打開 **Unity Hub**
2. 點左邊側邊欄的 **Projects**（專案）
3. 點右上角的 **Open**（開啟）
4. 在彈出的視窗中，瀏覽到你剛才解壓縮的 **Go-NoGo-main** 資料夾，選擇它，按「確認」或「開啟」
5. 專案會出現在 Projects 清單裡
6. 如果出現黃色警告說「版本不符」，直接點 **Continue** 或 **Open with 2022.3.xx** 就好
7. **點擊清單中的專案名稱**來開啟它
8. 第一次開啟會花比較久的時間（約 1~5 分鐘），Unity 需要處理所有的資源檔案
9. 看到 Unity 編輯器的完整畫面就代表成功了

---

## 第九步：測試遊戲能不能跑

1. 在 Unity 編輯器**最上方的選單列**，點 **Tools**
2. 滑鼠移到 **Go-No-Go**，再點 **建構遊戲場景**
3. 等幾秒鐘，畫面下方的 Console 面板會顯示「Go/No-Go 遊戲場景建構完成」
4. 點 Unity 編輯器**正上方中間**的 **▶ 播放按鈕**
5. 如果畫面中出現「**Go / No-Go 打地鼠**」的標題和「**開始遊戲**」按鈕，就代表一切準備就緒
6. 再按一次 **▶** 就會停止遊戲

### 順便測試程式碼編輯器

1. 在 Unity 編輯器下方的 **Project** 面板中，點開 **Assets > Scripts > GoNoGo** 資料夾
2. 雙擊 **GoNoGoManager** 這個檔案
3. 如果程式碼編輯器自動打開，並且你能看到一堆彩色的程式碼文字，就代表設定正確

**恭喜！環境準備完成，上課當天可以直接開始操作。**

---

## 遇到問題？

### 「Unity Hub 說版本不對，叫我下載新版本」

不需要額外下載。只要你裝的是 **2022.3** 開頭的任何版本，出現提示時直接點 **Continue** 就好。

### 「打開專案後下面出現紅色錯誤文字」

1. 點選單 **Tools > Go-No-Go > 建構遊戲場景**
2. 如果紅色錯誤還在，點選單 **Assets > Reimport All**，等它重新處理（約 2~5 分鐘）

### 「按了播放按鈕畫面全黑」

你可能還沒建構場景。先點選單 **Tools > Go-No-Go > 建構遊戲場景**，再按播放。

### 「雙擊程式碼檔案沒有反應 / 打開了錯誤的軟體」

回到第六步，確認 **External Script Editor** 有選到正確的編輯器。

### 「VS Code 打開程式碼沒有顏色」

確認有安裝 **C# Dev Kit** 延伸模組（第五步的 Mac 安裝流程第 5 點）。

### 「筆電很卡」

- 關掉其他不需要的程式（尤其是瀏覽器分頁，它很吃記憶體）
- 如果記憶體只有 4 GB，Unity 會非常吃力，建議跟同學共用一台 8 GB 以上的電腦

### 「收不到 Unity 的驗證信」

- 檢查垃圾郵件資料夾
- 確認 Email 有打對
- 回到 https://id.unity.com 重新寄一次驗證信

### 「Windows 可以用 VS Code 嗎？」

可以。如果你本來就習慣用 VS Code，按照第五步 Mac 的 VS Code 安裝方式操作即可（下載網址改選 Windows 版本），裝好後記得回到第六步把 Unity 的 External Script Editor 改選 **Visual Studio Code**。
