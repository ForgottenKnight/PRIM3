using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRunePosition : MonoBehaviour {

    public SimpleEvent runeEventSpawn;
    public GameObject rune;
    public Damageable[] possibleLocations;

	// Use this for initialization
	void Start () {
        int i = Random.Range(0, possibleLocations.Length);
        runeEventSpawn.enemiesList[0] = possibleLocations[i];

        Vector3 runePos = rune.transform.localPosition;
        Vector3 objPos = possibleLocations[i].transform.localPosition;

        runePos.x = objPos.x;
        runePos.y = objPos.y;

        rune.transform.localPosition = runePos;

        Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
