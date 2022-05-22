using UnityEngine;

/// <summary>
/// This class allows for swapping of audio clips, with a fade-out and fade-in effet.
/// </summary>
public class SwapAudio : MonoBehaviour {

     public GameObject gameStatus;

     public AudioClip audioClip;
     private AudioSource audioSource;

     private bool swappedAudioClips = false;

     // Start is called before the first frame update
     private void Start() {
          audioSource = GetComponent<AudioSource>();
     }

     // Update is called once per frame
     private void Update() {

          if (gameStatus.GetComponent<GameStatus>().isGameOver) {

               if (!swappedAudioClips) {
                    //slowly decrease the volume
                    audioSource.volume -= 0.3f * Time.deltaTime;
               }

               //when done...
               if (audioSource.volume <= 0f && !swappedAudioClips) {

                    if (!swappedAudioClips) {
                         //swap out audio clips
                         audioSource.clip = audioClip;

                         //play audio clip
                         audioSource.Play();

                         swappedAudioClips = true;
                    }
               }

               if (swappedAudioClips) {
                    audioSource.volume += 0.3f * Time.deltaTime;
               }

          }

     }
}
