using UnityEngine;

/// <summary>
/// This class helps to control the current game status, such as when the game is over.
/// </summary>
public class GameStatus : MonoBehaviour {

     /// <summary>
     /// The GameObject that keeps track of all buildings.
     /// </summary>
     public GameObject buildingsTracker;

     public bool isGameOver = false;

     /// <summary>
     /// A variable for keeping track of differences between the original variable and the current one.
     /// We use this so that we avoid continuous calling of methods.
     /// </summary>
     private bool isGameOver_DiffTracker = false;

     /// <summary>
     /// The GameObject responsible for the UI to show when the game is over.
     /// </summary>
     public GameObject gameOverUI;

     /// <summary>
     /// The GameObject responsible for the enemy missile spawner.
     /// We require this so that we can stop missiles from spawning when the game is over.
     /// </summary>
     public GameObject enemyMissileSpawner;

     /// <summary>
     /// The GameObject that keeps track of all player missile launchers.
     /// We require this so as to prevent players from firing missiles when the game is over.
     /// </summary>
     public GameObject missileLauncherManager;

     // Start is called before the first frame update
     private void Start() {
          isGameOver_DiffTracker = isGameOver;
     }

     // Update is called once per frame
     private void Update() {
          isGameOver = IsGameOver();

          if (IsGameOverChanged()) {
               //back to standard time
               Time.timeScale = 1.0f;

               DisplayGameOverUI();
               enemyMissileSpawner.GetComponent<EnemyMissileSpawner>().enableSpawning = false;
               missileLauncherManager.GetComponent<MissileLauncherManager>().DeactivateAllLaunchers();
          }
     }

     private bool IsGameOver() {
          return buildingsTracker.GetComponent<BuildingsTracker>().HasNoBuildingsLeft();
     }

     private bool IsGameOverChanged() {
          if (isGameOver_DiffTracker != isGameOver) {
               isGameOver_DiffTracker = isGameOver;
               return true;
          } else {
               return false;
          }
     }

     private void DisplayGameOverUI() {
          gameOverUI.SetActive(isGameOver);
     }
}
