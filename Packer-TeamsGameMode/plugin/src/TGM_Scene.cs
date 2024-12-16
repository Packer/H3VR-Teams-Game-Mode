using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace TeamsGameMode
{
    public class TGM_Scene : MonoBehaviour
    {
        public static TGM_Scene instance;
        [Header("Scene Setup")]
        public TeamSpawnRoom[] teams = new TeamSpawnRoom[2];
        public TGM_Area[] areas = new TGM_Area[2];
        public Transform mainMenu;

        [Header("Audio Overwrite")]
        [Tooltip("Start Game, or spawn items etc")]
        public AudioClip audioConfirm;
        [Tooltip("Regular Button Press")]
        public AudioClip audioPress;
        [Tooltip("Something broke or didn't accept input")]
        public AudioClip audioFail;
        [Tooltip("Rearm Player")]
        public AudioClip audioRearm;
        [Tooltip("Player's Team Objective point change")]
        public AudioClip audioPoint;
        [Tooltip("Get Kill")]
        public AudioClip audioElimination;
        [Tooltip("Player's Team Won")]
        public AudioClip audioTeamWon;
        [Tooltip("Player's Team Lost")]
        public AudioClip audioTeamLost;

        [System.Serializable]
        public class TeamSpawnRoom
        {
            [Tooltip("The area which a player can respawn, can be scaled")]
            public Transform respawnArea;   //Based on Gizmo
            [Tooltip("The team menu for spectating, scoreboard and rejoining the game")]
            public Transform teamMenu;
            [Tooltip("The Starting area for this Team")]
            public TGM_Area startSpawnArea;
            [Tooltip("Recommended Team Objective Score for this team, set to 0 or less to use default")]
            public int teamScore = 0;
        }

        /// <summary>
        /// Returns the input IFF Team Spawn Room
        /// </summary>
        /// <param name="iff"></param>
        /// <returns></returns>
        public static TeamSpawnRoom Team(int iff)
        {
            return instance.teams[iff];
        }


        public static Transform GetTeamSpawnRoomTransform(int team)
        {
            return instance.teams[team].respawnArea;
        }

        void Awake()
        {
            instance = this;
        }

        void OnDrawGizmos()
        {
            for (int i = 0; i < teams.Length; i++)
            {
                if (teams[i] == null)
                    continue;

                Color newGreen = Color.green;
                newGreen.a = 0.2f;
                Gizmos.color = newGreen;
                if (teams[i].respawnArea != null)
                    Gizmos.DrawCube(teams[i].respawnArea.position, teams[i].respawnArea.localScale / 2);
                
            }
        }

        void OnValidate()
        {
            if (teams.Length < 2)
            {
                List<TeamSpawnRoom> newTeams = teams.ToList();
                int missing = 2 - newTeams.Count;
                for (int i = 0; i < missing; i++)
                {
                    newTeams.Add(new TeamSpawnRoom());
                }
                teams = newTeams.ToArray();
            }

            if (areas.Length < 2)
            {
                List<TGM_Area> newAreas = areas.ToList();
                int missing = 2 - newAreas.Count;
                for (int i = 0; i < missing; i++)
                {
                    newAreas.Add(new TGM_Area());
                }
                areas = newAreas.ToArray();
            }
        }
    }
}
