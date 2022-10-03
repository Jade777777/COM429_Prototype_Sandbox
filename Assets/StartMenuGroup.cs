using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuGroup : MonoBehaviour
{
   public void PlayGame(string scene)
   {
        scene = ("Assets/_Ryan Beckett/Scenes/GameOverMenu.unity");
        SceneManager.LoadScene(scene);
   }
}