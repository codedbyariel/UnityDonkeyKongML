using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class EndDestroyBarrel : MonoBehaviour
{
    [SerializeField] private Transform Barrel_destroy_point;
    [SerializeField] private LayerMask barrelLayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Physics2D.OverlapCircle(Barrel_destroy_point.position, 0.4f, barrelLayer))
        {
            Debug.Log(Physics2D.OverlapCircle(Barrel_destroy_point.position, 0.4f, barrelLayer).gameObject.transform);
            Destroy(Physics2D.OverlapCircle(Barrel_destroy_point.position, 0.4f, barrelLayer).gameObject);
        }
    }
}
