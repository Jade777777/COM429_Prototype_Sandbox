using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
namespace JaredGoronkinPrototype2
{
    [RequireComponent(typeof(Collider))]
    public class SolarSystem : MonoBehaviour, IPlayerInteractable, IPawn
    {

        public GameObject SystemStatsUI;
        public GameObject SystemMainUI;
        public GameObject SelectCircle;
        public DrawCircle BuildRange;
        public GameObject ShipPrefab;
        public int BuildCost= 100;
        public Stats stats = new();
        public TMP_Text SystemStats;
        public int resources;

        private void Start()
        {
            stats.faction = null;

            BuildRange.gameObject.SetActive(false);
            GameStatus.Instance.MainPhaseStart.AddListener(() => OnMainPhaseStart());
            GameStatus.Instance.MainPhaseEnd.AddListener(() => OnMainPhaseEnd());
            GameStatus.Instance.CombatPhaseStart.AddListener(() => OnCombatPhaseStart());
            GameStatus.Instance.CombatPhaseEnd.AddListener(() => OnCombatPhaseEnd());
        }


        //actions to be taken

        HashSet<Faction> factionsInRange;
        public void CheckSettleRange(Faction faction)
        {

            factionsInRange = new();
            Collider[] colliders = Physics.OverlapSphere(transform.position, BuildRange.radius);
            foreach (Collider c in colliders)
            {
                if (c.transform.TryGetComponent<Ship>(out Ship s))
                {
                    factionsInRange.Add(s.stats.faction);
                    Debug.LogFormat(PlayerFactionControl.myFaction.Name);
                    Debug.Log(s.stats.faction.Name);
                }
            }
            if (stats.faction == null && factionsInRange.Contains(faction) && factionsInRange.Count == 1)
            {
                BuildRange.color = Color.green;
                BuildRange.ReDraw();
                Debug.Log("in range");
            }
            else
            {
                BuildRange.color = Color.red;
                BuildRange.ReDraw();
                Debug.Log("Not in range");
            }

        }
        public bool Settle(Faction faction)
        {

            if (stats.faction == null && factionsInRange.Contains(faction) && factionsInRange.Count == 1)
            {
                Debug.Log(faction.Name + " is succesfuly settling!");
                stats.faction = faction;
                stats.health = 1f;
                stats.sheilds = 0f;
                return true;
            }
            return false;

        }

        public void BuildShip()
        {
            if (stats.faction != null && stats.faction.Resources >= BuildCost)
            {
                stats.faction.Resources -= BuildCost;
                GameObject ship = Instantiate(ShipPrefab);
                float distFromSystem = 1.3f;
                Vector3 offset = Quaternion.AngleAxis(Random.value * 360, Vector3.up) * Vector3.forward * distFromSystem;
                ship.transform.position = transform.position + offset;
                ship.GetComponent<Ship>().stats.faction = stats.faction;
            }
            else
            {
                Debug.Log("You don't have enough resources");
            }
        }
        [System.Serializable]
        public struct Stats
        {
            public Faction faction;
            public float health;
            public float sheilds;


        }
        private void Update()
        {
            if (stats.faction == null)
            {
                SystemStats.text =
                    "Faction: " + "Uninhabited" + "<br>" +
                    "Health: " + stats.health + "<br>" +
                    "Sheilds: " + stats.sheilds;
            }
            else
            {
                SystemStats.text =
                    "Faction: " + stats.faction.Name + "<br>" +
                    "Health: " + stats.health + "<br>" +
                    "Sheilds: " + stats.sheilds;
            }
        }



        //Player Input
        public void HoverStart(Faction faction)
        {
            if (faction == stats.faction)
            {
                SystemStatsUI.SetActive(true);
                Debug.Log("Hovering over " + gameObject.name);
            }
        }

        public void HoverEnd(Faction faction)
        {
            if (faction == stats.faction)
            {
                SystemStatsUI.SetActive(false);
                Debug.Log("Stopped hovering over " + gameObject.name);
            }
        }



        public void OpenInteractionMenu(Faction faction)
        {
            if (faction == stats.faction||stats.faction == null) { 
                switch (GameStatus.Instance.GamePhase)
                {
                    case (Phase.Main):
                        SystemMainUI.SetActive(!SystemMainUI.activeSelf);
                        if (stats.faction == null)
                        {
                            BuildRange.gameObject.SetActive(SystemMainUI.activeSelf); 
                        }
                        else
                        {
                            BuildRange.gameObject.SetActive(false);
                        }
                        CheckSettleRange(PlayerFactionControl.myFaction);
                        break;
                    case (Phase.Combat):
                        break;
                    case (Phase.Transition):
                        break;
                }
            Debug.Log("Opening Interaction menu for " + gameObject.name);
        
            } 
        }

        public void Select(Faction faction)
        {
            if (faction == stats.faction)
            {
                Debug.Log(faction.Name + " has selected " + gameObject.name);
            }
        }
        //-----------

        //Game Phase Input
        public void OnMainPhaseStart()
        {
            if (stats.faction != null)
            {
                stats.faction.Resources += 40;
            }
        }
        public void OnMainPhaseEnd()
        {
            SystemMainUI.SetActive(false);
            BuildRange.gameObject.SetActive(false);
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