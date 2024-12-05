using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;


namespace TeamsGameMode
{
    public class TGM_Button : MonoBehaviour
    {
        public Text text;
        public Button button;
        public int index = -1;

        public void SelectClass(int index)
        {
            TGM_ClassMenu.instance.SpawnClass(index);
        }

        public void LaunchGame()
        {
            
        }

        public void ChooseGamemode(int index)
        {
            TGM_MainMenu.instance.SelectGamemode(index);
        }

        public void JoinTeam(int index)
        {
            GM.CurrentPlayerBody.SetPlayerIFF(index);

            //Teleport to team spawn room
            GM.CurrentMovementManager.TeleportToPoint(POSITIONGOHERE,
                true,
                SR_Manager.instance.supplyPoints[lastID].respawn.forward);
        }
    }
}
