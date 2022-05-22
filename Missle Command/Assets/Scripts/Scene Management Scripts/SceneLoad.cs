using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script allows the loading of scene via build index and scene name.
/// </summary>
public class SceneLoad : MonoBehaviour {

     /// <summary>
     /// Methods to load the scene.
     /// </summary>
     public enum LoadMethod {
          buildIndex, sceneName
     }

     /// <summary>
     /// The method to load the specified scene.
     /// </summary>
     public LoadMethod loadMethod = LoadMethod.buildIndex;

     /// <summary>
     /// The build index of the scene, as specified in Build Settings.
     /// </summary>
     public int sceneIndex = 0;

     /// <summary>
     /// The name of the scene to load.
     /// </summary>
     public string sceneName;

     public void LoadScene() {

          switch (loadMethod) {
               case LoadMethod.buildIndex:
                    SceneManager.LoadScene(sceneIndex);
                    break;

               case LoadMethod.sceneName:
                    SceneManager.LoadScene(sceneName);
                    break;
          }

     }

}
