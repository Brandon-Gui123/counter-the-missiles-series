using UnityEngine;

/// <summary>
/// This class helps to fast forward the game by manipulating time scales where required.
/// </summary>
public class FastForward : MonoBehaviour {

     /// <summary>
     /// The GameObject that keeps track of the player's missile launchers.
     /// We require this to check if all of the missile launchers are out or not.
     /// </summary>
     public GameObject missileLauncherManager;

     /// <summary>
     /// The GameObject that shows the fast forward icon.
     /// We require this to show to the user when the game is in fast-forward.
     /// </summary>
     public GameObject fastForwardIcon;

     /// <summary>
     /// Is the game currently in fast forward?
     /// </summary>
     public bool isInFastForward = false;

     /// <summary>
     /// A variable used to compare with the original variable to help check for differences between one another.
     /// </summary>
     private bool isInFastForward_DiffTracker = false;

     /// <summary>
     /// The time scale to use when the game is in fast forward.
     /// </summary>
     public float fasterTimeScale = 3.0f;

     /// <summary>
     /// The time scale the game is in before the fast forward occurs.
     /// </summary>
     public float originalTimeScale = 1.0f;

     // Start is called before the first frame update
     private void Start() {

          CheckMissileLauncherManager();

          //original time scale is the current time scale
          originalTimeScale = Time.timeScale;

          isInFastForward_DiffTracker = isInFastForward;
     }

     // Update is called once per frame
     private void Update() {

          //can we fast forward?
          isInFastForward = CanFastForward();

          if (HasFastForwardChanged()) {
               if (isInFastForward) {
                    Time.timeScale = fasterTimeScale;
               } else {
                    Time.timeScale = originalTimeScale;
               }
          }

          DisplayFastForwardIcon(isInFastForward);
     }

     private void CheckMissileLauncherManager() {
          if (!missileLauncherManager) {
               Debug.LogError("No missile launcher manager!");
          }
     }

     private void CheckFastForwardIcon() {
          if (!fastForwardIcon) {
               Debug.LogError("Fast forward icon element not attached! No indication would be shown when fast-forwarded!");
          }
     }

     private bool CanFastForward() {
          return missileLauncherManager.GetComponent<MissileLauncherManager>().CanFastForward();
     }

     private void DisplayFastForwardIcon(bool isFastForward) {
          fastForwardIcon.SetActive(isFastForward);
     }

     /// <summary>
     /// Did our variable for fast forwarding change?
     /// </summary>
     /// <returns>Returns a Boolean stating whether the difference is found.</returns>
     public bool HasFastForwardChanged() {
          if (isInFastForward_DiffTracker != isInFastForward) {
               isInFastForward_DiffTracker = isInFastForward;
               return true;
          } else {
               return false;
          }
     }
}
