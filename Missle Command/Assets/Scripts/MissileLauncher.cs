using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The missile launcher class contains a suite of methods for activation, resupplying, firing, deactivating etc.
/// </summary>
public class MissileLauncher : MonoBehaviour {

     /// <summary>
     /// Can this missile launcher fire missiles?
     /// </summary>
     public bool canLaunchMissiles = true;

     #region Camera - used for converting screen points to world points

     /// <summary>
     /// The GameObject that has a Camera component.
     /// Used for converting screen points to world points.
     /// </summary>
     [Header("Camera")]
     public GameObject cameraObject;

     /// <summary>
     /// The Camera component in the camera GameObject assigned in this script.
     /// </summary>
     private Camera cameraComponent;

     /// <summary>
     /// The position that a touch or click has been done.
     /// </summary>
     private Vector3 touchClickPosition;

     #endregion

     #region Missile Properties - determines how the missile behaves

     ///<summary>
     /// The missile that this launcher will fire. 
     ///</summary>
     [Header("Missile Properties")]
     public GameObject missileToLaunch;

     /// <summary>
     /// The speed at which to launch the missile.
     /// </summary>
     public float launchSpeed = 12.0f;

     /// <summary>
     /// The position in Vector3 to spawn the missile.
     /// </summary>
     private Vector3 missileSpawnPosition;

     #endregion

     #region Missile Destination Indication - shows where your launched missiles go

     /// <summary>
     /// The GameObject that is placed wherever the user clicks or taps to fire
     /// a missile from their launcher.
     /// This indicator will flash to show where that launched missile will explode.
     /// </summary>
     [Header("Missile Destination Indication")]
     public GameObject destinationIndicator;

     #endregion

     #region Launcher Ammunition - details on the launcher's ammo, such as how many missiles it can hold

     /// <summary>
     /// The maximum number of missiles the launcher can hold. 
     /// </summary>
     [Header("Launcher Ammunition")]
     public int ammoCapacity = 10;

     /// <summary>
     /// The current number of missiles the launcher is holding.
     /// </summary>
     public int currentAmmoCount = 0;

     /// <summary>
     /// Variable for helping to keep track of differences in the current ammunition count.
     /// </summary>
     private int ammoDifferenceTracker;

     #endregion

     #region Launcher Status - details on the launcher's behaviour, such as whether it is out of ammo or destroyed

     /// <summary>
     /// Whether the missile launcher is selected by the player.
     /// Selected missile launchers can fire missiles directed by the player.
     /// </summary>
     [Header("Launcher Status")]
     public bool isSelected = false;

     /// <summary>
     /// The variable for tracking differences in variable isSelected.
     /// </summary>
     private bool isSelected_DiffTracker = false;

     /// <summary>
     /// Whether the missile launcher is destroyed due to a nearby enemy explosion.
     /// </summary>
     public bool isDestroyed = false;

     /// <summary>
     /// Whether the missile launcher is low on ammo.
     /// A percentage value is provided to indicate the proportion of the
     /// number of missiles the launcher has to be considered "low on ammo".
     /// </summary>
     public bool isLowOnAmmo = false;

     /// <summary>
     /// The proportion of ammo that a launcher must have to be considered "low on ammo".
     /// </summary>
     [Range(0f, 1f)]
     [Tooltip(
          "The proportion of ammo that a launcher must have in order to " +
          "be considered \"low on ammo\""
     )]
     public float lowAmmoPercentage = 0.4f;

     #endregion

     #region Text Indicators - shows the current status of the missile launcher and its ammo count

     /// <summary>
     /// The GameObject containing the TextMeshPro component that will be responsible for displaying
     /// the number of missiles left for the missile launcher.
     /// </summary>
     [Header("Text Indications")]
     public GameObject ammoCounterTextObject;

     /// <summary>
     /// The TextMeshPro component responsible for displaying the ammo counter of this missile launcher.
     /// </summary>
     private TMPro.TextMeshPro ammoCounterTextMesh;

     /// <summary>
     /// The GameObject containing the TextMeshPro component that will display the status of the missile launcher.
     /// </summary>
     public GameObject statusTextObject;

     /// <summary>
     /// The TextMeshPro component responsible for displaying the status of this missile launcher.
     /// </summary>
     private TMPro.TextMeshPro statusTextMesh;

     /// <summary>
     /// The string of text to show when the ammo in the missile launcher is low.
     /// </summary>
     public string textForLowAmmo = "LOW";

     /// <summary>
     /// The string of text to show when the missile launcher can no longer be used,
     /// whether it is out of ammo, or destroyed.
     /// </summary>
     public string textForUnusable = "OUT";

     #endregion

     #region Colouring Indications - depicts the current state of the launcher by colouring the sprite

     /// <summary>
     /// The colour of the sprite when the missile launcher is selected.
     /// </summary>
     [Header("Colouring Indications")]
     public Color colorWhenSelected = new Color(255, 175, 65, 255);

     /// <summary>
     /// The colour of the sprite when the missile launcher is out of ammo.
     /// </summary>
     public Color colorWhenOutOfAmmo = new Color(200, 200, 200, 255);

     /// <summary>
     /// The original colour of the sprite.
     /// This colour is restored if the launcher is in normal status.
     /// </summary>
     private Color originalSpriteColor;

     /// <summary>
     /// The SpriteRenderer component of this GameObject.
     /// Is responsible for displaying the sprite, colouring it etc.
     /// </summary>
     private SpriteRenderer spriteRenderer;

     #endregion

     #region Constraints - limits firing area

     /// <summary>
     /// Prevent this missile launcher from firing below a certain y-coordinate.
     /// </summary>
     [Header("Constraints")]
     public float noFiringBelowY;

     #endregion

     private AudioSource audioSource;

     // Start is called before the first frame update
     private void Start() {

          //perform all the validity checks to ensure that we have everything we need
          CheckCamera();
          CheckMissileToLaunch();
          CheckDestinationIndicator();
          CheckAmmoCounterText();
          CheckStatusText();
          CheckSpriteRenderer();

          //get audio source component
          audioSource = GetComponent<AudioSource>();

          //get original sprite colour
          originalSpriteColor = spriteRenderer.color;

          //to be more realistic, missiles will spawn from the tip of the sprite
          missileSpawnPosition = GetSpriteTipPosition();

          //resupply our launcher
          ResupplyLauncher();

          //update difference trackers
          isSelected_DiffTracker = isSelected;
          ammoDifferenceTracker = currentAmmoCount;

          //update sprite colour
          UpdateSpriteColor();

          //update ammo counter
          UpdateAmmoCounter();

     }

     // Update is called once per frame
     private void Update() {

          if (HandleTouchInput() || HandleMouseInput()) {
               if (currentAmmoCount > 0 && !isDestroyed && isSelected) {

                    //we use Vector2 because we don't want the z-coordinates, which would affect
                    //our missile distance to destination
                    Vector2 worldPoint = cameraComponent.ScreenToWorldPoint(touchClickPosition);

                    if (worldPoint.y >= noFiringBelowY && canLaunchMissiles) {
                         Fire(missileSpawnPosition, worldPoint);
                    }
               }
          }

          //only update ammo text when ammo has changed so that we don't keep replacing the text
          //each Update call
          if (HasAmmoCountChanged() && !isDestroyed) {
               UpdateAmmoCounter();

               if (currentAmmoCount <= 0) {
                    statusTextMesh.text = textForUnusable;
                    spriteRenderer.color = colorWhenOutOfAmmo;
               } else if (IsLowOnAmmo()) {
                    statusTextMesh.text = textForLowAmmo;
               }
          }

          //is the launcher selected?
          if (HasSelectedChanged()) {
               //update color of the sprite
               UpdateSpriteColor();
          }
     }
     
     // Draws the Gizmos of this GameObject when it is selected in the Hierarchy
     private void OnDrawGizmosSelected() {

          Gizmos.color = Color.red;

          //draw a line representing the y-level where firing is not allowed
          Gizmos.DrawLine(new Vector2(-64, noFiringBelowY), new Vector2(64, noFiringBelowY));

     }
     
     #region Validity Checks - check for appropriate GameObjects and Components during Start and report to Console where necessary

     /// <summary>
     /// Checks if a GameObject has been assigned to the camera field, whether it has a Camera component
     /// and also helps to find a main camera if no camera GameObject is assigned.
     /// </summary>
     private void CheckCamera() {
          if (!cameraObject) {

               Debug.LogError("No main camera assigned to this missile launcher! Finding one now...");

               //find the main camera
               cameraObject = GameObject.FindGameObjectWithTag("MainCamera");

               if (!cameraObject) {
                    Debug.LogError("Unable to find the main camera. Make sure you have a GameObject tagged as \"MainCamera\" and has the Camera component.");

                    //exit method - no point going on without a camera
                    return;
               }
          }

          //get the camera's component
          cameraComponent = cameraObject.GetComponent<Camera>();

          if (!cameraComponent) {
               Debug.LogError("No Camera component found in the assigned camera GameObject! Did you forget to assign one?");
          }
     }

     /// <summary>
     /// Checks if the status text GameObject has been assigned, and helps to find
     /// one in case it isn't assigned.
     /// </summary>
     private void CheckStatusText() {
          if (!statusTextObject) {
               Debug.LogError("No GameObject is assigned as the status text object! Finding for that GameObject...");

               //attempting to find the status text object via hard-coded text
               statusTextObject = transform.Find("Status Text").gameObject;

               if (!statusTextObject) {
                    Debug.LogError("Unable to find the GameObject within this GameObject's children!");
               } else {
                    Debug.Log("Successfully found the GameObject.");
               }
          } else {

               //attempting to get TextMeshPro component from the assigned object
               statusTextMesh = statusTextObject.GetComponent<TMPro.TextMeshPro>();

               if (!statusTextMesh) {
                    Debug.LogError("Unable to find the TextMeshPro component in the status text object! The status text for this missile launcher will not be shown.");
               }

          }
     }

     /// <summary>
     /// Checks if the ammo counter text GameObject has been assigned, and helps to find
     /// one if isn't assigned.
     /// </summary>
     private void CheckAmmoCounterText() {
          if (!ammoCounterTextObject) {
               Debug.LogError("No GameObject is assigned as the ammo counter text object! Finding for one...");

               //attempting to find
               ammoCounterTextObject = transform.Find("Ammo Counter").gameObject;

               if (!ammoCounterTextObject) {
                    Debug.LogError("Unable to find that GameObject!");
               } else {
                    Debug.Log("Successfully found GameObject.");
               }
          } else {

               ammoCounterTextMesh = ammoCounterTextObject.GetComponent<TMPro.TextMeshPro>();

               if (!ammoCounterTextMesh) {
                    Debug.LogError("Unable to find the TextMeshPro component in the ammo counter text object! Ammo counter for this missile launcher will not be shown.");
               }

          }
     }

     /// <summary>
     /// Checks if the destination indicator GameObject has been assigned and
     /// informs if no such GameObject is assigned.
     /// </summary>
     private void CheckDestinationIndicator() {
          if (!destinationIndicator) {
               Debug.LogError("No indicator for missile available!");
          }
     }

     /// <summary>
     /// Checks if the missile to launch GameObject has been assigned.
     /// </summary>
     private void CheckMissileToLaunch() {
          if (!missileToLaunch) {
               Debug.LogError("No missile available to launch!");
          }
     }
     
     private void CheckSpriteRenderer() {
          if (!spriteRenderer) {
               spriteRenderer = GetComponent<SpriteRenderer>();

               if (!spriteRenderer) {
                    Debug.LogError("No SpriteRenderer found!");
               }
          }
     }

     #endregion

     /// <summary>
     /// Gets the position of the middle-top of this GameObject.
     /// </summary>
     /// <returns>A Vector3 representing the middle-top position of this GameObject.</returns>
     private Vector3 GetSpriteTipPosition() {
          /**
           * As positions of GameObjects are derived from its middle, we need to do some
           * manipulation in order to get positions other than the middle.
           * 
           * Also, since our sprite, by default has a length of 1 metre. To get the width of the sprite
           * when it has been scaled, we just use the lossy scale of that sprite.
           * We use lossy scale here so as to get the scale relative to the world and not the parent object.
           * 
           * Once we have the length, we divide it by half and add it to the sprite's y-coordinates in
           * order to move to the top. Since the x-coordinate is unchanged, we have essentially reached
           * the middle-top part of the sprite.
           * 
           */ // <-- explanation (expand to read)
          return new Vector3(transform.position.x, transform.position.y + transform.lossyScale.y / 2, 0);
     }
     
     /// <summary>
     /// Determines the rotation in the z-axis that a given GameObject must
     /// have to look at a certain point.
     /// </summary>
     /// <param name="origin">The position to start with.</param>
     /// <param name="target">The position that the GameObject will look at.</param>
     /// <returns>A float containing the value required for rotation.</returns>
     private float DetermineZAngle(Vector3 origin, Vector3 target) {

          /**
           * To understand this method, we need to use trigonometry.
           * 
           * Imagine two points, A and B, placed far apart.
           * A is on the left while B is on the right.
           * B is made higher than A.
           * 
           * Draw a straight line from A to B. This line would be the straight-line distance from A to B.
           * 
           * Now, draw a horizontal line across B. Then a vertical line along A.
           * 
           * The horizontal line and vertical line drawn will meet at some point. Let's label it, C.
           * You now have a triangle called ABC.
           * 
           * Angle CAB is what we are looking for. To get the angle, we can use the 
           * inverse tangent on the quotient of the ratio BC is to CA (BC / CA).
           * 
           * We can get the horizontal and vertical distances by subtracting the coordinates of the
           * position the missiles will spawn from the position of the cursor.
           * 
           * We can use Mathf.Atan to get the angle, but a better way would be to use Mathf.Atan2,
           * which is more reliable (division by zero is handled properly) and produces the
           * correct result when the cursor is below the spawn location for the missiles.
           * However, we do need to subtract 90 degrees to offset the extra rotation it incurs.
           */ // <-- expand for explanation

          //this is the distance for x-coordinates
          float horizontalDistance = target.x - origin.x;

          //this is the distance for y-coordinates
          float verticalDistance = target.y - origin.y;

          //calculate the angle to rotate the sprite (add 90 to bring it back to correct rotation)
          return Mathf.Atan2(verticalDistance, horizontalDistance) * Mathf.Rad2Deg - 90;

     }

     /// <summary>
     /// Fires a missile from the origin, heading towards the target
     /// </summary>
     /// <param name="origin">The position where the missile starts at.</param>
     /// <param name="target">The position the missile will go to.</param>
     private void Fire(Vector3 origin, Vector3 target) {

          //instantiate the missile
          GameObject missileInstance = Instantiate(missileToLaunch, origin, Quaternion.Euler(0, 0, DetermineZAngle(origin, target)));

          //instantiate the destination indicator
          GameObject destinationIndicatorInstance = Instantiate(destinationIndicator, target, Quaternion.identity);

          //attach details to the missile instance, such as its whereabouts
          AttachDetailsToMissile(missileInstance, launchSpeed, target, destinationIndicatorInstance);

          //play firing sound
          audioSource.Play();

          //decrement the number of missiles left in the launcher to signify emptying ammo store
          currentAmmoCount--;
     }

     /// <summary>
     /// Attaches required details to the Missile component in missiles.
     /// </summary>
     /// <param name="missileInstance">The missile GameObject instance to attach details to.</param>
     /// <param name="missileSpeed">The speed at which the missile moves at.</param>
     /// <param name="missileDestination">The position where the missile is meant to go to.</param>
     /// <param name="comboTrackerInstance">The GameObject responsible for tracking down combos.</param>
     private void AttachDetailsToMissile(
          GameObject missileInstance,
          float missileSpeed,
          Vector3 missileDestination,
          GameObject destinationIndicatorInstance
     ) {

          //get the component responsible for storing the info
          Missile missileScript = missileInstance.GetComponent<Missile>();

          //attach values to the script
          missileScript.playerMissileSpeed = missileSpeed;
          missileScript.destinationIndication = destinationIndicatorInstance;
          missileScript.destination = missileDestination;
          missileScript.isPlayerMissile = true;

     }

     //TODO: Use me if there are multiple touches and the current system can't detect it
     /**
      private List<Touch> HandleTouchInput() {

          if (Input.touchCount > 0) {

               //declare a list to hold valid touches
               List<Touch> validTouches = new List<Touch>();

               //handle each touch
               foreach (Touch touch in Input.touches) {

                    //get touches that just began
                    if (touch.phase == TouchPhase.Began) {
                         validTouches.Add(touch);
                    }

               }

               //return the list of valid touches
               return validTouches;

          } else {

               //no touches available
               return null;

          }

}
*/ //multiple touch HandleTouchInput method

     private bool HandleTouchInput() {

          if (Input.touchCount > 0) {

               //handle each touch
               foreach (Touch touch in Input.touches) {

                    //get touches that just began
                    if (touch.phase == TouchPhase.Began) {
                         touchClickPosition = touch.position;
                         return true;
                    }

               }

               //no valid touches
               return false;
          } else {

               //no touches
               return false;
          }

     }

     private bool HandleMouseInput() {

          if (Input.GetMouseButtonDown(0)) {
               touchClickPosition = Input.mousePosition;
               return true;
          } else {
               return false;
          }

     }

     private bool HasAmmoCountChanged() {

          if (ammoDifferenceTracker != currentAmmoCount) {
               ammoDifferenceTracker = currentAmmoCount;
               return true;
          } else {
               return false;
          }

     }

     private void UpdateAmmoCounter() {
          ammoCounterTextMesh.text = currentAmmoCount.ToString();
     }

     private bool IsLowOnAmmo() {
          return ((float) currentAmmoCount) / ammoCapacity <= lowAmmoPercentage;
     }

     private void UpdateStatusText() {

          if (isDestroyed || currentAmmoCount <= 0) {
               statusTextMesh.text = textForUnusable;
          } else if (IsLowOnAmmo()) {
               statusTextMesh.text = textForLowAmmo;
          } else {
               statusTextMesh.text = "";
          }

     }

     private void UpdateSpriteColor() {

          /**
           * When out of ammo, always show the colour when out of ammo.
           * When selected, show colour of selection only if the launcher has ammo.
           * When not selected and have ammo, show original colour.
           */ // <-- expand to see what we want to accomplish

          if (currentAmmoCount <= 0) {
               spriteRenderer.color = colorWhenOutOfAmmo;
          } else if (isSelected) {
               spriteRenderer.color = colorWhenSelected;
          } else {
               spriteRenderer.color = originalSpriteColor;
          }
     }

     public void DestroyLauncher() {

          //launcher is destroyed
          isDestroyed = true;

          //deactivate launcher's main components and ammo text
          GetComponent<PolygonCollider2D>().enabled = false;
          GetComponent<SpriteRenderer>().enabled = false;
          GetComponent<AudioSource>().enabled = false;
          ammoCounterTextObject.SetActive(false);

          //update status text
          statusTextMesh.text = textForUnusable;


     }

     public void ActivateLauncher() {

          //launcher is no longer destroyed (but no changes are made to the ammo)
          isDestroyed = false;

          //activate the launcher's main components, as well as its ammo text
          GetComponent<PolygonCollider2D>().enabled = true;
          GetComponent<SpriteRenderer>().enabled = true;
          GetComponent<AudioSource>().enabled = true;
          ammoCounterTextObject.SetActive(true);

          //update status text
          UpdateStatusText();

          //update sprite colour
          UpdateSpriteColor();
     }

     public void ResupplyLauncher() {

          //fill the launcher up with ammo up to its capacity
          currentAmmoCount = ammoCapacity;

          //update the status text
          if (!isDestroyed) {
               UpdateStatusText();
               UpdateSpriteColor();
          }

     }

     public void SelectLauncher() {

          isSelected = true;

          //change the colour of the sprite
          spriteRenderer.color = colorWhenSelected;

     }

     public void DeselectLauncher() {
          isSelected = false;
          spriteRenderer.color = originalSpriteColor;
     }

     private bool HasSelectedChanged() {
          if (isSelected_DiffTracker != isSelected) {
               isSelected_DiffTracker = isSelected;
               return true;
          } else {
               return false;
          }
     }
}
