# Mercury.Core

<p align="center">
  <img src="icon.png" alt="Mercury.Core Logo" width="96"/>
</p>

<p align="center">
  <strong>The core class library powering the Mercury music player.</strong><br/>
  YouTube Music search, stream resolution, lyrics, and browse — all in a pure .NET library.
</p>

<p align="center">
  <img alt=".NET" src="https://img.shields.io/badge/.NET-6.0-512BD4?style=flat-square&logo=dotnet"/>
  <img alt="C#" src="https://img.shields.io/badge/C%23-13+-239120?style=flat-square&logo=csharp"/>
  <img alt="License" src="https://img.shields.io/github/license/Aengstlicher1/Mercury.Core?style=flat-square"/>
</p>

---

## Overview

**Mercury.Core** is a pure .NET class library that provides all backend logic for the [Mercury](https://github.com/Aengstlicher1/Mercury) music player. It handles communication with the YouTube Music internal API, stream resolution, synchronized lyrics fetching, and home-feed browsing.

It is consumed by the Mercury Avalonia UI application as a **Git submodule** and has **no UI dependencies**, making it fully reusable in any .NET project.

---

## Features

- **Search** - Query YouTube Music for songs, videos, artists, albums, playlists, podcasts, and episodes
- **Stream Resolution** - Resolve playable audio/video stream URLs from YouTube video IDs
- **Lyrics** - Fetch plain and synchronized (timestamped) lyrics via [lrclib.net](https://lrclib.net)
- **Browse** - Retrieve the YouTube Music home feed with categorized content
- **Network Layer** - Internal HTTP client with proper YouTube Music API headers and request handling

---

## Project Structure

```
Mercury.Core/
├── src/
│   ├── Mercury.Core/
│   │   ├── YouTubeMusic.cs          # Static facade - entry point for all services
│   │   ├── Services/
│   │   │   ├── SearchService.cs     # YouTube Music search
│   │   │   ├── PlayerService.cs     # Audio/video stream resolution
│   │   │   ├── LyricsService.cs     # Synchronized lyrics from lrclib.net
│   │   │   └── BrowseService.cs     # Home feed / browse categories
│   │   ├── Models/
│   │   │   ├── Media.cs             # All Media (eg. Song, Video, Album, Playlist...)
│   │   │   ├── StreamInfo.cs        # Audio/video stream info
│   │   │   ├── StreamingData.cs     # Resolved stream container
│   │   │   ├── Thumbnail.cs         # Thumbnail model + ThumbArray wrapper
│   │   │   ├── LyricsResult.cs      # Plain + synced lyrics result
│   │   │   ├── Category.cs          # Browse category model
│   │   │   └── Enums.cs             # MediaType and other enums
│   │   ├── Json/
│   │   │   ├── Parsers/
│   │   │   │   ├── Generic/         # Generic parsers that are used in multiple places
│   │   │   │   ├── Search/          # Search result parsers
│   │   │   │   └── Browse/          # Browse feed parsers
│   │   ├── Network/
│   │   │   ├── Client.cs            # HTTP client for YouTube Music API
│   │   │   ├── ClientType.cs        # Client type configuration
│   │   │   ├── Endpoints.cs         # API endpoint definitions
│   │   │   └── RequestHandler.cs    # Request builder with headers and signing
│   │   └── Utils/
│   │   │   └── Syntax.cs            # JSON path syntax helpers
│   └── Mercury.Core.Test/           # Test project(for internal testing)
├── Mercury.Core.sln
├── icon.png
├── LICENSE
└── README.md
```

---

## Usage

### Static Facade

All services are accessible through the `YouTubeMusic` static class:

```csharp
using Mercury.Core;

// Search for songs
var results = await YouTubeMusic.Search.SearchAsync("Daft Punk");

// Resolve a stream
var streamData = await YouTubeMusic.Player.GetStreamingDataAsync("videoId");

// Fetch lyrics
var lyrics = await YouTubeMusic.Lyrics.GetLyricsAsync(results.First(x => x is Track));

// Browse home feed
var categories = await YouTubeMusic.Browse.GetHomeFeedAsync();
```

### Models

| Model | Description |
|---|---|
| `Media` | Abstract base for `Song`, `Video`, `Artist`, `Album`, `Playlist`, `Podcast`, `Episode` |
| `StreamingData` | Resolved stream container with `AudioStreamInfo` / `VideoStreamInfo` lists |
| `LyricsResult` | Contains `PlainLyrics` (string) and `SyncedLyrics` (timestamped lines) |
| `Thumbnail` | Thumbnail URL, width, height. `ThumbArray` provides collection helpers |
| `Category` | Browse category with a title and list of `Media` items |

---

## Requirements

- [.NET 6.0 SDK](https://dotnet.microsoft.com/download) or later

No external UI frameworks or platform-specific dependencies are required. Mercury.Core is a **pure .NET class library**.

---

## Building

```bash
git clone https://github.com/Aengstlicher1/Mercury.Core.git
cd Mercury.Core
dotnet build
```

---

## License

This project is licensed under the terms of the [MIT License](LICENSE).

---

<p align="center">
  Part of the <a href="https://github.com/Aengstlicher1/Mercury">Mercury Music Player</a> project.
</p>
