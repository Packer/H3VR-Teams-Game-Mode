using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using H3MP.Networking;
using FistVR;
using System;

namespace TeamsGameMode;

public class TGM_EndScreen : MonoBehaviour
{
    public static TGM_EndScreen instance;

    public GameObject endScreen;
    public LayerMask hitLayers;
    public float screenDistance = 3;
    private RaycastHit rayHit;

    [Header("UI")]
    public Text[] endScores;
    public Text resultText;
    public Image resultBackground;
    public Color[] teamColors;
    public Text killText;
    public Text deathText;
    public Text timeText;
    public Text playerScoreText;

    void Awake()
    {
        //Destroy old end screen
        if (instance != null)
            Destroy(instance.gameObject);

        instance = this;

        //Disable Screen until we need it
        SetEndScreen(false);
    }

    public void SetEndScreen(bool state)
    {
        endScreen.SetActive(state);

        if (state == false)
            return;

        //Winner
        if (TGM_Manager.instance.gamemode.winIFF == -1)
            resultText.text = "DRAW";
        else
            resultText.text = TGM_Manager.instance.gamemode.winIFF == GM.CurrentPlayerBody.GetPlayerIFF() ? "VICTORY" : "DEFEAT";

        resultBackground.color = teamColors[TGM_Manager.instance.gamemode.winIFF];

        //Display Stats
        killText.text = TGM_Manager.instance.localPlayer.kills.ToString();
        deathText.text = TGM_Manager.instance.localPlayer.deaths.ToString();
        playerScoreText.text = TGM_Manager.instance.localPlayer.score.ToString();

        //Display Scores
        for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
        {
            endScores[i].text = TGM_Manager.instance.team[i].currentScore.ToString();
        }

        TimeSpan time = TimeSpan.FromSeconds(Time.time - TGM_Manager.instance.startTime);
        timeText.text = time.Minutes + ":" + time.Seconds;
    }

    void FixedUpdate()
    {
        if (!endScreen.activeSelf)
            return;

        //Only show in Post game
        if (TGM_Manager.gameState != TGM_Manager.GameStateEnum.Postgame)
            endScreen.SetActive(false);

        if (Physics.Linecast(
            GM.CurrentPlayerBody.Head.position,
            GM.CurrentPlayerBody.Head.position + GM.CurrentPlayerBody.Head.forward * screenDistance,
            out rayHit,
            hitLayers))
        {
            endScreen.transform.position = rayHit.point;
        }
        else
        {
            endScreen.transform.position = GM.CurrentPlayerBody.Head.position + GM.CurrentPlayerBody.Head.forward * screenDistance;
        }


    }
}
