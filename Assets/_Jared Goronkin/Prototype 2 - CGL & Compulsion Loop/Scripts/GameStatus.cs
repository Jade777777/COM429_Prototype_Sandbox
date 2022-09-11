using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JaredGoronkinPrototype2
{
    public class GameStatus : MonoBehaviour
    {
        /// <summary>
        /// Waits for the signal and steps the game forward
        /// consists of an enum for the current phase, as well 
        /// </summary>
        enum Phase { Main, Combat };
        private void Start()
        {
            StartCoroutine(GameLoop());
        }

        IEnumerator GameLoop()
        {
            yield return null;
        }
    }

    
}
