using System;
using System.Collections;
using System.Collections.Generic;
using RockGen;
using UnityEngine;

namespace RockGenUnity
{
public class RockGeneratorGUIRuntime : MonoBehaviour
{
    RockBehavior rock;
    Mesh         mesh;

    void Start()
    {
        rock = FindObjectOfType<RockBehavior>();
    }

    void OnGUI()
    {
        if (rock == null || rock.generator == null) return;

        GUILayout.BeginArea(new Rect(0, 0, 400, 800));

        var newSetings = new RockGenerationSettings(rock.generator.Settings);
        if (RockGeneratorGUI.OnGUI(ref newSetings))
        {
            rock.generator.Settings = newSetings;
            rock.UpdateMesh();
        }

        GUILayout.EndArea();
    }
}
}
