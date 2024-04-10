using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public Animator Platform;
    public Animator Buttons;
    public Animator ScoreAnim;
    public Animator PlayerAnim;
    public GameObject[] Coins;
    public Rotate[] RotateScripts;
    public Text ScoreText;
    public Text highScoreText;
    public Text lastScoreText;
    public RectTransform PlatformTrans;
    
    private int whichCoin;
    public bool gameIsRuning;
    public int score;
    private float timer;
    private float canTaketimer;
    private bool canPush = true;
    private bool canTake = true;
    private int multiplier = 1;
    
    private void Start()
    {
        highScoreText.text = "High Score: " + PlayerPrefs.GetInt("HighScore");
    }
    
    private void Update()
    {
        if (!canPush && timer < 1.1f)
            timer += Time.deltaTime;
        

        else if (!canPush && timer >= 1.1f)
        {
            canPush = true;
            timer = 0;
        }

        if (!canTake && canTaketimer < .8f)
            canTaketimer += Time.deltaTime;
        
        else if (!canTake && canTaketimer >= .8f)
        {
            canTake = true;
            canTaketimer = 0;
        }

        if (Input.GetKeyUp(KeyCode.Escape) && gameIsRuning)
            FinishGame();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.CompareTag("Coin"))
        {
            col.gameObject.SetActive(false);
            RandomizeCoin();
            if (canTake)
            {
                canTake = false;
                score += multiplier;
            }
            ScoreText.text = score.ToString();
        }
        else if (col.collider.CompareTag("Obs"))
        {
            FinishGame();
            if (score > PlayerPrefs.GetInt("HighScore"))
            {
                PlayerPrefs.SetInt("HighScore", score); 
                highScoreText.text = "High Score: " + score;
            }
        }
    }
    
    public void StartGame()
    {
        score = 0;
        ScoreText.text = "0";
        Platform.SetTrigger("Start");
        Buttons.SetTrigger("Start");
        ScoreAnim.SetTrigger("Start");
        RandomizeCoin();
        for (int i = 0; i < RotateScripts.Length; i++)
        { 
            RotateScripts[i].Randomize();
        }
        gameIsRuning = true;
        multiplier = 1;
    }

    public void Button()
    {
        if (canPush && gameIsRuning)
        {
            if (PlatformTrans.position.y == -1337.4f)
            {
                FinishGame();
            }
            else
            {
                canPush = false;
                int n = whichCoin + 1;
                PlayerAnim.SetTrigger("Coin" + n);
                if (score % 15 == 0 && score != 0)
                    multiplier++;
            }
        }
    }

    private void RandomizeCoin()
    {
        whichCoin = Random.Range(0, 4);
        for (int i = 0; i < 4; i++)
        {
            if (i != whichCoin) 
                Coins[i].SetActive(false);
            else if (i == whichCoin)
                Coins[i].SetActive(true);
        }
    }
    
    void FinishGame()
    {
        multiplier = 0;
        gameIsRuning = false;
        Platform.SetTrigger("Restart");
        Buttons.SetTrigger("Restart");
        ScoreAnim.SetTrigger("Restart");
            
        for (int i = 0; i < RotateScripts.Length; i++)
        {
            RotateScripts[i].ScoreToChange = -1;
            RotateScripts[i].CurrentScore = 0;
        }
        for (int i = 0; i < 4; i++)
        {
            Coins[i].SetActive(false);
        }
        lastScoreText.text = "Last Score: " + score;
    }
}
