using System;
using System.Linq;
using System.Text.RegularExpressions;
using RockGen;
using UnityEngine;
using static UnityEngine.GUILayout;
using Matrix4x4 = RockGen.Matrix4x4;

namespace RockGenUnity
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

    public static bool OnGUI(ref RockGenerationSettings settings)
    {
        settings.StockDensity = SettingSlider(nameof(settings.StockDensity), settings.StockDensity, 2, 16);
        settings.TargetTriangleCount = Mathf.RoundToInt(SettingSlider(nameof(settings.TargetTriangleCount),
                                                                      settings.TargetTriangleCount,
                                                                      100, 2000));
        settings.Distortion = SettingSlider(nameof(settings.Distortion), settings.Distortion, -2, 2);

        return GUI.changed;
    }

    static float SettingSlider(string varName, float value, float min, float max)
    {
        BeginHorizontal();
        Label(Regex.Replace(varName, @"(\B[A-Z]+?(?=[A-Z][^A-Z])|\B[A-Z]+?(?=[^A-Z]))", " $1"),
              MaxWidth(MAX_LABEL_WIDTH));
        float newVal = HorizontalSlider(value, min, max, ExpandWidth(true));
        Label(value.ToString("N"), Width(5 * CHARACTER_WIDTH));
        EndHorizontal();
        return newVal;
    }
}
}
