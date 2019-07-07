using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterMissile : MonoBehaviour {

    [Header("Sprayed Missiles")]
    public int minMissilesToSpawn = 2;
    public int maxMissilesToSpawn = 5;
    
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
    public Animator missileAnimator;
    public float onLastSeconds = 1.0f;
    public int flashesPerSec = 8;

    // Start is called before the first frame update
    private void Start() {
        timeStart = Time.time;
        timeBeforeSpraying = Random.Range(minDuration, maxDuration);

        missileAnimator.SetFloat("flashesPerSec", flashesPerSec);
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

        if (timeLeft <= onLastSeconds) {
            
            //begin playing the animation
            missileAnimator.SetTrigger("beginFlashing");

        }

    }

    private int DecideNumMissilesToSpawn() {
        return Random.Range(minMissilesToSpawn, maxMissilesToSpawn);
    }

    private void SprayMissiles() {

        for (int i = 0; i < DecideNumMissilesToSpawn(); i++) {

            Vector3 rotationVectors = transform.rotation.eulerAngles;

            //determine an angle between the bounds, in addition to our current angle
            float angleZ = Random.Range(rotationVectors.z + minAngle, rotationVectors.z + maxAngle);

            GameObject missileInstance = Instantiate(missileToSpawn, transform.position, Quaternion.Euler(rotationVectors.x, rotationVectors.y, angleZ));

            //conserve speed of the missile
            missileInstance.GetComponent<Missile>().speed = GetComponent<Missile>().speed;

        }
    }
}
