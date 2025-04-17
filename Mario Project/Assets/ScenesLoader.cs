using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    // List of scene names to load additively
    public string[] scenesToLoad;

    void Start()
    {
        foreach (string sceneName in scenesToLoad)
        {
            // Load each scene additively
            SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }
    }
}
