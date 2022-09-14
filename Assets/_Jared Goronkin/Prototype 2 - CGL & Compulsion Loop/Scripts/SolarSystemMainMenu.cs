using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JaredGoronkinPrototype2
{
    public class SolarSystemMainMenu : MonoBehaviour
    {
        public SolarSystem solarSystem;
        public HashSet<Faction> factionsInRange;

        public void BuildButton()
        {
            solarSystem.Settle(PlayerFactionControl.myFaction);
        }
        
    }
}