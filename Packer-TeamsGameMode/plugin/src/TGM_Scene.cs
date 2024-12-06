using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.AI;

namespace TeamsGameMode
{
    public class TGM_Scene : MonoBehaviour
    {
        public static TGM_Scene instance;
        [Header("Setup")]
        public TeamSpawnRoom[] teams = new TeamSpawnRoom[2];
        public TGM_Area[] areas = new TGM_Area[2];
        public Transform mainMenu;

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

        public static Vector3 GetTeamSpawnRoom(int team)
        {
            Vector3 position = instance.teams[team].respawnArea.position;
            Vector3 scale = instance.teams[team].respawnArea.localScale;
            Vector3 randomPosition 
                = new Vector3(
                    Random.Range(-scale.x, scale.x), 
                    Random.Range(-scale.y, scale.y), 
                    Random.Range(-scale.z, scale.z));

            //Assign Position
            if(NavMesh.SamplePosition(randomPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                position = hit.position;

            return position;
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
