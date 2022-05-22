using UnityEngine;

/// <summary>
/// This class represents the damaging explosion that can destroy missiles and will destory buildings.
/// Contains helpful methods for exploding, imploding and seeing whether the explosion is caused by the player,
/// directly or indirectly.
/// </summary>
public class Explosion : MonoBehaviour {

     [HideInInspector] public float explosionSize;
     [HideInInspector] public float explosionSizeIncreaseRate;

     /// <summary>
     /// Is the explosion currently shrinking in size?
     /// </summary>
     [HideInInspector] public bool isImploding = false;

     /// <summary>
     /// Is the explosion directly/indirectly caused by the player?
     /// We require this to prevent damage to our buildings caused by explosions by the player's missile and the
     /// missiles destroyed by the player.
     /// </summary>
     [HideInInspector] public bool isByPlayerMissile = false;

     /// <summary>
     /// Is this GameObject marked for destruction?
     /// We use this to avoid calling methods too many times.
     /// </summary>
     [HideInInspector] public bool markedForDestruction = false;

     private AudioSource audioSource;

     // Start is called before the first frame update
     private void Start() {
          audioSource = GetComponent<AudioSource>();

          //randomize a pitch
          audioSource.pitch = Random.Range(0.7f, 1.2f);

          //start off with a scale of zero
          transform.localScale = Vector3.zero;
     }

     // Update is called once per frame
     private void Update() {
          if (transform.localScale.x >= explosionSize && !isImploding) {
               isImploding = true;
          }

          if (!isImploding) {
               Explode();
          } else {
               Implode();
          }

          if (isImploding && transform.localScale.x <= 0 && !markedForDestruction) {
               markedForDestruction = true;
               Destroy(gameObject);
          }
     }

     // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only)
     private void OnTriggerEnter2D(Collider2D collision) {

          if (!isByPlayerMissile) {

               //destroy buildings, missile launchers
               if (collision.gameObject.layer == 10) { //building layer
                    Destroy(collision.gameObject);
               }

               if (collision.gameObject.layer == 11) { //missile launcher layer
                    collision.gameObject.GetComponent<MissileLauncher>().DestroyLauncher();
               }

          }

          //destroy enemies
          if (collision.gameObject.layer == 8) { //enemies layer
               collision.gameObject.GetComponent<Missile>().Explode(isByPlayerMissile);
          }

     }

     private void Explode() {
          transform.localScale += Vector3.one * explosionSizeIncreaseRate * Time.deltaTime;
     }

     private void Implode() {
          transform.localScale -= Vector3.one * explosionSizeIncreaseRate * Time.deltaTime;
     }

}
