using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
namespace JaredGoronkinPrototype2
{
    public class PlayerFactionControl : MonoBehaviour
    {
        public static Faction myFaction;
        public static GameObject lastClickableClicked;
        IPlayerInteractable hoverTarget;
        public TMP_Text Resources;
        public float camSpeed = 5f;
        private void Awake()
        {
            myFaction = new Faction("Cognition Delegate");
        }
        private void Update()
        {
            Camera.main.transform.position+=(movement*Time.deltaTime*camSpeed);
            Resources.text = "Resources: " + myFaction.Resources;
            TryGetPlayerInteractableAtCursor(out IPlayerInteractable playerInteractable);
            if(hoverTarget != playerInteractable)
            {
                if (hoverTarget != null)
                {
                    hoverTarget.HoverEnd(myFaction);
                }

                hoverTarget = playerInteractable;

                if (hoverTarget != null)
                {
                    hoverTarget.HoverStart(myFaction);
                }
            }
        }
        int fNum = 0;
        public void OnCycleFactions(InputValue value)
        {
            Debug.Log("Player Changing factions");
            fNum++;
            myFaction = Faction.Factions[fNum % Faction.Factions.Count];
            fNum = Faction.Factions.IndexOf(myFaction);
        }
        Vector3 movement = Vector3.zero;
        public void OnMoveCamera(InputValue value)
        {
            movement = new Vector3(value.Get<Vector2>().x, 0, value.Get<Vector2>().y);
        }

        public void OnLeftClick(InputValue value)
        {
            lastClickableClicked = null;
            if ( TryGetPlayerInteractableAtCursor(out IPlayerInteractable playerInteractable))
            {
                playerInteractable.Select(myFaction);
                lastClickableClicked = (playerInteractable as MonoBehaviour).gameObject;
            }
        }
        public void OnRightClick(InputValue value)
        {
            lastClickableClicked = null;
            if (TryGetPlayerInteractableAtCursor(out IPlayerInteractable playerInteractable))
            {
                    playerInteractable.OpenInteractionMenu(myFaction);
            }
            
        }
        public void OnSpace(InputValue value)
        {

        }
        
        public void OnQuit(InputValue value)
        {
            Application.Quit();
            Debug.Log("Player pressed escape key. Quiting game.");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        private bool TryGetPlayerInteractableAtCursor(out IPlayerInteractable playerInteractable)
        {
            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 100)
                && hit.transform.gameObject.TryGetComponent(out playerInteractable))
            {
                return true;
            }
            playerInteractable = null;
            return false;
        }

    }
}