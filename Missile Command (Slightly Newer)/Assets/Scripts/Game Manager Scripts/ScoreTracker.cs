using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// This class helps to keep track of the user's score and updates the necessary UI.
/// </summary>
public class ScoreTracker : MonoBehaviour {

     /// <summary>
     /// A representation of the player's performance in-game.
     /// </summary>
     [Header("Score")]
     public long playerScore;

     [Header("Score Text UI")]

     /// <summary>
     /// The maximum number of zeroes the score text UI should keep.
     /// </summary>
     [Tooltip("Maximum number of zeroes that the score text UI should keep")]
     public int numZeroes = 9;

     /// <summary>
     /// The GameObject that shows the UI for scores.
     /// </summary>
     public GameObject scoreTextUI;

     // Start is called before the first frame update
     private void Start() {
          UpdateScoreText();
     }

     private void UpdateScoreText() {

          //calculate the number of zeroes left after adding in the score text
          int remainingZeroes = numZeroes - playerScore.ToString().Length;

          string scoreText = "Score: ";

          //concatenate zeroes
          for (int i = 0; i < remainingZeroes; i++) {
               scoreText += "0";
          }

          //finally, concatenate the player's score
          scoreText += playerScore;

          //all these becomes our score text
          scoreTextUI.GetComponent<Text>().text = scoreText;
     }

     /// <summary>
     /// Adds the specified amount to our current score.
     /// </summary>
     /// <param name="score">The specified amount to add to our current score.</param>
     public void AddToScore(int score) {
          playerScore += score;

          //update score text
          UpdateScoreText();
     }
}
