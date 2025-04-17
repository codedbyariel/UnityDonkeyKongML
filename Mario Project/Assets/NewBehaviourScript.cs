using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class SceneManagementController : MonoBehaviour
{
    private static SceneManagementController instance;
    public string targetSceneName;

    public static SceneManagementController Instance
    {
        get
        {
            if (instance == null)
            {
                Debug.LogError("SceneManagementController is not initialized.");
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ReloadScene(string sceneName)
    {
        StartCoroutine(ReloadSceneCoroutine(sceneName));
    }

    private IEnumerator ReloadSceneCoroutine(string sceneName)
    {
        // Unload the specified scene asynchronously
        AsyncOperation unloadOperation = SceneManager.UnloadSceneAsync(sceneName);

        // Wait until the scene is fully unloaded
        while (!unloadOperation.isDone)
        {
            yield return null;
        }

        // Load the scene additively
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Wait until the scene is fully loaded
        while (!loadOperation.isDone)
        {
            yield return null;
        }

        // Optionally, set the reloaded scene as the active scene
        Scene reloadedScene = SceneManager.GetSceneByName(sceneName);
        if (reloadedScene.IsValid())
        {
            SceneManager.SetActiveScene(reloadedScene);
        }
    }
    void Update()
    {
        Scene targetScene = SceneManager.GetSceneByName(targetSceneName);

        if (targetScene.IsValid() && targetScene.isLoaded)
        {
            // Find all GameObjects with the "Barrel" tag in the target scene
            GameObject[] barrels = GameObject.FindGameObjectsWithTag("Barrel");

            // Iterate through the found barrels and destroy them
            foreach (GameObject barrel in barrels)
            {
                // Ensure the barrel is part of the target scene
                if (barrel.scene == targetScene)
                {
                    Destroy(barrel);
                }
            }
        }
    }
}
