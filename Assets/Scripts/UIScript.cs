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
    
    [SerializeField] private TMP_Text enemyCounter;
    //enemyCounter
    GameObject[] taggedObjects;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stopwatchTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale > 0f)
        {
            stopwatchTime += Time.deltaTime;
            enemiesLeft();
        }
        time = TimeSpan.FromSeconds(stopwatchTime);
        display.text = time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
    }

    void enemiesLeft()
    {
        taggedObjects = GameObject.FindGameObjectsWithTag("Enemy");
        int enemyCount = taggedObjects.Length-1;
        enemyCounter.text = "x " + enemyCount.ToString();


    }
}
