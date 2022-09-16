using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace JaredGoronkinPrototype2
{
    public class ShipCombatMenu : MonoBehaviour
    {

        public Ship ship;

        private void OnEnable()
        {
            if (ship.CombatTarget != null)
            {
                ship.CombatTarget.Circle.SetActive(true);
            }
        }
        private void OnDisable()
        {
            if (ship.CombatTarget != null)
            {
                ship.CombatTarget.Circle.SetActive(false);
            }
        }
        public void ChooseCombatTarget()
        {
            
            StartCoroutine(SelectShip());
        }
        IEnumerator SelectShip()
        {
            if (ship.CombatTarget != null)
            {
                ship.CombatTarget.Circle.SetActive(false);
                ship.CombatTarget = null;
            }
            PlayerFactionControl.lastClickableClicked = null;

            yield return new WaitUntil(() => PlayerFactionControl.lastClickableClicked != null);
            ship.SetCombatTarget(PlayerFactionControl.lastClickableClicked.GetComponent<Ship>());

            if (ship.CombatTarget != null)
            {
                ship.CombatTarget.Circle.SetActive(true);
            }
        }
    }
}