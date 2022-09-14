using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JaredGoronkinPrototype2
{
    public class ShipCombatMenu : MonoBehaviour
    {

        public Ship ship;
        public void ChooseCombatTarget()
        {

            Debug.Log("Choose combat target");
            StartCoroutine(SelectShip());


        }
        IEnumerator SelectShip()
        {
            yield return null;


            ship.SetCombatTarget(null);
        }
    }
}