using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Rotate : MonoBehaviour
{
    public float speed = 100;
    public int minScore = 3;
    public int maxScore = 10;
    public Player PlayerScript;
    public Vector3 axis = new Vector3(0f, 0f, 1f);
    
    public int CurrentScore;
    public bool CanChange;
    public int ScoreToChange;

    private void Start()
    {
        Randomize();
    }
    
    private void FixedUpdate()
    {
        transform.Rotate(axis * speed * Time.deltaTime);
    }

    public void Randomize()
    {
        CurrentScore = PlayerScript.score;
        ScoreToChange = CurrentScore + Random.Range(minScore, maxScore + 1);
        CanChange = true;
    }

    private void Update()
    {
        CurrentScore = PlayerScript.score;
        if (CanChange && PlayerScript.score == ScoreToChange)
        {
            axis = -axis;
            CanChange = false;
            Randomize();
        }
    }
}
