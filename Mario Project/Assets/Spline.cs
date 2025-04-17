using System.Collections.Generic;
using UnityEngine;

public class Spline : MonoBehaviour
{
    private List<Transform> path;
    [SerializeField] private List<Transform> path1;
    [SerializeField] private List<Transform> path2;
    [SerializeField] private List<Transform> path3;
    [SerializeField] private List<Transform> path4;
    [SerializeField] private List<Transform> path5;
    [SerializeField] private List<Transform> path6;
    [SerializeField] private List<Transform> path7;

    private void ChoosePath(int num)
    {
        if (num == 0) path = path1;
        if (num == 1) path = path2;
        if (num == 2) path = path3;
        if (num == 3) path = path4;
        if (num == 4) path = path5;
        if (num == 5) path = path6;
        if (num == 6) path = path7;
    }

    public Vector3 GetPoint(float t, int num)
    {
        ChoosePath(num);

        int segmentCount = path.Count - 1;
        float totalLength = GetTotalPathLength(num);

        float targetDistance = t * totalLength;
        float accumulatedDistance = 0f;

        Vector3 currentPos = path[0].position; // Start from the first point

        for (int i = 0; i < segmentCount; i++)
        {
            float segmentLength = Vector3.Distance(path[i].position, path[i + 1].position);

            if (accumulatedDistance + segmentLength >= targetDistance)
            {
                float remainingDistance = targetDistance - accumulatedDistance; // Distance to move within this segment
                return Vector3.MoveTowards(path[i].position, path[i + 1].position, remainingDistance);
            }

            accumulatedDistance += segmentLength;
        }

        return path[segmentCount].position; // Return last point if t == 1
    }

    public float GetTotalPathLength(int num)
    {
        ChoosePath(num);

        float totalLength = 0f;
        for (int i = 0; i < path.Count - 1; i++)
        {
            totalLength += Vector3.Distance(path[i].position, path[i + 1].position);
        }
        return totalLength;
    }
}