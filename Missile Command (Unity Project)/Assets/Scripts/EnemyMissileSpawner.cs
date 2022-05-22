using UnityEngine;

public class EnemyMissileSpawner : MonoBehaviour {

     public bool enableSpawning = true;
     public bool doneSpawning = false;

     [Header("Spawning Times")]
     public float timePerSpawn = 1.75f;
     private float timeStart = 0.0f;

     [Header("Rotation Deviation to Spawned Missile")]
     public float minRotation;
     public float maxRotation;

     [Header("Missiles To Spawn")]
     public EnemyWave enemyWave;

     [Header("Missile Properties")]
     public float speedMultiplier = 2.0f;

     public AudioSource audioSource;

     // Start is called before the first frame update
     private void Start() {
          audioSource.GetComponent<AudioSource>();
          timeStart = Time.time;
     }

     // Update is called once per frame
     private void Update() {

          if (enableSpawning) {
               if (Time.time - timeStart > timePerSpawn) {

                    //pick location for missile to spawn
                    Vector3 locationToSpawn = PickLocationToSpawn();

                    //pick a random rotation to spawn missile
                    float deviationAngle = PickAngleDeviation(minRotation, maxRotation);

                    //spawn the missile
                    SpawnMissile(locationToSpawn, deviationAngle);

                    timeStart = Time.time;

               }
          }

          doneSpawning = enemyWave.missiles.Count <= 0;
     }

     private Vector3 PickLocationToSpawn() {

          //calculate both the top-left corner and bottom-right corner coordinates
          Vector3 topLeftCorner = new Vector3(
               transform.position.x - transform.lossyScale.x / 2,
               transform.position.y + transform.lossyScale.y / 2,
               0
          );

          Vector3 bottomRightCorner = new Vector3(
               transform.position.x + transform.lossyScale.x / 2,
               transform.position.y - transform.lossyScale.y / 2
          );

          //randomly spawn missiles within the coordinates of those 2 corners
          Vector3 spawnPosition = new Vector3(
               Random.Range(topLeftCorner.x, bottomRightCorner.x),
               Random.Range(bottomRightCorner.y, topLeftCorner.y),
               0
          );

          return spawnPosition;
     }

     private float PickAngleDeviation(float minRotation, float maxRotation) {

          //randomly select a rotation
          return Random.Range(minRotation, maxRotation);
     }

     private void SpawnMissile(Vector3 locationToSpawn, float rotationZ) {

          //to prevent index out of range
          if (enemyWave.missiles.Count > 0) {

               //pick a missile from the list of available missiles
               GameObject missileToSpawn = enemyWave.missiles[enemyWave.missiles.Count - 1];

               //give it the appropriate position and rotation
               GameObject spawnedMissileInstance = Instantiate(missileToSpawn, locationToSpawn, Quaternion.Euler(0, 0, 180 + rotationZ));

               //attach required details to the missile
               AttachDetailsToSpawnedMissile(spawnedMissileInstance, speedMultiplier);

               //remove missile in the enemy wave
               enemyWave.missiles.RemoveAt(enemyWave.missiles.Count - 1);
          }
     }

     private void AttachDetailsToSpawnedMissile(GameObject spawnedMissileInstance, float missileSpeedMultiplier) {
          //get the required component
          Missile missileScript = spawnedMissileInstance.GetComponent<Missile>();

          //attach required details
          missileScript.isPlayerMissile = false;
          missileScript.speedMultiplier = missileSpeedMultiplier;

     }

     public void SetEnemyWave(EnemyWave enemyWave) {
          this.enemyWave = enemyWave;

          //set details for missile speed and spawn interval
          timePerSpawn = enemyWave.timePerMissileSpawn;
          speedMultiplier = enemyWave.missileSpeedMultiplier;

          //play wave changed audio
          audioSource.Play();
     }
}
