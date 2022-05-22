using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncherManager : MonoBehaviour {

    public Camera mainCamComponent;

    public Transform missileLauncherParent;

    private List<Transform> missileLauncherChildren;

    private Vector2 touchClickPosition;

    public GameObject fastForwardUI;

    public float timeScale_SetTracker = 1.0f;

    public AudioSource noLauncherAudioSource;

    public GameStatusManager gameStatusManager;

    // Start is called before the first frame update
    private void Start() {

        missileLauncherChildren = new List<Transform>(missileLauncherParent.childCount);

        for (int i = 0; i < missileLauncherChildren.Capacity; i++) {
            missileLauncherChildren.Add(missileLauncherParent.GetChild(i));
        }

    }

    // Update is called once per frame
    private void Update() {

        if (ScreenIsClicked() || ScreenIsTouched()) {
            SelectLauncher(touchClickPosition);

            if (!HasLauncherSelected() && !gameStatusManager.isGameOver) {
                noLauncherAudioSource.Play();
            }
        }

        if (AreMissileLaunchersOut()) {
            SetTimeScale(2.0f);
            fastForwardUI.SetActive(true);
        } else {
            SetTimeScale(1.0f);
            fastForwardUI.SetActive(false);
        }
    }

    private bool ScreenIsClicked() {
        if (Input.GetMouseButtonDown(0)) {
            touchClickPosition = Input.mousePosition;
            return true;
        } else {
            return false;
        }
    }

    private bool ScreenIsTouched() {
        if (Input.touchCount > 0) {
            foreach (Touch touch in Input.touches) {
                if (touch.phase == TouchPhase.Began) {
                    touchClickPosition = touch.position;
                    return true;
                }
            }

            return false;
        } else {
            return false;
        }
    }

    private void SelectLauncher(Vector2 touchedClickedPosition) {

        //convert the touched/clicked position to world position
        Vector2 worldPoint = mainCamComponent.ScreenToWorldPoint(touchedClickedPosition);

        //perform a raycast from the world point we have converted
        RaycastHit2D hitInfo = Physics2D.Raycast(worldPoint, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Missile Launcher Selection Area"));

        //check to see if we have managed to hit a missile launcher's selection area
        if (hitInfo.collider != null) {

            //we want to get the index of the selection area's launcher in the list declared at the very top
            int selectionAreaParentIndex = missileLauncherChildren.IndexOf(hitInfo.transform.parent);
            
            //skip if the launcher is destroyed or out of ammo
            if (missileLauncherChildren[selectionAreaParentIndex].GetComponent<MissileLauncher>().currentState == MissileLauncher.LauncherState.Destroyed
                || missileLauncherChildren[selectionAreaParentIndex].GetComponent<MissileLauncher>().currentState == MissileLauncher.LauncherState.No_Ammo) {

                return;
            }

            bool deselectOtherLaunchers = true;

            //for-loop to select and deselect appropriate launchers
            for (int i = 0; i < missileLauncherChildren.Count; i++) {
                if (i == selectionAreaParentIndex) {
                    deselectOtherLaunchers = missileLauncherChildren[i].GetComponent<MissileLauncher>().SelectLauncher(true);
                } else {
                    if (deselectOtherLaunchers) {
                        missileLauncherChildren[i].GetComponent<MissileLauncher>().DeselectLauncher(true);
                    }
                }
            }
        }

    }

    public void ResupplyRepairAllLaunchers() {
        foreach (Transform missileLauncher in missileLauncherChildren) {

            MissileLauncher missileLauncherScript = missileLauncher.GetComponent<MissileLauncher>();
            missileLauncherScript.ResupplyLauncher();
            missileLauncherScript.RepairLauncher();
            
        }
    }

    public bool AreMissileLaunchersOut() {
        foreach (Transform missileLauncher in missileLauncherChildren) {
            if (missileLauncher.GetComponent<MissileLauncher>().currentState != MissileLauncher.LauncherState.Destroyed &&
                missileLauncher.GetComponent<MissileLauncher>().currentState != MissileLauncher.LauncherState.No_Ammo) {
                return false;
            }
        }

        return true;
    }
    
    public void SetTimeScale(float timeScale) {
        if (timeScale_SetTracker == timeScale) {
            return;
        } else {
            timeScale_SetTracker = timeScale;
            Time.timeScale = timeScale;
        }
    }

    public void StopLauncherFire() {
        foreach (Transform missileLauncher in missileLauncherChildren) {
            missileLauncher.GetComponent<MissileLauncher>().canFireMissiles = false;
        }
    }

    public bool HasLauncherSelected() {
        foreach (Transform missileLauncher in missileLauncherChildren) {
            if (missileLauncher.GetComponent<MissileLauncher>().currentState == MissileLauncher.LauncherState.Selected) {
                return true;
            }
        }

        return false;
    }
}
