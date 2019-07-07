using UnityEngine;

public class EnemyMissileSpawner : MonoBehaviour {

    [Header("Spawner Behaviour")]
    public bool enableSpawning = true;
    public bool doneSpawning = false;

    [Header("Spawning Quantity")]
    public int spawnQuantity = 20;

    [Header("Spawning Times")]
    public float minDelayPerSpawn = 0.5f;
    public float maxDelayPerSpawn = 1.5f;
    private float durationTillNextSpawn;
    private float timeStart = 0.0f;

    [Header("Rotation Deviation to Spawned Missile")]
    public float minRotation;
    public float maxRotation;

    [Header("Missile Types")]
    public GameObject regularMissile;

    [Range(0, 1)]
    public float regularMissileChance = 0.5f;

    [Space]
    public GameObject clusterMissile;

    [Range(0, 1)]
    public float clusterMissileChance = 0.5f;

    [Header("Missile Properties")]
    public float missileSpeed = 2.0f;

    // Start is called before the first frame update
    private void Start() {
        durationTillNextSpawn = Random.Range(minDelayPerSpawn, maxDelayPerSpawn);
        timeStart = Time.time;
    }

    // Update is called once per frame
    private void Update() {

        if (enableSpawning && !doneSpawning) {
            if (Time.time - timeStart > durationTillNextSpawn) {

                //pick location for missile to spawn
                Vector3 locationToSpawn = PickLocationToSpawn();

                //pick a random rotation to spawn missile
                float deviationAngle = PickAngleDeviation(minRotation, maxRotation);

                //spawn the missile
                SpawnMissile(locationToSpawn, deviationAngle);

                spawnQuantity--;

                timeStart = Time.time;

                //randomly select duration till next spawn
                durationTillNextSpawn = Random.Range(minDelayPerSpawn, maxDelayPerSpawn);
            }
        }

        if (spawnQuantity <= 0) {
            doneSpawning = true;
        }
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

    private GameObject PickMissileToSpawn(Vector2 locationToSpawn, float rotationZ) {
        //randomly generate a number
        float randomNum = Random.Range(0f, 1f);

        //check chances to decide which missile to spawn
        if (randomNum <= regularMissileChance) {
            //spawn regular missile
            return Instantiate(regularMissile, locationToSpawn, Quaternion.Euler(0, 0, 180 + rotationZ));
        } else {
            //spawn cluster missile
            return Instantiate(clusterMissile, locationToSpawn, Quaternion.Euler(0, 0, 180 + rotationZ));
        }
    }

    private void SpawnMissile(Vector3 locationToSpawn, float rotationZ) {

        //instantiate missile at given position with a given rotation
        GameObject missileSpawned = PickMissileToSpawn(locationToSpawn, rotationZ);

        //attach required details
        AttachDetailsToSpawnedMissile(missileSpawned, missileSpeed);
    }

    private void AttachDetailsToSpawnedMissile(GameObject spawnedMissileInstance, float missileSpeed) {
        //get the required component
        Missile missileScript = spawnedMissileInstance.GetComponent<Missile>();

        //attach required details
        missileScript.isPlayerMissile = false;
        missileScript.speed = missileSpeed;

    }
}