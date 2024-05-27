using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public GameObject[] coins; //up down left right
    public Animator buttonsAnim;
    public Animator anim;
    public Animator scoreAnim;
    public Animator platformAnim;
    public Text Score;
    public Text LastScore;
    public Text BestScore;
    public Text FPSTxt;
    
    private bool gameIsRunning;
    private int coinIndex;
    private bool animIsRunning;
    private int score;
    private bool canPress;
    private Vector3 playerStartPos;
    private bool addedScore = true;
    private int tempIndex;
    private bool canStart = true;
    private bool changedTheme;
    private ThemeManager TM;
    //private float deltaTime; //FPSCounter

    private void Start()
    {
        playerStartPos = transform.position;
        Application.targetFrameRate = 60;
        TM = FindObjectOfType<ThemeManager>();
        
    }
    
    void Update()
    {
        //Debug.Log("gir: " + gameIsRunning + "; air: " + animIsRunning + "; cp: " + canPress + "; as: " + addedScore);
        /*deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        FPSTxt.text = string.Format("{0:0.} fps", fps);*/
        if (score % 5 == 0 && score > 0 && !changedTheme)
        {
            changedTheme = true;
            TM.NextTheme();
        }
    }

    public void MButton()
    {
        if (gameIsRunning && !animIsRunning && canPress && addedScore && coins[coinIndex].activeSelf)
        {
            canPress = false;
            animIsRunning = true; 
            anim.SetTrigger("Coin" + (coinIndex + 1));
            //transform.DOMove(coins[coinIndex].transform.position, .5f).SetEase(Ease.Linear);
            addedScore = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Coin"))
        {
            if (!addedScore)
            {
                //transform.DOMove(playerStartPos, .5f).SetEase(Ease.Linear).OnComplete(StopAnim);
                score++;
                other.gameObject.SetActive(false);
                //Debug.Log(coinIndex);
                Score.text = score.ToString();
                addedScore = true;
                /*DOTween.Sequence()
                    .AppendInterval(.5f)
                    .AppendCallback(Randomize);*/
                Randomize();
                changedTheme = false;
            }
        }
        else if (other.CompareTag("Obs"))
        {
            canStart = true;
            for(int i = 0; i < coins.Length; i++) 
                coins[i].SetActive(false);
            //transform.DOMove(playerStartPos, .4f).SetEase(Ease.Linear).OnComplete(StopAnim);
            buttonsAnim.SetBool("GameIsRunning", false);
            scoreAnim.SetBool("GameIsRunning", false);
            platformAnim.SetBool("GameIsRunning", false);
            gameIsRunning = false;
            LastScore.text = "Last Score: " + score;
            if (score > PlayerPrefs.GetInt("Score"))
                PlayerPrefs.SetInt("Score", score);
            BestScore.text = "Best Score: " + PlayerPrefs.GetInt("Score");
            score = 0;
            Score.text = "0";
            addedScore = true;
        }
    }
    
    public void PlayButton()
    {
        if (canStart)
        {
            if (!gameIsRunning) gameIsRunning = true;
            canPress = true;
            Randomize();
            buttonsAnim.SetBool("GameIsRunning", true);
            scoreAnim.SetBool("GameIsRunning", true);
            platformAnim.SetBool("GameIsRunning", true);
            canStart = false;
        }
    }

    public void fix()
    {
        Debug.Log(transform.position);
    }

    private void Randomize()
    {
        if (gameIsRunning)
        {
            tempIndex = Random.Range(0, 4);
            while (tempIndex == coinIndex)
                tempIndex = Random.Range(0, 4);
            coinIndex = tempIndex;
            for (int i = 0; i < coins.Length; i++)
                coins[i].SetActive(i == coinIndex);
        }
    }

    public void RunAnim()
    {
        animIsRunning = true;
    }

    public void StopAnim()
    {
        animIsRunning = false;
        canPress = true;
    }
}