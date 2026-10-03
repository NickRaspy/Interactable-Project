using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VRSimulation : MonoBehaviour
{
    [SerializeField] private bool simulateVR;
    [SerializeField] private GameObject vrSimulationObject;
    
    private void Awake()
    {
#if UNITY_EDITOR
        if (simulateVR)
        {
            Instantiate(vrSimulationObject);
        }
#endif
    }
}
