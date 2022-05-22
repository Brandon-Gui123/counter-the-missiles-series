using UnityEngine;

/// <summary>
/// This class helps to keep track of the buildings in the game.
/// </summary>
public class BuildingsTracker : MonoBehaviour {

     /// <summary>
     /// The parent GameObject housing all the buildings as its children.
     /// </summary>
     public GameObject buildingsParent;

     // Start is called before the first frame update
     private void Start() {

          //perform validity checks to ensure we got the parent GameObject
          CheckBuildingsParent();

     }

     private void CheckBuildingsParent() {
          if (!buildingsParent) {
               Debug.LogError("No cities parent GameObject is attached!");
          }
     }

     /// <summary>
     /// Do we have any buildings left? If yes, game continues. Otherwise, game ends.
     /// </summary>
     /// <returns>A Boolean indicating the prescence of buildings. True for present, false for absent.</returns>
     public bool HasNoBuildingsLeft() {
          return buildingsParent.transform.childCount <= 0;
     }
}
