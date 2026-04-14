using FistVR;
using System.Collections;
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
        public ObstacleAvoidanceType avoidanceQuailty = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
        public Transform playerResetPoint;
        [HideInInspector]
        public Vector3 defaultResetPosition;
        [HideInInspector]
        public Quaternion defaultResetRotation;

        [Header("Menus")]
        public Transform mainMenu;
        public Transform teamSetupMenu;
        public Transform profilesMenu;
        public Transform itemSpawner;

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

        public delegate void SceneLoadedDelegate();
        public static event SceneLoadedDelegate SceneLoadedEvent;

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
            [Tooltip("The time between wave respawns, attacking teams should have lower than defending")]
            public float teamSpawnTime = 5f;
        }

        /// <summary>
        /// Returns the input IFF Team Spawn Room
        /// </summary>
        /// <param name="iff"></param>
        /// <returns></returns>
        public static TeamSpawnRoom Team(int iff)
        {
            if (instance == null || instance.teams == null || instance.teams.Length >= iff)
                return null;

            return instance.teams[iff];
        }

        public static Transform GetTeamSpawnRoomTransform(int team)
        {
            return instance.teams[team].respawnArea;
        }

        void Awake()
        {
            instance = this;
            defaultResetPosition = playerResetPoint.position;
            defaultResetRotation = playerResetPoint.rotation;
        }

        void Start()
        {
            StartCoroutine(Setup());
        }

        public IEnumerator Setup()
        {
            yield return StartCoroutine(TGM_ModLoader.LoadAssets());

            //Create our Gamemode assets
            Instantiate(TGM_ModLoader.tgmAssets.manager);
            yield return new WaitForEndOfFrame();

            Instantiate(TGM_ModLoader.tgmAssets.mainMenu, mainMenu.position, mainMenu.rotation * Quaternion.Euler(0, 180, 0));
            Instantiate(TGM_ModLoader.tgmAssets.teamSetup, teamSetupMenu.position, teamSetupMenu.rotation * Quaternion.Euler(0, 180, 0));
            Instantiate(TGM_ModLoader.tgmAssets.profileMenu, profilesMenu.position, profilesMenu.rotation * Quaternion.Euler(0, 180, 0));
            yield return new WaitForEndOfFrame();

            Instantiate(TGM_ModLoader.tgmAssets.spectator);
            Instantiate(TGM_ModLoader.tgmAssets.compass);
            Instantiate(TGM_ModLoader.tgmAssets.endScreen);
            yield return new WaitForEndOfFrame();

            //Everything has loaded in, invoke the event to get any extras in before we set everything up
            if (SceneLoadedEvent != null)
                SceneLoadedEvent.Invoke();
            TGM_Manager.instance.Setup();
        }

        void OnDrawGizmos()
        {

            if (teams != null)
            {
                for (int i = 0; i < teams.Length; i++)
                {
                    if (teams[i] == null)
                        continue;

                    Color newColor = Color.green;
                    newColor.a = 0.2f;
                    Gizmos.color = newColor;
                    if (teams[i].respawnArea != null)
                        Gizmos.DrawCube(teams[i].respawnArea.position, teams[i].respawnArea.localScale / 2);

                }
            }

            //MATRIX

            if (mainMenu != null)
            {
                Color newColor = Color.grey;
                newColor.a = 0.2f;
                Gizmos.color = newColor;

                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawLine(mainMenu.position, mainMenu.position + mainMenu.forward);
                Gizmos.matrix = Matrix4x4.TRS(mainMenu.position, mainMenu.rotation, new Vector3(1.92f, 1.08f, 0.01f));
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }

            if (teamSetupMenu != null)
            {
                Color newColor = Color.grey;
                newColor.a = 0.2f;
                Gizmos.color = newColor;

                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawLine(teamSetupMenu.position, teamSetupMenu.position + teamSetupMenu.forward);
                Gizmos.matrix = Matrix4x4.TRS(teamSetupMenu.position, teamSetupMenu.rotation, new Vector3(1.92f, 1.08f, 0.01f));
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }

            if (profilesMenu != null)
            {
                Color newColor = Color.grey;
                newColor.a = 0.2f;
                Gizmos.color = newColor;

                Gizmos.matrix = Matrix4x4.identity;
                Gizmos.DrawLine(profilesMenu.position, profilesMenu.position + profilesMenu.forward);
                Gizmos.matrix = Matrix4x4.TRS(profilesMenu.position, profilesMenu.rotation, new Vector3(0.96f, 1.08f, 0.01f));
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
                Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            }

            //Matrix Manipulated Below        
            if (itemSpawner != null)
            {
                Gizmos.color = new Color(0.4f, 0.4f, 0.9f, 0.5f);
                Gizmos.matrix = itemSpawner.localToWorldMatrix;
                //Item Spawner
                Vector3 vector = new Vector3(0f, 0.7f, 0.25f);
                Vector3 size = new Vector3(2.3f, 1.2f, 0.5f);
                Vector3 vector2 = Vector3.forward;
                Gizmos.DrawCube(vector, size);
                Gizmos.DrawLine(vector, vector + vector2 * 0.5f);
            }
        }

        void OnValidate()
        {

            if (teams != null)
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
            }

            if (areas != null)
            {
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
}
