using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

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
        _pressedMBtn = true;
    }

    public IEnumerator RunTutorial()
    {
        condition = Condition.PlayBtn;
        panel.SetActive(true);
        panel.GetComponent<Image>().DOFade(.6f, 1f);
        playBtnText.DOFade(1, 2f);
        playButton.DOColor(Color.white, 1f);
        buttonsAnim.SetTrigger("TutorialMode");
        while (!_pressedPlayBtn)
            yield return new WaitForSeconds(.1f);
        panel.GetComponent<Image>().DOFade(.4f, .5f);
        playBtnText.DOFade(0, .5f);
        yield return new WaitForSeconds(.5f);
        panel.GetComponent<Image>().DOFade(.6f, .5f);
        playButton.DOColor(FindObjectOfType<ThemeManager>().CurrentTheme.backgroundColor, 1f);
        tapText.DOFade(1, 1f);
        yield return new WaitForSeconds(1f);
        condition = Condition.Tap;
        while (!_pressedMBtn)
            yield return new WaitForSeconds(.1f);
        panel.GetComponent<Image>().DOFade(.4f, .5f);
        tapText.DOFade(0, .5f);
        yield return new WaitForSeconds(.5f);
        panel.GetComponent<Image>().DOFade(.6f, .5f);
        fiveFoldText.DOFade(1f, 1f);
        yield return new WaitForSeconds(1f);
        condition = Condition.FiveFold;
        _pressedMBtn = false;
        while (!_pressedMBtn)
            yield return new WaitForSeconds(.1f);
        panel.GetComponent<Image>().DOFade(0, 1f);
        fiveFoldText.DOFade(0, 1f);
        condition = Condition.Completed;
        yield return new WaitForSeconds(1f);
        panel.SetActive(false);
        FindObjectOfType<Player>().tutorialMode = false;
        PlayerPrefs.SetInt("CompletedTutorial", 1);
    }
}