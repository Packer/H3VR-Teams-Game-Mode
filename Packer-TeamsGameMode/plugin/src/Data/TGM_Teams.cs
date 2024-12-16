using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using FistVR;

namespace TeamsGameMode
{
    [Serializable]
    public class TGM_Teams
    {
        public static TGM_Teams instance;

        public Team[] teams;

        public class Team
        {
            public string teamName; //Currently Unused, should be a Team Color Name
            public int iff; //Matches the array index, for internal access
            public TGM_PlayerTeam playerTeam;   //Player Team
            public TGM_SosigTeam sosigTeam;     //Sosig Team
            public int sosigCount = 8;      //Total amount of sosigs on this team
            public int scoreGoal = 80;
            public List<Sosig> sosigs = new List<Sosig>();
            public Color color;

            //Tracking
            public int currentScore = 80;
            public TGM_Area currentSpawnArea;

            public Team()
            {


                //Color
                float spacing = iff > 7 ? 0.075f : 0.125f;
                color = Color.HSVToRGB(spacing * iff, 1f, 1f);
            }
        }

        public TGM_Teams()
        {
            teams = new Team[2];
            for (int i = 0; i < 2; i++)
            {
                teams[i] = new Team();
                if(TGM_Scene.Team(i).teamScore > 0)
                    teams[i].scoreGoal = TGM_Scene.Team(i).teamScore;
            }
        }


        public Vector3 GetTeamRoomSpawnPoint(int team)
        {
            return TGM_Manager.instance.gamemode.GetTeamRoomSpawnPoint(team);
        }
    }
}
