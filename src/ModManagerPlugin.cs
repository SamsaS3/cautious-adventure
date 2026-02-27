using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Bootstrap;
using UnityEngine;

namespace GorillaTagModManager;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class ModManagerPlugin : BaseUnityPlugin
{
    public const string PluginGuid = "com.cautiousadventure.gorillatag.modmanager";
    public const string PluginName = "GorillaTag Mod Manager";
    public const string PluginVersion = "1.0.0";

    private readonly Dictionary<string, bool> _pluginStates = new();
    private bool _windowOpen;
    private Rect _windowRect = new(20f, 20f, 500f, 500f);
    private Vector2 _scrollPos;

    private void Start()
    {
        RefreshPluginStateCache();
        Logger.LogInfo("GorillaTag Mod Manager loaded. Press F7 to open the manager window.");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F7))
        {
            _windowOpen = !_windowOpen;
            RefreshPluginStateCache();
        }
    }

    private void OnGUI()
    {
        DrawOpenButton();

        if (_windowOpen)
        {
            _windowRect = GUI.Window(91337, _windowRect, DrawManagerWindow, "Mod Manager");
        }
    }

    private void DrawOpenButton()
    {
        const float width = 180f;
        const float height = 40f;
        var buttonRect = new Rect(20f, Screen.height - height - 20f, width, height);

        if (GUI.Button(buttonRect, _windowOpen ? "Close Mod Manager" : "Open Mod Manager"))
        {
            _windowOpen = !_windowOpen;
            RefreshPluginStateCache();
        }
    }

    private void DrawManagerWindow(int windowId)
    {
        GUILayout.Label("Toggle mods on/off while in-game.");
        GUILayout.Label("Note: not every mod supports runtime toggling.");

        if (GUILayout.Button("Refresh Mod List", GUILayout.Height(30f)))
        {
            RefreshPluginStateCache();
        }

        _scrollPos = GUILayout.BeginScrollView(_scrollPos, GUILayout.ExpandHeight(true));

        foreach (var plugin in Chainloader.PluginInfos.Values.OrderBy(info => info.Metadata.Name))
        {
            var instance = plugin.Instance;
            var key = plugin.Metadata.GUID;
            var displayName = $"{plugin.Metadata.Name} ({plugin.Metadata.Version})";

            if (key == PluginGuid)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(displayName);
                GUILayout.Label("(this manager)");
                GUILayout.EndHorizontal();
                continue;
            }

            var canToggle = instance is Behaviour;
            if (!_pluginStates.ContainsKey(key))
            {
                _pluginStates[key] = canToggle && ((Behaviour)instance).enabled;
            }

            GUILayout.BeginHorizontal();
            GUILayout.Label(displayName, GUILayout.Width(350f));

            if (!canToggle)
            {
                GUILayout.Label("Cannot toggle", GUILayout.Width(120f));
            }
            else
            {
                var currentlyEnabled = ((Behaviour)instance).enabled;
                var toggleLabel = currentlyEnabled ? "Disable" : "Enable";

                if (GUILayout.Button(toggleLabel, GUILayout.Width(120f)))
                {
                    var behaviour = (Behaviour)instance;
                    behaviour.enabled = !behaviour.enabled;
                    _pluginStates[key] = behaviour.enabled;
                    Logger.LogInfo($"{plugin.Metadata.Name} toggled {(behaviour.enabled ? "ON" : "OFF")}");
                }
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();

        GUI.DragWindow(new Rect(0f, 0f, 5000f, 20f));
    }

    private void RefreshPluginStateCache()
    {
        _pluginStates.Clear();

        foreach (var entry in Chainloader.PluginInfos.Values)
        {
            if (entry.Instance is Behaviour behaviour)
            {
                _pluginStates[entry.Metadata.GUID] = behaviour.enabled;
            }
        }
    }
}
