using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace TeamsGameMode
{
    [Serializable]
    public class TGM_Player
    {
        [Header("Sosig")]
        public bool isSosig = false;    //is this a sosig

        [Header("Human")]
        public int playerIndex = 0; //H3MP data
        public int classIndex = 0; // Class ID

        [Header("Stats")]
        public int iff = 0; //Default team
        public int kills = 0;
        public int deaths = 0;
        public int killStreak = 0;  //Kills in a row before dying

        [Header("Data")]
        public List<FVRPhysicalObject> playersItems = new List<FVRPhysicalObject>();

        public void DestroyPlayersItems()
        {
            for (int i = 0; i < playersItems.Count; i++)
            {
                if (playersItems[i] != null)
                    UnityEngine.MonoBehaviour.Destroy(playersItems[i].gameObject);
            }
            playersItems.Clear();
        }
    }
}
