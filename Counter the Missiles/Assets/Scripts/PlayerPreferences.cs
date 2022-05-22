using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPreferences : MonoBehaviour {

    public static PlayerPreferences preferences;

    [SerializeField]
    private bool useVibration = false;

    // Awake is called when the script instance is being loaded
    private void Awake() {
        //the following is a singleton pattern that only allows one gameObject with this
        //script component attached to be in the scene
        //any other gameObjects with this script component would be destroyed
        if (!preferences) {

            //allow for data persistance by preventing the GameObject with
            //this script component from being destroyed upon scene loads
            DontDestroyOnLoad(gameObject);
            preferences = this;

        } else if (preferences != this) {

            //we are interested in the current player's preferences so we keep this
            //and destroy the other preferences
            Destroy(gameObject);
        }
    }
    
    public void SavePlayerPreferences() {
        PlayerPrefs.SetInt("Use Vibration", (useVibration) ? 1 : 0);

        //save prefs
        PlayerPrefs.Save();
    }

    public void LoadPlayerPreferences() {
        if (SystemInfo.supportsVibration) {
            int vibrationInt = PlayerPrefs.GetInt("Use Vibration", 0);

            if (vibrationInt != 0 || vibrationInt != 1) {
                Debug.LogError("The \"Use Vibration\" setting is assigned an invalid value. Adjusting setting back to 0.");
                vibrationInt = 0;
                PlayerPrefs.SetInt("Use Vibration", 0);
            }

            useVibration = (vibrationInt == 1) ? true : false;
        }
    }

    public void SetUseVibration(bool useVibration) {
        this.useVibration = useVibration;
    }

    public bool GetUseVibration() {
        return useVibration;
    }

}
