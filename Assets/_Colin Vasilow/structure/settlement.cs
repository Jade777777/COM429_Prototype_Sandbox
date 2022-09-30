using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settlement : planetaryStructure
{
    public settlement(GameObject parentPlanet) {
        parentSystem = parentPlanet;
    }

    public override string giveDescription(){
        return "settlement";
    }
}
