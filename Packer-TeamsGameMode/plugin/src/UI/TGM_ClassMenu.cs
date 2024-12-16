using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TeamsGameMode
{
    public class TGM_ClassMenu : MonoBehaviour
    {
        public static TGM_ClassMenu instance;

        public GameObject buttonPrefab;
        public Transform buttonContent;

        public List<TGM_Button> buttons = new List<TGM_Button>();


        void Awake()
        {
            instance = this;
        }


        public void Setup(TGM_PlayerClass[] classes)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                TGM_Button button = Instantiate(buttonPrefab, buttonContent).GetComponent<TGM_Button>();
                buttons.Add(button);
            }
        }

        public void ClearButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            buttons.Clear();
        }


        public void SpawnClass(int id)
        {
            if (id < 0)
            {
                TeamGameModePlugin.Logger.LogMessage(TeamGameModePlugin.Name + "Spawn Class is -1 or less and will not be spawned");
                return;
            }

            int team = TGM_Manager.instance.localPlayer.iff;


        }

    }
}
