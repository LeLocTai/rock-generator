using System;
using UnityEngine;

namespace RockGen.Unity {
public class GUIChangedScope : IDisposable
{
    readonly bool wasChanged;

    public GUIChangedScope()
    {
        wasChanged  = GUI.changed;
        GUI.changed = false;
    }

    public void Dispose()
    {
        GUI.changed |= wasChanged;
    }
}
}