using System;
using FistVR;
using UnityEngine;
using H3MP.Networking;


namespace TeamsGameMode
{
    [Serializable]
    public class TGM_TeamDeathmatch : TGM_Gamemode
    {
        public TGM_TeamDeathmatch(string modeName = "", string modeDescription = "", Sprite modeThumbnail = null)
        {
            name = modeName;
            description = modeDescription;
            thumbnail = modeThumbnail;
        }

        public override TGM_Profile LoadDefaultProfile()
        {
            TGM_Profile profile = base.LoadDefaultProfile();
            //Do custom default settings here

            return profile;
        }

        public override void Setup()
        {
            base.Setup();
            //Defaults
            for (int i = 0; i < 2; i++)
            {
                TGM_Manager.instance.team[i].scoreGoal = 60;
            }
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
            RespawnTime();

            if (Networking.IsClient())
                return;

            //Check for Win Condition
            for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
            {
                TGM_Team team = TGM_Manager.instance.team[i];
                if (team.currentScore == team.scoreGoal)
                {
                    //Won
                    TGM_Manager.instance.SetGameState(TGM_Manager.GameStateEnum.Gameover);
                    return;
                }
            }    
        }

        public override void OnPlayerKilled(int playerIndex, int killerIFF)
        {
            base.OnPlayerKilled(playerIndex, killerIFF);
            TGM_Manager.instance.localPlayer.deaths++;
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
            if(s.GetDiedFromIFF() == 0 || s.GetDiedFromIFF() == 1)
                TGM_Manager.instance.team[s.GetDiedFromIFF()].currentKills++;
            int iff = s.GetIFF();
            AdjustTeamScore(iff, 1);
            TGM_Manager.instance.localPlayer.kills++;
        }
    }
}
