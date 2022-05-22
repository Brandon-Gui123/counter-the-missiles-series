using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour {
    
    public enum LoadSceneMethod {
        BuildIndex, SceneName
    }

    public LoadSceneMethod loadSceneMethod;

    public int buildIndex;

    public string sceneName;

    public void LoadSpecifiedScene() {
        Time.timeScale = 1.0f;
        Load(loadSceneMethod, buildIndex, sceneName);
    }

    public void Load(LoadSceneMethod loadSceneMethod, int buildIndex, string sceneName) {
        if (loadSceneMethod == LoadSceneMethod.BuildIndex) {
            SceneManager.LoadScene(buildIndex);
        } else {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void RestartCurrentScene() {
        Time.timeScale = 1.0f;
        Load(loadSceneMethod, SceneManager.GetActiveScene().buildIndex, SceneManager.GetActiveScene().name);
    }

}
