# Gorilla Tag Mod Manager (BepInEx)

This repository contains a BepInEx plugin for Gorilla Tag that adds an in-game **Mod Manager** button and window.

## Features

- Adds an **Open Mod Manager** button in-game (bottom-left corner).
- Press **F7** to toggle the manager window.
- Lists loaded BepInEx plugins.
- Lets you toggle compatible plugin components on/off at runtime.

> Runtime toggling depends on the target mod. Some mods may not support being disabled while the game is running.

## Build

```bash
dotnet build -c Release
```

## Install

1. Build the project.
2. Copy `bin/Release/netstandard2.1/GorillaTagModManager.dll` into your Gorilla Tag BepInEx plugins folder:
   - `Gorilla Tag/BepInEx/plugins/`
3. Launch Gorilla Tag.

## Use

- Click **Open Mod Manager** in-game, or press **F7**.
- Click **Enable/Disable** next to compatible mods.
