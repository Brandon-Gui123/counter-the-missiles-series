using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {

    /// <summary>
    /// Whether this explosion was caused by the player, directly or indirectly.
    /// Directly-caused player explosions come straight from the player's missiles
    /// while indirectly-caused player explosions come from explosions caused by the
    /// player's explosions exploding the enemy missile, which creates an explosion.
    /// This is useful to keep track of whether or not the missile is destroyed by the player.
    /// </summary>
    [HideInInspector]
    public bool causedByPlayer = false;

    #region Explosion Animation - details for animation speed
    
    /// <summary>
    /// The possible states of an explosion.
    /// </summary>
    public enum ExplosionState {
        Exploding, Imploding
    }

    /// <summary>
    /// The current state of the explosion.
    /// </summary>
    private ExplosionState explosionState = ExplosionState.Exploding;

    /// <summary>
    /// Whether the explosion sprite gets to rotate as an animation.
    /// </summary>
    [Header("Explosion Animation")]
    public bool doRotation = true;

    /// <summary>
    /// The speed at which the explosion sprite rotates at.
    /// </summary>
    public float rotationSpeed = 3.0f;

    /// <summary>
    /// The speed at which the explosion will expand.
    /// </summary>
    public float explosionSpeed = 1.0f;

    /// <summary>
    /// The size of the explosion where it no longer expands and will start shrinking.
    /// </summary>
    public float explodeSize = 5;

    /// <summary>
    /// The speed at which the explosion will shrink.
    /// </summary>
    public float implosionSpeed = 1.0f;

    #endregion

    #region Combo Tracking

    public GameObject comboTracker;

    #endregion

    #region Audio
    
    [Header("Audio")]
    [Range(0, 2)]
    public float minPitch = 0.8f;

    [Range(0, 2)]
    public float maxPitch = 1.2f;

    public AudioSource audioSource;

    #endregion

    [Header("Parent")]
    public string explosionParentName = "Explosions Active";
    private Transform explosionParent;

    // Start is called before the first frame update
    private void Start() {
        
        explosionParent = GameObject.Find(explosionParentName).transform;

        //assign the GameObject itself to the explosion parent GameObject, which keeps track of all explosions
        transform.SetParent(explosionParent);

        //reset the scaling to 1, for all axis
        transform.localScale = Vector3.zero;

        //is by the player and has a combo tracker on?
        if (causedByPlayer && comboTracker) {
            comboTracker.GetComponent<ComboTracker>().AddToList(gameObject);
        }

        //randomize a pitch
        float playbackPitch = Random.Range(minPitch, maxPitch);

        audioSource.pitch = playbackPitch;
    }

    // Update is called once per frame
    private void Update() {

        //always rotate the explosion
        if (doRotation) {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        switch (explosionState) {
            case ExplosionState.Exploding:
                transform.localScale += Vector3.one * explosionSpeed * Time.deltaTime;

                if (transform.localScale.x >= Vector3.one.x * explodeSize) {
                    explosionState = ExplosionState.Imploding;
                }

                break;

            case ExplosionState.Imploding:
                if (transform.localScale.x > 0) {
                    transform.localScale += Vector3.one * -implosionSpeed * Time.deltaTime;
                } else {

                    //unsubscribe if caused by player and have a combo tracker instance
                    if (causedByPlayer && comboTracker) {
                        comboTracker.GetComponent<ComboTracker>().RemoveFromList(gameObject);
                    }

                    //we can destroy this explosion, since it has reduced in size already
                    Destroy(gameObject);
                    
                }
                break;
        }
    }
    
}
