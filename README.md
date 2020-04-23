# Rock Generator
WIP.

**Demo**: http://leloctai.com/rock-generator/

Everything in the RockGen namespace should work with netstandard 2.0, outside of Unity.

For use in Unity, some type conversion is required.
 
## Example

```c#
// No default are provided at the moment
// Check out the demo page to see what each setings does

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

GetComponent<MeshFilter>().mesh = Convert.ToUnityMesh(generator.MakeRock());
```

Look at the Rock Fountain sample for more details.

## Dependencies

- [MeshDecimator](https://github.com/Whinarn/MeshDecimator)

## TODO

 - Simpler work flow in Unity.
 - 

## Sponsors

Me. My free Unity assets are backed by my paid one. Check them out:

 - [Translucent Image - Fast blur behind UI](https://leloctai.com/asset/translucentimage/)
