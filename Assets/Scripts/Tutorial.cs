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
    public Condition condition;
    private bool _pressedPlayBtn;
    private bool _pressedMBtn;
    public Button[] buttons;

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
        foreach (Button button in buttons)
            button.interactable = false;
        yield return new WaitForSeconds(1f);
        while (!_pressedPlayBtn)
        {
            if (!FindObjectOfType<Player>().tutorialMode)
            {
                condition = Condition.Completed;
                panel.SetActive(false);
                panel.GetComponent<Image>().DOFade(0f, .5f);
                playBtnText.DOFade(0, .5f);
                playButton.DOColor(FindObjectOfType<ThemeManager>().CurrentTheme.backgroundColor, .5f);
                foreach (Button button in buttons)
                    button.interactable = true;
                yield return new WaitForSeconds(.5f);
                yield break;
            }
            yield return new WaitForSeconds(.1f);
        }
        foreach (Button button in buttons)
            button.interactable = true;
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