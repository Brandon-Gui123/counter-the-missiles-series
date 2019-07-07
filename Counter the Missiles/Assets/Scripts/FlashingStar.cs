using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingStar : MonoBehaviour {

    private float flashPeriod;

    [SerializeField]
    private SpriteRenderer spriteRenderer;

    private bool gyroscopeUsable;
    private bool accelerometerSupported;

    private Vector3 originalPosition;

    // Start is called before the first frame update
    void Start() {
        //randomize a duration before the sprite starts flashing
        flashPeriod = Random.Range(0.4f, 0.6f);

        //invoke the FlashStar function after some time
        InvokeRepeating(nameof(FlashStar), 0.5f, flashPeriod);

        if (SystemInfo.supportsGyroscope) {
            //attempt to enable it
            if (Input.gyro != null) {
                Input.gyro.enabled = true;
                gyroscopeUsable = true;
            }
        }

        accelerometerSupported = SystemInfo.supportsAccelerometer;

        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update() {
        if (gyroscopeUsable) {
            Gyroscope gyroscope = Input.gyro;

            Quaternion deviceAttituide = gyroscope.attitude;

            //because gyroscope is right-handed, while Unity is left handed, we need
            //to make a change
            float xPos = originalPosition.x + GyroToUnity(deviceAttituide).eulerAngles.normalized.x * 5;
            float yPos = originalPosition.y + GyroToUnity(deviceAttituide).eulerAngles.normalized.y * 5;

            transform.position = new Vector3(xPos, yPos, transform.position.z);

        } else if (accelerometerSupported) {
            Vector3 accel = Input.acceleration;
            transform.position = originalPosition + accel * 2;
        }
    }

    private Quaternion GyroToUnity(Quaternion q) {
        return new Quaternion(q.x, q.y, -q.z, -q.w);
    }

    void FlashStar() {
        spriteRenderer.enabled = !spriteRenderer.enabled;
    }
}