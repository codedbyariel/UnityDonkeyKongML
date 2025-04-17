using UnityEngine;

public class BarrelSpawnerScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform Barrel;
    [SerializeField] private Transform Spawner;
    [SerializeField] private bool toSpawn=true;
    [SerializeField] private float timerStart;
    private float timer;
    [SerializeField] private GameObject RestarterObject;
    void Start()
    {
        
        Barrel.gameObject.SetActive(false);
        if (toSpawn)
        {
            Instantiate(Barrel, Spawner.position, Quaternion.identity).gameObject.SetActive(true);
        }
        //toSpawn = true;
        timer = timerStart;
    }

    // Update is called once per frame
    void Update()
    {
        if((RestarterObject.GetComponent<SpriteRenderer>().material.color == Color.green)&& toSpawn)
        {
            timer = timerStart;
            Instantiate(Barrel, Spawner.position, Quaternion.identity).gameObject.SetActive(toSpawn);
        }
        
        if ((toSpawn)&&(timerEnded()))
        {
            Instantiate(Barrel, Spawner.position, Quaternion.identity).gameObject.SetActive(toSpawn);
        }
        timer -= Time.deltaTime;

    }

    private bool timerEnded()
    {
        if (timer <= 0)
        {
            timer = timerStart;
            return true;
        }
        return false;
    }
}
