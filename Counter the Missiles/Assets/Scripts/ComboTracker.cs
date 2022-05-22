using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboTracker : MonoBehaviour {

    public int currentCombo = 0;

    public int currentScore = 0;

    public GameObject scorePopupTextPrefab;

    public GameObject scorePopupText;

    private List<GameObject> explosionList = new List<GameObject>();

    public GameStatusManager gameStatusManager;
    
    // Start is called before the first frame update
    private void Start() {

        gameObject.transform.parent = GameObject.Find("Combo Trackers Active").transform;

        gameStatusManager = GameObject.FindGameObjectWithTag("GameManager").transform.Find("Game Status Manager").GetComponent<GameStatusManager>();

    }

    // Update is called once per frame
    private void Update() {

        if (explosionList.Count <= 0) {
            Destroy(gameObject);
        }

    }

    public void AddToList(GameObject explosionRef) {
        explosionList.Add(explosionRef);
    }

    public void RemoveFromList(GameObject explosionRef) {
        explosionList.Remove(explosionRef);
    }

    public void IncrementCombo(Vector2 scorePopupPosition, int baseScore) {

        currentCombo++;

        currentScore += CalculateScore(baseScore, currentCombo);

        if (!scorePopupText) {
            //instantiate a popup text to fill the empty slot in
            scorePopupText = Instantiate(scorePopupTextPrefab, scorePopupPosition, Quaternion.identity);
        }

        //attach score
        scorePopupText.GetComponent<ScorePopup>().score = currentScore;

        //reset animation
        scorePopupText.GetComponent<ScorePopup>().PopupScore();

        //add to game score
        AddToGameScore(CalculateScore(baseScore, currentCombo));
    }

    public int CalculateScore(int baseScore, int combo) {
        return baseScore * combo;
    }

    public void AddToGameScore(int score) {
        gameStatusManager.score += score;
    }
}
