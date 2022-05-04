using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototyping.Scripts.ScriptableObjects;
using UnityEngine.UI;
using Prototyping.Scripts.Controllers;

public class SetTrapCount : MonoBehaviour
{
    [SerializeField] private LevelConfig levelConfig;
    [SerializeField] private GameObject trap;
    Text countText;
    private int count = 0;
    
    private void OnEnable() 
    {
        TrapsController.updateButtonCount += UpdateCount;
    }

    private void OnDisable() 
    {
        TrapsController.updateButtonCount -= UpdateCount;
    }

    private void Start() 
    {
        countText = GetComponentInChildren<Text>();

        foreach (var trapData in levelConfig.trapsAvailable)
        {
            if (trapData.trap == trap)
            {
                count = trapData.limit;
                countText.text = count.ToString();
                break;
            }
        }
    }

    private void UpdateCount(GameObject trap)
    {
        if (trap == this.trap)
        {
            count -= 1;
            countText.text = count.ToString();
        }
    }
}
