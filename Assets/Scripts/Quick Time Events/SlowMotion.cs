using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    public float timeScale;

    private float startTimeScale;
    private float startFixedDeltaTime;


    void Start()
    {
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;

        
    }

    
    void Update()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartSlowMotion();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            EndSlowMotion();
        }
    }

    public void StartSlowMotion() 
    {
        Time.timeScale = timeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * timeScale;
    }

    public void EndSlowMotion()
    {
        Time.timeScale = startTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime;
    }
}
