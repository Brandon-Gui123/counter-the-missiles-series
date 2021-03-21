using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The missile launcher class contains a suite of methods for activation, resupplying, firing, deactivating etc.
/// </summary>
public class MissileLauncher : MonoBehaviour {

     public bool canLaunchMissiles = true;

     [Header("Camera")]
     #region Camera - for converting screen to world points

     /// <summary>
     /// The GameObject that represents the main camera.
     /// </summary>
     public GameObject mainCamera;

     /// <summary>
     /// The Camera component from the main camera.
     /// </summary>
     private Camera cam;

     /// <summary>
     /// The position that a touch or click has been done.
     /// </summary>
     private Vector3 touchClickPosition;

     #endregion

     [Header("Missile Properties")]
     #region Missile Properties - determines how the missile behaves

     ///<summary>
     /// The missile that this launcher will fire. 
     ///</summary>
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

     [Header("Missile Destination Indication")]
     #region Missile Destination Indication - shows where your launched missiles go
     /// <summary>
     /// The GameObject that is placed wherever the user clicks or taps to fire
     /// a missile from their launcher.
     /// This indicator will flash to show where that launched missile will explode.
     /// </summary>
     public GameObject destinationIndicator;

     #endregion

     [Header("Launcher Ammunition")]
     #region Launcher Ammunition - details on the launcher's ammo, such as how many missiles it can hold

     /// <summary>
     /// The maximum number of missiles the launcher can hold. 
     /// </summary>
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

     [Header("Launcher Status")]
     #region Launcher Status - details on the launcher's behaviour, such as whether it is out of ammo or destroyed

     /// <summary>
     /// Whether the missile launcher is selected by the player.
     /// Selected missile launchers can fire missiles directed by the player.
     /// </summary>
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

     [Header("Text Indications")]
     #region Text Indicators - shows the current status of the missile launcher and its ammo count

     /// <summary>
     /// The GameObject that has the TextMesh for rendering the ammo counter.
     /// </summary>
     public GameObject ammoCounterText;

     /// <summary>
     /// The TextMesh responsible for showing the current number of missiles in the launcher.
     /// </summary>
     private TextMesh ammoCounterTextMesh;

     /// <summary>
     /// The GameObject that has the TextMesh for rendering status text.
     /// </summary>
     public GameObject statusText;

     /// <summary>
     /// The TextMesh responsible for showing the current status of the missile launcher,
     /// such as whether it is low on ammo, or unusable.
     /// </summary>
     private TextMesh statusTextMesh;

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

     [Header("Colouring Indications")]
     #region Colouring Indications - depicts the current state of the launcher by colouring the sprite

     /// <summary>
     /// The colour of the sprite when the missile launcher is selected.
     /// </summary>
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

     [Header("Constraints")]
     #region Constraints - limits firing area

     /// <summary>
     /// Prevent this missile launcher from firing below a certain y-coordinate.
     /// </summary>
     public float noFiringBelowY;

     #endregion

     private AudioSource audioSource;

     // Start is called before the first frame update
     private void Start() {

          //perform all the validity checks to ensure that we have everything we need
          CheckMainCamera();
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
                    Vector2 worldPoint = cam.ScreenToWorldPoint(touchClickPosition);

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

     #region Validity Checks - check for appropriate GameObjects and Components during Start and report to Console where necessary

     private void CheckMainCamera() {
          if (!mainCamera) {
               mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

               if (!mainCamera) {
                    Debug.LogError("There are no Main Cameras available!");
               }
          } else {
               if (!cam) {
                    cam = mainCamera.GetComponent<Camera>();

                    if (!cam) {
                         Debug.LogError("There is no Camera component attached!");
                    }
               }
          }
     }

     private void CheckStatusText() {
          if (!statusText) {
               Debug.LogError("No status text available! A default GameObject has been added with TextMesh.");

               //add default TextMesh as child
               GameObject statusTextDefault = new GameObject("Status Text");
               statusTextMesh = statusTextDefault.AddComponent<TextMesh>();
               statusTextDefault.transform.SetParent(transform);
          } else {
               statusTextMesh = statusText.GetComponent<TextMesh>();

               if (!statusTextMesh) {
                    Debug.LogError("No TextMesh component available! A default TextMesh component has been added.");
                    statusTextMesh = statusText.AddComponent<TextMesh>();
               }
          }
     }

     private void CheckAmmoCounterText() {
          if (!ammoCounterText) {
               Debug.LogError("No ammo counter text available! A default GameObject has been added with TextMesh.");

               //add default TextMesh as child
               GameObject ammoCounterTextDefault = new GameObject("Ammo Counter");
               ammoCounterTextMesh = ammoCounterTextDefault.AddComponent<TextMesh>();
               ammoCounterTextDefault.transform.SetParent(transform);

          } else {
               ammoCounterTextMesh = ammoCounterText.GetComponent<TextMesh>();

               if (!ammoCounterTextMesh) {
                    Debug.LogError("No TextMesh component available! A default TextMesh component has been added.");
                    ammoCounterTextMesh = ammoCounterText.AddComponent<TextMesh>();
               }
          }
     }

     private void CheckDestinationIndicator() {
          if (!destinationIndicator) {
               Debug.LogError("No indicator for missile available!");
          }
     }

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
          AttachRequiredDetailsToMissile(missileInstance, launchSpeed, target, destinationIndicatorInstance);

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
     private void AttachRequiredDetailsToMissile(
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
          ammoCounterText.SetActive(false);

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
          ammoCounterText.SetActive(true);

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
