using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CityManager : MonoBehaviour {

    public Transform cityParent;

    public List<City> cities;

    public GameStatusManager gameStatusManager;

    // Start is called before the first frame update
    private void Start() {

        cities = new List<City>(cityParent.childCount);

        for (int i = 0; i < cities.Capacity; i++) {
            cities.Add(cityParent.GetChild(i).GetComponent<City>());
        }

    }

    // Update is called once per frame
    private void Update() {

        if (AreAllCitiesDestroyed()) {

            //the game is over... tell the game manager it's time to pack up
            gameStatusManager.GameOver();

        }

    }

    public bool AreAllCitiesDestroyed() {
        foreach (City city in cities) {
            if (city.currentState != City.CityState.Destroyed) {
                return false;
            }
        }

        return true;
    }
}
