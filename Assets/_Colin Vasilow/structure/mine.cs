using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mine : planetaryStructure
{
    public mine(GameObject parentPlanet) {
        parentSystem = parentPlanet;
    }

    public override string giveDescription(){
        return "mine";
    }
}