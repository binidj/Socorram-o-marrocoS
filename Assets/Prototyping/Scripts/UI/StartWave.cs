using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class StartWave : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private GameObject disableOnWaveStart;
    [SerializeField] private GameObject activeOnWaveStart;
    public delegate void StartWaveEvent();
    public static event StartWaveEvent startWaveEvent;

    private void Start()
    {
        button.onClick.AddListener(TriggerEvent);
    }

    private void TriggerEvent()
    {
        startWaveEvent?.Invoke();
        disableOnWaveStart.SetActive(false);
        activeOnWaveStart.SetActive(true);
    }
}
