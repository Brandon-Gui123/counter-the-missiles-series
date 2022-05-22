using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A serializable class containing information on the enemy wave, such as how many missiles, the speed multiplier of the missiles
/// and the time per missile spawned.
/// </summary>
[System.Serializable]
public class EnemyWave {

     /// <summary>
     /// A list of missile GameObjects that help depict the type of missiles that will spawn in this
     /// wave and how many of each type are there to spawn.
     /// </summary>
     public List<GameObject> missiles;

     /// <summary>
     /// The speed multiplier in this wave. 
     /// Missiles use speed multipliers like this to determine their speed.
     /// Higher speed multiplier means faster-moving missiles.
     /// </summary>
     public float missileSpeedMultiplier = 1.0f;

     /// <summary>
     /// The duration between each missile spawn this wave.
     /// Lower value means more spawns over the specified period of time.
     /// </summary>
     public float timePerMissileSpawn = 1.75f;


}
