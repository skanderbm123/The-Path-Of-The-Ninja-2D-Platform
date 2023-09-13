using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelLoaderScript : MonoBehaviour
{
    public Animator transition;
    void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
        }
            //LoadNextLevel();  
    }

    public void LoadNextLevel()
    {
        StartCoroutine(LoadLevel(1));
    }

    IEnumerator LoadLevel(int levelIndex)
    {
        //Play Anim
        transition.SetTrigger("Start");
        // wait
        yield return new WaitForSeconds(2);
        //load scene

        SceneManager.LoadScene(1);
    }
}
