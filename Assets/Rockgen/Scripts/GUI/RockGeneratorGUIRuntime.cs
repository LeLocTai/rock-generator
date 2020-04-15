using UnityEngine;

namespace RockGen.Unity
{
public class RockGeneratorGUIRuntime : MonoBehaviour
{
    RockBehavior rock;
    Mesh         mesh;
    GUIStyle     bgStyle;

    void Start()
    {
        rock = FindObjectOfType<RockBehavior>();

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
