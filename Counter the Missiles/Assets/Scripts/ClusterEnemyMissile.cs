using UnityEngine;

/// <summary>
/// This class is for the special enemy missile that splits into more enemy missiles.
/// </summary>
public class ClusterEnemyMissile : MonoBehaviour {

     [Header("Sprayed Missiles")]
     /// <summary>
     /// The number of missiles to spawn after the cluster missile sprays.
     /// </summary>
     public int missilesToSpawn = 5;

     /// <summary>
     /// The missile to spawn after the cluster missile sprays.
     /// </summary>
     public GameObject missileToSpawn;

     [Header("Spray Deviation")]
     public float minAngle;
     public float maxAngle;

     [Header("Time for Spraying")]
     public float minDuration = 3.0f;
     public float maxDuration = 6.0f;
     private float timeBeforeSpraying;
     private float timeStart = 0.0f;
     private float timeLeft = 0.0f;

     [Header("Animation")]
     public Animation flashingAnimation;
     public float onLastSeconds = 1.0f;

     // Start is called before the first frame update
     private void Start() {
          timeStart = Time.time;
          timeBeforeSpraying = Random.Range(minDuration, maxDuration);
     }

     // Update is called once per frame
     private void Update() {

          //calculate time left before the missile sprays
          timeLeft = timeBeforeSpraying - (Time.time - timeStart);

          if (timeLeft <= 0) {

               //spray missiles
               SprayMissiles();

               //destory the cluster missile
               Destroy(gameObject);
          }

          if (timeLeft <= onLastSeconds && !flashingAnimation.isPlaying) {
               //begin playing the animation
               flashingAnimation.Play();

          }

     }

     private void SprayMissiles() {

          for (int i = 0; i < missilesToSpawn; i++) {

               Vector3 rotationVectors = transform.rotation.eulerAngles;

               //determine an angle between the bounds, in addition to our current angle
               float angleZ = Random.Range(rotationVectors.z + minAngle, rotationVectors.z + maxAngle);

               GameObject missileInstance = Instantiate(missileToSpawn, transform.position, Quaternion.Euler(rotationVectors.x, rotationVectors.y, angleZ));

               //conserve speed of the missile
               missileInstance.GetComponent<Missile>().speedMultiplier = GetComponent<Missile>().speedMultiplier;

          }
     }

}
