using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blinking : MonoBehaviour {

    /// <summary>
    /// The number of times to blink per second.
    /// </summary>
    public byte blinkingRate;

    /// <summary>
    /// Whether the blinking animation is running.
    /// </summary>
    private bool isBlinking = false;

    /// <summary>
    /// Whether the blinking animation is running.
    /// </summary>
    public bool IsBlinking {
        get {
            return isBlinking;
        }
    }

    /// <summary>
    /// The duration between blinks.
    /// </summary>
    private float blinkingPeriod;
    
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        //calculate the duration between each change in the sprite renderer's state
        //we multiply by two because a blink consists of 2 changes to the sprite renderer's state
        blinkingPeriod = 1f / (blinkingRate * 2);

        //if we are invoking the method (because we previewed it in Play Mode), stop
        if (IsInvoking(nameof(ToggleSpriteRenderer))) {
            CancelInvoke(nameof(ToggleSpriteRenderer));
        }

        //start the blinking
        ToggleBlinking(true);
    }

    private void ToggleSpriteRenderer() {
        spriteRenderer.enabled = !spriteRenderer.enabled;
    }
    
    public void ToggleBlinking(bool doBlinking) {

        if (doBlinking) {
            //start the blinking
            InvokeRepeating(nameof(ToggleSpriteRenderer), 0, blinkingPeriod);
            isBlinking = true;
        } else {
            //stop the blinking
            CancelInvoke(nameof(ToggleSpriteRenderer));
            isBlinking = false;
        }

    }
}
