using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using H3MP.Networking;
using FistVR;

namespace TeamsGameMode;

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


    [Header("Page: Game Settings")]
    //public List<TGM_Settings.Setting> TGM_Settings.gameSettings = new List<TGM_Settings.Setting>();      //DELETE ME
    //public List<TGM_Settings.Setting> gamemodeSettings = new List<TGM_Settings.Setting>();  //TODO DELETE ME
    public GameObject gameSettingPrefab;
    public GameObject gamemodeSettingPrefab;
    public GameObject requestButton;

    //Put Base Game settings here
    //Save Data?
    [Header("Page: Team Select")]

    [Header("Page: Spectator")]
    public Text spectateName;

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
        {
            //Multiplayer
            requestButton.SetActive(true);
            startButton.SetActive(false);
        }
        else
        {
            //Single Player
            requestButton.SetActive(false);
            startButton.SetActive(true);
        }

        //Default page
        OpenPage(Page.Gamemode);
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
            //btn.texts[0].text = gm.name; //Just use Images for gamemodes
            if(gm.thumbnail != null)
                btn.buttons[0].image.sprite = gm.thumbnail;
            btn.index = i;
        }
    }

    public void SelectGamemode(int index)
    {
        TeamGameModePlugin.Logger.LogMessage($"Gamemode Selected: " + TGM_Manager.gamemodes[index].name);

        //If not already assigned, assign the gamemode
        if (TGM_Manager.instance.gamemode != TGM_Manager.gamemodes[index])
        {
            TGM_Manager.instance.gamemode = TGM_Manager.gamemodes[index];
            TGM_Manager.instance.gamemode.LoadDefaultProfile();
        }

        //Reset Team stats for 2nd playthrough
        for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
        {
            TGM_Manager.instance.team[i].ResetTeamTracking();
        }

        //Reset player Tracking
        TGM_Manager.instance.localPlayer.ResetPlayer();


        OpenPage(Page.GameSettings);
        UpdateSettings();
        TGM_Manager.instance.SetGameState(TGM_Manager.GameStateEnum.Setup);
    }

    void OnDisable()
    {
        //GM.CurrentSceneSettings.PlayerDeathFromIFFEvent -= TGM_Manager.instance.gamemode.OnPlayerKilled;
    }

    //-------------------------------------------------------------------------------------
    // Game Settings
    //-------------------------------------------------------------------------------------

    void SetupSettings()
    {
        for (int i = 0; i < TGM_Settings.gameSettings.Count; i++)
        {
            TGM_Button btn = Instantiate(gameSettingPrefab, gameSettingPrefab.transform.parent).GetComponent<TGM_Button>();
            btn.gameObject.SetActive(true);
            btn.index = i;

            //Assign button to Setting
            TGM_Settings.gameSettings[i].button = btn;
        }

        UpdateSettings();
    }
    
    public void UpdateSettings()
    {
        //Main Menu
        for (int i = 0; i < TGM_Settings.gameSettings.Count; i++)
        {
            TGM_Button btn = TGM_Settings.gameSettings[i].button;
            if (btn == null)
                continue;

            btn.value = TGM_Settings.gameSettings[i].value;
            btn.texts[0].text = TGM_Settings.gameSettings[i].description;

            //Current Setting
            if (TGM_Settings.gameSettings[i].type == TGM_Settings.Setting.SettingType.Strings)
                btn.texts[1].text = TGM_Settings.gameSettings[i].settings[TGM_Settings.gameSettings[i].value]; //Text
            else if (TGM_Settings.gameSettings[i].type == TGM_Settings.Setting.SettingType.FirstString)
            {
                //Set to first String name (Default) else use raw numbers
                if (TGM_Settings.gameSettings[i].value == 0)
                    btn.texts[1].text = TGM_Settings.gameSettings[i].settings[TGM_Settings.gameSettings[i].value]; //Text
                else
                    btn.texts[1].text = TGM_Settings.gameSettings[i].value.ToString(); //Number
            }
            else
                btn.texts[1].text = TGM_Settings.gameSettings[i].value.ToString(); //Number


            if (Networking.IsClient() && !TGM_Settings.gameSettings[i].localOnly)
            {
                btn.buttons[0].gameObject.SetActive(false);
                btn.buttons[1].gameObject.SetActive(false);
            }
        }

        //Teams Setup
        for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
        {
            TGM_TeamSetup.instance.scoreCountText[i].text = TGM_Manager.instance.team[i].scoreGoal.ToString();
            TGM_TeamSetup.instance.sosigsCountText[i].text = TGM_Manager.instance.team[i].sosigLimit.ToString();

            //Check if mods are loaded in yet
            if (TGM_ModLoader.playerTeams == null || TGM_ModLoader.playerTeams.Count == 0)
                break;

            int playerIndex = TGM_Manager.instance.team[i].playerTeam;

            if (playerIndex >= 0 && playerIndex < TGM_ModLoader.playerTeams.Count)
            {
                //Display new Profile Infomation
                TGM_TeamSetup.instance.playerTeamTitles[i].text = TGM_ModLoader.playerTeams[playerIndex].name;
                TGM_TeamSetup.instance.playerTeamDescriptions[i].text = TGM_ModLoader.playerTeams[playerIndex].description;
                TGM_TeamSetup.instance.playerTeamThumbnails[i].sprite = TGM_ModLoader.playerTeams[playerIndex].thumbnail;
            }

            int sosigIndex = TGM_Manager.instance.team[i].sosigTeam;
            if (sosigIndex >= 0 && sosigIndex < TGM_ModLoader.sosigTeams.Count)
            {
                TGM_TeamSetup.instance.sosigTeamTitles[i].text = TGM_ModLoader.sosigTeams[sosigIndex].name;
                TGM_TeamSetup.instance.sosigTeamDescriptions[i].text = TGM_ModLoader.sosigTeams[sosigIndex].description;
                TGM_TeamSetup.instance.sosigTeamThumbnails[i].sprite = TGM_ModLoader.sosigTeams[sosigIndex].thumbnail;
            }
        }

        if (Networking.H3MP)
        {
            //Send Settings

        }
    }

    public void RequestSettings()
    {
        if (Networking.H3MP)
        {
            //Request settings from Host
        }
    }

    //-------------------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------------------

    public void OpenPage(Page index)
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].SetActive(false);
        }
        pages[(int)index].SetActive(true);
    }

    public void StartGame()
    {
        //Setup Sosig Teams
        for (int t = 0; t < TGM_Manager.instance.team.Length; t++)
        {
            TGM_Team team = TGM_Manager.instance.team[t];

            team.sosigsData.Clear();

            for (int s = 0; s < team.sosigLimit; s++)
            {
                TGM_Player sosig = new TGM_Player()
                {
                    isSosig = true,
                    playerName = TGM_NameDB.GetRandomName(),
                    iff = t,
                };
                team.sosigsData.Add(sosig);
            }
        }

        TGM_Manager.instance.SetGameState(TGM_Manager.GameStateEnum.Pregame);
        OpenPage(Page.JoinTeam);
        TeamGameModePlugin.Logger.LogMessage($"Game Started");
    }

    public void ToggleSpectatorCamera()
    {
        if (GM.CurrentSceneSettings.GetCamObjectPoint() == TGM_Spectator.instance.spectatorCamera.transform)
            GM.CurrentSceneSettings.SetCamObjectPoint(null);
        else
            GM.CurrentSceneSettings.SetCamObjectPoint(TGM_Spectator.instance.spectatorCamera.transform);
        TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.Press);
    }
}