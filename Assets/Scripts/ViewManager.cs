using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ViewManager : MonoBehaviour
{
    //[SerializeField]
    //private string 
    //// Start is called before the first frame update
    //void Start()
    //{        
    //}

    public void LoadView(string viewName)
    {
        SceneManager.LoadScene(viewName);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
