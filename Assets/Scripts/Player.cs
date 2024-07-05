using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using YG;

public class Player : MonoBehaviour
{
    #region publicVariables
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
    [Header("Continue Game")] 
    public GameObject continuePanel;
    public Button[] continuePanelButtons;
    public Image timerImg;
    public Text timerTxt;
    public GameObject authPanel;
    [Header("Audio")] 
    public Sprite musicOn;
    public Sprite musicOff;
    public AudioSource music;
    public Image musicImg;
    public Sprite sfxOn;
    public Sprite sfxOff;
    public Image sfxImg;
    public GameObject coinSfxPref;
    public GameObject obsSfxPref;
    public GameObject btnSfxPref;
    #endregion
    
    #region privateVariables
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
    private int continueCount = 2;
    private bool stopTimer;
    private bool isSecondAd;
    private bool gotsecondAd;
    private int speedStreak;
    private int lastSpeed = 1;
    private bool musicIsOn = true;
    private bool sfxIsOn = true;
    //private float deltaTime; //FPSCounter
    #endregion

    private void OnEnable() => YandexGame.RewardVideoEvent += Continue;
    private void OnDisable() => YandexGame.RewardVideoEvent -= Continue;
    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            YandexGame.SaveProgress();
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 60; //30FPS lock on Android problem solution
        if (!YandexGame.auth)
            authPanel.SetActive(true);
        TM = FindObjectOfType<ThemeManager>();
        if (YandexGame.lang == "en")
        {
            lastScoreText.text = "Last Score: 0";
            bestScoreText.text = "Best Score: " + YandexGame.savesData.bestScore;
        }
        else if (YandexGame.lang == "ru")
        {
            lastScoreText.text = "Последний Счёт: 0";
            bestScoreText.text = "Лучший Счёт: " + YandexGame.savesData.bestScore;
        }
        
        //TODO remove on release for yandex games
        canEndGameWithEscape = Application.platform != RuntimePlatform.WebGLPlayer;

        tutorialMode = !YandexGame.savesData.completedTutorial;
        
        if(tutorialMode)
            FindObjectOfType<Tutorial>().StartCoroutine(FindObjectOfType<Tutorial>().RunTutorial());
        musicIsOn = YandexGame.savesData.musicIsOn;
        sfxIsOn = YandexGame.savesData.sfxIsOn;
        musicImg.sprite = musicIsOn ? musicOn : musicOff;
        music.mute = !musicIsOn;
        sfxImg.sprite = sfxIsOn ? sfxOn : sfxOff;
    }
    
    void Update()
    {
        //FPSCounter
        /*deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        float fps = 1.0f / deltaTime;
        FPSTxt.text = string.Format("{0:0.} fps", fps);*/
        
        if (isSecondAd)
            YandexGame.CloseVideoEvent += getSecondAd;
        else
            YandexGame.CloseVideoEvent -= getSecondAd;
        
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
                if (score > 20)
                {
                    if (lastSpeed == temp)
                    {
                        if (speedStreak < 3)
                            speedStreak++;
                        else if (speedStreak == 3)
                        {
                            speedStreak = 0;
                            temp = temp == 1 ? 2 : 1;
                        }
                    }
                    else if (lastSpeed != temp)
                    {
                        speedStreak = 0;
                    }
                    lastSpeed = temp;
                }
                else if (score == 20)
                {
                    temp = 2;
                    lastSpeed = temp;
                }
                for (int i = 0; i < obstacles.Length; i++)
                {
                    if (score < 20)
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
    
    #region Auth

    public void CloseAuth()
    {
        authPanel.SetActive(false);
    }

    public void Auth()
    {
        authPanel.SetActive(false);
        YandexGame.AuthDialog();
    }

    #endregion
    #region UI
    
    //UI Play Button
    public void PlayButton()
    {
        if(sfxIsOn) 
            Instantiate(btnSfxPref);
        StartGame();
    }
    public void MButton()
    {
        //Main Button - The button that is responding when tapping on screen
        if (gameIsRunning && !animIsRunning && canPress && addedScore && coins[coinIndex].activeSelf && 
            FindObjectOfType<Tutorial>().condition != Tutorial.Condition.FiveFold)
        {
            if (tutorialMode &&
                FindObjectOfType<Tutorial>().condition is Tutorial.Condition.Tap or Tutorial.Condition.FiveFold ||
                !tutorialMode)
            {
                canPress = false;
                decreasedHealth = false;
                anim.SetTrigger("Coin" + (coinIndex + 1));
                addedScore = false;
            }
        }
    }
    public void MusicBtn()
    {
        if(sfxIsOn)
            Instantiate(btnSfxPref);
        musicIsOn = !musicIsOn;
        music.mute = !music.mute;
        musicImg.sprite = musicIsOn ? musicOn : musicOff;
        YandexGame.savesData.musicIsOn = musicIsOn;
    }

    public void SFXBtn()
    {
        if(sfxIsOn)
            Instantiate(btnSfxPref);
        sfxIsOn = !sfxIsOn;
        sfxImg.sprite = sfxIsOn ? sfxOn : sfxOff;
        YandexGame.savesData.sfxIsOn = sfxIsOn;
    }
    
    #endregion
    #region Mechanics
    
    private void StartGame()
    {
        if (canStart)
        {
            score = 0;
            health = 3;
            continueCount = 2;
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
            lastSpeed = 1;
            speedStreak = 0;
        }
    }

    private void EndGame(bool saveProgress)
    {
        continuePanel.SetActive(false);
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
        YandexGame.FullscreenShow();
        gameIsRunning = false;
        if (score > YandexGame.savesData.bestScore && saveProgress)
        {
            YandexGame.savesData.bestScore = score;
            if(YandexGame.auth)
                YandexGame.NewLeaderboardScores("LeaderBoard", score);
            YandexGame.SaveProgress();
        }
        if (YandexGame.lang == "en")
        {
            lastScoreText.text = "Last Score: " + score;
            bestScoreText.text = "Best Score: " + YandexGame.savesData.bestScore;
        }
        else if (YandexGame.lang == "ru")
        {
            lastScoreText.text = "Последний Счёт: " + score;
            bestScoreText.text = "Лучший Счёт: " + YandexGame.savesData.bestScore;
        }

        scoreText.text = "0";
        addedScore = true;
        for (int i = 0; i < obstacles.Length; i++)
            obstacles[i].speed = defaultSpeed;
        lastSpeed = 1;
        speedStreak = 0;
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

    private void OnTriggerEnter2D(Collider2D other)
    {
        //Collecting Coin
        if (other.CompareTag("Coin"))
        {
            if (!addedScore)
            {
                if (sfxIsOn)
                    Instantiate(coinSfxPref);
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
                if (sfxIsOn)
                    Instantiate(obsSfxPref);
                decreasedHealth = true;
                health--;
                if (health <= 0)
                {
                    if (continueCount > 0)
                    {
                        continuePanel.SetActive(true);
                        foreach (Button btn in continuePanelButtons)
                            btn.interactable = true;
                        StartCoroutine(RunTimer());
                        timerImg.fillAmount = 1;
                        timerTxt.text = "5";
                        continueCount--;
                        isSecondAd = false;
                    }
                    else
                    {
                        EndGame(true);
                    }
                }
                else
                {
                    hearts[health].sprite = emptyHeart;
                }
            }
        }
    }
    
    #endregion
    #region Ad&Continue
    
    private void Continue(int id)
    {
        isSecondAd = false;
        if (id == 1)
            health = 1;
        else if (id == 2)
            health = 3;
        continuePanel.SetActive(false);
        stopTimer = true;
    }
    
    public IEnumerator RunTimer()
    {
        stopTimer = false;
        timerImg.fillAmount = 1;
        int count = 5;
        while (count >= 0)
        {
            if (!stopTimer)
            {
                timerImg.DOFillAmount(.2f * count, 1f);
                timerTxt.text = count.ToString();
                yield return new WaitForSeconds(1);
                count--;
            }
            else yield break;
        }
        if(!stopTimer)
            EndGame(true);
    }

    public void CloseContinue()
    {
        EndGame(true);
        stopTimer = true;
    }

    public void getSecondAd()
    {
        if (!gotsecondAd)
        {
            YandexGame.RewVideoShow(2);
            gotsecondAd = true;
        }
    }
    
    public void ContinueBtn(bool x2)
    {
        foreach (Button btn in continuePanelButtons)
            btn.interactable = false;
        YandexGame.RewVideoShow(x2? 2 : 1);
        gotsecondAd = false;
        if (x2)
        {
            isSecondAd = true;
            foreach (Image img in hearts)
            {
                img.sprite = heart;
            }
        }
    }

    #endregion
    #region AnimTriggers
    
    public void RunAnim()
    {
        animIsRunning = true;
    }

    public void StopAnim()
    {
        animIsRunning = false;
        canPress = true;
    }
    
    #endregion
}