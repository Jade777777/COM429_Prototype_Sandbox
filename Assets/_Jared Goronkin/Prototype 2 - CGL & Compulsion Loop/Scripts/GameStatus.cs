using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace JaredGoronkinPrototype2
{
    public class GameStatus : MonoBehaviour
    {

        //Waits for the signal and steps the game forward

        public UnityEvent MainPhaseStart;
        public UnityEvent MainPhaseEnd;
        public UnityEvent CombatPhaseStart;
        public UnityEvent CombatPhaseEnd;
        
        public Phase GamePhase { get; private set; }

        public static GameStatus Instance { get; private set; }

        private void Awake()
        {
            if(Instance!= null)
            {
                Debug.LogError("There should only be 1 Game Status!");
            }
            Instance = this;
        }
        private void Start()
        {
            StartCoroutine(GameLoop());
        }


        
        private bool endPhase = false;
        public void EndPhase()
        {
            endPhase = true;
        }
        IEnumerator GameLoop()
        {
            yield return null;
            while (true) {
                GamePhase = Phase.Transition;
                CombatPhaseEnd.Invoke();
                Debug.Log("Starting " + GamePhase);
                yield return new WaitForSeconds(0.2f);


                endPhase = false;
                GamePhase = Phase.Main;
                MainPhaseStart.Invoke();
                Debug.Log("Starting " +GamePhase);



                yield return new WaitUntil(() => endPhase);
                GamePhase = Phase.Transition;
                MainPhaseEnd.Invoke();
                Debug.Log("Starting " + GamePhase);
                yield return new WaitForSeconds(0.2f);


                endPhase = false;
                GamePhase = Phase.Combat;
                CombatPhaseStart.Invoke();
                Debug.Log("Starting " +GamePhase);
                yield return new WaitUntil(() => endPhase);

            } }
    }
    public enum Phase { Main, Combat, Transition };


}
