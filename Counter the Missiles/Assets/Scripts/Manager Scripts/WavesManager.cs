using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WavesManager : MonoBehaviour {

    public EnemyMissileSpawner enemyMissileSpawner;

    /// <summary>
    /// The parent that holds all currently active missiles.
    /// </summary>
    [Header("Parents")]
    public Transform missilesActiveParent;

    /// <summary>
    /// The parent that holds all currently active explosions.
    /// </summary>
    public Transform explosionsActiveParent;

    /// <summary>
    /// The parent that holds and manages all missile launchers.
    /// </summary>
    public Transform missileLauncherManager;

    [Header("Wave Information")]
    public int currentWave = 1;

    #region Spawning Delay

    /// <summary>
    /// The lowest possible minimum spawn delay that an enemy missile spawner can have.
    /// The minimum spawn delay of an enemy missile spawner is the shortest delay between missile
    /// spawns.
    /// </summary>
    [Header("Spawning Delay")]
    [Tooltip("Lowest possible for the minimum spawn delay of the missile spawner.")]
    public float shortestMinSpawnDelay = 0.4f;

    /// <summary>
    /// The highest possible minimum spawn delay that an enemy missile spawner can have.
    /// The minimum spawn delay of an enemy missile spawner is the shortest delay between missile
    /// spawns.
    /// </summary>
    [Tooltip("Highest possible for the minimum spawn delay of the missile spawner.")]
    public float longestMinSpawnDelay = 1.0f;

    /// <summary>
    /// The decrement made to the minimum spawn delay after every wave.
    /// </summary>
    [Tooltip("The decrement made to the minimum spawn delay after every wave.")]
    public float minSpawnDelayDecrement = 0.1f;

    /// <summary>
    /// The shortest possible maximum spawn delay that an enemy missile spawner can have.
    /// The maximum spawn delay is the longest possible delay between missile spawns.
    /// </summary>
    [Space, Tooltip("Lowest possible for the maximum spawn delay of the missile spawner.")]
    public float shortestMaxSpawnDelay = 1.2f;

    /// <summary>
    /// The longest possible maximum spawn delay that an enemy missile spawner can have.
    /// The maximum spawn delay is the longest possible duration between missile spawns.
    /// </summary>
    [Tooltip("Highest possible for the maximum spawn delay of the missile spawner.")]
    public float longestMaxSpawnDelay = 2.0f;

    /// <summary>
    /// The decrement made to the maximum spawn delay after every wave.
    /// </summary>
    [Tooltip("The decrement made to the maximum spawn delay after every wave.")]
    public float maxSpawnDelayDecrement = 0.1f;

    #endregion

    #region Spawning Quantity

    /// <summary>
    /// The lowest number of missiles that a missile spawner can spawn.
    /// </summary>
    [Header("Spawning Quantity")]
    [Tooltip("The lowest number of missiles that a missile spawner can spawn.")]
    public int minSpawnQuantity = 20;

    /// <summary>
    /// The highest number of missiles that a missile spawner can spawn.
    /// </summary>
    [Tooltip("The highest number of missiles that a missile spawner can spawn.")]
    public int maxSpawnQuantity = 35;

    /// <summary>
    /// The extra number of missiles to spawn after each wave.
    /// This is a <see langword="float"/> so as to allow for gradual changes.
    /// </summary>
    [Tooltip("The extra number of missiles to spawn after each wave.")]
    public float spawnQuantityIncrement = 2;

    #endregion

    #region Spawnable Type Chance

    /**
     * We don't add the chance for the regular missile because we can
     * simply determine the chance for that from the cluster missile's
     * spawn chances by doing some math.
     * e.g. 1 - 0.5 = 0.5
     */

    /// <summary>
    /// The minimum chance that a cluster missile can spawn.
    /// </summary>
    [Header("Cluster Missile Spawning")]
    [Tooltip("The minimum chance that a cluster missile can spawn.")]
    [Range(0, 1)]
    public float minClusterSpawn = 0.0f;

    /// <summary>
    /// The maximum chance that a cluster missile can spawn.
    /// </summary>
    [Tooltip("The maximum chance that a cluster missile can spawn.")]
    [Range(0, 1)]
    public float maxClusterSpawn = 0.8f;

    /// <summary>
    /// The increment made to the chance that a cluster missile can spawn,
    /// after every wave.
    /// </summary>
    [Tooltip("The increment made to the chance of cluster missiles spawning.")]
    [Range(0, 1)]
    public float clusterSpawnIncrement = 0.15f;

    #endregion

    #region Missile Speed

    /// <summary>
    /// The slowest an enemy missile can get.
    /// </summary>
    [Header("Missile Speed")]
    [Tooltip("The slowest an enemy missile can get.")]
    public float minMissileSpeed = 1.1f;

    /// <summary>
    /// The fastest an enemy missile can get.
    /// </summary>
    [Tooltip("The fastest an enemy missile can get.")]
    public float maxMissileSpeed = 2.2f;

    /// <summary>
    /// How much faster missiles get after every wave.
    /// </summary>
    [Tooltip("How much faster missiles get after every wave.")]
    public float missileSpeedIncrement = 0.15f;

    #endregion

    #region Next Wave Sound

    [Header("Audio Source")]
    [SerializeField]
    private AudioSource audioSource;

    #endregion

    #region GUI

    [Header("GUI")]
    public TMPro.TextMeshProUGUI waveCounterGUI;
    public TMPro.TextMeshProUGUI wavesSurvivedGUI;

    #endregion

    // Start is called before the first frame update
    private void Start() {

        //pass wave details to spawner when we start
        PassWaveDetailsToSpawner(enemyMissileSpawner);

        //play next wave audio
        audioSource.Play();

        waveCounterGUI.text = currentWave.ToString();
        wavesSurvivedGUI.text = (currentWave - 1).ToString();

    }

    // Update is called once per frame
    private void Update() {
        if (CanBeginNextWave()) {

            //advance to next wave
            currentWave++;

            //update wave counter text
            waveCounterGUI.text = currentWave.ToString();
            wavesSurvivedGUI.text = (currentWave - 1).ToString();

            //pass in new wave details to spawner
            PassWaveDetailsToSpawner(enemyMissileSpawner);

            //play the next wave audio
            audioSource.Play();

            //resupply all missile launchers
            missileLauncherManager.GetComponent<MissileLauncherManager>().ResupplyRepairAllLaunchers();
        }
    }

    /// <summary>
    /// Passes wave details to the given missile spawner.
    /// </summary>
    /// <param name="missileSpawner">An EnemyMissileSpanwer component.</param>
    private void PassWaveDetailsToSpawner(EnemyMissileSpawner missileSpawnerScript) {

        //pass spawn delay details
        missileSpawnerScript.minDelayPerSpawn = Mathf.Clamp(longestMinSpawnDelay - ((currentWave - 1) * minSpawnDelayDecrement), shortestMinSpawnDelay, longestMinSpawnDelay);
        missileSpawnerScript.maxDelayPerSpawn = Mathf.Clamp(longestMaxSpawnDelay - ((currentWave - 1) * maxSpawnDelayDecrement), shortestMaxSpawnDelay, longestMaxSpawnDelay);

        //pass spawn quantity details
        missileSpawnerScript.spawnQuantity = Mathf.Clamp(Mathf.FloorToInt(minSpawnQuantity + ((currentWave - 1) * spawnQuantityIncrement)), minSpawnQuantity, maxSpawnQuantity);

        //pass missile spawn chance
        missileSpawnerScript.clusterMissileChance = Mathf.Clamp(minClusterSpawn + ((currentWave - 1) * clusterSpawnIncrement), minClusterSpawn, maxClusterSpawn);

        if (missileSpawnerScript.clusterMissileChance < maxClusterSpawn) {
            missileSpawnerScript.regularMissileChance = Mathf.Clamp(1 - (minClusterSpawn + ((currentWave - 1) * clusterSpawnIncrement)), 0, 1);
        }

        //pass missile speed
        missileSpawnerScript.missileSpeed = Mathf.Clamp(minMissileSpeed + ((currentWave - 1) * missileSpeedIncrement), minMissileSpeed, maxMissileSpeed);

        //mark the spawner as "not done spawning"
        missileSpawnerScript.doneSpawning = false;
    }

    private bool CanBeginNextWave() {
        /**
         * We can only begin the next wave if:
         * 1. There are no more missiles on-screen.
         * 2. There are no more explosions on-screen.
         * 3. The missile spawner has no more missiles to spawn.
         */
        return missilesActiveParent.childCount <= 0 && explosionsActiveParent.childCount <= 0 && enemyMissileSpawner.doneSpawning;
    }
}