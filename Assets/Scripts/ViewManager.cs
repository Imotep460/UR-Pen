using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Use ViewManager to help navigate the scenes in the project.
/// NOTE; ViewManager.cs MUST be added a a component on a GameObject in a scene for it to work.
/// </summary>
public class ViewManager : MonoBehaviour
{
    /// <summary>
    /// Load a given view
    /// NB; viewName is casesensitive, please write the name of the scene EXCATLY as it's written in the editor.
    /// </summary>
    /// <param name="viewName"></param>
    public void LoadView(string viewName)
    {
        SceneManager.LoadScene(viewName);
    }

    /// <summary>
    /// Quit/close the application.
    /// </summary>
    public void Quit()
    {
        Application.Quit();
    }

    /// <summary>
    /// Pause the game. Simply switch the Time.timeScale between 0.00f and 1.00f.
    /// </summary>
    public void TogglePause()
    {
        Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;
    }

    /// <summary>
    /// Call the SavingService.cs SaveGame method on the current active scene.
    /// </summary>
    public void saveGame()
    {
        // Save the game to a file called "SaveGame.json"
        SavingService.SaveGame("SaveGame.json");
    }

    /// <summary>
    /// Call the LoadGame() method from SavingService.cs
    /// Input fileName is case sensitive, make sure input written excatly like it is a Application.persistentDataPath.
    /// </summary>
    /// <param name="fileName"></param>
    public void loadGame()
    {
        SavingService.LoadGame("SaveGame.json");
    }
}
