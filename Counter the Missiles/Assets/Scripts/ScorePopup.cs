using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePopup : MonoBehaviour {

    public float timeBeforeDisappearing;
    public float shrinkSpeed = 1.0f;

    public Animator scorePopupAnimator;

    public int score = 0;

    public TMPro.TextMeshPro scoreTextMesh;

    public float maxFontSize = 16;
    public float fontSizeIncrement = 0.2f;

    // Start is called before the first frame update
    private void Start() {
        scorePopupAnimator.SetFloat("timeBeforeDisappearingFreq", 1 / timeBeforeDisappearing);
        scorePopupAnimator.SetFloat("shrinkSpeed", shrinkSpeed);

        scorePopupAnimator.SetTrigger("popupScore");

        scoreTextMesh.text = score.ToString();
    }

    // Update is called once per frame
    private void Update() {

        AnimatorStateInfo currentAnimatorStateInfo = scorePopupAnimator.GetCurrentAnimatorStateInfo(0);

        if (currentAnimatorStateInfo.IsName("Shrink")) {
            if (currentAnimatorStateInfo.normalizedTime >= 1) {
                //destroy this object
                Destroy(gameObject);
            }
        }

    }

    public void PopupScore() {
        scorePopupAnimator.SetTrigger("popupScore");
        scoreTextMesh.text = score.ToString();

        scoreTextMesh.fontSize = Mathf.Clamp(scoreTextMesh.fontSize + fontSizeIncrement, 12, maxFontSize);
    }
}
