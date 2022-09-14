using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JaredGoronkinPrototype2
{
    [RequireComponent(typeof(Collider))]
    public class Ship : MonoBehaviour, IPlayerInteractable, IPawn
    {

        public GameObject PlayerStatUI;
        public GameObject PlayerMainUI;
        public GameObject PlayerCombatUI;
        public GameObject Circle;
        public HashSet<Faction> FactionHasScanInfo;
        
        public Stats stats = new();
        private Vector3 MoveTarget;
        private Ship CombatTarget;
        public static List<Ship> Ships { get; private set; } = new();

        private void Awake()
        {
            Ships.Add(this);

        }
        


        private void OnDestroy()
        {
            Ships.Remove(this);
        }


        
        //Actions to be taken
        public Vector3 SetMoveTarget(Vector3 target)
        {
            target.y = 0;
            MoveTarget = Vector3.MoveTowards(transform.position, target, stats.warpRange);
            return MoveTarget;
        }
        IEnumerator Move()
        {
            Vector3 start = transform.position;

            float warpTime = 0.3f;
            float warpTimer = 0f;
            while (warpTimer <= warpTime)
            {

                transform.position = Vector3.Lerp(start, MoveTarget, warpTimer / warpTime);
                warpTimer += Time.deltaTime;
                yield return null;
            }
            transform.position = MoveTarget;
            yield return null;
            MoveTarget = transform.position;
        }


        

        public bool SetCombatTarget(Ship ship)
        {
            if(Vector3.Distance(ship.transform.position,transform.position)<= stats.weaponPower)
            {
                CombatTarget = ship;
                return true;
            }
            else
            {
                return false;
            }

        }
        public void StartCombat()
        {
            if (CombatTarget != null)
            {
                //Calculate combat stuff
            }
            else
            {
                //skip combat
            }
            CombatTarget = null;
        }


        //--------------------





        [System.Serializable]
        public struct Stats
        {
            public Faction faction;
            public float health;
            public float sheilds;
            public float weaponPower;
            public float weaponRange;
            public float thrusters;

            public bool scanners;
            public float scannerRange;

            public float warpRange;
        }

        private void Start()
        {
            MoveTarget = transform.position;
            stats.faction = PlayerFactionControl.myFaction;
            Debug.Log("Starting faction is " + stats.faction.Name);
            GameStatus.Instance.MainPhaseStart.AddListener(() => OnMainPhaseStart());
            GameStatus.Instance.MainPhaseEnd.AddListener(() => OnMainPhaseEnd());
            GameStatus.Instance.CombatPhaseStart.AddListener(() => OnCombatPhaseStart());
            GameStatus.Instance.CombatPhaseEnd.AddListener(() => OnCombatPhaseEnd());
        }
        //Player Input
        public void HoverStart(Faction faction)
        {
            PlayerStatUI.SetActive(true);
            Debug.Log("Hovering over " + gameObject.name);
        }
        
        public void HoverEnd(Faction faction)
        {
            PlayerStatUI.SetActive(false);
            Debug.Log("Stopped hovering over " + gameObject.name);
        }


        public void OpenInteractionMenu(Faction faction)
        {
            switch (GameStatus.Instance.GamePhase)
            {
                case (Phase.Main):
                    PlayerMainUI.SetActive(!PlayerMainUI.activeSelf);
                    break;
                case (Phase.Combat):
                    PlayerCombatUI.SetActive(!PlayerCombatUI.activeSelf);
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
        //------------

        //Game Phase Input
        public void OnMainPhaseStart()
        {

        }
        public void OnMainPhaseEnd()
        {
            PlayerMainUI.SetActive(false);

            StartCoroutine(Move());
        }
        public void OnCombatPhaseStart()
        {

        }
        public void OnCombatPhaseEnd()
        {
            PlayerCombatUI.SetActive(false);
            StartCombat();
            FactionHasScanInfo = new();
        }
        //-----------

 

    }
}