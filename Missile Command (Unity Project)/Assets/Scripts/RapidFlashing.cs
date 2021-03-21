using UnityEngine;

/// <summary>
/// This class allows a sprite to rapidly flash at a given number of times per second.
/// </summary>
public class RapidFlashing : MonoBehaviour {

     /// <summary>
     /// Number of times the sprite will appear on screen.
     /// The number of times the sprite will change its state (from visible to invisible),
     /// is twice of the flashing rate.
     /// </summary>
     [Min(0f), Tooltip("Number of times the sprite will appear on the screen")]
     public byte flashingRate = 10;

     private float durationPerFlash;

     private SpriteRenderer spriteRenderer;

     private float timeStart;

     // Start is called before the first frame update
     private void Start() {
          spriteRenderer = GetComponent<SpriteRenderer>();

          if (!spriteRenderer) {
               Debug.LogError("The sprite renderer for the GameObject is not found!");
          }

          durationPerFlash = 1f / flashingRate;
     }

     // Update is called every frame, if the MonoBehaviour is enabled
     private void Update() {

          //we want to get the duration per flash
          if (Time.time - timeStart >= durationPerFlash) {

               ToggleSpriteRenderer();
               timeStart = Time.time;

          }
     }

     private void ToggleSpriteRenderer() {
          spriteRenderer.enabled = !spriteRenderer.enabled;
     }

}
