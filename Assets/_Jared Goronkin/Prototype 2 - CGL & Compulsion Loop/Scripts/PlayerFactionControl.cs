using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JaredGoronkinPrototype2
{
    public class PlayerFactionControl : MonoBehaviour
    {
        Faction myFaction;
        IPlayerInteractable hoverTarget;
        private void Start()
        {
            myFaction = new Faction("Cognition Delegate");
        }

        private void Update()
        {
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

        public void OnLeftClick(InputValue value)
        {
            if( TryGetPlayerInteractableAtCursor(out IPlayerInteractable playerInteractable))
            {
                playerInteractable.Select(myFaction);
            }
        }
        public void OnRightClick(InputValue value)
        {
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