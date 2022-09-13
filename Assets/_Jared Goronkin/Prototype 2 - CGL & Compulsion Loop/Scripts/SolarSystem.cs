using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JaredGoronkinPrototype2
{
    [RequireComponent(typeof(Collider))]
    public class SolarSystem : MonoBehaviour, IPlayerInteractable, IPawn
    {

        public GameObject SystemStatsUI;
        public GameObject SystemMainUI;
        public GameObject Circle;
        private void Start()
        {
            GameStatus.Instance.MainPhaseStart.AddListener(() => OnMainPhaseStart());
            GameStatus.Instance.MainPhaseEnd.AddListener(() => OnMainPhaseEnd());
            GameStatus.Instance.CombatPhaseStart.AddListener(() => OnCombatPhaseStart());
            GameStatus.Instance.CombatPhaseEnd.AddListener(() => OnCombatPhaseEnd());
        }


        //actions to be taken
        public void Build(Faction faction)
        {
            //check if faction is in range andif no other faction is in range
        }

        struct Stats
        {
            public Faction faction;
            public float health;
            public float sheilds;
            public float weaponPower;
            public float weaponRange;

            public float thrusters;



            public bool scanners;
            public float scannerRange;
        }




        //Player Input
        public void HoverStart(Faction faction)
        {
            SystemStatsUI.SetActive(true);
            Debug.Log("Hovering over " + gameObject.name);
        }

        public void HoverEnd(Faction faction)
        {
            SystemStatsUI.SetActive(false);
            Debug.Log("Stopped hovering over " + gameObject.name);
        }



        public void OpenInteractionMenu(Faction faction)
        {
            switch (GameStatus.Instance.GamePhase)
            {
                case (Phase.Main):
                    SystemMainUI.SetActive(!SystemMainUI.activeSelf);
                    break;
                case (Phase.Combat):
                    break;
                case (Phase.Transition):
                    break;
            }
            Debug.Log("Opening Interaction menu for " + gameObject.name);
        }

        public void Select(Faction faction)
        {
            Debug.Log(faction.Name + " has selected " + gameObject.name);
        }
        //-----------
        //Game Phase Input
        public void OnMainPhaseStart()
        {

        }
        public void OnMainPhaseEnd()
        {
            SystemMainUI.SetActive(false);
        }
        public void OnCombatPhaseStart()
        {

        }
        public void OnCombatPhaseEnd()
        {

        }
        //-----------
    }
}