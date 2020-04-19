using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Rockgen.Unity;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace RockGen.Unity
{
public static class RockGeneratorGUI
{
    static readonly int MAX_LABEL_WIDTH;
    static readonly int CHARACTER_WIDTH;

    static readonly Vector3 DEFAULT_POS = new Vector3(2.5f, 2.5f, 2.5f);

    static RockGeneratorGUI()
    {
        CHARACTER_WIDTH = 7;

        var settings = new RockGenerationSettings();

        MAX_LABEL_WIDTH = new[] {
            nameof(settings.StockDensity),
            nameof(settings.TargetTriangleCount),
            nameof(settings.Distortion),
        }.Max(n => n.Length) * CHARACTER_WIDTH;
    }

    public static event Action<string> fileExported;

    public static bool OnGUI(RockGenerator generator)
    {
        var newSettings = new RockGenerationSettings(generator.Settings);

        newSettings.StockDensity = SettingSlider(nameof(newSettings.StockDensity),
                                                 newSettings.StockDensity, 2, 16, "N1");
        newSettings.TargetTriangleCount = Mathf.RoundToInt(
            SettingSlider(nameof(newSettings.TargetTriangleCount),
                          newSettings.TargetTriangleCount, 100, 2000,
                          "N0"
            )
        );
        newSettings.Distortion = SettingSlider(nameof(newSettings.Distortion),
                                               newSettings.Distortion, -2, 2);


        using (var gcs = new GUIChangedScope())
        {
            var Uniformity = Mathf.Clamp01(1 - newSettings.GridSettings.Randomness);
            Uniformity = SettingSlider(nameof(Uniformity), Uniformity, 0, 1);

            if (GUI.changed)
            {
                newSettings.GridSettings = new VoronoiGridSettings(newSettings.GridSettings) {
                    Randomness = 1 - Uniformity
                };
            }
        }

        using (var vs = new VerticalScope("Box"))
        using (var gcs = new GUIChangedScope())
        {
            var scale = newSettings.Scale;
            Label("Size:");
            var newX = SettingSlider("X", (float) scale.X, 0.1f, 2);
            var newY = SettingSlider("Y", (float) scale.Y, 0.1f, 2);
            var newZ = SettingSlider("Z", (float) scale.Z, 0.1f, 2);

            if (GUI.changed)
            {
                var m = UnityEngine.Matrix4x4.TRS(DEFAULT_POS,
                                                  Quaternion.identity,
                                                  new Vector3(newX, newY, newZ));
                newSettings.Transform = Convert.ToRMatrix(m);
            }
        }

        if (Button("Make Rock"))
        {
            var newGridSettings = new VoronoiGridSettings(newSettings.GridSettings);
            newGridSettings.Randomness += 1e-6f;

            newSettings.GridSettings = newGridSettings;
        }

        var settingsChanged = GUI.changed;
        if (settingsChanged)
            generator.Settings = newSettings;

        if (Button("Save this one"))
        {
            var path = FileSaver.SaveMesh(generator.LatestMesh, "rock");
            OnFileExported(path);
        }

        return settingsChanged;
    }

    static float SettingSlider(string varName,
                               float  value, float min, float max,
                               string format    = "N",
                               bool   autoLabel = true)
    {
        BeginHorizontal();
        if (autoLabel)
        {
            var label = Regex.Replace(varName, @"([A-Z]+?)", " $1");
            Label(label, MaxWidth(MAX_LABEL_WIDTH));
        }

        float newVal = HorizontalSlider(value, min, max, ExpandWidth(true));
        Label(value.ToString(format), Width(5 * CHARACTER_WIDTH));
        EndHorizontal();
        return newVal;
    }

    static void OnFileExported(string path)
    {
        fileExported?.Invoke(path);
    }
}
}
