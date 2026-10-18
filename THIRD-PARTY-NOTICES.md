# Third-party notices

## Unicode Unihan data (`kCangjie`)

- Component: Cangjie (倉頡) input-method codes from the Unicode Character Database (Unihan).
- File: `src/Flow.Launcher.Plugin.QuickCode/Data/cangjie.tsv` (generated from `Unihan_DictionaryLikeData.txt`, `kCangjie` field).
- Unicode version: 18.0.0 (UCD dated 2026-07-31).
- Licence: Unicode License V3 (https://www.unicode.org/license.txt) — permissive.
- Source URL: https://www.unicode.org/Public/18.0.0/ucd/Unihan.zip (the `Unihan_DictionaryLikeData.txt` file inside).

The generated table is embedded as a compiled resource. The original Unihan download is not redistributed.

## Flow.Launcher.Plugin (NuGet, 5.3.2)

- Component: the Flow Launcher plugin SDK, referenced at build time.
- Licence: MIT (https://github.com/Flow-Launcher/Flow.Launcher).
- Not bundled: Flow Launcher ships this assembly itself; the plugin zip excludes it.
