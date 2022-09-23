using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JaredGoronkinPrototype2
{
    public class ShipMainMenu : MonoBehaviour, IPawn
    {
        public Ship ship;
        public GameObject target;
        public GameObject line;
        LineRenderer lr;

        private void Awake()
        {
            
            
            lr = line.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lr.startColor = new Color32(0x4D, 0xBC, 0x68, 0xFF);
            lr.endColor = Color.black;
            lr.startWidth = 0.0f;
            lr.endWidth = 0.1f;
            line.SetActive(false);
            target.SetActive(false);
        }
        public void SelectMoveTarget()
        {
            StartCoroutine(SelectPosition());
        }
        IEnumerator SelectPosition()
        {
    
            target.SetActive(true);
            line.SetActive(true);
            lr.SetPosition(0, target.transform.position);
            lr.SetPosition(1, ship.transform.position);
            
        

            do
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane hPlane = new(Vector3.up, Vector3.zero);
                hPlane.Raycast(ray, out float distance);
                Vector3 targetPosition = ray.GetPoint(distance);
                target.transform.position = ship.SetMoveTarget(targetPosition);
                lr.SetPosition(0, target.transform.position);
                yield return null;

            } while (!Input.GetMouseButtonUp(0));
            
        }



        private void Start()
        {
            GameStatus.Instance.MainPhaseStart.AddListener(() => OnMainPhaseStart());
            GameStatus.Instance.MainPhaseEnd.AddListener(() => OnMainPhaseEnd());
            GameStatus.Instance.CombatPhaseStart.AddListener(() => OnCombatPhaseStart());
            GameStatus.Instance.CombatPhaseEnd.AddListener(() => OnCombatPhaseEnd());
        }

        public void OnCombatPhaseEnd()
        {
            
        }

        public void OnCombatPhaseStart()
        {

        }

        public void OnMainPhaseEnd()
        {
            Debug.Log("destorying this thing $$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$$");
            target.SetActive(false);
            line.SetActive(false);
        }

        public void OnMainPhaseStart()
        {

        }

    }
}