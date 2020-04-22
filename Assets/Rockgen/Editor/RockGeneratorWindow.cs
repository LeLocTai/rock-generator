using UnityEditor;
using UnityEngine;

namespace RockGen.Unity
{
public class RockGeneratorWindow : EditorWindow
{
    private readonly RockGenerationSettings defaultSettings = new RockGenerationSettings {
        GridSettings = new VoronoiGridSettings {
            Size       = 10,
            Randomness = .75f
        },
        StockDensity        = 8,
        TargetTriangleCount = 220,
        Distortion          = .5f,
        PatternSize         = 1.35f,
        Transform = Convert.ToRMatrix(UnityEngine.Matrix4x4.TRS(new Vector3(2.5f, 2.5f, 2.5f),
                                                                Quaternion.identity,
                                                                Vector3.one))
    };

    static Texture2D backgroundTexture;

    [MenuItem("Tools/Rock Generator")]
    private static void ShowWindow()
    {
        var window = GetWindow<RockGeneratorWindow>();
        window.titleContent = new GUIContent("Rock Generator");
        window.Show();
    }

    RockGenerator      generator;
    MeshDecimator.Mesh mesh;

    GameObject previewObj;
    MeshFilter previewMeshFilter;
    Editor     previewEditor;
    GUIStyle   previewBackground;

    void Initialize()
    {
        RockGeneratorGUI.fileExported += HandleFileExported;

        generator         = new RockGenerator {Settings = defaultSettings};
        previewObj        = new GameObject {hideFlags   = HideFlags.HideAndDontSave};
        previewMeshFilter = previewObj.AddComponent<MeshFilter>();

        var renderer = previewObj.AddComponent<MeshRenderer>();
        renderer.material = Resources.Load<Material>("Rock");

        previewObj.SetActive(false);

        backgroundTexture = new Texture2D(1, 1);
        backgroundTexture.SetPixel(0, 0, new Color(.5f, .5f, .5f));
        backgroundTexture.Apply();
        previewBackground = new GUIStyle {normal = {background = backgroundTexture}};

        CreateMesh();
    }

    ~RockGeneratorWindow()
    {
        RockGeneratorGUI.fileExported -= HandleFileExported;
        DestroyImmediate(previewObj);
        DestroyImmediate(previewEditor);
    }

    static void HandleFileExported(string path)
    {
        AssetDatabase.Refresh();
    }

    void CreateMesh()
    {
        mesh = generator.MakeRock();

        previewMeshFilter.mesh = Convert.ToUnityMesh(mesh);

        if (previewEditor)
            DestroyImmediate(previewEditor);

        previewEditor = Editor.CreateEditor(previewObj);
    }

    private void OnGUI()
    {
        if (generator == null) Initialize();

        if (RockGeneratorGUI.OnGUI(generator))
            CreateMesh();

        previewObj.SetActive(true);
        previewEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(200, 200), previewBackground);
        previewObj.SetActive(false);
    }
}
}
