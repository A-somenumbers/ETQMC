using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UIScript : MonoBehaviour
{
    //timePassed
    private float stopwatchTime;
    [SerializeField] private TMP_Text display;
    public static TimeSpan time;

    //enemyCounter
    [SerializeField] private TMP_Text enemyCounter;
    GameObject[] taggedObjects;
    
    //triple
    [SerializeField] GameObject[] ts;
    public GameObject tsUI;

    //doublespeed
    public GameObject dsUI;
    [SerializeField] private TMP_Text ds;
    public GameObject playerObject;
    public PlayerMovement reff;
    public static TimeSpan dtime;
    private float dsTimer;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stopwatchTime = 0f;
        tsUI.SetActive(false);
        dsUI.SetActive(false);
        reff = playerObject.GetComponent<PlayerMovement>();
        dsTimer  = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        dsTimer = playerObject.GetComponent<PlayerMovement>().getDashTime();
        ts = GameObject.FindGameObjectsWithTag("shot");
        if(ts.Length == 0)
        {
            tsUI.SetActive(true);
        }
        if (reff.SpeedUp)
        {
            dsUI.SetActive(true);
        } else
        {
            dsUI.SetActive(false);
        }

        if(Time.timeScale > 0f)
        {
            stopwatchTime += Time.deltaTime;
            enemiesLeft();
        }
        time = TimeSpan.FromSeconds(stopwatchTime);
        display.text = time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
        dtime = TimeSpan.FromSeconds(dsTimer);
        ds.text = dtime.Minutes.ToString() + ":" + dtime.Seconds.ToString() + ":" + dtime.Milliseconds.ToString();
    }

    void enemiesLeft()
    {
        taggedObjects = GameObject.FindGameObjectsWithTag("Enemy");
        int enemyCount = taggedObjects.Length-1;
        enemyCounter.text = "x " + enemyCount.ToString();


    }

    void itemTimer()
    {
        
    }
}
