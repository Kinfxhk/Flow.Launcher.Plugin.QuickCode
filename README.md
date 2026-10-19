# Flow.Launcher.Plugin.QuickCode

Launch Chinese-named programs (e.g. 記事本、小算盤、小畫家) by typing their 速成 (Quick) or 倉頡 (Cangjie) code — a replacement for the manual workaround of adding pinyin aliases to every Chinese-named shortcut for HK/TW 速成/倉頡 users.

## Features

- Indexes all `.lnk` and `.url` shortcuts under the Windows Start Menu (`%ProgramData%\Microsoft\Windows\Start Menu\Programs` and `%APPDATA%\Microsoft\Windows\Start Menu\Programs`, recursive).
- Match a program by its 速成 code, its full 倉頡 code, or a prefix of the 倉頡 code.
- Space-separated tokens map one-to-one onto the name's Chinese characters (non-Han characters are skipped); a single token with no spaces is treated as concatenated 速成 codes (each character taking 1 or 2 letters).
- Latin words in a name can be matched by a case-insensitive word prefix (e.g. `vis` → `Visual Studio`).
- Exact 速成 matches rank above full 倉頡, which ranks above prefixes; shorter names rank first.
- Subtitle shows each program's 速成 and 倉頡 codes plus its shortcut path; Enter launches it, and the context menu has 開啟檔案位置 (open file location).
- Re-indexes when Flow Launcher reloads plugin data, and at most every 10 minutes.

Not in v1: settings page, 注音/pinyin, file search, custom dictionaries, UWP/Store app indexing.

## Install

1. Download `Flow.Launcher.Plugin.QuickCode-v1.0.0.zip`.
2. Unzip it into `%APPDATA%\FlowLauncher\Plugins\QuickCode-1.0.0\` and restart Flow Launcher.

Or use the Package Manager: `pm install <zip url>`.

## Build

Run `./build.ps1` (PowerShell 5.1 or pwsh). It restores, builds Release, runs the tests and publishes the zip to `dist/`.

## Privacy

This plugin only reads the shortcut file names and paths under your Start Menu. It never sends any data anywhere and makes no network requests.

## Licence

MIT. The bundled 倉頡 table is derived from Unicode Unihan data under the Unicode License V3 — see THIRD-PARTY-NOTICES.md.

---

# Flow.Launcher.Plugin.QuickCode（繁體中文）

以速成或倉頡碼開啟中文名稱的程式（例如記事本、小算盤、小畫家），取代過去要為每個中文捷徑手動加入拼音別名的做法，讓香港及台灣的速成／倉頡使用者也能快速搜尋程式。

## 功能

- 索引 Windows 開始功能表內的所有 `.lnk` 與 `.url` 捷徑（`%ProgramData%\Microsoft\Windows\Start Menu\Programs` 及 `%APPDATA%\Microsoft\Windows\Start Menu\Programs`，包含子資料夾）。
- 以程式的速成碼、完整倉頡碼，或倉頡碼的前綴進行比對。
- 以空格分隔的輸入會逐一對應名稱中的中文字（非漢字字元會被略過）；沒有空格的單一輸入會視為連續的速成碼（每個字佔 1 或 2 個字母）。
- 名稱中的英文字可用大小寫不敏感的字首比對（例如 `vis` → `Visual Studio`）。
- 完全速成碼比對優先於完整倉頡碼，其次為前綴；名稱較短者優先。
- 副標題顯示每個程式的速成碼與倉頡碼及其捷徑路徑；Enter 開啟程式，右鍵選單提供「開啟檔案位置」。
- 在 Flow Launcher 重新載入資料時重新索引，並最多每 10 分鐘自動重新索引一次。

v1 不包含：設定頁面、注音／拼音、檔案搜尋、自訂字典、UWP／商店應用程式索引。

## 安裝

1. 下載 `Flow.Launcher.Plugin.QuickCode-v1.0.0.zip`。
2. 解壓縮至 `%APPDATA%\FlowLauncher\Plugins\QuickCode-1.0.0\`，然後重新啟動 Flow Launcher。

或使用套件管理員：`pm install <zip url>`。

## 建置

執行 `./build.ps1`（PowerShell 5.1 或 pwsh）。它會還原、以 Release 組態建置、執行測試，並將 zip 輸出至 `dist/`。

## 隱私

此外掛只會讀取開始功能表內捷徑的檔名與路徑，不會傳送任何資料，也不會發出任何網路要求。

## 授權

MIT。內建的倉頡碼表衍生自 Unicode Unihan 資料，依 Unicode License V3 授權——詳見 THIRD-PARTY-NOTICES.md。

## Sponsor / 贊助

If this tool helps you, you can buy me a coffee. 如果這個工具對你有幫助，歡迎請我喝杯咖啡。

- Buy Me a Coffee: https://buymeacoffee.com/kinfxhk
