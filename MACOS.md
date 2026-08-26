# macOS (arm64)

This document covers what is different about the macOS build. Everything not mentioned here
behaves as it does on Windows and Linux.

macOS support is **source-only**: there is no published macOS artifact and CI does not build
one. `.github/workflows/release.yml` is untouched by this port.

## Contents

- [Building and running](#building-and-running)
- [The `sudo` requirement](#the-sudo-requirement)
- [Where the game lives](#where-the-game-lives)
- [Finding the ddstats block](#finding-the-ddstats-block)
- [OpenGL differences](#opengl-differences)
- [What is not supported](#what-is-not-supported)

## Building and running

```bash
dotnet build src/DevilDaggersInfo.Tools.Engine/DevilDaggersInfo.Tools.Engine.csproj
dotnet build src/DevilDaggersInfo.Tools/DevilDaggersInfo.Tools.csproj
dotnet run  --project src/DevilDaggersInfo.Tools/DevilDaggersInfo.Tools.csproj
```

Build the two projects individually rather than the solution. In `Debug` the platform
constant follows the build host, so a macOS host defines `OSX` and the right arm compiles.
In `Release` it follows `RuntimeIdentifier`, which is unset for a plain solution build — so
none of `WINDOWS`, `LINUX` or `OSX` is defined, no platform arm compiles, and the build
fails on the platform-specific files rather than on anything macOS-specific. Pass
`-r osx-arm64` if you want a Release build.

Only `osx-arm64` is wired up. An Intel Mac would need `osx-x64` adding to the `Choose`
block in `DevilDaggersInfo.Tools.csproj` and has not been tested.

## The `sudo` requirement

**Reading another process's memory on macOS requires root.** There is no entitlement or
per-user permission that substitutes for it on a locally built, unsigned binary: `task_for_pid`
against a process you do not own returns `KERN_FAILURE` regardless of what the user has
clicked in System Settings.

So the three features that read or write the game's memory work only when the app is
launched with `sudo`:

- practice mode's live stats,
- reading a replay out of a running game,
- injecting a replay into one.

Everything else — the spawnset editor, asset editor, mod manager, replay editor's file
handling, and custom leaderboards — needs no elevation and works normally.

This is the one place the port changes shared UI, and it is worth understanding why. When
memory is unavailable the honest answer differs by cause, and the two causes need different
things from the user:

| Status | Means | What the user is told |
|---|---|---|
| `Unresolved` | The game is not running | "Devil Daggers is not running." |
| `Resolved` | The block was located | — |
| `MemoryUnreadable` | The game's memory could not be read at all | Restart under `sudo`, and which features that affects |
| `BlockNotFound` | Memory read fine, but holds no stats block | Start a run, or this is an unknown build |

`MemoryUnreadable` and `BlockNotFound` are produced only by the macOS scan path. Windows and
Linux resolve to `Unresolved` or `Resolved` and nothing else, so the branches that check for
them are unreachable on those platforms by construction — the "game not running" screen is
byte-for-byte what it was before this branch.

Gating on `BlockAddressStatus` rather than on `IsInitialized` is deliberate. `IsInitialized`
is also false when the game simply is not running, which happens on every platform, so
branching on it would have changed Windows and Linux behaviour.

## Where the game lives

On macOS the game ships as an `.app` bundle, and `dd/`, `res/` and `mods/` live inside
`Contents/Resources` rather than beside the executable. The bundle's parent directory is
therefore *not* a valid installation directory and fails validation with
`File 'dd/survival' does not exist.`

`OSXValues.DefaultInstallationPath` points inside the bundle accordingly:

```
~/Library/Application Support/Steam/steamapps/common/devildaggers/Devil Daggers.app/Contents/Resources
```

The process is named `Devil Daggers` — capitals, and a space — where the Linux one is
`devildaggers`, so process lookup normalises the name before comparing.

## Finding the ddstats block

Windows and Linux both derive from `MarkerOffsetMemoryService`: they read a pointer at an
offset the DevilDaggers.info API supplies, and declare `RequiresMarkerOffset => true`.

There is no API route serving that offset for macOS, so `OSXMemoryService` declares
`RequiresMarkerOffset => false` and finds the block by scanning instead — walking every
readable region of the game's address space via `mach_vm_region` and searching each for the
block's marker.

Four things about that scan are load-bearing:

- **A marker match is not a block.** The game's own string literal contains the same bytes.
  Every candidate is validated with `MainBlock.IsValid`, which checks the marker, its
  terminating null, a plausible format version, and that both 32-byte name fields contain a
  null. False positives are real and were observed during development.
- **`MainBlock.IsValid` must be called before `new MainBlock(...)`.** The constructor slices
  the name fields up to their null byte and throws `ArgumentOutOfRangeException` on a buffer
  without one. On the render loop, that is a crash.
- **The scan is expensive.** It reads on the order of gigabytes across a few hundred regions
  and takes seconds, varying several-fold run to run with heap layout. The resolved address
  is cached with the pid it was found in and reused until it stops reading back as a block, and
  a scan that comes up empty sets a five-second cooldown before another may start — otherwise
  the ~300 Hz render loop would start one continuously.
- **Nothing in it may throw.** `GameMemoryServiceWrapper.Scan()` runs from that render loop,
  so an escaping exception takes the app down instead of reporting the problem. Failures
  return null or no-op and log their reason once behind a `bool` field, so the log does not
  fill at 300 lines a second. This includes native symbol resolution: an unresolvable
  P/Invoke throws at its *call* site, so those are wrapped in
  `catch (DllNotFoundException or EntryPointNotFoundException)`.

A read of an unmapped address returns `KERN_INVALID_ADDRESS` rather than signalling, so
speculative probing during the walk is safe.

Two conventions in the Mach bindings, both of which cost time to find:

- **Omit `SetLastError`.** Mach reports failure through the `kern_return_t` return value, not
  through `errno`.
- **Pass `vm_region` info as an `int*`** from a `stackalloc int[9]`, rather than as an
  `[Out] int[]`. This sidesteps `LibraryImport` array marshalling entirely.

## OpenGL differences

Two `#if OSX` branches in `Container.cs`, both about the context rather than about rendering:

- macOS refuses to create a Core-profile context that is not also forward-compatible.
  Without `OpenGLForwardCompat`, `glfwCreateWindow` returns null instead of a window, with
  no error.
- Debug output is `glDebugMessageCallback`, which is OpenGL 4.3 / `KHR_debug`. macOS caps at
  4.1 and never exposed that extension, so the `Debug`-only debug context and its callback
  are compiled out with `#if DEBUG && !OSX`. Windows and Linux debug builds keep both.

## What is not supported

- **Intel Macs.** `osx-x64` is not wired into the csproj and has not been tested.
- **`AppOperatingSystem`.** The enum ships in the `DevilDaggersInfo.Web.ApiSpec.Tools`
  package and has no macOS member, so the macOS build reports `Linux`. This only affects
  what the server is told about the submitting client's OS. Adding a member upstream would
  be the real fix.
- **Release solution builds without a RID.** See [Building and running](#building-and-running).
- **A published artifact.** Source builds only.

## Where the platform code lives

Platform compilation is driven by the `WINDOWS` / `LINUX` / `OSX` `DefineConstants` set in
`DevilDaggersInfo.Tools.csproj`, and only four files switch on platform: `Container.cs`,
`ContentManager.cs`, `Ui/Config/ConfigLayout.cs`, and the csproj itself. Everything else goes
behind `INativeMemoryService`, `INativeWindowingService`, and `IPlatformSpecificValues`, with
a parallel implementation under `NativeInterface/Services/{Windows,Linux,OSX}/` and a values
class under `Platforms/`. A new `#if` anywhere else is a sign something belongs behind one of
those three interfaces instead.
