using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class universeGeneration : MonoBehaviour
{
    //constants
    //sizes refer to the width of each of the squares
    private const int ZONE_SIZE = 4, SQUARE_SIZE = 3;
    //public variables
    public int universeLength = 2, universeWidth = 2;
    //private variables
    public GameObject[] zones;
    private int zonesGenerated;


    void Start()
    {
        
    }

    public void GenerateUniverse(){

        //clean slate for demonstration /*
        foreach (Transform child in transform) {
            Destroy(child.gameObject    );
        }

        //generate universe
        for (int i = 0; i < universeLength; i++) {
            for (int j = 0; j < universeWidth; j++) {

                //generate each space zone
                for (int k = 0; k < ZONE_SIZE; k++) {
                    for (int l = 0; l < ZONE_SIZE; l++) {
                        Vector3 zonePosition = new Vector3((j * ZONE_SIZE) * SQUARE_SIZE + l * SQUARE_SIZE, 0, (i * ZONE_SIZE) * SQUARE_SIZE + k * SQUARE_SIZE);
                        GameObject newZone = Instantiate(zones[Random.Range(0, zones.Length)]);
                        if (newZone.tag == "System") {
                            newZone.GetComponent<planet>().generateStructure(Random.Range(0,3), Random.Range(0,3), Random.Range(0,2));
                        }
                        newZone.transform.position = zonePosition;
                        newZone.transform.SetParent(transform);
                        newZone.name = ("Space Zone " + (zonesGenerated) + ": (" + k + "," + l + ")");
                    }
                }
                zonesGenerated++;
            }
        }
    }

}
