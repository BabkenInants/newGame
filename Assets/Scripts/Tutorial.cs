using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using YG;

public class Tutorial : MonoBehaviour
{
    public GameObject panel;
    public Text playBtnText;
    public Text tapText;
    public Text fiveFoldText;
    public Image playButton;
    public Animator buttonsAnim;
    public Condition condition;
    private bool _pressedPlayBtn;
    private bool _pressedMBtn;

    public enum Condition
    {
        PlayBtn, Tap, FiveFold, Completed
    }

    public void PlayBtnT()
    {
        _pressedPlayBtn = true;
    }

    public void MainBtn()
    {
        if(condition is Condition.Tap or Condition.FiveFold)
            _pressedMBtn = true;
    }
    
    public IEnumerator RunTutorial()
    {
        condition = Condition.PlayBtn;
        panel.SetActive(true);
        panel.GetComponent<Image>().DOFade(.6f, .5f);
        playBtnText.DOFade(1, 1f);
        playButton.DOColor(Color.white, .5f);
        buttonsAnim.SetTrigger("TutorialMode");
        yield return new WaitForSeconds(1f);
        while (!_pressedPlayBtn)
            yield return new WaitForSeconds(.1f);
        panel.GetComponent<Image>().DOFade(.4f, .25f);
        playBtnText.DOFade(0, .25f);
        yield return new WaitForSeconds(.25f);
        panel.GetComponent<Image>().DOFade(.6f, .25f);
        playButton.DOColor(FindObjectOfType<ThemeManager>().CurrentTheme.backgroundColor, .5f);
        tapText.DOFade(1, .5f);
        yield return new WaitForSeconds(.5f);
        condition = Condition.Tap;
        while (!_pressedMBtn)
            yield return new WaitForSeconds(.1f);
        panel.GetComponent<Image>().DOFade(.4f, .25f);
        tapText.DOFade(0, .25f);
        yield return new WaitForSeconds(.25f);
        panel.GetComponent<Image>().DOFade(.6f, .25f);
        fiveFoldText.DOFade(1f, .5f);
        yield return new WaitForSeconds(.5f);
        condition = Condition.FiveFold;
        _pressedMBtn = false;
        while (!_pressedMBtn)
            yield return new WaitForSeconds(.1f);
        panel.GetComponent<Image>().DOFade(0, .5f);
        fiveFoldText.DOFade(0, .5f);
        condition = Condition.Completed;
        yield return new WaitForSeconds(.5f);
        panel.SetActive(false);
        FindObjectOfType<Player>().tutorialMode = false;
        YandexGame.savesData.completedTutorial = true;
    }
}