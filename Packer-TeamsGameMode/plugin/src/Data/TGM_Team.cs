using FistVR;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using Sodalite.Api;
using Random = UnityEngine.Random;

namespace TeamsGameMode;

public class TGM_Team
{
    public delegate void CreateSosigDelegate(Sosig s);
    public static event CreateSosigDelegate CreateSosigEvent;

    public string teamName; //Currently Unused, should be a Team Color Name
    public int iff; //Matches the array index, for internal access
    public int playerTeam = 0;
    public int sosigTeam = 0;
    //public TGM_PlayerTeam playerTeam;   //Player Team
    //public TGM_SosigTeam sosigTeam;     //Sosig Team
    public int sosigLimit = 8;      //Total amount of sosigs on this team
    public int scoreGoal = 20;
    public List<Sosig> sosigs = new List<Sosig>();
    public List<TGM_Player> sosigsData = new List<TGM_Player>();
    public Color color;

    //Tracking
    public int currentKills = 0;
    public int currentScore = 0;
    public TGM_Area currentSpawnArea;

    /// <summary>
    /// The time which respawning occurs
    /// </summary>
    public float respawnTime = 0;

    public TGM_PlayerTeam GetPlayerTeam()
    {
        return TGM_ModLoader.playerTeams[playerTeam];
    }

    public TGM_SosigTeam GetSosigTeam()
    {
        return TGM_ModLoader.sosigTeams[sosigTeam];
    }

    /// <summary>
    /// Returns all Team tracking values back to thier default values
    /// </summary>
    public void ResetTeamTracking()
    {
        currentKills = 0;
        currentScore = 0;
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
            Vector3[] data = Global.GetRandomPlayerSpawnPoint(currentSpawnArea.spawnPoints);
            GM.CurrentMovementManager.TeleportToPoint(data[0], true, data[1]);
            TGM_Manager.instance.localPlayer.awaitingRespawn = false;
            TGM_ClassMenu.instance.spawnButtonText.text = "Spawn";
        }

        //Spawn Sosigs
        if (sosigs.Count < sosigLimit)
        {
            int sosigRemain = sosigLimit - sosigs.Count;
            SosigAPI.SpawnOptions _spawnOptions = new SosigAPI.SpawnOptions
            {
                SpawnState = Sosig.SosigOrder.Assault,
                SpawnActivated = true,
                EquipmentMode = SosigAPI.SpawnOptions.EquipmentSlots.All,
                SpawnWithFullAmmo = true,
                IFF = iff
            };

            for (int i = 0; i < sosigRemain; i++)
            {
                Transform spawnArea = currentSpawnArea.sosigSpawnPoints[Random.Range(0, currentSpawnArea.sosigSpawnPoints.Length)];
                Vector3[] spawnPoint = Global.GetValidSpawnPoint(spawnArea);
                Sosig s = CreateTeamSosig(_spawnOptions, spawnPoint[0], spawnArea.rotation);

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

            TGM_SosigTeam selectedSosigTeam = TGM_ModLoader.sosigTeams[sosigTeam];

            if (selectedSosigTeam == null)
            {
                Debug.Log("MISSING SOSIG TEAM");
                return null;
            }
            if (selectedSosigTeam.sosigSet == null)
            {
                Debug.Log("MISSING SOSIG SET");
                return null;
            }

            for (int i = 0; i < selectedSosigTeam.sosigSet.Length; i++)
            {
                if (currentKills > selectedSosigTeam.sosigSet[i].minKills
                    && currentKills <= selectedSosigTeam.sosigSet[i].maxKills)
                {
                    sets.Add(selectedSosigTeam.sosigSet[i]);
                }
            }

            //Backup make sure we have at least ONE sosig set
            if (sets.Count <= 0)
                sets.Add(selectedSosigTeam.sosigSet[0]);

            TGM_SosigTeam.SosigSet selectedSet = sets[Random.Range(0, sets.Count)];
            sosigID = selectedSet.sosigEnemyIDs[Random.Range(0, selectedSet.sosigEnemyIDs.Length)];
        }

        Sosig sosig =
            SosigAPI.Spawn(
                IM.Instance.odicSosigObjsByID[(SosigEnemyID)sosigID],
                spawnOptions,
                position,
                rotation);

        //Assign to Empty Slot
        for (int i = 0; i < sosigsData.Count; i++)
        {
            if (sosigsData[i].sosig == null)
            {
                sosigsData[i].sosig = sosig;
                //TODO Display name here
                break;
            }
        }

        DisableSosigWeaponPickup(sosig);

        //Set Agents to quailty level
        NavMeshAgent agent = sosig.GetComponent<NavMeshAgent>();

        agent.obstacleAvoidanceType = TGM_Scene.instance.avoidanceQuailty;
        agent.stoppingDistance = 1;

        sosigs.Add(sosig);

        //IF FRIENDLY!
        if (TGM_Settings.GetSetting(TGMSettingEnum.ShowFriendlies) == 1
            && spawnOptions.IFF == GM.CurrentPlayerBody.GetPlayerIFF())
        {
            GameObject arrow = TGM_Manager.Instantiate(
                TGM_ModLoader.tgmAssets.iffPrefab,
                sosig.Links[0].R.transform.position + (Vector3.up * 0.75f),
                sosig.Links[0].R.transform.rotation,
                sosig.Links[0].R.transform);

            arrow.transform.localRotation = Quaternion.Euler(0, 0, 180);
        }

        return sosig;
    }

    public void ClearAllTeamSosigs()
    {
        for (int i = 0; i < sosigs.Count; i++)
        {
            if(sosigs[i] != null)
                sosigs[i].ClearSosig();
        }
        sosigs.Clear();

        for (int i = 0; i < sosigsData.Count; i++)
        {
            if (sosigsData[i].sosig != null)
                sosigsData[i].sosig.ClearSosig();
        }

        sosigsData.Clear();
    }

    public static void DisableSosigWeaponPickup(Sosig s)
    {
        if (TGM_Settings.GetSetting(TGMSettingEnum.SosigWeapons) == 1)
            return;

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

    /*
    public static void DestroySosigWeapons(Sosig s)
    {
        foreach (var item in s.Inventory.Slots)
        {
            if (item.HeldObject == null)
                continue;
            else
                Object.Destroy(item.HeldObject.gameObject);
        }

        foreach (var item in s.Hands)
        {
            if (item.HeldObject == null)
                continue;
            else
                Object.Destroy(item.HeldObject.gameObject);
        }
    }
    */
}