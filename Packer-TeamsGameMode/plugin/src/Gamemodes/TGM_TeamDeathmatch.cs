using System;
using FistVR;
using UnityEngine;
using H3MP.Networking;


namespace TeamsGameMode;

[Serializable]
public class TGM_TeamDeathmatch : TGM_Gamemode
{

    public TGM_TeamDeathmatch(string modeName = "", string modeDescription = "", Sprite modeThumbnail = null)
    {
        name = modeName;
        description = modeDescription;
        thumbnail = modeThumbnail;
    }

    public override void LoadDefaultProfile()
    {
        base.LoadDefaultProfile();
        //Do Gamemode Settings here

    }

    public override void Setup()
    {
        base.Setup();
        //Defaults
        for (int i = 0; i < 2; i++)
        {
            TGM_Manager.instance.team[i].scoreGoal = 20;
        }
    }

    public override void Pregame()
    {
        base.Pregame();
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
        //TDM always valid
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

        RespawnTime();

        //Check for Win Condition
        for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
        {
            TGM_Team team = TGM_Manager.instance.team[i];
            if (team.currentScore >= team.scoreGoal)
            {
                winIFF = team.iff;
                //Won
                TGM_Manager.instance.SetGameState(TGM_Manager.GameStateEnum.Postgame);
                return;
            }
        }    
    }

    public override void OnPlayerKilled(bool killedSelf, int iff)
    {
        base.OnPlayerKilled(killedSelf, iff);

        if (TGM_Manager.gameState != TGM_Manager.GameStateEnum.Gameplay)
            return;

        AdjustTeamScore(Global.GetEnemyIFF(GM.CurrentPlayerBody.GetPlayerIFF()), 1);

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
        if (markerPoint != null)
        {
            TGM_Compass.instance.CreateMarker(
                TGM_Compass.instance.markerSprites[(int)TGM_Compass.MarkerEnum.Attack],
                iff == 0 ? Color.red : Color.blue,
                markerPoint);
        }
    }

    public override void AdjustTeamScore(int teamID, int amount)
    {
        if (TGM_Manager.gameState != TGM_Manager.GameStateEnum.Gameplay)
            return;

        base.AdjustTeamScore(teamID, amount);

        //Increase player score
        if(teamID == GM.CurrentPlayerBody.GetPlayerIFF())
            TGM_Manager.instance.localPlayer.score += amount;
    }

    public override void OnSosigCreate(Sosig s)
    {
        base.OnSosigCreate(s);

        int enemyIFF = TGM_Sosigs.GetEnemyIFF(s.GetIFF());
        //GO attack other team!
        TGM_Sosigs.OrderSosigToLocations(s, TGM_Manager.instance.team[enemyIFF].currentSpawnArea.GetRandomAttackArea());
        
        //Defend Area
        //TGM_Sosigs.OrderSosigToLocations(s, TGM_Teams.GetTeam(s.GetIFF()).currentSpawnArea.GetRandomDefendArea());
    }

    public override void OnSosigKilled(Sosig s)
    {
        base.OnSosigKilled(s);

        //Score
        AdjustTeamScore(Global.GetEnemyIFF(s.GetIFF()), 1);

        //Remove from sosig team count
        for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
        {
            if (TGM_Manager.instance.team[i].sosigs.Contains(s))
                TGM_Manager.instance.team[i].sosigs.Remove(s);
        }
    }
}
