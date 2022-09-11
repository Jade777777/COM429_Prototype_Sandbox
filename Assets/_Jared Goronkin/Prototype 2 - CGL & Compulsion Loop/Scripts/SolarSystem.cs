using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JaredGoronkinPrototype2
{
    [RequireComponent(typeof(Collider))]
    public class SolarSystem : MonoBehaviour, IPlayerInteractable
    {
        public void HoverEnd(Faction faction)
        {
            Debug.Log("Stopped hovering over " + gameObject.name);
        }

        public void HoverStart(Faction faction)
        {
            Debug.Log("Hovering over " + gameObject.name);
        }

        public void OpenInteractionMenu(Faction faction)
        {
            Debug.Log("Opening Interaction menu for " + gameObject.name);
        }

        public void Select(Faction faction)
        {
            Debug.Log(faction.Name + " has selected " + gameObject.name);
        }


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}