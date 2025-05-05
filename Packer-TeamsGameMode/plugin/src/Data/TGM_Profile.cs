using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeamsGameMode
{
    public class TGM_Profile
    {
        public string name = "New Profile";
        public List<string> playerTeams;
        public List<string> sosigTeams;

        public int gamemode = 0;
        public int[] gameSettings;
        public int[] gamemodeSettings;
    }

    public enum SettingEnum
    {
        SpawnLock = 0,
        SpawnWaveTime = 1,
        TimeLimit = 2,
        CanRespawn = 3,
        ShowFriendlies = 4,
        ItemsOnDeath = 5,
        SosigWeapons = 6,
        PlayerHealth = 7,
    }
}
