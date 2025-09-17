using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using H3MP.Networking;
using FistVR;

namespace TeamsGameMode
{
    public class TGM_MainMenu : MonoBehaviour
    {
        public static TGM_MainMenu instance;

        public static bool handSide = false;    //Left = false, Right = true

        [Header("Pages")]
        public GameObject[] pages;
        public GameObject startButton;
        public enum Page
        {
            Gamemode = 0,
            GameSettings = 1,
            JoinTeam = 2,
            WaitForHost = 3,
            Spectator = 4,
        }

        [System.Serializable]
        public class Setting
        {
            public string description;
            public SettingEnum setting; //For Internal use only, doesn't do anything
            public string[] settings;
            public SettingType type = SettingType.Strings;
            public int value = 0;
            public int intMin = 0;
            public int intMax = 64;
            [Tooltip("If true, does not sync over H3MP")]
            public bool localOnly = false;
            [HideInInspector]
            public bool active = true;  //Disabled via code if not valid

            public enum SettingType
            {
                Strings = 0,
                Int = 1,
            }
        }

        [Header("Page: Game Settings")]
        public List<Setting> gameSettings = new List<Setting>();
        public GameObject gameSettingPrefab;
        public List<Setting> gamemodeSettings = new List<Setting>();
        public GameObject gamemodeSettingPrefab;

        //Put Base Game settings here
        //Save Data?
        [Header("Page: Team Select")]


        [Header("Gamemode Window")]
        public GameObject gamemodeBtnPrefab;
        [HideInInspector]
        public TGM_Button[] gamemodesBtns;


        public delegate void SetupDelegate();
        public static event SetupDelegate PreGamemodeSetupEvent;

        void Awake()
        {
            instance = this;
        }

        public void Setup()
        {
            if (PreGamemodeSetupEvent != null)
                PreGamemodeSetupEvent.Invoke();

            //Populate Main Menu
            SetupGamemodes();
            SetupSettings();

            if (Networking.IsClient())
                startButton.SetActive(false);
        }


        //-------------------------------------------------------------------------------------
        // Game Modes
        //-------------------------------------------------------------------------------------

        public void SetupGamemodes()
        {
            for (int i = 0; i < TGM_Manager.gamemodes.Count; i++)
            {
                //Empty or Not Valid continue
                if (!TGM_Manager.gamemodes[i].IsGamemodeValid())
                    continue;

                TGM_Gamemode gm = TGM_Manager.gamemodes[i];

                TGM_Button btn = Instantiate(gamemodeBtnPrefab, gamemodeBtnPrefab.transform.parent).GetComponent<TGM_Button>();
                btn.gameObject.SetActive(true);
                btn.texts[0].text = gm.name;
                if(gm.thumbnail != null)
                    btn.buttons[0].image.sprite = gm.thumbnail;
                btn.index = i;
            }
        }

        public void SelectGamemode(int index)
        {
            TeamGameModePlugin.Logger.LogMessage($"Gamemode Selected: " + TGM_Manager.gamemodes[index].name);
            TGM_Manager.instance.gamemode = TGM_Manager.gamemodes[index];
            GM.CurrentSceneSettings.SosigKillEvent += TGM_Manager.instance.gamemode.OnSosigKilled;
            TGM_Manager.profile = TGM_Manager.instance.gamemode.LoadDefaultProfile();
            OpenPage(Page.GameSettings);
            UpdateSettings();
            TGM_Manager.instance.SetGameState(TGM_Manager.GameStateEnum.Setup);
        }

        void OnDisable()
        {
            GM.CurrentSceneSettings.SosigKillEvent -= TGM_Manager.instance.gamemode.OnSosigKilled;
            //GM.CurrentSceneSettings.PlayerDeathFromIFFEvent -= TGM_Manager.instance.gamemode.OnPlayerKilled;
        }

        //-------------------------------------------------------------------------------------
        // Game Settings
        //-------------------------------------------------------------------------------------

        void SetupSettings()
        {
            for (int i = 0; i < gameSettings.Count; i++)
            {
                TGM_Button btn = Instantiate(gameSettingPrefab, gameSettingPrefab.transform.parent).GetComponent<TGM_Button>();
                btn.gameObject.SetActive(true);
                btn.index = i;
                btn.value = gameSettings[i].value;

                btn.texts[0].text = gameSettings[i].description;

                //Current Setting
                if (gameSettings[i].type == Setting.SettingType.Strings)
                    btn.texts[1].text = gameSettings[i].settings[gameSettings[i].value]; //Text
                else
                    btn.texts[1].text = gameSettings[i].value.ToString(); //Number

                if (Networking.IsClient() && !gameSettings[i].localOnly)
                {
                    btn.buttons[0].gameObject.SetActive(false);
                    btn.buttons[1].gameObject.SetActive(false);
                }
            }
        }

        public void UpdateSettings()
        {
            for (int i = 0; i < gameSettings.Count; i++)
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

        public void StartGame()
        {
            TGM_Manager.instance.SetGameState(TGM_Manager.GameStateEnum.Gameplay);
            OpenPage(Page.JoinTeam);
            TeamGameModePlugin.Logger.LogMessage($"Game Started");
        }
    }
}