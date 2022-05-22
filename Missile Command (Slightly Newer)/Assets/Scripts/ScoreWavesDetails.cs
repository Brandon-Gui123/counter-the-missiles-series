using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class used to display both the score details and wave details.
/// This is currently used in the game over screen.
/// </summary>
public class ScoreWavesDetails : MonoBehaviour {

     /// <summary>
     /// The main UI for displaying the score.
     /// Text from this UI will be transferred over to the UI in the game over screen.
     /// </summary>
     public GameObject scoreTextUI;

     /// <summary>
     /// The GameObject that generates enemy waves.
     /// Wave number from this GameObject will be transferred over.
     /// </summary>
     public GameObject enemyWaveGenerator;

     /// <summary>
     /// The UI for displaying the score when the game is over.
     /// </summary>
     public GameObject scoreText;

     /// <summary>
     /// The UI for displaying the number of waves survived by the player.
     /// </summary>
     public GameObject wavesText;

     // Start is called before the first frame update
     private void Start() {

          scoreText.GetComponent<Text>().text = scoreTextUI.GetComponent<Text>().text;
          wavesText.GetComponent<Text>().text = "Waves survived: " + (enemyWaveGenerator.GetComponent<EnemyWaveGenerator>().currentWave - 1);

     }
}
