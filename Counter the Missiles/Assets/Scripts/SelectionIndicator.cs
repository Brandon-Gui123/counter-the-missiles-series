using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionIndicator : MonoBehaviour {

    public Animator selectionIndicatorAnimator;
    
    public void PlayAnimation() {
        selectionIndicatorAnimator.SetTrigger("playSelectionAnimation");
        selectionIndicatorAnimator.SetBool("isInterrupted", false);
    }

    public void InterruptAnimation() {
        selectionIndicatorAnimator.SetBool("isInterrupted", true);
    }
}
