using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ThemeManager : MonoBehaviour
{
    public Theme[] Themes;
    public Image[] UIImages;
    //public SpriteRenderer Platform;
    private int index;
    private int maxIndex;
    private Color bgColor;
    
    [Serializable]
    public class Theme
    {
        public Color backgroundColor;
        //public Color PlatformColor;
    }

    public void Start()
    {
        index = 0;
        maxIndex = Themes.Length - 1;
    }

    public void NextTheme()
    {
        index += index == maxIndex? -maxIndex : 1;
        bgColor = Themes[index].backgroundColor;
        for (int i = 0; i < UIImages.Length; i++)
        {
            UIImages[i].DOColor(bgColor, 2f);
        }

        //Platform.color = Themes[index].PlatformColor;
    }
}