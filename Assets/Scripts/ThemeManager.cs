using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public Theme[] Themes;
    public Image[] UIImages;
    public Theme CurrentTheme;
    //public SpriteRenderer Platform;
    private int index;
    private int maxIndex;
    private Color bgColor;
    
    [Serializable]
    public class Theme
    {
        public Color backgroundColor;
    }

    public void Start()
    {
        index = 0;
        maxIndex = Themes.Length - 1;
        bgColor = Themes[0].backgroundColor;
        CurrentTheme = Themes[index];
        for (int i = 0; i < UIImages.Length; i++)
        {
            UIImages[i].color = bgColor;
        }
    }

    public void NextTheme()
    {
        index += index == maxIndex? -maxIndex : 1;
        bgColor = Themes[index].backgroundColor;
        CurrentTheme = Themes[index];
        for (int i = 0; i < UIImages.Length; i++)
        {
            UIImages[i].DOColor(bgColor, 2f);
        }
    }
}