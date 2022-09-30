using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spaceZoneGen : MonoBehaviour
{
    //public variables
    public GameObject[] tiles;
    public GameObject[] typesOfStructures;
    //private variables
    private int numberOfToSq;

    void Start()
    {
        numberOfToSq = transform.childCount;
        foreach (Transform child in transform) {
            GameObject chosenTile = tiles[Random.Range(0, tiles.Length)];
            Instantiate(chosenTile, child.transform);
        }
    }

}
