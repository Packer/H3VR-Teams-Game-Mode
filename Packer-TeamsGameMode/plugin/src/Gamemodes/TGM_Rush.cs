using System;
using FistVR;
using UnityEngine;
using H3MP.Networking;
using System.Collections.Generic;


namespace TeamsGameMode;

[Serializable]
public class TGM_Rush : TGM_Gamemode
{
    List<Rush_CapturePoint> capturePoints = new List<Rush_CapturePoint>();
    int captureRatio = 2;  //1 in X will go to attack positions (Rest goes to Objective)
    int redSpawnRatio = 0;

    public TGM_Rush(string modeName = "", string modeDescription = "", Sprite modeThumbnail = null)
    {
        name = modeName;
        description = modeDescription;
        thumbnail = modeThumbnail;
    }

    public override void LoadDefaultProfile()
    {
        base.LoadDefaultProfile();
        //Do Gamemode Settings here
        TGM_Settings.SetSetting(TGMSettingEnum.TimeLimit, 720);
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

        //Hide Objective UI
        for (int i = 0; i < TGM_TeamSetup.instance.teamObjectiveAdjust.Length; i++)
        {
            TGM_TeamSetup.instance.teamObjectiveAdjust[i].SetActive(false);
        }
    }

    public override void Pregame()
    {
        base.Pregame();

        //Blue instantly spawns:
        TGM_Scene.instance.teams[blueIFF].teamSpawnTime = 1;

        //Area Setup
        SetupAreas();

        //Clear old Radios
        for (int i = 0; i < capturePoints.Count; i++)
        {
            if (capturePoints[i] != null)
            {
                if (capturePoints[i].spawnedPrefab != null)
                    TGM_Manager.Destroy(capturePoints[i].spawnedPrefab);
                TGM_Manager.Destroy(capturePoints[i].gameObject);
            }
        }
        capturePoints.Clear();

        //All Blu Areas get a Radio
        for (int i = 0; i < TGM_Scene.instance.areas.Length; i++)
        {
            if (TGM_Scene.instance.areas[i].iff == blueIFF)
            {
                GameObject prefab = TGM_Scene.instance.rushCapturePrefab;

                Rush_CapturePoint capturePoint = TGM_Manager.Instantiate(
                    prefab,
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

    public override void GameplayStart()
    {
        //Set spawn time back to the same as Red
        TGM_Scene.instance.teams[blueIFF].teamSpawnTime = TGM_Scene.instance.teams[redIFF].teamSpawnTime;
    }

    public override void Postgame()
    {
        base.Postgame();

        int localIFF = GM.CurrentPlayerBody.GetPlayerIFF();

        if (winIFF == -1)
            TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.TeamDraw);
        else if (localIFF == winIFF || localIFF < 0)
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

        //Do we have a prefab for Rush Radios
        if (TGM_Scene.instance.rushCapturePrefab == null)
            return false;

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

        float remainTime = Time.time - TGM_Manager.instance.startTime;
        TimeSpan time = TimeSpan.FromSeconds(remainTime);

        TGM_Compass.instance.gameTimeText.text = time.Minutes + ":" + (time.Seconds < 10 ? "0" + time.Seconds : time.Seconds);

        if (Networking.IsClient())
            return;

        //Only Blue gets Respawn in Pregame
        if (TGM_Manager.gameState == TGM_Manager.GameStateEnum.Pregame)
        {
            if (Time.time >= TGM_Manager.instance.team[blueIFF].respawnTime)
            {
                TeamGameModePlugin.Logger.LogDebug("Gamemode: RespawnTime at: " + Time.time + " for team " + blueIFF);
                TGM_Manager.instance.team[blueIFF].Respawn();
                TGM_Manager.instance.team[blueIFF].respawnTime = Time.time + TGM_Scene.instance.teams[blueIFF].teamSpawnTime;
            }
        }

        //Only update during gameplay
        if (TGM_Manager.gameState != TGM_Manager.GameStateEnum.Gameplay)
            return;

        //If hit our Timelimit
        if (TGM_Settings.GetSetting(TGMSettingEnum.TimeLimit) > 0)
        {
            if (Time.time - TGM_Manager.instance.startTime >= TGM_Settings.GetSetting(TGMSettingEnum.TimeLimit))
            {
                AdjustTeamScore(redIFF, -TGM_Manager.instance.team[redIFF].currentScore);
                AdjustTeamScore(blueIFF, TGM_Manager.instance.team[blueIFF].scoreGoal);
                winIFF = blueIFF;
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
        if (iff != redIFF && iff != blueIFF)
            return;

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

        /*
        Transform markerPoint =
            (TGM_Manager.instance.team[enemyIFF].currentSpawnArea.capturePoint != null) ?
            TGM_Manager.instance.team[enemyIFF].currentSpawnArea.capturePoint :
            TGM_Manager.instance.team[enemyIFF].currentSpawnArea.objective;
        */
        
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
        if (TGM_Manager.gameState != TGM_Manager.GameStateEnum.Gameplay || teamIFF == -1)
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

        //IF RED SCORED
        if (teamIFF == redIFF)
        {
            //Debug.Log("RED " + "A");
            //Audio Capture
            int localIFF = GM.CurrentPlayerBody.GetPlayerIFF();

            //Debug.Log("RED " + localIFF);
            //If Red or Spectator
            if (amount > 0)
            {
                if (localIFF <= redIFF)
                    TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.ObjectiveFriendlyCaptured);
                else
                    TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.ObjectiveEnemyCaptured);
            }

            //Score
            int redScore = TGM_Manager.instance.team[redIFF].currentScore;
            int blueArea = redScore + 1;
            //Debug.Log("RED score " + redScore);
            //Debug.Log("RED blue: " + blueArea);

            //Set All capture points (For Networking reasons)
            for (int i = 0; i < capturePoints.Count; i++)
            {
                //Debug.Log("RED cap " + i);
                capturePoints[i].canCapture = false;
            }

            if (blueArea <= capturePoints.Count)
            {
                //Debug.Log("CAPTURED!!! " + redScore);
                //Update Spawns
                TGM_Manager.instance.team[redIFF].currentSpawnArea = TGM_Scene.instance.areas[redScore];
                TGM_Manager.instance.team[redIFF].currentSpawnArea.iff = redIFF;
                TGM_Manager.instance.team[blueIFF].currentSpawnArea = TGM_Scene.instance.areas[blueArea];
                TGM_Manager.instance.team[blueIFF].currentSpawnArea.iff = blueIFF;

                //Captures at 1 behind blue Area
                capturePoints[redScore].canCapture = true;

                TGM_Scene.UpdateAllAreas();

                if(blueArea < capturePoints.Count)
                    SetSosigOrders();
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

            //If in pregame, get sosigs to GUARD their positions
            if (TGM_Manager.gameState == TGM_Manager.GameStateEnum.Pregame)
            {
                s.CurrentOrder = Sosig.SosigOrder.GuardPoint;
                s.FallbackOrder = Sosig.SosigOrder.GuardPoint;
                s.SetCurrentOrder(Sosig.SosigOrder.GuardPoint);
            }
        }
        else
        {
            //ATTACKERS
            int enemyIFF = TGM_Sosigs.GetEnemyIFF(s.GetIFF());
            if (redSpawnRatio++ >= captureRatio)
            {
                //Move to attack positions
                TGM_Sosigs.OrderSosigToLocations(s, TGM_Manager.instance.team[enemyIFF].currentSpawnArea.GetRandomAttackArea());
                redSpawnRatio = 0;
            }
            else
            {
                //Get on the objective!
                TGM_Sosigs.OrderSosigToLocations(s, TGM_Manager.instance.team[enemyIFF].currentSpawnArea.GetObjectiveArea());
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

    void SetSosigOrders()
    {
        for (int iff = 0; iff < TGM_Manager.instance.team.Length; iff++)
        {
            for (int x = 0; x < TGM_Manager.instance.team[iff].sosigs.Count; x++)
            {
                if (TGM_Manager.instance.team[iff].sosigs == null)
                    continue;
                Sosig s = TGM_Manager.instance.team[iff].sosigs[x];

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
                        //Move to attack positions
                        TGM_Sosigs.OrderSosigToLocations(s, TGM_Manager.instance.team[enemyIFF].currentSpawnArea.GetRandomAttackArea());
                        redSpawnRatio = 0;
                    }
                    else
                    {
                        //Get on the objective!
                        TGM_Sosigs.OrderSosigToLocations(s, TGM_Manager.instance.team[enemyIFF].currentSpawnArea.GetObjectiveArea());
                    }
                }
            }
        }
    }

    void SetupAreas()
    {
        //Assign all areas to Team Blue
        for (int i = 0; i < TGM_Scene.instance.areas.Length; i++)
        {
            TGM_Scene.instance.areas[i].iff = blueIFF;
        }

        //Set Red Spawn
        TGM_Manager.instance.team[redIFF].currentSpawnArea = TGM_Scene.instance.teams[redIFF].startSpawnArea;
        TGM_Manager.instance.team[redIFF].currentSpawnArea.iff = redIFF;

        //Set BLU Spawn
        if (TGM_Scene.instance.areas[1] != TGM_Manager.instance.team[redIFF].currentSpawnArea)    //If Next spot is not owned by Red
            TGM_Manager.instance.team[blueIFF].currentSpawnArea = TGM_Scene.instance.areas[1]; //2nd Area belongs to Blue
        TGM_Manager.instance.team[blueIFF].currentSpawnArea.iff = blueIFF;
    }
}