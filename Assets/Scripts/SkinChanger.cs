using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinChanger : MonoBehaviour
{
    public Player PlayerScript;
    public Image Background;
    public Image PlayButton;
    public Sprite[] Backgrounds;
    public Sprite[] PlayButtons;
    private int queue = 0;
    private int skinNum;
    private int lastChange;

    private void Start()
    {
        skinNum = Backgrounds.Length;
        Debug.Log("skinNum: " + skinNum);
    }

    private void Update()
    {
        if (PlayerScript.score % 5 == 0 && PlayerScript.score != 0 && PlayerScript.gameIsRuning)
        {
            if ( PlayerScript.score != lastChange)
            {
                ChangeSkin();
                lastChange = PlayerScript.score;
            }
        }

        if (!PlayerScript.gameIsRuning || PlayerScript.score == 0)
        {
            lastChange = 0;
        }
        
        Debug.Log("Queue: " + queue);
        Debug.Log("LastChange: " + lastChange);
    }

    private void ChangeSkin()
    {
        if (queue < skinNum - 1)
        {
            queue++;
        }
        else if (queue == skinNum - 1)
        {
            queue = 0;
        }

        Background.sprite = Backgrounds[queue];
        PlayButton.sprite = PlayButtons[queue];
    }
}
