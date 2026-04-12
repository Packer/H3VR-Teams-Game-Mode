using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamsGameMode;

public class TGM_Profile
{
    [NonSerialized]
    public static TGM_Profile profile = new TGM_Profile();

    public string name = "New Profile";
    public List<string> playerTeams;
    public List<string> sosigTeams;

    public int gamemode = 0;
    public List<int> gameSettings;
    public List<int> gamemodeSettings;

    /*
    public static void UpdateSettings()
    {
        TGM_Profile newProfile = new TGM_Profile();
        newProfile.name = profile.name;
        newProfile.playerTeams = profile.playerTeams;
        newProfile.sosigTeams = profile.sosigTeams;

        for (int i = 0; i < TGM_Settings.gameSettings.Count; i++)
        {

        }
    }


    public static int GetSetting(TGMSettingEnum setting)
    {
        return profile.gameSettings[(int)setting];
    }

    public static void SetSetting(TGMSettingEnum setting, int value)
    {
        profile.gameSettings[(int)setting] = value;
    }
    */
}
