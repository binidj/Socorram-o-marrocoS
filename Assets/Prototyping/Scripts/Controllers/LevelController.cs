using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Prototyping.Scripts.Entities;

public class LevelController : MonoBehaviour
{
    private void OnEnable() 
    {
        EnemyHealthManager.endLevelEvent += GoToNextLevel;
    }

    private void OnDisable()
    {
        EnemyHealthManager.endLevelEvent -= GoToNextLevel;
    }

    public void GoToNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
