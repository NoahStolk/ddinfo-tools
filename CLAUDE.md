# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What this is

Cross-platform Devil Daggers modding tools, practice tools, and custom leaderboards client. A single ImGui desktop app that runs on Windows and Linux and bundles what used to be several separate Windows-only tools (Survival/Spawnset Editor, Asset Editor, Custom Leaderboards, Mod Manager, Replay Editor, Practice).

## Toolchain

- .NET SDK `10.0.100` or later (`global.json` rolls forward `latestMajor`).
- C# `LangVersion=14.0`, `Nullable=enable`, `WarningsAsErrors=nullable`, `AnalysisMode=All`, `InvariantGlobalization=true` (set in `src/Directory.Build.props`). Treat nullable warnings as build-breaking.
- Analyzers enabled repo-wide: Nullable.Extended, Roslynator, SonarAnalyzer.CSharp, StyleCop.Analyzers.
- Solution file is **`.slnx`** (XML), not `.sln`: `src/DevilDaggersInfo.Tools.slnx`.
- `.editorconfig`: `.cs` files use **tabs** (size 4). `.csproj`/`.yml` use 2-space indent.

## Common commands

Run the app from the repo root:

```bash
dotnet run --project src/DevilDaggersInfo.Tools/DevilDaggersInfo.Tools.csproj
```

Build / test the whole solution (mirrors CI):

```bash
dotnet build src/DevilDaggersInfo.Tools.slnx -c Release
dotnet test  src/DevilDaggersInfo.Tools.slnx -c Release --no-build
```

There is no test project in the solution today — `dotnet test` is a no-op kept for parity with CI. Don't assume tests exist.

Produce a release build (single-file, trimmed, self-contained):

```bash
# Linux helper:
scripts/build-release.sh
# CI publishes both win-x64 and linux-x64 with the same flags from .github/workflows/release.yml,
# triggered by pushing a `v*` tag.
```

## Project layout

Three projects, all under `src/`:

- **`DevilDaggersInfo.Tools`** — the application. Entry point `Program.cs` constructs a `Container` (StrongInject) and runs `Application`. Output assembly name `ddinfo-tools`.
- **`DevilDaggersInfo.Tools.Engine`** — thin rendering/input layer over Silk.NET (GLFW + OpenGL) and ImGui.NET (`Shader`, `Texture`, loaders, math helpers, intersections).
- **`DevilDaggersInfo.Tools.Engine.Content`** — asset/content types (`MeshContent`, `ModelContent`, `ShaderContent`, `TextureContent`, parsers). Empty `.csproj` — picks up everything from `Directory.Build.props`.

External NuGet dependencies of note: `DevilDaggersInfo.Core` (spawnset/mod/replay parsers, AES encryption), `DevilDaggersInfo.Web.ApiSpec.Tools` (server contracts), `Silk.NET.{GLFW,OpenGL}`, `ImGui.NET`, `StrongInject`, `Serilog.Sinks.File`, `NativeFileDialogSharp`, `SixLabors.ImageSharp`.

## Architecture you need to know before editing

**Compile-time DI via StrongInject.** All app singletons are registered as attributes on `Container` (`src/DevilDaggersInfo.Tools/Container.cs`). Adding a new window/service usually means adding a `[Register<T>(Scope.SingleInstance)]` line there *and* taking the dependency through a constructor. `Application` is the root; `Program.cs` resolves it via `Owned<Application>`.

**`Root` is a legacy global and is `[Obsolete]`.** New code should take dependencies via constructor injection through `Container`. `Root` still exposes a Serilog `Log`, the loaded ImGui fonts, an `AesBase32Wrapper`, and the platform-specific `GameMemoryService` / `GameWindowService` / `IPlatformSpecificValues` — prefer to leave those calls alone rather than expand them.

**Platform compilation is driven by `WINDOWS` / `LINUX` `DefineConstants`** set in `DevilDaggersInfo.Tools.csproj`. In `Debug`, the constant follows the build host OS; in `Release`, it follows `RuntimeIdentifier` (with a CI fallback to `LINUX`). `OutputType` switches between `WinExe` and `Exe` accordingly. Anywhere you see `#if WINDOWS` / `#elif LINUX` (notably in `Root.cs`), there must be a parallel implementation under `NativeInterface/Services/{Windows,Linux}/` implementing `INativeMemoryService` / `INativeWindowingService`. Keep both sides in sync.

**Native file dialogs have a Wayland-specific path.** `Container.CreateNativeFileDialog` returns `NativeFileDialogWayland` when `XDG_SESSION_TYPE=wayland` (or `WAYLAND_DISPLAY` is set on Linux), otherwise `NativeFileDialog` (NativeFileDialogSharp). Both implement `INativeFileDialog`, and `Application.Main` polls `_nativeFileDialog.Update()` each frame.

**Main loop** (`Application.Run` → `Main` → `Render`) is capped at ~300 Hz with `Thread.Yield` and uses GLFW's `SwapInterval(1)` for VSync. ImGui is dockspace-based (`ImGui.DockSpaceOverViewport`). UI is organized by feature under `Ui/<Feature>/` (AssetEditor, CustomLeaderboards, ModManager, Practice, ReplayEditor, SpawnsetEditor, Main, Popups, Config). 3D scenes live under `Scenes/`.

**Trimming and AOT-ish constraints.**
- `EnableTrimAnalyzer=true`, `SuppressTrimAnalysisWarnings=false` — trim warnings are real and must be fixed, not ignored. Release publishes with `PublishTrimmed=True` + `PublishSingleFile=True` + `SelfContained=True`.
- `JsonSerializerIsReflectionEnabledByDefault=false` — every JSON (de)serialization must go through a source-generated `JsonSerializerContext`. The contexts live in `JsonSerializerContexts/` (`ApiModelsContext`, `AssetPathsContext`, `UserJsonModelsContext`). When you add a new serialized type, register it on the appropriate context.

**User data location.** `UserSettings` and `UserCache` persist to `Environment.SpecialFolder.LocalApplicationData/ddinfo-tools/` (`settings`, `cache`, `imgui.ini`). They are static singletons today (marked `// TODO: Rewrite to instance.`); load order in `Program.cs` is `UserSettings.Load()` → `UserCache.Load()` *before* the container is constructed, because `Container` reads `UserCache.Model` when creating the GLFW window and ImGui controller.

**Encryption secret.** `Root.AesBase32Wrapper` reads `Content/encryption.ini` as an embedded resource. The file is **not** in the repo — CI writes it from the `ENCRYPTION` GitHub secret before publish (see `release.yml`). Local debug builds run fine without it; anything that needs the wrapper falls back to `null` and logs.

**Networking** lives in `Networking/`. `ApiHttpClient` + `ApiResult`/`ApiError` are the transport; `Networking/TaskHandlers/` contains one file per server endpoint (fetch leaderboards, upload submissions, etc.) that wraps the call in an async handler the UI awaits.

**Game integration.** `GameMemory/` and `GameWindow/` are platform-agnostic services that delegate to the per-OS `INativeMemoryService` / `INativeWindowingService` to read live game state from a running Devil Daggers process. `GameMemoryServiceWrapper` is the DI-friendly shell around the static `Root.GameMemoryService`.

## Conventions

- `Directory.Build.props` is the source of truth for `TargetFramework`, language version, nullable, analyzers, and `RuntimeIdentifiers`. Don't redefine these per project.
- Tab indentation in `.cs`. Match existing brace and using-order style — StyleCop will flag you otherwise.
- `[Obsolete]` markers (e.g. `Root`) are intentional — don't clear them by suppression; migrate callers off instead.
- The `CHANGELOG.md` is hand-edited. When changing user-visible behavior, add a bullet under `## [unreleased]` in the appropriate section.
