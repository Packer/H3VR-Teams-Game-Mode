using System;
using FistVR;
using UnityEngine;
using H3MP.Networking;


namespace TeamsGameMode
{
    [Serializable]
    public class TGM_TeamDeathmatch : TGM_Gamemode
    {
        public TGM_Area[] teamAreas;

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

            int iff = s.GetIFF();

            AdjustTeamScore(iff, 1);
        }
    }
}
