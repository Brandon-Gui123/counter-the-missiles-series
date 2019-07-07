using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameStatusManager : MonoBehaviour {

    /// <summary>
    /// The current player's score.
    /// </summary>
    public int score;

    public TMPro.TextMeshProUGUI scoreUI;

    public TMPro.TextMeshProUGUI finalScoreUI;

    public bool timeScaleSet = false;

    public AudioSource standardSound;

    public AudioSource gameOverSound;

    public bool gameOverSoundPlaying = false;

    public GameObject gameOverScreen;
    public GameObject blackOverlay;
    private Image blackOverlayImage;
    private float opaqueness;

    public MissileLauncherManager missileLauncherManager;

    public bool isGameOver;

    #region Parents

    [Header("Parents")]
    public Transform missilesActiveParent;
    public Transform comboTrackersActiveParent;


    #endregion

    // Start is called just before any of the Update methods is called the first time
    private void Start() {
        blackOverlayImage = blackOverlay.GetComponent<Image>();
    }


    // Update is called once per frame
    private void Update() {
        scoreUI.text = score.ToString();
        finalScoreUI.text = score.ToString();
    }

    public void GameOver() {

        isGameOver = true;

        missileLauncherManager.StopLauncherFire();

        //wait till all combos, player missiles and player-caused explosions to finish
        if (PlayerMissilesQuantity(missilesActiveParent) <= 0 && comboTrackersActiveParent.childCount <= 0) {
            if (!timeScaleSet) {
                Time.timeScale = 1.0f;
                timeScaleSet = true;
            }

            Time.timeScale = Mathf.Clamp(Time.timeScale - (0.2f * Time.unscaledDeltaTime), 0, 1);

            standardSound.volume -= 0.2f * Time.unscaledDeltaTime;
            gameOverSound.volume = Mathf.Clamp(gameOverSound.volume + 0.2f * Time.unscaledDeltaTime, 0, 0.7f);

            if (!gameOverSoundPlaying) {
                gameOverSound.Play();
                gameOverSoundPlaying = true;
            }

            gameOverScreen.SetActive(true);

            opaqueness += 1f * Time.unscaledDeltaTime;
            blackOverlayImage.color = new Color(0, 0, 0, opaqueness);
        }
    }

    public int PlayerMissilesQuantity(Transform missilesActiveParent) {
        int quantity = 0;

        for (int i = 0; i < missilesActiveParent.childCount; i++) {
            if (missilesActiveParent.GetChild(i).GetComponent<Missile>().isPlayerMissile) {
                quantity++;
            }
        }

        return quantity;
    }
}
