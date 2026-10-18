# Handoff — Flow.Launcher.Plugin.QuickCode

## What was built

A Flow Launcher plugin (`qc`) that launches Chinese-named Start Menu programs by 速成 (Quick) or 倉頡 (Cangjie) code.

- `src/Flow.Launcher.Plugin.QuickCode` — class library, `net9.0-windows10.0.19041.0`, `UseWPF=false`, single NuGet dependency `Flow.Launcher.Plugin` 5.3.2.
  - `CangjieTable` — pure lookup over an embedded `cangjie.tsv` (Unihan 18.0.0 `kCangjie`, 29,189 entries). 速成 = first + last letter.
  - `Matcher` — pure matcher (quick/full/prefix per token, concatenated 速成 single token, Latin word-prefix, score = quality then shorter-name-first).
  - `StartMenuIndexer` — enumerates `.lnk`/`.url` under both Start Menu "Programs" folders, recursive.
  - `Main` — implements `IAsyncPlugin`, `IAsyncReloadable`, `IContextMenu`. Reindexes on reload and every 10 min. Enter launches via `Process.Start(UseShellExecute=true)`; context menu 開啟檔案位置.
- `tests/QuickCode.Tests` — xUnit, 17 tests (table, quick-code conversion, matcher happy paths, mixed name, no-match, tie-breaking).
- `tools/GenTable.ps1` — regenerates `Data/cangjie.tsv` from the Unihan download.
- `tools/QuickCodeCli` — console harness (NOT shipped) for the real check.
- Packaging: `dist/Flow.Launcher.Plugin.QuickCode-v1.0.0.zip` contains only `plugin.json`, the dll, and `Images/icon.png` (64x64, generated with System.Drawing; no copied icons). No Flow.Launcher.Plugin.dll or JetBrains dlls.

## Decisions (simplest option satisfying the spec)

- UWP/Store apps via `shell:AppsFolder` — SKIPPED. The Windows Shell COM API (`IShellFolder`/`IShellItem`) is non-trivial interop; the spec permits skipping. Noted in README.
- Single-token (no space) queries are matched three ways: (a) quick/full/prefix against the first Han char, (b) concatenated 速成 splits (1–2 letters/char), (c) leading Latin word prefix. This keeps full-倉頡 typing working for single-character names (e.g. `yrsu` → 記) while still supporting the required `yujndm` → 記事本.
- No `.sln` file: `build.ps1` drives `src` and `tests` by explicit path. The CLI is built separately by hand.

## Commands run (real output)

- `dotnet build src/Flow.Launcher.Plugin.QuickCode/... -c Release` → 0 warnings, 0 errors.
- `dotnet test tests/QuickCode.Tests/... -c Release` → **Passed: 17, Failed: 0, Skipped: 0** (net9.0, run on the installed .NET 10 runtime via `<RollForward>LatestMajor</RollForward>`).
- `pwsh -File build.ps1` → exit 0; produced `dist/Flow.Launcher.Plugin.QuickCode-v1.0.0.zip` (166,938 bytes).
- `tools/QuickCodeCli` (real machine) → indexed **145** Start Menu shortcuts, **17** Chinese-named; queried 3 of them (`Riot用戶端`, `【小狼毫】安裝選項`, `【小狼毫】檢查新版本`) by computed 速成, concatenated 速成, and full 倉頡 — all matched. Full output in `evidence/real-index.txt`.

## NOT verified

- Context menu 開啟檔案位置 was not clicked in Flow.
- `pm install <url>` from the Flow plugin store was not tested (plugin not submitted to the store).
- UWP/Store app indexing is intentionally absent (not implemented).

## Known limits

- Only `.lnk`/`.url` in the two Start Menu "Programs" folders are indexed; no UWP apps, no files, no pinyin/注音.
- 速成 codes are derived from the Unihan 倉頡 table (first + last letter); this matches the standard Quick method but does not carry user-custom 速成 dictionaries.
- Context menu is identified via `Result.ContextData` (shortcut path) using Flow's `IContextMenu` interface.

## Definition of done

1. ✅ `./build.ps1` exits 0, tests pass, zip in `dist/`.
2. ✅ Real check ran on this machine; evidence committed at `evidence/real-index.txt`.
3. ✅ Everything committed on `main`.
4. ✅ This file.


## Reviewer verification (it-dog)

- KinTest VM, real Flow Launcher 2.1.3: the release zip was unzipped into `%APPDATA%\FlowLauncher\Plugins\QuickCode-1.0.0`; Flow log shows `Total init cost for <QuickCode 速成/倉頡>`. A test shortcut 記事本測試.lnk → notepad was found by `qc yu jn dm`, `qc yujndm` and `qc yrsu jlln dm`; Enter launched Notepad each time. Evidence: `evidence/vm/`.
