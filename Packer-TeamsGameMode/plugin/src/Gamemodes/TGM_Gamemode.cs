using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        /// Called each Update frame
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

        public virtual void WinConditionCheck()
        { 

        }

        /// <summary>
        /// Method to adjust a teams score
        /// </summary>
        /// <param name="teamID"></param>
        /// <param name="amount"></param>
        public virtual void AdjustScore(int teamID, int amount)
        {
        }

        /// <summary>
        /// Hook into when a Sosig is killed
        /// </summary>
        /// <param name="s"></param>
        public virtual void OnSosigKilled(Sosig s)
        {

        }

        /// <summary>
        /// Hook into when a player is Killed
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <param name="killerIFF"></param>
        public virtual void OnPlayerKilled(int playerIndex, int killerIFF)
        {

        }

        public virtual Vector3 GetTeamRoomSpawnPoint(int team)
        {
            return Vector3.zero;
        }

        public virtual void GetLevelSpawnPoint(int team)
        {
            
        }
    }
}
