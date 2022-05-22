using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class provides the ability to restart (reset) the current scene.
/// </summary>
public class RestartScene : MonoBehaviour {

     /// <summary>
     /// The time scale to use when restarting this scene with this script.
     /// </summary>
     public float resetToTimeScale = 1.0f;

     /// <summary>
     /// Restarts the current scene and loads its values that were assigned to it before it starts.
     /// </summary>
     public void RestartCurrentScene() {

          //we want things to go normal and not sped-up (fixes issue where restarting does not reset time sc
          Time.timeScale = resetToTimeScale;

          //reload the current scene by loading the current scene.
          SceneManager.LoadScene(SceneManager.GetActiveScene().name);

     }

}
