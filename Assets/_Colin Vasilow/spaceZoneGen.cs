using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spaceZoneGen : MonoBehaviour
{
    //constants
    private const int sideLength = 3;
    //public variables
    public GameObject[] tiles;
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
