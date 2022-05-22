using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsToggle : MonoBehaviour {

    [SerializeField]
    private Toggle vibrationToggle;

    // Start is called just before Update is called
    private void Start() {
        vibrationToggle.isOn = PlayerPreferences.preferences.GetUseVibration();
    }
    
    public void SetVibration(bool useVibration) {
        PlayerPreferences.preferences.SetUseVibration(useVibration);
    }

}
