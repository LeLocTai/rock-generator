using RockGen;
using UnityEngine;

namespace RockGenUnity
{
public class RockGeneratorWindow : UnityEditor.EditorWindow
{
    private readonly RockGenerationSettings defaultSettings = new RockGenerationSettings {
        GridSettings = new VoronoiGridSettings {
            Size       = 6,
            Randomness = .75f
        },
        StockDensity        = 8,
        TargetTriangleCount = 1000,
        Distortion          = 1,
        Transform = Convert.ToRMatrix(UnityEngine.Matrix4x4.TRS(new Vector3(2.5f, 2.5f, 2.5f),
                                                                Quaternion.identity,
                                                                Vector3.one * 1.35f))
    };

    [UnityEditor.MenuItem("Tools/Rock Generator")]
    private static void ShowWindow()
    {
        var window = GetWindow<RockGeneratorWindow>();
        window.titleContent = new GUIContent("Rock Generator");
        window.Show();
    }

    RockGenerator generator;
    Mesh          mesh;

    void Initialize()
    {
        generator = new RockGenerator();

        CreateMesh(defaultSettings);
    }

    void CreateMesh(RockGenerationSettings settings)
    {
        generator.Settings = settings;

        mesh = Convert.ToUnityMesh(generator.MakeRock());
    }

    private void OnGUI()
    {
        if (generator == null) Initialize();

        var newSetings = new RockGenerationSettings(generator.Settings);
        if (RockGeneratorGUI.OnGUI(ref newSetings))
            CreateMesh(newSetings);

        if (mesh)
            GUILayout.Label(mesh.vertexCount.ToString());
    }
}
}
