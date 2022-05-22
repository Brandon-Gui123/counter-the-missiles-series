using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasScaling : MonoBehaviour {

    public Camera mainCameraComponent;

    public Canvas canvas;

    public bool sizeCanvasWithScreen = true;

    public float unitsWide;
    public float unitsTall;

    public RectTransform rectTransform;

    // Start is called before the first frame update
    private void Start() {
        if (sizeCanvasWithScreen) {

        }
    }

    // Update is called once per frame
    private void Update() {

    }

    // This function is called when the script is loaded or a value is changed in the inspector (Called in the editor only)
    private void OnValidate() {
        if (sizeCanvasWithScreen) {

            Rect rect = new Rect(0, 0, mainCameraComponent.scaledPixelWidth, mainCameraComponent.scaledPixelHeight);
            

        } else {
            Vector2 scale = ScaleCanvas(unitsWide, unitsTall);
            transform.localScale = scale;
        }
    }

    private Vector2 ScaleCanvas(float width, float height) {
        Rect rect = rectTransform.rect;
        return new Vector2(width / rect.width, height / rect.height);
    }
}
