using UnityEngine;

namespace RockGen.Unity
{
public class DragOrbit : MonoBehaviour
{
    public Transform target;
    public float     xSpeed      = 20.0f;
    public float     ySpeed      = 20.0f;
    public float     scrollSpeed = 5f;
    public float     yMinLimit   = -90f;
    public float     yMaxLimit   = 90f;
    public float     distanceMin = 10f;
    public float     distanceMax = 10f;

    float rotationYAxis = 0.0f;
    float rotationXAxis = 0.0f;
    float distance      = 2.0f;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationYAxis = angles.y;
        rotationXAxis = angles.x;

        distance = Vector3.Distance(transform.position, target.transform.position);
    }

    void LateUpdate()
    {
        if (GUIUtility.hotControl != 0) return;

        if (Input.GetMouseButton(0))
        {
            rotationYAxis += xSpeed * distance * Input.GetAxis("Mouse X");
            rotationXAxis -= ySpeed * distance * Input.GetAxis("Mouse Y");
            rotationXAxis =  ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
        }

        Quaternion rotation = Quaternion.Euler(rotationXAxis, rotationYAxis, 0);

        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * scrollSpeed, distanceMin, distanceMax);

        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position    = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
}
