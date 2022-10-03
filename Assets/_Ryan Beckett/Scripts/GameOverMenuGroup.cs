using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverMenuGroup : MonoBehaviour
{
    public void ToStart(string scene)
    {
        scene = ("Assets/_Ryan Beckett/Scenes/StartMenu.unity");
        SceneManager.LoadScene(scene);
    }


   public void QuitGame()
   {
      Application.Quit();
      Debug.Log("Game is exiting");
   }
}
