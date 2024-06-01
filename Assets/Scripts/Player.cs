using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public GameObject[] coins; //up down left right
    public Rotate[] obstacles;
    public Image[] hearts;
    public Sprite emptyHeart;
    public Sprite heart;
    [Header("Animators")]
    public Animator buttonsAnim;
    public Animator anim;
    public Animator scoreAnim;
    public Animator platformAnim;
    public Animator heartsAnim;
    [Header("UI Links")]
    public Text scoreText;
    public Text lastScoreText;
    public Text bestScoreText;
    //public Text FPSTxt; //FPSCounter
    [Header("Settings")] 
    [Range(1, 20)]
    public int themeScore = 5;
    [Range(1, 20)]
    public int directionScore = 3;
    [Range(1, 20)]
    public int speedScore = 5;
    public int speedToAdd = 10;
    public int defaultSpeed = 100;
    [Header("Tutorial")]
    public bool tutorialMode = true;
    
    private int coinIndex;
    private int score;
    private int health = 3;
    private int tempIndex;
    private bool gameIsRunning;
    private bool animIsRunning;
    private bool canPress;
    private bool addedScore = true;
    private bool canStart = true;
    private bool changedTheme;
    private bool changedDirection;
    private bool changedSpeed;
    private bool canEndGameWithEscape;
    private bool decreasedHealth;
    private ThemeManager TM;
    //private float deltaTime; //FPSCounter

    private void Start()
    {
        Application.targetFrameRate = 60; //30FPS lock on Android problem solution
        TM = FindObjectOfType<ThemeManager>();
        lastScoreText.text = "Last Score: 0";
        bestScoreText.text = "Best Score: " + PlayerPrefs.GetInt("Score");
        
        //TODO remove on release for yandex games
        canEndGameWithEscape = Application.platform != RuntimePlatform.WebGLPlayer;

        //tutorialMode = PlayerPrefs.GetInt("CompletedTutorial") == 0;
        
        if(tutorialMode)
            FindObjectOfType<Tutorial>().StartCoroutine(FindObjectOfType<Tutorial>().RunTutorial());
    }
    
    void Update()
    {
        //FPSCounter
        /*deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        FPSTxt.text = string.Format("{0:0.} fps", fps);*/
        
        if (gameIsRunning)
        {
            //Theme Changer
            if (score % themeScore == 0 && score > 0 && !changedTheme)
            {
                changedTheme = true;
                TM.NextTheme();
            }

            //Obstacle direction changer
            if (score % directionScore == 0 && score > 0 && !changedDirection && canPress)
            {
                changedDirection = true;
                Rotate firstObs = obstacles[Random.Range(0, obstacles.Length)];
                Rotate secondObs = obstacles[Random.Range(0, obstacles.Length)];
                while (firstObs == secondObs)
                    secondObs = obstacles[Random.Range(0, obstacles.Length)];
                firstObs.direction = -firstObs.direction;
                secondObs.direction = -secondObs.direction;
            }

            //Obstacle speed changer
            if (score % speedScore == 0 && score > 0 && !changedSpeed && canPress)
            {
                changedSpeed = true;
                int temp = Random.Range(1, 3);
                for (int i = 0; i < obstacles.Length; i++)
                {
                    if (score < 15)
                        obstacles[i].speed += speedToAdd;
                    else
                        obstacles[i].speed += temp == 1 ? speedToAdd : -speedToAdd;
                }
            }

            //TODO remove on release for yandex games
            if (Input.GetKeyUp(KeyCode.Escape) && canEndGameWithEscape)
            {
                EndGame(true);
            }
        }
    }

    public void MButton()
    {
        //Main Button - The button that is responding when tapping on screen
        if (gameIsRunning && !animIsRunning && canPress && addedScore && coins[coinIndex].activeSelf && 
            FindObjectOfType<Tutorial>().condition != Tutorial.Condition.FiveFold)
        {
            canPress = false;
            decreasedHealth = false;
            anim.SetTrigger("Coin" + (coinIndex + 1));
            addedScore = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Collecting Coin
        if (other.CompareTag("Coin"))
        {
            if (!addedScore)
            {
                score++;
                addedScore = true;
                other.gameObject.SetActive(false);
                scoreText.text = score.ToString();
                Randomize();
                changedTheme = false;
                changedDirection = false;
                changedSpeed = false;
            }
        }
        //Losing Game
        else if (other.CompareTag("Obs"))
        {
            if (!decreasedHealth)
            {
                decreasedHealth = true;
                health--;
                if (health <= 0)
                    EndGame(true);
                else
                {
                    hearts[health].sprite = emptyHeart;
                }
            }
        }
    }
    
    //UI Play Button
    public void PlayButton()
    {
        StartGame();
    }
    
    private void StartGame()
    {
        if (canStart)
        {
            score = 0;
            health = 3;
            decreasedHealth = false;
            gameIsRunning = true;
            canPress = true;
            Randomize();
            for (int i = 0; i < hearts.Length; i++)
                hearts[i].sprite = heart;
            buttonsAnim.SetBool("GameIsRunning", true);
            scoreAnim.SetBool("GameIsRunning", true);
            platformAnim.SetBool("GameIsRunning", true);
            heartsAnim.SetBool("GameIsRunning", true);
            canStart = false;
        }
    }

    private void EndGame(bool saveProgress)
    {
        canStart = true;
        for(int i = 0; i < coins.Length; i++) 
            coins[i].SetActive(false);
        buttonsAnim.SetBool("GameIsRunning", false);
        scoreAnim.SetBool("GameIsRunning", false);
        platformAnim.SetBool("GameIsRunning", false);
        heartsAnim.SetBool("GameIsRunning", false);
        obstacles[0].direction.z = 1;
        obstacles[1].direction.z = 1;
        obstacles[2].direction.z = -1;
        obstacles[3].direction.z = -1;
        for (int i = 0; i < obstacles.Length; i++)
        {
            obstacles[i].ResetPos();
        }
        gameIsRunning = false;
        lastScoreText.text = "Last Score: " + score;
        Debug.Log(score);
        if (score > PlayerPrefs.GetInt("Score") && saveProgress)
            PlayerPrefs.SetInt("Score", score);
        bestScoreText.text = "Best Score: " + PlayerPrefs.GetInt("Score");
        scoreText.text = "0";
        addedScore = true;
        for (int i = 0; i < obstacles.Length; i++)
            obstacles[i].speed = defaultSpeed;
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