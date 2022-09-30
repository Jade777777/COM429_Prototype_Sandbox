using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planet : pawn
{
    //constants
    private const int distanceFromCenter = 1;
    public GameObject[] typesOfStructures;

    private GameObject[,] slots = {{null, null, null}, {null, null, null}, {null, null, null}};
    public bool generateStructure(int cordX, int cordY, int typesOfStructure) {
        if((cordX <= 2 && cordX >=0) && (cordX <= 2 && cordX >=0) && slots[cordX,cordY] == null) {
            GameObject newSettlement = Instantiate(typesOfStructures[typesOfStructure], transform);
            newSettlement.transform.position = new Vector3(transform.position.x + (cordX - 1), 0, this.transform.position.z + cordY - 1);
            slots[cordX,cordY] = newSettlement;
            newSettlement.GetComponent<planetaryStructure>().parentSystem = this.gameObject;
            return true;
        }
        else{return false;}
    }
}