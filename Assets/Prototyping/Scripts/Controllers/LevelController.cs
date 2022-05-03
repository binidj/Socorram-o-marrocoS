using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prototyping.Scripts.Entities;

public class LevelController : MonoBehaviour
{
    public void GoToNextLevel()
    {
        Debug.Log("Call gonext");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadLevel()
    {
        Debug.Log("Call reload");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
