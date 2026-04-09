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
            TGM_MainMenu.Setting setting = TGM_MainMenu.instance.gameSettings[index];

            //Loop Around
            if (value + amount > setting.intMax)
                value = setting.intMin;
            else if (value + amount < setting.intMin)
                value = setting.intMax;
            else
                value += amount;
            TGM_MainMenu.instance.UpdateSettings();
        }

        public void ChooseGamemode()
        {
            TGM_MainMenu.instance.SelectGamemode(index);

            TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.Confirm);
        }

        public void SelectProfile()
        {
            TGM_ProfileMenu.instance.SetProfile(texts[0].text);
        }

        public void SelectBrowserItem()
        {
            TGM_TeamSetup.instance.SelectBrowser(index);
        }

        public void JoinTeam(int iff)
        {
            GM.CurrentPlayerBody.SetPlayerIFF(iff);

            Transform spawn = TGM_Scene.GetTeamSpawnRoomTransform(iff);

            //Teleport to team spawn room
            GM.CurrentMovementManager.TeleportToPoint(Global.GetValidSpawnPoint(spawn),
                true,
                spawn.rotation.eulerAngles);


            Instantiate(TGM_ModLoader.tgmAssets.classMenu, spawn.position + (Vector3.up * 1.25f), spawn.rotation);

            TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.Confirm);
        }

        void OnValidate()
        {
            return;

            BoxCollider box = gameObject.GetComponent<BoxCollider>();

            if (box == null)
                return;

            RectTransform rect = gameObject.GetComponent<RectTransform>();

            if (!rect)
                return;

            box.size = new Vector3(rect.sizeDelta.x, rect.sizeDelta.y, 1);
        }
    }
}
