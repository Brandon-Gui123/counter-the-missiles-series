using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class City : MonoBehaviour {

    /// <summary>
    /// The possible states that a city can be in.
    /// A city's behaviour is based on its state. For example, a normal city can be collided with and destroyed.
    /// But a destroyed city can no longer be collided with.
    /// </summary>
    public enum CityState { Destroyed, Normal }

    /// <summary>
    /// The current state of the city.
    /// This depicts what kind of behaviour the city will exhibit.
    /// For example, if this city is normal, it can be destroyed by enemy missiles.
    /// However, if this city is destroyed, it can no longer be collided with enemy missiles.
    /// </summary>
    [Header("City State")]
    public CityState currentState = CityState.Normal;
    private CityState currentState_DiffTracker;

    [Header("Sprite Indications")]
    public SpriteRenderer citySpriteRenderer;
    public Sprite spriteWhenNormal;
    public Sprite spriteWhenDestroyed;

    private bool isVibrationSupported;

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only)
    private void OnTriggerEnter2D(Collider2D collision) {

        switch (collision.gameObject.layer) {
            case 11: //explosion layer
                if (!collision.gameObject.GetComponent<Explosion>().causedByPlayer) {
                    currentState = CityState.Destroyed;

#if UNITY_ANDROID
//preprocessor directive used for compiling to just Android devices
                    if (isVibrationSupported) {
                        Handheld.Vibrate();
                    }

#endif
                }
                break;
        }

    }

    // Start is called just before any of the Update methods is called the first time
    private void Start() {
        currentState_DiffTracker = currentState;
        UpdateSprite(currentState);

        isVibrationSupported = SystemInfo.supportsVibration;
    }

    // Update is called every frame, if the MonoBehaviour is enabled
    private void Update() {

        if (HasCurrentStateChanged()) {
            UpdateSprite(currentState);

            GetComponent<PolygonCollider2D>().enabled = !(currentState == CityState.Destroyed);
        }

    }

    // This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
    private void OnValidate() {
        UpdateSprite(currentState);

        GetComponent<PolygonCollider2D>().enabled = !(currentState == CityState.Destroyed);
    }

    private bool HasCurrentStateChanged() {
        if (currentState_DiffTracker != currentState) {
            currentState_DiffTracker = currentState;
            return true;
        } else {
            return false;
        }
    }

    private void UpdateSprite(CityState cityState) {
        switch (cityState) {
            case CityState.Normal:
                citySpriteRenderer.sprite = spriteWhenNormal;
                break;

            case CityState.Destroyed:
                citySpriteRenderer.sprite = spriteWhenDestroyed;
                break;
        }
    }

}
