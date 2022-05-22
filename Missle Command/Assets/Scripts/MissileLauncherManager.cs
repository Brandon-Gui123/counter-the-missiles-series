using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class responsible for keeping track with all missile launchers.
/// </summary>
public class MissileLauncherManager : MonoBehaviour {

     private Camera mainCamera;

     [HideInInspector]
     public List<GameObject> missileLaunchers;

     // Start is called before the first frame update
     private void Start() {

          //validity checks to ensure that we have everything
          CheckMainCamera();

          //obtain all child launchers
          GetChildLaunchers();
     }

     // Update is called once per frame
     private void Update() {

          if (Input.touchCount > 0) {

               foreach (Touch touch in Input.touches) {
                    if (touch.phase == TouchPhase.Began) {
                         SelectLauncher(touch.position);
                    }
               }

          }

          if (Input.GetMouseButtonDown(0)) {
               SelectLauncher(Input.mousePosition);
          }
     }

     private void CheckMainCamera() {
          if (!mainCamera) {
               GameObject mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

               if (!mainCamera) {
                    Debug.LogError("No main camera (tagged with \"MainCamera\") found!");
                    return;
               }

               this.mainCamera = mainCamera.GetComponent<Camera>();

               if (!this.mainCamera) {
                    Debug.LogError("No main camera component found in GameObject tagged with \"MainCamera\"!");
               }
          }
     }

     private void GetChildLaunchers() {
          for (int i = 0; i < transform.childCount; i++) {
               missileLaunchers.Add(transform.GetChild(i).gameObject);
          }
     }

     private void SelectLauncher(Vector3 position) {

          //convert screen point to world point
          Vector3 worldPosition = mainCamera.ScreenToWorldPoint(position);

          //raycast from the world point we selected
          RaycastHit2D hitInfo = Physics2D.Raycast(worldPosition, Vector2.zero);

          //did we collide into a player's launcher?
          if (hitInfo && hitInfo.collider.gameObject.layer == 11) {

               //deselect seleted launchers
               DeselectLauncher();

               //get the index where the selected GameObject is
               int selectedIndex = missileLaunchers.IndexOf(hitInfo.collider.gameObject);

               //select that launcher, if it isn't destroyed
               if (!missileLaunchers[selectedIndex].GetComponent<MissileLauncher>().isDestroyed || missileLaunchers[selectedIndex].GetComponent<MissileLauncher>().currentAmmoCount > 0) {
                    missileLaunchers[selectedIndex].GetComponent<MissileLauncher>().SelectLauncher();
               }
          }
     }

     /// <summary>
     /// Resupplies all launchers under this missile launcher manager.
     /// </summary>
     public void ResupplyAllLaunchers() {
          for (int i = 0; i < missileLaunchers.Count; i++) {
               missileLaunchers[i].GetComponent<MissileLauncher>().ResupplyLauncher();
          }
     }

     /// <summary>
     /// Activates all launchers under this missile launcher manager.
     /// </summary>
     public void ActivateAllLaunchers() {
          for (int i = 0; i < missileLaunchers.Count; i++) {
               missileLaunchers[i].GetComponent<MissileLauncher>().ActivateLauncher();
          }
     }

     private void DeselectLauncher() {

          //get the missile launcher that was selected
          for (int i = 0; i < missileLaunchers.Count; i++) {
               if (missileLaunchers[i].GetComponent<MissileLauncher>().isSelected) {
                    missileLaunchers[i].GetComponent<MissileLauncher>().DeselectLauncher();
               }
          }

     }

     /// <summary>
     /// Can we fast forward the game?
     /// Checks if all our launchers are out.
     /// </summary>
     /// <returns>A Boolean indicating if all our launchers are out.</returns>
     public bool CanFastForward() {
          for (int i = 0; i < missileLaunchers.Count; i++) {
               //if have ammo and isn't destroyed
               if (missileLaunchers[i].GetComponent<MissileLauncher>().currentAmmoCount > 0 && !missileLaunchers[i].GetComponent<MissileLauncher>().isDestroyed) {
                    return false;
               }
          }

          return true;
     }

     /// <summary>
     /// Deactivates all missile launchers under the missile launcher manager.
     /// </summary>
     public void DeactivateAllLaunchers() {
          for (int i = 0; i < missileLaunchers.Count; i++) {
               missileLaunchers[i].GetComponent<MissileLauncher>().canLaunchMissiles = false;
          }
     }

}
