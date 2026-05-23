using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MakeScrollViewAutoScroll : MonoBehaviour
{
    public ScrollRect scrollRect;
    public float scrollSpeed = 20f;
    public TMP_Text text;
    public GameObject toHide;
    public void Update()
    {
        toHide.SetActive(false);
        text.alignment = TextAlignmentOptions.Center;
        text.text = "\n\nGame Logo Design: \nNial George\n\nLead Designer: \nRudra Raghava\n\nProgramming: \nRudra Raghava\n\nUI and Menu Design: \nRudra Raghava\n\nSoundtrack Previewers: \nArjun Reddy\nKarthik Kalyanaraman\n\nGame Name: \nAditya -\n\nLevel Design: \nRudra Raghava\n\nSound Design: \nRudra Raghava\n\nMusic: \nRudra Raghava\n\nBeta Testers: \nJaya Raghava\nNial George\nRishab Agnihotri\nKaeshava Kalyanaraman\n\nPlaytesters:\n-\n\nPublisher:\nSpark 'n Spin Studios\n\nTools and Software Used:\nUnity\nCakewalk by Bandlab\nVisual Studio Code\nCanva\n The COMPLETE Undertale Soundfont (Mr_Incognito)\nThe COMPLETE DELTARUNE Soundfont (Mr_Incognito)\nDeltarune Chapter 2 Sounfont (Could not find credit)\n\nSpecial Thanks: \nRaghava KK\nSonia Lewis\nAnaga Raghava\nJaya Raghava\nAadya Raghava\nYou, the Player!\n\nTHANK YOU FOR PLAYING!";
        float newPos = scrollRect.verticalNormalizedPosition - (scrollSpeed / scrollRect.content.rect.height) * Time.deltaTime;
        scrollRect.verticalNormalizedPosition = Mathf.Clamp01(newPos);
    }
}
