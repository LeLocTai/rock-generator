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
        bgTex.SetPixel(0, 0, Color.black);
        bgTex.Apply();
        bgStyle = new GUIStyle {
            normal        = {background = bgTex},
            fixedWidth    = 400,
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
