using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
    public void LoadView(string viewName)
    {
        SceneManager.LoadScene(viewName);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void saveGame()
    {
        // Save the game to a file called "SaveGame.json"
        SavingService.SaveGame("SaveGame");
    }
}
