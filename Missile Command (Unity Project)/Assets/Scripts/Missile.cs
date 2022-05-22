using UnityEngine;

/// <summary>
/// This class represents a missile that flies across the screen.
/// Contains helpful methods for exploding the missile.
/// </summary>
public class Missile : MonoBehaviour {

     [Header("Ownership")]
     public bool isPlayerMissile;

     [Header("Locations (player missiles only)")]
     public Vector3 startPosition;
     public Vector3 destination;
     private float distanceToDestination;

     [Header("Missile Properties")]
     public float playerMissileSpeed = 15.0f;
     public float baseSpeed = 1.0f;
     public float speedMultiplier = 1.0f;
     private float calculatedSpeed = 1.0f;

     [Header("Explosion Properties")]
     public GameObject explosion;
     public float explosionSize = 3.0f;
     public float explosionSizeIncreaseRate = 3.0f;

     [Header("Scoring")]
     public int baseScore;
     public int scoreMultiplier;
     public int calculatedScore;
     public GameObject scoreTracker;

     [HideInInspector]
     public GameObject destinationIndication;

     // Start is called before the first frame update
     private void Start() {

          //perform validity checks
          CheckExplosion();
          CheckScoreTracker();

          startPosition = transform.position;

          //calculate distance to reach destination
          if (isPlayerMissile) {
               distanceToDestination = Vector3.Distance(startPosition, destination);
          }

          //get the calculated speed of the missiles
          calculatedSpeed = baseSpeed * speedMultiplier;

          //get the calculated points the missile will give when destroyed by a player
          calculatedScore = baseScore * scoreMultiplier;

          //set all non-player missiles to destroy themselves to prevent forever falling
          if (!isPlayerMissile) {
               Destroy(gameObject, 20f);
          }

     }

     // Update is called once per frame
     private void Update() {
          //move the missile
          MoveMissile();

          //for player missiles, check if the missile has reached its destination
          if (isPlayerMissile && HasReachedDestination()) {
               //we can simply explode the player's missile
               Explode(true);
          }
     }

     // This function is called when the MonoBehaviour will be destroyed
     private void OnDestroy() {
          if (isPlayerMissile) {
               Destroy(destinationIndication);
          }
     }

     // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only)
     private void OnTriggerEnter2D(Collider2D collision) {
          if (!isPlayerMissile) {

               //non-player missiles can hit the ground, players and buildings
               //also, explosions from missiles destroyed by players don't damage stuff (be nice and fair)
               if (collision.gameObject.layer == 9
                    || collision.gameObject.layer == 10
                    || collision.gameObject.layer == 11) {

                    //explode missile (explosion is not by the player's missile)
                    Explode(false);
               }

          }
     }

     #region Validity Checks - ensure that the correct stuff is assigned

     private void CheckExplosion() {
          if (!explosion) {
               Debug.LogError("No explosion is attached to this missile!");
          }
     }

     private void CheckScoreTracker() {
          if (!scoreTracker) {
               scoreTracker = GameObject.FindGameObjectWithTag("Score Tracker");
          }
     }

     #endregion

     private void MoveMissile() {

          if (isPlayerMissile) {
               transform.Translate(Vector3.up * playerMissileSpeed * Time.deltaTime);
          } else {
               //move the missile relative to its "up" vector
               transform.Translate(Vector3.up * calculatedSpeed * Time.deltaTime);
          }

     }

     private bool HasReachedDestination() {
          //calculate the current distance travelled by the missile
          float distanceTravelled = Vector3.Distance(startPosition, transform.position);

          return distanceTravelled >= distanceToDestination;
     }

     public void Explode(bool isByPlayerMissile) {

          //instantiate an explosion at where the missile is
          GameObject explosionInstance = Instantiate(explosion, transform.position, Quaternion.identity);

          //attach details to explosion
          AttachDetailsToExplosion(explosionInstance, explosionSize, explosionSizeIncreaseRate, isByPlayerMissile);

          //add points
          if (isByPlayerMissile) {
               scoreTracker.GetComponent<ScoreTracker>().AddToScore(calculatedScore);
          }

          //destroy this missile
          Destroy(gameObject);
     }

     private void AttachDetailsToExplosion(GameObject explosionInstance, float explosionSize, float explosionSizeIncreaseRate, bool isByPlayerMissile) {
          //get the required component
          Explosion explosionScript = explosionInstance.GetComponent<Explosion>();

          //attach required details
          explosionScript.explosionSize = explosionSize;
          explosionScript.explosionSizeIncreaseRate = explosionSizeIncreaseRate;
          explosionScript.isByPlayerMissile = isByPlayerMissile;

     }

}
