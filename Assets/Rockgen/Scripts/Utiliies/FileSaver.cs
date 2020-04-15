using System.IO;
using System.Runtime.InteropServices;
using RockGen.Unity;
using UnityEngine;

namespace Rockgen.Unity
{
public static class FileSaver
{
    [DllImport("__Internal")]
    public static extern void TriggerDownloadTextFile(string data, string fileName);

    public static string SaveMesh(MeshDecimator.Mesh mesh, string fileName)
    {
        var fullFileName = Path.ChangeExtension(fileName, "obj");
        var data = WaveFrontObjExporter.ToObjString(mesh);

#if UNITY_WEBGL && !UNITY_EDITOR
        TriggerDownloadTextFile(data, fullFileName);
        return fileName;
#else
        var path = Path.Combine(Application.dataPath, fullFileName);
        File.WriteAllText(path, data);

        return path;
#endif
    }
}
}
