using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static UnityEngine.GUILayout;

namespace RockGen.Unity
{
public class RockGeneratorGUI
{
    static readonly int MAX_LABEL_WIDTH;
    static readonly int CHARACTER_WIDTH;

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
            ));
        newSettings.Distortion = SettingSlider(nameof(newSettings.Distortion),
                                               newSettings.Distortion, -2, 2);

        var settingsChanged = GUI.changed;
        if (settingsChanged)
            generator.Settings = newSettings;

        if (Button("Save"))
        {
            var path = Path.Combine(Application.dataPath, "rock.obj");
            WaveFrontObjExporter.Export(generator.LatestMesh, path);
            OnFileExported(path);
        }

        return settingsChanged;
    }

    static float SettingSlider(string varName,
                               float  value, float min, float max,
                               string format = "N")
    {
        BeginHorizontal();
        Label(Regex.Replace(varName, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1"),
              MaxWidth(MAX_LABEL_WIDTH));
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
