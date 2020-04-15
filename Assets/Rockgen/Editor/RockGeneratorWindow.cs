using RockGen;
using UnityEditor;
using UnityEngine;

namespace RockGen.Unity
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

    [MenuItem("Tools/Rock Generator")]
    private static void ShowWindow()
    {
        var window = GetWindow<RockGeneratorWindow>();
        window.titleContent = new GUIContent("Rock Generator");
        window.Show();
    }

    RockGenerator generator;
    Mesh          mesh;

    GameObject previewObj;
    MeshFilter previewMeshFilter;
    Editor     previewEditor;
    GUIStyle   previewBackground;

    void Initialize()
    {
        generator         = new RockGenerator();
        previewObj        = new GameObject("Rock Preview") {hideFlags = HideFlags.HideAndDontSave};
        previewMeshFilter = previewObj.AddComponent<MeshFilter>();
        previewMeshFilter.mesh = mesh;

        var renderer = previewObj.AddComponent<MeshRenderer>();
        renderer.material = Resources.Load<Material>("Rock");

        previewBackground = new GUIStyle {normal = {background = Texture2D.blackTexture}};

        CreateMesh(defaultSettings);
    }

    void CreateMesh(RockGenerationSettings settings)
    {
        generator.Settings = settings;

        mesh = Convert.ToUnityMesh(generator.MakeRock());

        previewMeshFilter.mesh = mesh;

        if (previewEditor)
            DestroyImmediate(previewEditor);

        previewEditor = Editor.CreateEditor(previewObj);
    }

    private void OnGUI()
    {
        if (generator == null) Initialize();

        var newSetings = new RockGenerationSettings(generator.Settings);
        if (RockGeneratorGUI.OnGUI(ref newSetings))
            CreateMesh(newSetings);


        previewEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200, 200), previewBackground);

        if (GUILayout.Button("Save")) { }
    }
}
}
