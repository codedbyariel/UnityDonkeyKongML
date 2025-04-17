using UnityEngine;

public class LinearInterpolation : MonoBehaviour
{
    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;
    [SerializeField] private Transform pointC;
    [SerializeField] private Transform interpolateTransform;

    private float t;

    void Update()
    {
        // Increase the interpolation value
        t += Time.deltaTime;
        t %= 1f; // Loops back to 0

        // Interpolate between two points
        interpolateTransform.position = Vector3.Lerp(pointA.position, pointB.position, t);
    }
}
