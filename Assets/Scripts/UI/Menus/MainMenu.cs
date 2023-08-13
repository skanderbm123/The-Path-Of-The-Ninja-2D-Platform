using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   
    public void Continue()
    {
        //PlayerPrefs.GetString(Level selected)
        SceneManager.LoadScene("Level1_1");
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void LevelSelect()
    {
        // level selector
        
    }
}
