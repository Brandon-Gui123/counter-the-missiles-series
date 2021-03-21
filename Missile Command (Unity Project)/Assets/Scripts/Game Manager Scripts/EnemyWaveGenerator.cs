using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Contains methods for feeding information to the enemy missile spawner and for generating waves.
/// </summary>
public class EnemyWaveGenerator : MonoBehaviour {

     /// <summary>
     /// Allows for generation of enemy waves.
     /// </summary>
     public bool generateEnemyWaves = true;

     /// <summary>
     /// The GameObject holding the current game status.
     /// Is used to determine if waves should be generated as we won't need to generate more waves when the game is over.
     /// </summary>
     [SerializeField]
     private GameObject gameStatus;

     [Header("Enemy Spawner")]
     #region Enemy Spawner - GameObjects pertaining to the enemy missile spawner

     /// <summary>
     /// The GameObject depicted as the enemy missile spawner.
     /// </summary>
     [SerializeField]
     private GameObject enemyMissileSpawner;

     /// <summary>
     /// The enemy missile spawner script in the missile spawner.
     /// </summary>
     [SerializeField]
     private EnemyMissileSpawner enemyMissileSpawnerScript;

     #endregion

     [Header("Enemy Missile Types")]
     #region Enemy Missile Types - the types to spawn in the missile spawner

     /// <summary>
     /// The regular enemy missile. This missile explodes upon contact with your launcher, buildings, the ground or an explosion.
     /// </summary>
     public GameObject regularEnemyMissile;

     /// <summary>
     /// The cluster enemy missile.
     /// This special missile can split into multiple regular enemy missiles after some time while
     /// still descending in the air.
     /// </summary>
     public GameObject clusterEnemyMissile;

     #endregion

     [Header("Spawn Quantity")]
     #region Spawn Quantity - the maximum and minimum number of missiles a spawner can spawn.

     public int minSpawnQuantity = 20;
     public int maxSpawnQuantity = 30;

     /// <summary>
     /// The amount that will add on to the number of missiles the spawner can spawn.
     /// Higher wave counts will add more missiles to the spawner's stash.
     /// </summary>
     [Tooltip("Additional number of missiles to spawn during coming waves")]
     public int spawnQuantityIncrement = 2;

     #endregion

     [Header("Enemy Cluster Missile Spawn Chance")]
     #region Enemy Cluster Missile Spawn Chance - chance for that special missile to spawn

     [Range(0f, 1f)] public float minClusterMissileSpawnChance = 0.0f;
     [Range(0f, 1f)] public float maxClusterMissileSpawnChance = 0.7f;

     /// <summary>
     /// Additional chance for the cluster missile to spawn.
     /// Higher wave counts add more chance.
     /// </summary>
     [Tooltip("Additional chance for cluster missiles to spawn in coming waves.")]
     public float clusterMissileChanceIncrement = 0.15f;

     #endregion

     [Header("Enemy Missile Speed Multiplier")]
     #region Enemy Missile Speed Multiplier - how fast can it go?

     public float minSpeedMultiplier = 1.0f;
     public float maxSpeedMultiplier = 2.0f;

     /// <summary>
     /// Additional factor to add to the multiplier after each wave.
     /// </summary>
     [Tooltip("Additional factor to add to the multiplier after each wave")]
     public float speedMultiplierIncrement = 0.15f;

     #endregion

     [Header("Enemy Missile Spawning Times")]
     #region Enemy Missile Spawning Times - how fast will it spawn?

     public float minTimePerSpawn = 1.2f;
     public float maxTimePerSpawn = 1.75f;

     /// <summary>
     /// Time decrement to spawn times after each wave.
     /// </summary>
     [Tooltip("Time decrement to spawn times after each wave")]
     public float timePerSpawnDecrement = 0.1f;

     #endregion

     [Header("Wave Details")]
     #region Current Wave Details - info on the current enemy wave

     public int currentWave = 1;
     private float currentSpeedMultiplier;
     private float currentTimePerSpawn;
     private float currentClusterMissileSpawnChance = 0.0f;
     private int currentSpawnQuantity;

     #endregion

     [Header("Miscelleanous")]
     #region Miscelleanous - not exactly related to wave generation, but is helpful anyways

     /// <summary>
     /// The GameObject containing the UI element that shows the wave number.
     /// We require this so that we can update the UI element to reflect the correct wave number.
     /// </summary>
     [Tooltip("The GameObject containing the UI element that shows the wave number")]
     public GameObject currentWaveText;

     /// <summary>
     /// The parent GameObject of all the player's missile launchers.
     /// We require this so that we can resupply and activate all of the missile launchers after
     /// the wave is loaded.
     /// </summary>
     [Tooltip("The parent GameObject of all player's missile launchers")]
     public GameObject missileLauncherManager;

     #endregion

     // Start is called just before any of the Update methods is called the first time
     private void Start() {

          //perform validity checks to ensure we got the right stuff
          CheckEnemyMissileSpawner();
          CheckEnemyMissileSpawnerScript();
          CheckCurrentWaveText();
          CheckMissileLauncherManager();

          //start the first wave
          enemyMissileSpawnerScript.SetEnemyWave(GenerateEnemyWave(currentWave));

          //update the current wave to show the correct wave at the start
          UpdateCurrentWaveText();
     }

     // Update is called every frame, if the MonoBehaviour is enabled
     private void Update() {
          if (IsReadyForNextWave()) {
               currentWave++;
               enemyMissileSpawnerScript.SetEnemyWave(GenerateEnemyWave(currentWave));
               UpdateCurrentWaveText();

               //resupply all missile launchers
               missileLauncherManager.GetComponent<MissileLauncherManager>().ResupplyAllLaunchers();
               missileLauncherManager.GetComponent<MissileLauncherManager>().ActivateAllLaunchers();
          }
     }

     #region Validity Checks - to ensure that we have the correct stuff prior to running

     private void CheckEnemyMissileSpawner() {
          if (!enemyMissileSpawner) {
               Debug.LogError("No enemy missile spawner found!");
          }
     }

     private void CheckEnemyMissileSpawnerScript() {
          if (!enemyMissileSpawnerScript) {
               enemyMissileSpawnerScript = enemyMissileSpawner.GetComponent<EnemyMissileSpawner>();

               if (!enemyMissileSpawnerScript) {
                    Debug.LogError("EnemyMissileSpawner script not found in the enemy spawner!");
               }
          }
     }

     private void CheckCurrentWaveText() {
          if (!currentWaveText) {
               Debug.LogError("No GameObject responsible for displaying the current wave is assigned!");
          } else {
               if (!currentWaveText.GetComponent<Text>()) {
                    Debug.LogError("The Text component is not available in the GameObject!");
               }
          }
     }

     private void CheckMissileLauncherManager() {
          if (!missileLauncherManager) {
               Debug.LogError("The GameObject as the missile launcher manager is not attached!");
          } else {
               if (!missileLauncherManager.GetComponent<MissileLauncherManager>()) {
                    Debug.LogError("The MissileLauncherManager script is missing from the GameObject!");
               }
          }
     }

     #endregion

     /// <summary>
     /// Generates enemy missiles via the wave details and consolidates them in a list.
     /// </summary>
     /// <returns>A list of GameObjects that contains enemy missiles generated.</returns>
     private List<GameObject> GenerateEnemyMissiles() {

          //to hold the missiles that will spawn
          List<GameObject> missilesToSpawn = new List<GameObject>(currentSpawnQuantity);

          //iterate through our capacity of the list
          for (int i = 0; i < missilesToSpawn.Capacity; i++) {

               //generate a random number
               float randomFloat = Random.Range(0f, 1f);

               //compare number against the chance of enemy cluster missile spawn
               if (randomFloat <= currentClusterMissileSpawnChance) {
                    missilesToSpawn.Add(clusterEnemyMissile);
               } else {
                    missilesToSpawn.Add(regularEnemyMissile);
               }
          }

          return missilesToSpawn;
     }

     /// <summary>
     /// Generates an enemy wave given the current wave information.
     /// </summary>
     /// <param name="waveNumber">The number for the wave to generate.</param>
     /// <returns>An EnemyWave object containing details of the current wave, as well as the missiles that spawn.</returns>
     public EnemyWave GenerateEnemyWave(int waveNumber) {

          //calculate speed multiplier
          currentSpeedMultiplier = Mathf.Clamp(minSpeedMultiplier + (speedMultiplierIncrement * (waveNumber - 1)), minSpeedMultiplier, maxSpeedMultiplier);

          //calculate time per spawn
          currentTimePerSpawn = Mathf.Clamp(maxTimePerSpawn - (timePerSpawnDecrement * (waveNumber - 1)), minSpeedMultiplier, maxSpeedMultiplier);

          //calculate cluster missile spawn chance
          currentClusterMissileSpawnChance = Mathf.Clamp(minClusterMissileSpawnChance + (clusterMissileChanceIncrement * (waveNumber - 1)), minClusterMissileSpawnChance, maxClusterMissileSpawnChance);

          //calculate number of missiles to spawn
          currentSpawnQuantity = Mathf.Clamp(minSpawnQuantity + (spawnQuantityIncrement * (waveNumber - 1)), minSpawnQuantity, maxSpawnQuantity);

          //construct the object
          EnemyWave enemyWave = new EnemyWave {
               missiles = GenerateEnemyMissiles(),
               missileSpeedMultiplier = currentSpeedMultiplier,
               timePerMissileSpawn = currentTimePerSpawn
          };

          return enemyWave;
     }

     /// <summary>
     /// Updates the current wave text to reflect the current wave number.
     /// </summary>
     private void UpdateCurrentWaveText() {
          currentWaveText.GetComponent<Text>().text = "Wave " + currentWave;
     }

     /// <summary>
     /// Is the player ready for the next coming wave?
     /// Players are considered ready when there are no more missiles and explosions.
     /// </summary>
     /// <returns></returns>
     public bool IsReadyForNextWave() {
          if (enemyMissileSpawnerScript.doneSpawning) {

               //find all GameObject elements that are essential to the game
               //these objects must be cleared for the wave to proceed to the next level
               return !gameStatus.GetComponent<GameStatus>().isGameOver && !GameObject.FindGameObjectWithTag("Essentials");

          } else {
               return false;
          }
     }
}
