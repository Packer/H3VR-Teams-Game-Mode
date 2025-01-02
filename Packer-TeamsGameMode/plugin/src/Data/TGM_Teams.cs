using System;
using System.Collections.Generic;
using UnityEngine;
using FistVR;
using Sodalite.Api;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace TeamsGameMode
{
    [Serializable]
    public class TGM_Teams
    {
        public static TGM_Teams instance;
        public delegate void CreateSosigDelegate(Sosig s);
        public static event CreateSosigDelegate CreateSosigEvent;

        public Team[] teams;

        public static Team GetTeam(int iff)
        {
            return instance.teams[iff];
        }

        [Serializable]
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
            public int currentKills = 0;
            public int currentScore = 80;
            public TGM_Area currentSpawnArea;

            public Team()
            {
                color = Color.HSVToRGB(0.125f * iff, 1f, 1f);

                //Hard Code teams
                switch (iff)
                {
                    default:
                    case 0:
                        teamName = "Red";
                        color = Color.red;
                        break;
                    case 1:
                        teamName = "Blue";
                        color = Color.blue;
                        break;
                }
            }

            /// <summary>
            /// Triggered on wave timer
            /// </summary>
            public void Respawn()
            {
                int localIFF = GM.CurrentPlayerBody.GetPlayerIFF();
                //Spawn Local Player
                if (localIFF == iff 
                    && TGM_Manager.instance.localPlayer.awaitingRespawn)
                {
                    Vector3[] data = currentSpawnArea.GetRandomPlayerSpawnPoint();
                    GM.CurrentMovementManager.TeleportToPoint(data[0], true, data[1]);
                    TGM_Manager.instance.localPlayer.awaitingRespawn = false;
                }

                //Spawn Sosigs
                if (sosigs.Count < sosigCount)
                {
                    int sosigRemain = sosigCount - sosigs.Count;
                    SosigAPI.SpawnOptions _spawnOptions = new SosigAPI.SpawnOptions
                    {
                        SpawnState = Sosig.SosigOrder.PathTo,
                        SpawnActivated = true,
                        EquipmentMode = SosigAPI.SpawnOptions.EquipmentSlots.All,
                        SpawnWithFullAmmo = true,
                        IFF = iff
                    };

                    for (int i = 0; i < sosigRemain; i++)
                    {
                        Transform spawnArea = currentSpawnArea.spawnPoints[Random.Range(0, currentSpawnArea.spawnPoints.Length)];
                        Vector3 spawnScale = spawnArea.localScale / 2;
                        Vector3 spawnPoint 
                            = spawnArea.position 
                            + new Vector3(
                                Random.Range(-spawnScale.x, spawnScale.x), 
                                Random.Range(-spawnScale.y, spawnScale.y), 
                                Random.Range(-spawnScale.z, spawnScale.z));

                        //Validate Sosig Spawn
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(spawnPoint, out hit, Mathf.Max(spawnScale.y * 2, 1f), NavMesh.AllAreas))
                            spawnPoint = hit.position;
                        else
                        {
                            TeamGameModePlugin.Logger.LogMessage($"Invalid Sosig Spawn Point: " + spawnArea.name);
                            continue;
                        }

                        Sosig s = CreateTeamSosig(_spawnOptions, spawnPoint, spawnArea.rotation);

                        if (CreateSosigEvent != null)
                            CreateSosigEvent.Invoke(s);
                    }
                }
            }


            public Sosig CreateTeamSosig(SosigAPI.SpawnOptions spawnOptions, Vector3 position, Quaternion rotation, int sosigID = -2)
            {
                //If not custom sosig, use team ID
                if (sosigID == -2)
                {
                    List<TGM_SosigTeam.SosigSet> sets = new List<TGM_SosigTeam.SosigSet>();

                    for (int i = 0; i < sosigTeam.sosigSet.Length; i++)
                    {
                        if (currentKills > sosigTeam.sosigSet[i].minKills
                            && currentKills <= sosigTeam.sosigSet[i].maxKills)
                        {
                            sets.Add(sosigTeam.sosigSet[i]);
                        }
                    }

                    //Backup make sure we have at least ONE sosig set
                    if (sets.Count <= 0)
                        sets.Add(sosigTeam.sosigSet[0]);

                    TGM_SosigTeam.SosigSet selectedSet = sets[Random.Range(0, sets.Count)];
                    sosigID = selectedSet.sosigEnemyIDs[Random.Range(0, selectedSet.sosigEnemyIDs.Length)];
                }

                Sosig sosig =
                    SosigAPI.Spawn(
                        IM.Instance.odicSosigObjsByID[(SosigEnemyID)sosigID],
                        spawnOptions,
                        position,
                        rotation);

                DisableSosigWeaponPickup(sosig);

                //Set Agents to quailty level
                NavMeshAgent agent = sosig.GetComponent<NavMeshAgent>();

                agent.obstacleAvoidanceType = TGM_Scene.instance.avoidanceQuailty;
                agent.stoppingDistance = 1;

                sosigs.Add(sosig);

                return sosig;
            }

            public static void DisableSosigWeaponPickup(Sosig s)
            {
                //DO nothing if sosig weapons enabled
                //if (profile.sosigWeapons)
                //    return;

                foreach (var item in s.Inventory.Slots)
                {
                    if (item.HeldObject == null)
                        continue;

                    FVRPhysicalObject obj = item.HeldObject.GetComponent<FVRPhysicalObject>();
                    if (obj != null)
                        obj.IsPickUpLocked = true;
                }

                foreach (var item in s.Hands)
                {
                    if (item.HeldObject == null)
                        continue;

                    FVRPhysicalObject obj = item.HeldObject.GetComponent<FVRPhysicalObject>();
                    if (obj != null)
                        obj.IsPickUpLocked = true;
                }
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
    }
}
