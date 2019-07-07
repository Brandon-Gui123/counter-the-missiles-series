using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    /// <summary>
    /// Whether this missile is fired by the player's missile launcher or not.
    /// Player missiles are handled differently from enemy missiles.
    /// </summary>
    [HideInInspector]
    public bool isPlayerMissile = false;

    #region Explosion Details - fields used for the explosion of this missile

    [Header("Explosion Details")]
    /// <summary>
    /// The GameObject to be used as the explosion for this missile.
    /// </summary>
    public GameObject explosion;

    /// <summary>
    /// The size of the explosion when the missile explodes.
    /// </summary>
    public float explosionSize;

    #endregion

    #region Player Missile Properties - fields that only apply when the missile is the player's

    [HideInInspector]
    public Vector2 missileDestination;

    [HideInInspector]
    public GameObject destinationIndicator;

    [HideInInspector]
    public Vector2 startingPosition;

    [HideInInspector]
    public float distanceToDestination;

    [HideInInspector]
    public GameObject comboTracker;

    #endregion

    /// <summary>
    /// The speed at which the missile will move.
    /// </summary>
    [Header("Movement")]
    //[HideInInspector]
    public float speed;

    [Header("Parent")]
    public string missileParentName = "Missiles Active";
    private Transform missileParent;


    #region Scoring

    [Header("Scoring")]
    public int score;

    #endregion


    /// <summary>
    /// Whether the missile has already collided with an explosion.
    /// This helps to prevent multiple <see cref="OnTriggerEnter2D"/> calls to this <see cref="GameObject"></see>,
    /// where each call will increase the combo and results in crazy high, unexpected scores and combos.
    /// </summary>
    private bool collidedWithExplosion = false;
    
    // Start is called before the first frame update
    private void Start() {

        missileParent = GameObject.Find(missileParentName).transform;

        //assign this GameObject itself to the GameObject which keeps missiles as its children
        //this would help keeping track of missiles easier
        transform.SetParent(missileParent);

        if (isPlayerMissile) {

            //get the starting position
            startingPosition = transform.position;

            //calculate the distance to the player's missile's destination
            distanceToDestination = Vector2.Distance(transform.position, missileDestination);
        }

        //all missiles are set to destroy itself after 20 seconds of it spawning
        //just in case if it escaped the game's boundaries
        Destroy(gameObject, 20);

    }

    // Update is called once per frame
    private void Update() {

        //move the missile
        MoveMissile();

        if (isPlayerMissile) {
            if (HasReachedDestination()) {

                //we can now explode this missile
                Explode(true, Instantiate(comboTracker, transform.position, Quaternion.identity));
            }
        }
    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only)
    private void OnTriggerEnter2D(Collider2D collision) {

        if (collidedWithExplosion) {
            return;
        }

        switch (collision.gameObject.layer) {
            //collision with boundary, building or missile launcher
            case 8:     //boundary
            case 9:     //building
            case 10:    //missile launcher
                Explode(false, null);
                break;

            //collision with another explosion
            case 11:
                //check if the explosion is caused or effected by the player
                //we do this to carry over combo values

                //perform an overlap check so that we increment one to only one explosion's combo
                //and not to all of the explosions that this missile is inside
                
                if (collision.gameObject.GetComponent<Explosion>().causedByPlayer) {
                    Collider2D[] overlappingExplosions = Physics2D.OverlapCircleAll(transform.position, transform.lossyScale.x / 2, LayerMask.GetMask("Explosions"));

                    //get the explosion whose combo tracker has the highest combo
                    GameObject maxComboExplosion = overlappingExplosions[0].gameObject;

                    foreach (Collider2D overlappingExplosion in overlappingExplosions) {
                        if (overlappingExplosion.gameObject.GetComponent<Explosion>().comboTracker.GetComponent<ComboTracker>().currentCombo > maxComboExplosion.GetComponent<Explosion>().comboTracker.GetComponent<ComboTracker>().currentCombo) {
                            maxComboExplosion = overlappingExplosion.gameObject;
                        }
                    }

                    Explode(true, maxComboExplosion.GetComponent<Explosion>().comboTracker);

                } else {
                    Explode(false, null);
                }


                collidedWithExplosion = true;
                
                break;
        }
    }

    private void AttachDetailsToExplosion(GameObject explosionInstance, bool causedByPlayer, GameObject comboTrackerInstance) {
        Explosion explosionScript = explosionInstance.GetComponent<Explosion>();

        explosionScript.causedByPlayer = causedByPlayer;
        explosionScript.explodeSize = explosionSize;

        if (causedByPlayer) {
            explosionScript.comboTracker = comboTrackerInstance;
        }
    }

    private void Explode(bool causedByPlayer, GameObject comboTracker) {

        if (!isPlayerMissile && comboTracker) {
            comboTracker.GetComponent<ComboTracker>().IncrementCombo(transform.position, score);
        }

        //instantiate an explosion
        GameObject missileExplosion = Instantiate(explosion, transform.position, Quaternion.identity);

        //track down combos on explosions caused by player missiles
        //attach required details to explosion
        AttachDetailsToExplosion(missileExplosion, causedByPlayer, comboTracker);

        //destroy this missile
        Destroy(gameObject);

        //destroy the destination indicator
        Destroy(destinationIndicator);

    }

    private bool HasReachedDestination() {
        //calculate the distance travelled by the missile
        float distanceTravelled = Vector2.Distance(startingPosition, transform.position);

        //have we travelled more than the required distance?
        //we do this because just finding the distance between the current position of the missile to the
        //destination because the value given doesn't tell if we have reached or we have already reached and is travelling
        //further away from our destination
        return distanceTravelled >= distanceToDestination;
    }

    private void MoveMissile() {
        transform.Translate(Vector3.up * speed * Time.deltaTime);
    }



}
