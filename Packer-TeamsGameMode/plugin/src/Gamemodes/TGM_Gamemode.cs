using System;
using FistVR;
using UnityEngine;


namespace TeamsGameMode
{
    [Serializable]
    public class TGM_Gamemode
    {
        public string name;
        public string description;
        public Sprite thumbnail;

        /// <summary>
        /// Called when gamemode is selected
        /// </summary>
        public virtual void Setup()
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
            TGM_Teams.instance.teams[teamID].currentScore += amount;
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
    }
}
