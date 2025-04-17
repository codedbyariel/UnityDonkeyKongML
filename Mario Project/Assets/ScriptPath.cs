using UnityEngine;

public class ScriptPath : MonoBehaviour
{
    [SerializeField] private Spline spline;
    [SerializeField] private float speed = 2.0f; // Speed in world units per second
    [SerializeField] private Transform barrel;
    private int RandonPathNum;
    [SerializeField] private GameObject RestarterObject;

    private float t;
    private float pathLength;

    void Start()
    {
        RandonPathNum = Random.Range(0, 7);
        //Debug.Log("RandonPathNum: " + RandonPathNum);

        pathLength = spline.GetTotalPathLength(RandonPathNum);
    }

    void Update()
    {
        if (pathLength <= 0) return; // Prevent division by zero

        float deltaT = (speed / pathLength) * Time.deltaTime; // Convert speed to t movement
        t += deltaT;
        t %= 1f; // Ensure looping

        barrel.position = spline.GetPoint(t, RandonPathNum);
        if ((RestarterObject.GetComponent<SpriteRenderer>().material.color == Color.green))
        {
            Destroy(barrel.gameObject);
        }
    }
}
