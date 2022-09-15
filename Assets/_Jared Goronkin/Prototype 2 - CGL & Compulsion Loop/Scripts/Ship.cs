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
            pCell = new();

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
                Debug.Log("I'VE GOT A TARGET TIME TO KILL!");
            }
            else
            {
                Debug.Log("I think I will pass, I don't condone violence.");
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

    
    public class PowerCell
    {
        GameObject pCell;

        Vector3 a = new Vector3(-0.5f, 0, 0);
        Vector3 b = new Vector3(0, Mathf.Sqrt(1.25f), 0);
        Vector3 c = new Vector3(0.5f, 0, 0);

        float weaponPower = 0;
        float sheildPower = 0;
        float thrusterPower = 0;

        Vector3 sliderPosition;


        Vector3[] newVertices;
        Vector2[] newUV;
        int[] newTriangles;

        public PowerCell()
        {
            pCell = new("Power Cell");
            pCell.AddComponent<MeshFilter>();
            pCell.AddComponent<MeshRenderer>();
            MeshCollider col = pCell.AddComponent<MeshCollider>();
            Mesh mesh = pCell.GetComponent<MeshFilter>().mesh;
            mesh.Clear();

          
            mesh.vertices = new Vector3[] { a, b, c };
            mesh.triangles = new int[] { 0, 1, 2 };


            col.sharedMesh = mesh;
        }

        public void SetSliderPosition()
        {
            RaycastHit hit;

            if (!Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
            {
                return;
            }

            MeshCollider meshCollider = hit.collider as MeshCollider;
            if (hit.collider == null||hit.collider.gameObject!= pCell)
            {
                return;
            }

            Vector3 baryCenter = hit.barycentricCoordinate;

            Debug.Log(hit.barycentricCoordinate);
        }


    }

}