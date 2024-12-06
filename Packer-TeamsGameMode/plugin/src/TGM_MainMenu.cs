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

        [System.Serializable]
        public class Setting
        {
            public SettingEnum setting; //For Internal use only, doesn't do anything
            public string description;
            public string[] settings;
            public SettingType type = SettingType.Strings;
            public int value = 0;
            public int intMin = 0;
            public int intMax = 64;
            [Tooltip("If true, does not sync over H3MP")]
            public bool localOnly = false;
            public bool active = true;  //Disabled via code if not valid

            public enum SettingType
            {
                Strings = 0,
                Int = 1,
            }
        }

        [Header("Page: Game Settings")]
        public List<Setting> settings = new List<Setting>();
        public GameObject settingPrefab;
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
            SetupSettings();
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

                TGM_Button btn = Instantiate(gamemodeBtnPrefab, gamemodeBtnPrefab.transform).GetComponent<TGM_Button>();
                btn.texts[0].text = gm.name;
                btn.buttons[0].image.sprite = gm.thumbnail;
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

        void SetupSettings()
        {
            for (int i = 0; i < settings.Count; i++)
            {
                //If only 2 teams, hide team setting
                if (i == (int)SettingEnum.Teams && TGM_Scene.instance.teams.Length <= 2)
                    continue;

                TGM_Button btn = Instantiate(settingPrefab, settingPrefab.transform).GetComponent<TGM_Button>();
                btn.index = i;
                btn.value = settings[i].value;

                btn.texts[0].text = settings[i].description;

                //Current Setting
                if (settings[i].type == Setting.SettingType.Strings)
                    btn.texts[1].text = settings[i].settings[settings[i].value]; //Text
                else
                    btn.texts[1].text = settings[i].value.ToString(); //Number
            }
        }

        public void UpdateSettings()
        {
            for (int i = 0; i < settings.Count; i++)
            {

            }


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

public enum SettingEnum
{
    SpawnLock = 0,
    SpawnWaveTime = 1,
    TimeLimit = 2,
    Teams = 3,
    CanRespawn = 4
}