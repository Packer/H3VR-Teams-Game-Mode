using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TeamsGameMode
{
    public class TGM_MainMenu : MonoBehaviour
    {
        public static TGM_MainMenu instance;

        [Header("Pages")]
        public GameObject[] pages;
        public enum Page
        {
            Gamemode = 0,
            GameSettings = 1,
            JoinTeam = 2,
            WaitForHost = 3,
        }

        [Header("Page: Game Settings")]
        public Text optionSpawnLockText;
        public Text optionSpawnWaveTime;
        public Text optionTimeLimit;
        public Text optionTeams;
        public Text optionCanRespawn;


        //Put Base Game settings here
        //Save Data?
        [Header("Page: Team Select")]


        [Header("Gamemode Window")]
        public GameObject gamemodeBtnPrefab;
        public Transform gamemodeContent;
        public TGM_Button[] gamemodesBtns;


        [Header("Team Window")]
        public GameObject[] teamWindow;


        [Header("Join Game")]
        public GameObject joinMenu;


        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            //Populate Main Menu
            SetupGamemodes();
        }


        //-------------------------------------------------------------------------------------
        // Game Modes
        //-------------------------------------------------------------------------------------

        public void SetupGamemodes()
        {
            for (int i = 0; i < TGM_Manager.instance.gamemodes.Length; i++)
            {
                //Empty or Not Valid continue
                if (TGM_Manager.instance.gamemodes[i] == null 
                    || !TGM_Manager.instance.gamemodes[i].IsGamemodeValid())
                    continue;

                TGM_Gamemode gm = TGM_Manager.instance.gamemodes[i];

                TGM_Button btn = Instantiate(gamemodeBtnPrefab, gamemodeContent).GetComponent<TGM_Button>();
                btn.text.text = gm.name;
                btn.button.image.sprite = gm.thumbnail;
                btn.index = i;
            }
        }

        public void SelectGamemode(int index)
        {
            TGM_Manager.instance.gamemode = TGM_Manager.instance.gamemodes[index];
            OpenPage(Page.GameSettings);
            UpdateSettings();
        }

        //-------------------------------------------------------------------------------------
        // Game Settings
        //-------------------------------------------------------------------------------------

        public void UpdateSettings()
        {
            if (TeamGameModePlugin.h3mp)
            {
                //Send Settings

            }
        }

        public void RequestSettings()
        {
            if (TeamGameModePlugin.h3mp)
            {
                //Request settings from Host
            }
        }

        //-------------------------------------------------------------------------------------
        // Methods
        //-------------------------------------------------------------------------------------

        void OpenPage(Page index)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                pages[i].SetActive(false);
            }
            pages[(int)index].SetActive(true);
        }
    }
}
