using System;
using FistVR;
using UnityEngine;


namespace TeamsGameMode
{
    [Serializable]
    public class TGM_Gamemode
    {
        public TGM_Gamemode(string modeName = "", string modeDescription = "", Sprite modeThumbnail = null)
        {
            name = modeName;
            description = modeDescription;
            thumbnail = modeThumbnail;
        }

        public string name;
        public string description;
        public Sprite thumbnail;

        public virtual TGM_Profile LoadDefaultProfile()
        {
            TGM_Profile profile = new TGM_Profile();
            profile.gameSettings = new int[Enum.GetNames(typeof(SettingEnum)).Length];
            profile.gameSettings[(int)SettingEnum.SpawnLock] = 2;   //Per Class Spawn Locking
            profile.gameSettings[(int)SettingEnum.PlayerHealth] = -1;   //Per Class Health

            return profile;
        }

        /// <summary>
        /// Called when gamemode is selected
        /// </summary>
        public virtual void Setup()
        {

        }

        /// <summary>
        /// Called at the start of the pregame countdown
        /// </summary>
        public virtual void Pregame()
        {

        }

        /// <summary>
        /// Called at the start of the gameplay round
        /// </summary>
        public virtual void GameplayStart()
        {

        }

        /// <summary>
        /// Called at the start of the post game
        /// </summary>
        public virtual void Postgame()
        {

        }

        public virtual void GameOver()
        {

        }

        /// <summary>
        /// Called each Update frame while in Gameplay Game State
        /// </summary>
        public virtual void Update() 
        {
            
        }

        /// <summary>
        /// Before the gamemode is added to the selectible list, check if all its critria for it to work is met
        /// </summary>
        /// <returns></returns>
        public virtual bool IsGamemodeValid()
        {
            TeamGameModePlugin.Logger.LogMessage("Set Gamemode " + name + " to " + false);
            //Check if all required data is avalible for this gamemode to work
            return false;
        }

        /// <summary>
        /// Method to adjust a teams score
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="amount"></param>
        public virtual void AdjustTeamScore(int teamID, int amount)
        {
            TGM_Manager.instance.team[teamID].currentScore += amount;
        }

        public virtual void OnSosigCreate(Sosig s)
        {
            //Command Sosig here
        }

        /// <summary>
        /// Hook into when a Sosig is killed
        /// </summary>
        /// <param name="s"></param>
        public virtual void OnSosigKilled(Sosig s)
        {
            s.ClearSosig();
        }

        /// <summary>
        /// Hook into when a player is Killed
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <param name="killerIFF"></param>
        public virtual void OnPlayerKilled(int playerIndex, int killerIFF)
        {
            AdjustTeamScore(GM.CurrentPlayerBody.GetPlayerIFF(), 1);
        }

        /// <summary>
        /// A base respawning timer for each team
        /// </summary>
        public virtual void RespawnTime()
        {
            for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
            {
                if (Time.time >= TGM_Manager.instance.team[i].respawnTime)
                {
                    TGM_Manager.instance.team[i].Respawn();
                    TGM_Manager.instance.team[i].respawnTime = Time.time + TGM_Scene.instance.teams[i].teamSpawnTime;
                    Debug.Log("Respawn triggered at " + Time.time + " for team " + i);
                }

            }
        }
    }
}
