using UnityEngine;

namespace RockGen.Unity.Demo
{
public class RockFountain : MonoBehaviour
{
    public GameObject prefab;

    RockGenerator generator;

    void Start()
    {
        var settings = new RockGenerationSettings {
            GridSettings = new VoronoiGridSettings {
                Size       = 5,
                Randomness = .75f
            },
            StockDensity        = 8,
            TargetTriangleCount = 220,
            Distortion          = .5f,
            PatternSize         = 1.35f,
            Transform = Convert.FromUnityMatrix(UnityEngine.Matrix4x4.TRS(new Vector3(2, 2, 2),
                                                                          Quaternion.identity,
                                                                          Vector3.one))
        };

        generator = new RockGenerator {Settings = settings};
    }

    Mesh CreateRandomRock()
    {
        // Copy existing settings
        var settings = new RockGenerationSettings(generator.Settings);

        // Modify rock shape
        UnityEngine.Matrix4x4 rockTransform = UnityEngine.Matrix4x4.TRS(
            new Vector3(2, 2, 2),
            Random.rotation,
            new Vector3(Random.Range(.5f, 1.5f),
                        Random.Range(.5f, 1.5f),
                        Random.Range(.5f, 1.5f))
        );
        settings.Transform = Convert.FromUnityMatrix(rockTransform);

        // Apply settings. Generator will optimize generation base on which settings was changed.
        generator.Settings = settings;

        // Generator do not use Unity's types, so it can be used outside of Unity
        // Notable different are: Matrix4x4, Mesh
        return Convert.ToUnityMesh(generator.MakeRock());
    }

    void Update()
    {
        if (!Input.GetMouseButton(0))
            return;

        var mesh = CreateRandomRock();

        var rock = Instantiate(prefab, transform.position, Quaternion.identity);
        rock.GetComponent<MeshFilter>().sharedMesh   = mesh;
        rock.GetComponent<MeshCollider>().sharedMesh = mesh;
        Destroy(rock, 10);
    }
}
}
