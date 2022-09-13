using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JaredGoronkinPrototype2
{
    public class ShipMainMenu : MonoBehaviour, IPawn
    {
        public Ship ship;
        public GameObject target;
        GameObject targetInstance;


        public void SelectMoveTarget()
        {
            StartCoroutine(SelectPosition());
        }
        IEnumerator SelectPosition()
        {
            Destroy(targetInstance);
            targetInstance = Instantiate(target);

            
            LineRenderer lr = targetInstance.AddComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Legacy Shaders/Particles/Alpha Blended Premultiply"));
            lr.startColor = new Color32(0x4D,0xBC,0x68, 0xFF) ;
            lr.endColor = Color.black;
            lr.startWidth = 0.0f;
            lr.endWidth = 0.1f;
            lr.SetPosition(0, targetInstance.transform.position);
            lr.SetPosition(1, ship.transform.position);
            
        

            do
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                Plane hPlane = new(Vector3.up, Vector3.zero);
                hPlane.Raycast(ray, out float distance);
                Vector3 targetPosition = ray.GetPoint(distance);
                targetInstance.transform.position = ship.SetMoveTarget(targetPosition);
                lr.SetPosition(0, targetInstance.transform.position);
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
            Destroy(targetInstance);
        }

        public void OnMainPhaseStart()
        {

        }

    }
}