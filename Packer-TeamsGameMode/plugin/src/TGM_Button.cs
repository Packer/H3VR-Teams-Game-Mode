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
        public Text[] texts;
        public Button[] buttons;
        public int index = -1;
        public int value = 0;

        public void SelectClass(int index)
        {
            TGM_ClassMenu.instance.SpawnClass(index);
        }

        public void LaunchGame()
        {
            
        }

        public void AdjustSettingValue(int amount)
        {
            TGM_MainMenu.Setting setting = TGM_MainMenu.instance.settings[index];

            //Loop Around
            if (value + amount > setting.intMax)
                value = setting.intMin;
            else if (value + amount < setting.intMin)
                value = setting.intMax;
            else
                value += amount;
            TGM_MainMenu.instance.UpdateSettings();
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
