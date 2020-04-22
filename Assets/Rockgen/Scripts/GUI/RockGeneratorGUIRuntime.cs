using UnityEngine;

namespace RockGen.Unity
{
public class RockGeneratorGUIRuntime : MonoBehaviour
{
    private readonly RockGenerationSettings defaultSettings = new RockGenerationSettings {
        GridSettings = new VoronoiGridSettings {
            Size       = 10,
            Randomness = .75f
        },
        StockDensity        = 16,
        TargetTriangleCount = 2000,
        Distortion          = .5f,
        PatternSize         = 1.35f,
        Transform = Convert.ToRMatrix(UnityEngine.Matrix4x4.TRS(new Vector3(2.5f, 2.5f, 2.5f),
                                                                Quaternion.identity,
                                                                Vector3.one))
    };


    RockBehavior rock;
    Mesh         mesh;
    GUIStyle     bgStyle;

    void Start()
    {
        rock = FindObjectOfType<RockBehavior>();

        rock.generator.Settings = defaultSettings;
        rock.UpdateMesh();


        var bgTex = new Texture2D(1, 1);
        bgTex.SetPixel(0, 0, new Color(.15f, .15f, .15f, .95f));
        bgTex.Apply();
        bgStyle = new GUIStyle {
            normal = {
                background = bgTex
            },
            padding       = new RectOffset(8, 8, 8, 8),
            fixedWidth    = 360,
            stretchHeight = false,
        };
    }

    void OnGUI()
    {
        if (rock == null || rock.generator == null) return;

        GUILayout.BeginVertical(bgStyle);

        if (RockGeneratorGUI.OnGUI(rock.generator))
        {
            rock.UpdateMesh();
        }

        GUILayout.EndVertical();
    }
}
}
