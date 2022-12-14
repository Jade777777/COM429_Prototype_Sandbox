using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace JaredGoronkinPrototype2
{
    
    [RequireComponent(typeof(Collider))]
    public class Ship : MonoBehaviour, IPlayerInteractable, IPawn
    {

        public GameObject PlayerStatUI;
        public TMP_Text PlayerStatText;
        public GameObject PlayerMainUI;
        public GameObject PlayerCombatUI;
        public GameObject Circle;
        public GameObject WeaponRange;

        public HashSet<Faction> FactionHasScanInfo;


        public Stats stats = new();
        private Vector3 MoveTarget;
        public Ship CombatTarget;

        public PowerCell pCell;
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
            MoveTarget = Vector3.MoveTowards(transform.position, target, stats.warpRange*pCell.thrusterPower);
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

            if (CombatTarget != null)
            {

                CombatTarget = null;
            }
            if (Vector3.Distance(ship.transform.position, transform.position) <= stats.weaponRange)
            {
                Debug.Log("Ship In Range");
                CombatTarget = ship;

                return true;
            }
            else
            {
                Debug.Log("failure");
                return false;
            }

        }
        public void StartCombat()
        {
            if (CombatTarget != null)
            {
                CombatTarget.DealDamage(stats.weaponPower * pCell.weaponPower);
                Debug.Log("I'VE GOT A TARGET TIME TO KILL!");
            }
            else
            {
                Debug.Log("I think I will pass, I don't condone violence.");
            }
            CombatTarget = null;
        }
        public void RegenSheilds()
        {
            stats.sheilds += stats.sheildRegen * pCell.sheildPower;
            stats.sheilds = Mathf.Clamp(stats.sheilds, 0, stats.maxSheilds);
            if (stats.health <= 0)
            {
                Destroy(gameObject);
            }
        }
        public void DealDamage(float damage)
        {
            stats.sheilds -= damage;
            if (stats.sheilds < 0)
            {
                stats.health += stats.sheilds;
                stats.sheilds = 0;
            }
            if (stats.health < 0)
            {
                Debug.Log("Uh oh...");
            }

        }

        //--------------------
        


    


        [System.Serializable]
        public struct Stats
        {
            public Faction faction;
            public float health;
            public float sheilds;
            public float maxSheilds;
            public float sheildRegen;
            public float weaponPower;
            public float weaponRange;

            public bool scanners;
            public float scannerRange;

            public float warpRange;
        }
        private void Update()
        {
            PlayerStatText.text =
                "Faction: " + stats.faction.Name + "\n" +
                "Health: " + stats.health + "\n" +
                "Sheilds: " + stats.sheilds + "\n" +
                "";
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
        }
        
        public void HoverEnd(Faction faction)
        {
            PlayerStatUI.SetActive(false);
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
        }

        public void Select(Faction faction)
        {
            Debug.Log(faction.Name + " has selected " + gameObject.name);
        }
        //------------

        //Game Phase Input
        public void OnMainPhaseStart()
        {
            RegenSheilds();
        }
        public void OnMainPhaseEnd()
        {
            PlayerMainUI.SetActive(false);

            StartCoroutine(Move());
        }
        public void OnCombatPhaseStart()
        {
            WeaponRange.SetActive(true);
            WeaponRange.GetComponent<DrawCircle>().radius = stats.weaponRange-0.6f;
            WeaponRange.GetComponent<DrawCircle>().ReDraw();
        }
        public void OnCombatPhaseEnd()
        {
            WeaponRange.SetActive(false);
            PlayerCombatUI.SetActive(false);
            StartCombat();
            FactionHasScanInfo = new();
        }
        //-----------

 

    }

}