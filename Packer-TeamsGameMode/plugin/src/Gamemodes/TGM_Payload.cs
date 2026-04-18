using System;
using FistVR;
using UnityEngine;
using H3MP.Networking;
using System.Collections.Generic;


namespace TeamsGameMode;

[Serializable]
public class TGM_Payload: TGM_Gamemode
{
    List<Rush_CapturePoint> capturePoints = new List<Rush_CapturePoint>();
    int captureRatio = 4;  //1 in X will get the objective
    int redSpawnRatio = 0;

    public TGM_Payload(string modeName = "", string modeDescription = "", Sprite modeThumbnail = null)
    {
        name = modeName;
        description = modeDescription;
        thumbnail = modeThumbnail;
    }

    public override void LoadDefaultProfile()
    {
        base.LoadDefaultProfile();
        //Do Gamemode Settings here
        TGM_Settings.SetSetting(TGMSettingEnum.TimeLimit, 480);

    }

    public override void Setup()
    {
        base.Setup();
        //Defaults
        for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
        {
            if(TGM_Manager.instance.team[i].scoreGoal == -1)
                TGM_Manager.instance.team[i].scoreGoal = TGM_Scene.instance.areas.Length - 1;
        }
        TGM_MainMenu.instance.UpdateSettings();
    }

    public override void Pregame()
    {
        base.Pregame();

        //Assign all areas to Team BLU
        for (int i = 0; i < TGM_Scene.instance.areas.Length; i++)
        {
            TGM_Scene.instance.areas[i].iff = 1;
        }

        //Set Red Spawn
        TGM_Manager.instance.team[redIFF].currentSpawnArea.iff = redIFF;

        //Set BLU Spawn
        if(TGM_Scene.instance.areas[blueIFF] != TGM_Manager.instance.team[redIFF].currentSpawnArea)    //If Next spot is not owned by Red
            TGM_Manager.instance.team[blueIFF].currentSpawnArea = TGM_Scene.instance.areas[blueIFF];
        TGM_Manager.instance.team[blueIFF].currentSpawnArea.iff = blueIFF;


        //All Blu Areas get a Radio
        for (int i = 0; i < TGM_Scene.instance.areas.Length; i++)
        {
            if (TGM_Scene.instance.areas[i].iff == blueIFF)
            {
                Rush_CapturePoint capturePoint = TGM_Manager.Instantiate(
                    TGM_ModLoader.tgmAssets.rushCapturePointPrefab,
                    TGM_Scene.instance.areas[i].objective.position,
                    TGM_Scene.instance.areas[i].objective.rotation).GetComponent<Rush_CapturePoint>();

                capturePoints.Add(capturePoint);

                //Enable first capture Point
                if (TGM_Scene.instance.areas[i] == TGM_Manager.instance.team[blueIFF].currentSpawnArea)
                    capturePoint.canCapture = true;
            }
        }


        TGM_Scene.UpdateAllAreas();
    }

    public override void Postgame()
    {
        base.Postgame();

        int localIFF = GM.CurrentPlayerBody.GetPlayerIFF();

        if (localIFF == winIFF || localIFF < 0)
            TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.TeamWon);
        else
            TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.TeamLost);
    }

    public override void GameOver()
    {
        base.GameOver();
    }

    public override bool IsGamemodeValid()
    {
        TeamGameModePlugin.Logger.LogMessage(PluginInfo.NAME + "Set Gamemode " + name + " to " + true);

        //If any area does not have an objective we can
        for (int i = 0; i < TGM_Scene.instance.areas.Length; i++)
        {
            if (TGM_Scene.instance.areas[i].objective == null)
                return false;
        }

        //Checks passed
        return true;
    }

    public override void Update()
    {
        base.Update();

        if (Networking.IsClient())
            return;

        //Only update during gameplay
        if (TGM_Manager.gameState != TGM_Manager.GameStateEnum.Gameplay)
            return;


        //If hit our Timelimit
        if (TGM_Settings.GetSetting(TGMSettingEnum.TimeLimit) > 0)
        {
            if (Time.time - TGM_Manager.instance.startTime >= TGM_Settings.GetSetting(TGMSettingEnum.TimeLimit))
            {
                //Defending Team Wins (BLU)!
                winIFF = blueIFF;
                TGM_Manager.instance.team[blueIFF].currentScore = TGM_Manager.instance.team[blueIFF].scoreGoal;
                TGM_Manager.instance.SetGameState(TGM_Manager.GameStateEnum.Postgame);
            }
        }

        RespawnTime();

        /*
        //Check for Win Condition
        TGM_Team team = TGM_Manager.instance.team[redIFF];
        if (team.currentScore >= team.scoreGoal)
        {
            winIFF = team.iff;
            TGM_Manager.instance.SetGameState(TGM_Manager.GameStateEnum.Postgame);
            return;
        }
        */
    }

    public override void OnPlayerKilled(bool killedSelf, int iff)
    {
        base.OnPlayerKilled(killedSelf, iff);

        if (TGM_Manager.gameState != TGM_Manager.GameStateEnum.Gameplay)
            return;


        //Player Stats
        TGM_Manager.instance.localPlayer.deaths++;
        TGM_Manager.instance.localPlayer.score += killedSelf ? -1 : 1;  //Minus score if killed self
    }

    public override void OnJoinTeam(int iff)
    {
        base.OnJoinTeam(iff);

        int enemyIFF = TGM_Sosigs.GetEnemyIFF(GM.CurrentPlayerBody.GetPlayerIFF());

        Transform markerPoint =
            (TGM_Manager.instance.team[enemyIFF].currentSpawnArea.capturePoint != null) ?
            TGM_Manager.instance.team[enemyIFF].currentSpawnArea.capturePoint :
            TGM_Manager.instance.team[enemyIFF].currentSpawnArea.objective;

        //Give player some direction to the combat area
        /*
        if (markerPoint != null)
        {
            TGM_Compass.instance.CreateMarker(
                TGM_Compass.instance.markerSprites[(int)TGM_Compass.MarkerEnum.Attack],
                iff == redIFF ? Color.red : Color.blue,
                markerPoint);
        }
        */
    }

    public override void AdjustTeamScore(int teamIFF, int amount)
    {
        if (TGM_Manager.gameState != TGM_Manager.GameStateEnum.Gameplay)
            return;

        TGM_Manager.instance.team[teamIFF].currentScore += amount;

        //Increase player score
        if (teamIFF == GM.CurrentPlayerBody.GetPlayerIFF())
            TGM_Manager.instance.localPlayer.score += amount;

        //Score hit the goal, they win!
        if (TGM_Manager.instance.team[teamIFF].currentScore >= TGM_Manager.instance.team[teamIFF].scoreGoal)
        {
            winIFF = teamIFF;
            TGM_Manager.instance.SetGameState(TGM_Manager.GameStateEnum.Postgame);
            return;
        }


        //Next Capture Point!
        if (teamIFF == redIFF)
        {
            //Audio Capture
            int localIFF = GM.CurrentPlayerBody.GetPlayerIFF();

            //If Red or Spectator
            if (localIFF <= redIFF)
                TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.ObjectiveFriendlyCaptured);
            else
                TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.ObjectiveEnemyCaptured);

            //Score
            int score = TGM_Manager.instance.team[redIFF].currentScore;

            //Set All capture points (For Networking reasons)
            for (int i = 0; i < capturePoints.Count; i++)
            {
                capturePoints[i].canCapture = false;
            }

            if (score < capturePoints.Count)
            {
                Debug.Log("CAPTURED!!!");
                capturePoints[score + 1].canCapture = true;

                TGM_Manager.instance.team[redIFF].currentSpawnArea = TGM_Manager.instance.team[blueIFF].currentSpawnArea;
                TGM_Manager.instance.team[blueIFF].currentSpawnArea = TGM_Scene.instance.areas[score + 1];
            }
        }
    }

    public override void OnSosigCreate(Sosig s)
    {
        base.OnSosigCreate(s);

        int iff = s.GetIFF();

        if (iff == blueIFF)
        {
            //Defenders
            TGM_Sosigs.OrderSosigToLocations(s, TGM_Manager.instance.team[iff].currentSpawnArea.GetRandomDefendArea());
        }
        else
        {
            //ATTACKERS
            int enemyIFF = TGM_Sosigs.GetEnemyIFF(s.GetIFF());
            if (redSpawnRatio++ >= captureRatio)
            {
                //Get on the objective!
                TGM_Sosigs.OrderSosigToLocations(s, TGM_Manager.instance.team[enemyIFF].currentSpawnArea.GetObjectiveArea());
                redSpawnRatio = 0;
            }
            else
            {
                //Move to attack positions
                TGM_Sosigs.OrderSosigToLocations(s, TGM_Manager.instance.team[enemyIFF].currentSpawnArea.GetRandomAttackArea());
            }
        }
    }

    public override void OnSosigKilled(Sosig s)
    {
        base.OnSosigKilled(s);

        //Remove from sosig team count
        for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
        {
            if (TGM_Manager.instance.team[i].sosigs.Contains(s))
                TGM_Manager.instance.team[i].sosigs.Remove(s);
        }
    }
}