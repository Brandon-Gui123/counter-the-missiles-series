using UnityEngine;

public class MissileLauncher : MonoBehaviour {

    /// <summary>
    /// Toggles whether or not this missile launcher can fire missiles.
    /// This switch is can be accessed from other scripts, 
    /// which can toggle this missile launcher under various conditions.
    /// </summary>
    public bool canFireMissiles;

#if UNITY_EDITOR
    /**
     * We use this directive to ensure that certain contents of this script will only be compiled
     * for just the Unity Editor and not for our builds. This helps with improving performance and
     * reducing the size of our build, though it may be negligible but every byte counts right?
     * 
     * As mentioned above, this helps with performance since, for this script, we are not
     * going to run validation checks on our assigned GameObjects because it is assumed that
     * the build we release will be considered not missing of assigned GameObjects.
     * Validation can impact performance on low-end devices like mobile (yes, remember we are
     * developing for mobile devices in mind).
     */

    #region Debugging

    /// <summary>
    /// Whether or not to do the validations of <see cref="GameObject"/>s in the Start method.
    /// </summary>
    [Header("Debugging (editor only)")]
    public bool doStartValidations;

    #endregion

#endif

    #region Audio

    public AudioSource audioSource;

    public AudioSource lowAmmoAudioSource;

    private bool lowAmmoSoundPlayed = false;

    #endregion

    #region Camera - fields helpful for converting screen to world points

    /// <summary>
    /// The Camera component obtained from the assigned main camera object.
    /// </summary>
    [Header("Camera")]
    public Camera camComponent;

    #endregion

    #region Firing Missiles - fields to determine the missile's properties

    /// <summary>
    /// The GameObject used as a missile for this missile launcher.
    /// </summary>
    [Header("Firing Missiles")]
    public GameObject missileToUse;

    /// <summary>
    /// The speed at which the missile is being launched at towards its destination.
    /// This is with respect to Time.deltaTime.
    /// </summary>
    public float launchSpeed;

    /// <summary>
    /// The position that the missile will spawn (get launched from).
    /// </summary>
    private Vector2 missileSpawnPosition;

    /// <summary>
    /// The GameObject used as a destination indicator to show where the player-launched
    /// missile will explode.
    /// </summary>
    public GameObject destinationIndicator;

    #endregion

    #region Ammunition - fields holding information about this launcher 's ammo reserve

    /// <summary>
    /// The maximum number of missiles that this missile launcher can hold at a given instance.
    /// </summary>
    [Header("Ammunition")]
    public int ammoCapacity = 10;

    /// <summary>
    /// The current number of missiles in this missile launcher.
    /// If no more missiles are left (ammo count is zero), this missile launcher
    /// can no longer fire.
    /// </summary>
    public int currentAmmoCount = 10;

    /// <summary>
    /// This variable is used to keep track of changes that was made to the current ammo count.
    /// This helps to eliminate the need of constantly updating the text showing the ammo of
    /// this missile launcher, which can help with performance.
    /// </summary>
    private int currentAmmoCount_DiffTracker;

    #endregion

    #region Status - fields containing information on what happened to this launcher

    /// <summary>
    /// The possible states of a missile launcher.
    /// Each state determines how the missile launcher will behave.
    /// </summary>
    public enum LauncherState { Destroyed, No_Ammo, Selected, Unselected }

    /// <summary>
    /// The current state of this missile launcher.
    /// </summary>
    [Header("Status")]
    public LauncherState currentState = LauncherState.Unselected;

    /// <summary>
    /// A difference tracker for the variable <see cref="currentState"/>.
    /// This helps to keep detect changes made to the variable mentioned beforehand.
    /// Useful for when you want to call a certain method every time something changes and not on every frame.
    /// </summary>
    private LauncherState currentState_DiffTracker;

    #endregion

    #region Alerts - fields that alert to the user about events to this launcher

    /// <summary>
    /// The percentage of the current ammo count to its ammo capacity of a
    /// missile launcher where it will be considered to be low on ammo.
    /// </summary>
    [Header("Alerts"), Range(0f, 1f)]
    public float percentageForLowAmmo = 0.4f;

    #endregion

    #region Text Indications - text mesh pro components that render strings to display an indication

    /// <summary>
    /// The TextMeshPro component responsible for rendering the
    /// string that shows the current missile launcher's ammo count.
    /// </summary>
    [Header("Text Indications")]
    public TMPro.TextMeshPro ammoCounterTextMesh;

    /// <summary>
    /// The TextMeshPro component responsible for rendering
    /// the string which gives the missile launcher's status,
    /// such as whether it is "LOW" on ammo or "OUT", meaning unusable.
    /// </summary>
    public TMPro.TextMeshPro statusTextMesh;

    /// <summary>
    /// The string of text for the status of the missile launcher
    /// when it is low on ammo.
    /// </summary>
    public string textForLowAmmo = "LOW";

    /// <summary>
    /// The string of text for the status of the missile launcher
    /// when it cannot be used, such as when it is out of ammo or
    /// destroyed by an enemy missile.
    /// </summary>
    public string textForUnusable = "OUT";

    #endregion

    #region Colouring Indications - sprite renderer colours to show the status of the launcher

    /// <summary>
    /// The colour of the sprite when the missile launcher is selected.
    /// </summary>
    [Header("Colouring Indications")]
    public Color colorWhenSelected = new Color(255, 175, 65, 255);

    /// <summary>
    /// The colour of the sprite when the missile launcher is neither selected nor unusable.
    /// </summary>
    public Color colorWhenIdle = new Color(255, 100, 80, 255);

    /// <summary>
    /// The colour of the sprite when the missile launcher is unusable.
    /// </summary>
    public Color colorWhenUnusable = new Color(200, 200, 200, 255);

    #endregion

    #region Sprite Indications - sprites used to show the status of the launcher

    /// <summary>
    /// The sprite renderer for this missile launcher.
    /// The sprite renderer is responsible for displaying the launcher's sprite.
    /// </summary>
    [Header("Sprite Indications")]
    public SpriteRenderer launcherSpriteRenderer;

    /// <summary>
    /// The sprite of the missile launcher when it is destroyed by an enemy missile.
    /// </summary>
    public Sprite spriteWhenDestroyed;

    /// <summary>
    /// The sprite of the missile launcher when it is normal.
    /// Usually under normal game conditions, this is not required.
    /// However, if custom game conditions were to be ever implemented and
    /// missile launcher start out as destroyed, this would be a good way to switch over to the
    /// normal sprite once the missile launcher becomes available for use.
    /// </summary>
    public Sprite spriteWhenNormal;

    #endregion

    #region Constraints

    public float noFiringBelowY = -1;

    #endregion

    private bool isVibrationSupported;

    // Start is called just before any of the Update methods is called the first time
    private void Start() {

#if UNITY_EDITOR

        if (doStartValidations) {

            //perform validity checks
            DoValidityChecks();
        }

#endif

        //position of the tip of the launcher
        //we will use this to spawn missiles at the tip
        //more realistic
        missileSpawnPosition = new Vector2(
            transform.position.x,
            transform.position.y + transform.lossyScale.y / 2
        );

        //assign the initial values to all our diff trackers
        currentAmmoCount_DiffTracker = currentAmmoCount;
        currentState_DiffTracker = currentState;

        //perform a status text update
        UpdateStatusText();

        //perform an ammo counter text update
        UpdateAmmoCounterText();

        //update sprite at the start
        UpdateSprite();

        //update the sprite's colour
        UpdateSpriteColour();

        isVibrationSupported = SystemInfo.supportsVibration;
    }

    // Update is called every frame, if the MonoBehaviour is enabled
    private void Update() {

        //allow firing when our missile launcher is not destroyed, or isn't deselected
        //(i.e. is selected and not destroyed)
        if (currentState != LauncherState.Destroyed && currentState != LauncherState.Unselected) {

            //handle touch inputs
            if (Input.touchCount > 0) {
                foreach (Touch touch in Input.touches) {
                    if (touch.phase == TouchPhase.Began) {
                        //TODO: check if our touch is valid, since we don't want to fire while tapping the pause button

                        //prevent multi-touch from firing more than the current ammo reserve we have
                        if (currentAmmoCount <= 0) {
                            break;
                        }

                        //assume it is valid for now, and allow firing
                        if (currentState != LauncherState.No_Ammo && canFireMissiles) {
                            FireMissile(touch.position);
                        }
                    }
                }
            } else if (Input.GetMouseButtonDown(0)) {
                //TODO: check if our click position is valid, since we don't want to fire while tapping the pause button

                //assume it is valid for now, and allow firing
                if (currentState != LauncherState.No_Ammo && canFireMissiles) {
                    FireMissile(Input.mousePosition);
                }
            }
        }

        if (HasAmmoCountChanged()) {
            UpdateAmmoCounterText();

            //are we low on ammo?
            if (IsLowOnAmmo()) {
                if (!lowAmmoSoundPlayed) {
                    lowAmmoAudioSource.Play();
                    lowAmmoSoundPlayed = true;
                }
            }

            //have we ran out of ammo? This launcher cannot be used if it ran out of ammo!
            if (currentAmmoCount <= 0) {
                currentState = LauncherState.No_Ammo;
            }

            UpdateStatusText();
        }

        //update our status of this launcher
        if (HasCurrentStateChanged()) {

            ammoCounterTextMesh.enabled = currentState != LauncherState.Destroyed;

            //toggle polygon collider based on whether or not the launcher is destroyed
            GetComponent<PolygonCollider2D>().enabled = !(currentState == LauncherState.Destroyed);

            UpdateStatusText();
            UpdateSprite();
            UpdateSpriteColour();
        }

    }

    // OnTriggerEnter2D is called when the Collider2D other enters the trigger (2D physics only)
    private void OnTriggerEnter2D(Collider2D collision) {

        switch (collision.gameObject.layer) {

            case 11: //explosions
                //destroy when explosion is not caused (directly or indirectly) by player
                if (!collision.gameObject.GetComponent<Explosion>().causedByPlayer) {
                    currentState = LauncherState.Destroyed;

#if UNITY_ANDROID
//preprocessor directives for compiling to just Android
                    if (isVibrationSupported) {
                        Handheld.Vibrate();
                    }

#endif
                }
                break;

        }

    }

    // Implement this OnDrawGizmos if you want to draw gizmos that are also pickable and always drawn
    private void OnDrawGizmosSelected() {

        //draw a line at they y-level constraint
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(-10, noFiringBelowY), new Vector2(10, noFiringBelowY));

    }

    /// <summary>
    /// Attaches the given arguments into the appropriate script for the missile before it is launched.
    /// </summary>
    /// <param name="missile">The missile instance that is launched.</param>
    /// <param name="destination">The position where the missile will move to.</param>
    private void AttachDetailsToLaunchedMissile(GameObject missile, Vector2 destination, GameObject destIndicator) {
        Missile missileScript = missile.GetComponent<Missile>();

        missileScript.isPlayerMissile = true;
        missileScript.speed = launchSpeed;
        missileScript.missileDestination = destination;
        missileScript.destinationIndicator = destIndicator;
    }

    private bool CanUse() {
        /** 
         * A missile launcher can be used if it fulfills the following conditions:
         * 1. It is not destroyed.
         * 2. It has ammo available.
         */
        return currentState != LauncherState.Destroyed && currentState != LauncherState.No_Ammo;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Calls a collection of methods that will validate the assigned fields for this <see cref="GameObject"/>.
    /// </summary>
    private void DoValidityChecks() {
        ValidateAmmoCounterText();
        ValidateDestinationIndicator();
        ValidateMainCamera();
        ValidateMissile();
        ValidateSpriteRenderer();
        ValidateStatusText();
    }

#endif

    public void DeselectLauncher(bool interruptSelectionAnimation) {
        if (currentState != LauncherState.No_Ammo && currentState != LauncherState.Destroyed) {
            currentState = LauncherState.Unselected;

            if (interruptSelectionAnimation) {
                transform.Find("Selection Indicator").GetComponent<SelectionIndicator>().InterruptAnimation();
            }
        }
    }

    /// <summary>
    /// Determines the rotation in the z-axis that a given GameObject must
    /// have to look at a certain point.
    /// </summary>
    /// <param name="origin">The position to start with.</param>
    /// <param name="target">The position that the GameObject will look at.</param>
    /// <returns>A float containing the value required for rotation.</returns>
    private float DetermineZAngle(Vector2 origin, Vector2 target) {

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
    /// Fires a missile from the tip of the missile launcher.
    /// </summary>
    /// <param name="screenPosition">The position the user tapped or clicked.</param>
    private void FireMissile(Vector2 screenPosition) {
        //convert the screen position to a world position
        Vector2 worldPoint = camComponent.ScreenToWorldPoint(screenPosition);

        if (worldPoint.y < noFiringBelowY) {
            return;
        }

        //display an indicator to show that the missile will explode upon arriving specified position
        GameObject destIndicator = Instantiate(destinationIndicator, worldPoint, Quaternion.identity);

        //calculate the rotation about the z-axis that will be assigned to the missile
        float rotationZ = DetermineZAngle(missileSpawnPosition, worldPoint);

        //instantiate missile
        GameObject missile = Instantiate(missileToUse, missileSpawnPosition, Quaternion.Euler(0, 0, rotationZ));

        //attach details to the missile's script
        AttachDetailsToLaunchedMissile(missile, worldPoint, destIndicator);

        //reduce ammo by one
        currentAmmoCount--;

        //play audio
        audioSource.Play();
    }

    /// <summary>
    /// Checks if the ammo count has changed and returns a <see langword="bool"/> based on the result.
    /// True for yes, and no for otherwise.
    /// </summary>
    /// <returns>Returns a <see langword="bool"/> based on whether the ammo count has changed.</returns>
    private bool HasAmmoCountChanged() {
        if (currentAmmoCount != currentAmmoCount_DiffTracker) {
            currentAmmoCount_DiffTracker = currentAmmoCount;
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Checks if the current state has changed and returns a <see langword="bool"/> based on the comparison made.
    /// </summary>
    /// <returns>A <see langword="bool"/> representing whether or not the state has changed. True for yes, no for otherwise.</returns>
    private bool HasCurrentStateChanged() {
        if (currentState_DiffTracker != currentState) {
            currentState_DiffTracker = currentState;
            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Whether the missile launcher is low on ammo.
    /// A launcher is considered low on ammo if the percentage of the current ammo count over the ammo capacity
    /// of this launcher is less than or equal to the specified field <see cref="percentageForLowAmmo"/>.
    /// </summary>
    /// <returns>A <see langword="bool"/> stating if the missile launcher is low on ammo, based on the calculations of this method.</returns>
    private bool IsLowOnAmmo() {
        //we use a float cast on one of the operands to do the correct form of division (i.e. we don't want purely integeral division).
        return ((float) currentAmmoCount) / ammoCapacity <= percentageForLowAmmo;
    }

    public void RepairLauncher() {
        if (currentState == LauncherState.Destroyed) {
            currentState = LauncherState.Unselected;
        }
    }

    public void ResupplyLauncher() {
        currentAmmoCount = ammoCapacity;

        if (currentState != LauncherState.Selected) {
            currentState = LauncherState.Unselected;
        }

        //we can play the low ammo sound again
        lowAmmoSoundPlayed = false;
    }

    public bool SelectLauncher(bool playSelectionAnimation) {

        if (currentState != LauncherState.Destroyed && currentState != LauncherState.No_Ammo) {

            //this launcher gets selected!
            currentState = LauncherState.Selected;

            //play the selection indication animator by setting its trigger
            if (playSelectionAnimation) {
                transform.Find("Selection Indicator").GetComponent<SelectionIndicator>().PlayAnimation();
            }

            return true;
        } else {
            return false;
        }
    }

    /// <summary>
    /// Updates the ammo counter text when the ammo count changes.
    /// </summary>
    private void UpdateAmmoCounterText() {
        ammoCounterTextMesh.text = currentAmmoCount.ToString();
    }

    /// <summary>
    /// Updates the sprite of the missile launcher based on its current state.
    /// </summary>
    private void UpdateSprite() {
        if (currentState == LauncherState.Destroyed) {
            //use the "destroyed" sprite
            launcherSpriteRenderer.sprite = spriteWhenDestroyed;
        } else {
            launcherSpriteRenderer.sprite = spriteWhenNormal;
        }
    }

    /// <summary>
    /// Updates the sprite's colour based on the current status of the missile launcher.
    /// </summary>
    private void UpdateSpriteColour() {
        switch (currentState) {
            case LauncherState.No_Ammo:
            case LauncherState.Destroyed:
                launcherSpriteRenderer.color = colorWhenUnusable;
                break;

            case LauncherState.Selected:
                launcherSpriteRenderer.color = colorWhenSelected;
                break;

            case LauncherState.Unselected:
                launcherSpriteRenderer.color = colorWhenIdle;
                break;
        }
    }

    /// <summary>
    /// Updates the status text based on the current status and current ammunition of this missile launcher.
    /// </summary>
    private void UpdateStatusText() {

        switch (currentState) {
            //destroyed missile launcher states will take priority first
            case LauncherState.Destroyed:
            case LauncherState.No_Ammo:
                statusTextMesh.text = textForUnusable;
                break;

            default:
                //is it low on ammo?
                if (IsLowOnAmmo()) {
                    statusTextMesh.text = textForLowAmmo;
                    break;
                }

                //by default, don't show any text
                //do not show the status text when there's nothing wrong with the launcher
                statusTextMesh.text = "";
                break;
        }

    }

    #region Validity Checks and Assignment on Start (aids debugging)

#if UNITY_EDITOR
    /** We only want to do these validation checks on the Unity Editor
     * just to make debugging easier.
     * Since we want everything to be assigned before we begin building the game, these
     * methods here are no longer required.
     */

    /// <summary>
    /// Checks if a TextMeshPro component is assigned to the ammo counter text field and
    /// finds one in the children of this parent if none is being assigned to the field.
    /// </summary>
    private void ValidateAmmoCounterText() {
        if (!ammoCounterTextMesh) {
            Debug.LogError("No TextMeshPro component assigned as the ammo counter text of this missile launcher! Finding one now...");

            //find the GameObject in the children of this parent
            GameObject ammoCounterTextObject = transform.Find("Ammo Counter Text").gameObject;

            if (!ammoCounterTextObject) {
                Debug.LogError("Cannot find the ammo counter text GameObject. Aborting find.");
                return;
            } else {
                Debug.Log("Found the ammo counter text GameObject!");
            }

            //get the component from the status text object
            ammoCounterTextMesh = ammoCounterTextObject.GetComponent<TMPro.TextMeshPro>();

            if (!ammoCounterTextMesh) {
                Debug.LogError("Unable to find the TextMeshPro component! Ammo counter text for this missile launcher will not be shown.");
            } else {
                Debug.Log("Managed to find the TextMeshPro component from found ammo counter text GameObject!");
            }
        }
    }

    /// <summary>
    /// Checks if a destination indicator GameObject is assigned.
    /// </summary>
    private void ValidateDestinationIndicator() {
        if (!destinationIndicator) {
            Debug.LogError("No destination indicator GameObject available. There will be no indicator for where the missile will go to.");
        }
    }

    /// <summary>
    /// Checks if a GameObject has been assigned to the main camera field,
    /// finds it if it is not assigned and gets the camera component from
    /// said GameObject.
    /// </summary>
    private void ValidateMainCamera() {
        if (!camComponent) {

            Debug.LogError("No Camera component assigned! Finding one now...");

            //find the camera GameObject
            GameObject mainCameraObject = GameObject.FindGameObjectWithTag("MainCamera");

            if (!mainCameraObject) {
                Debug.LogError("Unable to find the Main Camera GameObject!");
                return;
            } else {
                Debug.Log("Found the Main Camera GameObject!");
            }

            camComponent = mainCameraObject.GetComponent<Camera>();

            if (!camComponent) {
                Debug.LogError("Unable to find the Camera component in the Main Camera GameObject!");
            } else {
                Debug.Log("Found the Camera component!");
            }
        }
    }

    /// <summary>
    /// Checks if a missile <see cref="GameObject"/> is assigned.
    /// </summary>
    private void ValidateMissile() {
        if (!missileToUse) {
            Debug.LogError("No missile GameObject is assigned! This launcher will not be able to fire missiles.");
        }
    }

    /// <summary>
    /// Checks for the presence of a <see cref="SpriteRenderer"/> component in this missile launcher.
    /// </summary>
    private void ValidateSpriteRenderer() {
        if (!launcherSpriteRenderer) {
            launcherSpriteRenderer = GetComponent<SpriteRenderer>();

            if (!launcherSpriteRenderer) {
                Debug.LogError("No SpriteRenderer component for this missile launcher is found!");
            }
        }
    }

    /// <summary>
    /// Checks if a TextMeshPro component has been assigned to the status text field and
    /// finds one in the children of this parent if the field mentioned above is not assigned.
    /// </summary>
    private void ValidateStatusText() {
        if (!statusTextMesh) {
            Debug.LogError("No TextMeshPro component assigned as the status text of this missile launcher! Finding one now...");

            //find the GameObject in the children of this parent
            GameObject statusTextObject = transform.Find("Status Text").gameObject;

            if (!statusTextObject) {
                Debug.LogError("Cannot find the status text GameObject. Aborting find.");
                return;
            } else {
                Debug.Log("Found the status text GameObject!");
            }

            //get the component from the status text object
            statusTextMesh = statusTextObject.GetComponent<TMPro.TextMeshPro>();

            if (!statusTextMesh) {
                Debug.LogError("Unable to find the TextMeshPro component! Status text for this missile launcher will not be shown.");
            } else {
                Debug.Log("Managed to find the TextMeshPro component from found status text GameObject!");
            }
        }
    }

#endif

    #endregion

}
