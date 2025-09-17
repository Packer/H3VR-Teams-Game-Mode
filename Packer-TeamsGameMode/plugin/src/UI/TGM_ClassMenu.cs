using FistVR;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TeamsGameMode
{
    public class TGM_ClassMenu : MonoBehaviour
    {
        public static TGM_ClassMenu instance;

        [Header("Menu")]
        public Text spawnButtonText;
        public GameObject buttonPrefab;
        public Transform buttonContent;

        private List<TGM_Button> buttons = new List<TGM_Button>();


        [Header("Spawn Points")]
        public Transform[] mainSpawns;
        public Transform[] ammoSpawns;

        private float spawnRange = 0.1f;

        // --- Spawn Positions
        public static int spawnMainIndex = 0;
        public static float spawnMainOffset = 0;

        public static void ResetSpawnPoints()
        {
            spawnMainIndex = 0;
            spawnMainOffset = 0;
        }
        // -----------------------


        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            Setup(TGM_Manager.instance.team[GM.CurrentPlayerBody.GetPlayerIFF()].GetPlayerTeam().playerClasses);
        }

        void Update()
        {
            if (TGM_Manager.instance.localPlayer.awaitingRespawn)
            {
                spawnButtonText.text = Mathf.Abs((Time.time - TGM_Manager.instance.team[TGM_Manager.instance.localPlayer.iff].respawnTime)).ToString("F2");
            }
        }


        public void Setup(TGM_PlayerClass[] classes)
        {
            for (int i = 0; i < classes.Length; i++)
            {
                TGM_Button button = Instantiate(buttonPrefab, buttonContent).GetComponent<TGM_Button>();
                button.gameObject.SetActive(true);
                buttons.Add(button);
            }
        }

        public void ClearButtons()
        {
            for (int i = 0; i < buttons.Count; i++)
            {
                Destroy(buttons[i].gameObject);
            }
            buttons.Clear();
        }

        public void JoinRespawn()
        {
            if (TGM_Manager.instance.localPlayer.awaitingRespawn == false)
            {
                TGM_Manager.instance.localPlayer.awaitingRespawn = true;
                SM.PlayGlobalUISound(SM.GlobalUISound.Beep, GM.CurrentPlayerBody.transform.position);
            }
            else
            {
                TGM_Manager.instance.localPlayer.awaitingRespawn = false;
                spawnButtonText.text = "Spawn";
                SM.PlayGlobalUISound(SM.GlobalUISound.Boop, GM.CurrentPlayerBody.transform.position);
            }
        }

        public void SpawnClass(int id)
        {
            if (id < 0)
            {
                TeamGameModePlugin.Logger.LogMessage(PluginInfo.NAME + "Spawn Class is -1 or less and will not be spawned");
                return;
            }

            int team = TGM_Manager.instance.localPlayer.iff;

            //Spawn Locking Per class?
            if (TGM_Manager.profile.gameSettings[(int)SettingEnum.SpawnLock] == 2)
            {
                GM.CurrentSceneSettings.IsSpawnLockingEnabled = TGM_Manager.instance.team[team].GetPlayerTeam().playerClasses[id].canSpawnLock;
            }
            else
            {
                GM.CurrentSceneSettings.IsSpawnLockingEnabled = TGM_Manager.profile.gameSettings[(int)SettingEnum.SpawnLock] >= 1 ? true : false;
            }

            //Set Player Health
            int healthSetting = TGM_Manager.profile.gameSettings[(int)SettingEnum.PlayerHealth];
            //If -1 use player class health otherwise use global setting
            if (healthSetting == -1)
                healthSetting = TGM_Manager.instance.team[team].GetPlayerTeam().playerClasses[id].playerHealth;
            GM.CurrentPlayerBody.SetHealthThreshold(healthSetting);
            GM.CurrentPlayerBody.Health = healthSetting;

            //Despawn any previously owned / held items
            TGM_Manager.instance.localPlayer.DestroyPlayersItems();

            //Spawn all weapons / items
            TGM_PlayerClass.SubClass subClass 
                = TGM_Manager.instance.team[team].GetPlayerTeam().playerClasses[id].subClasses[
                    Random.Range(0, TGM_Manager.instance.team[team].GetPlayerTeam().playerClasses[id].subClasses.Length)];

            //Spawn Locking
            if (TGM_Manager.profile.gameSettings[(int)SettingEnum.SpawnLock] == 2)
                GM.CurrentSceneSettings.IsSpawnLockingEnabled = TGM_Manager.instance.team[team].GetPlayerTeam().playerClasses[id].canSpawnLock;
            else
                GM.CurrentSceneSettings.IsSpawnLockingEnabled = TGM_Manager.profile.gameSettings[(int)SettingEnum.SpawnLock] == 0 ? false : true;

            //Spawn our sub classes items
            for (int x = 0; x < subClass.items.Length; x++)
            {
                SpawnItemSet(subClass.items[x]);
            }
        }

        public static Transform GetMainSpawnPoint()
        {
            Transform spawnPoint = instance.mainSpawns[spawnMainIndex];
            spawnMainIndex++;
            if (spawnMainIndex >= instance.mainSpawns.Length)
            {
                spawnMainIndex = 0;
                spawnMainOffset += 0.5f;
            }
            return spawnPoint;;
        }

        public static void SpawnItemSet(TGM_PlayerClass.ItemSet itemSet)
        {
            FVRObject spawnFVRObject = Global.GetObjectID(itemSet.objectID[Random.Range(0, itemSet.objectID.Length)]);

            //Spawn Object IDs
            for (int i = 0; i < itemSet.objectCount; i++)
            {
                FVRPhysicalObject newItem = null;

                //Randomise each Object
                if (!itemSet.uniformObjects && i != 0)
                    spawnFVRObject = Global.GetObjectID(itemSet.objectID[Random.Range(0, itemSet.objectID.Length)]);

                Transform mainSpawn;
                if (spawnFVRObject.Category != FVRObject.ObjectCategory.Firearm
                    && spawnFVRObject.Category != FVRObject.ObjectCategory.MeleeWeapon)
                    mainSpawn = instance.ammoSpawns[3];
                else
                    mainSpawn = GetMainSpawnPoint();

                newItem = Global.SpawnFVRObject(
                    spawnFVRObject,
                    mainSpawn.position + (Vector3.up * spawnMainOffset),
                    mainSpawn.rotation.eulerAngles);

                //Add to our local player item tracking
                if (newItem != null)
                    TGM_Manager.instance.localPlayer.playersItems.Add(newItem);
                else
                    return;

                //Spawn Required Secondary Pieces for Main
                if (itemSet.requiredSecondaryPieces == true 
                    && spawnFVRObject.RequiredSecondaryPieces != null 
                    && spawnFVRObject.RequiredSecondaryPieces.Count > 0)
                {
                    //Loop through and spawn each Piece
                    for (int s = 0; s < spawnFVRObject.RequiredSecondaryPieces.Count; s++)
                    {
                        if (spawnFVRObject.RequiredSecondaryPieces[s] != null)
                        {
                            Object.Instantiate(
                                spawnFVRObject.RequiredSecondaryPieces[s].GetGameObject(),
                                instance.ammoSpawns[4].position,
                                instance.ammoSpawns[4].rotation);
                        }
                    }
                }

                //Spawn Ammo
                if (itemSet.ammoCount > 0)
                {
                    FVRObject spawnFVRAmmo;

                    if (itemSet.ammoContainerID == "")
                        spawnFVRAmmo = Global.GetAmmo(
                                spawnFVRObject,
                                itemSet.minCapacity,
                                itemSet.maxCapacity,
                                (FireArmRoundClass)itemSet.ammoFireArmRoundClass,
                                (AmmoEnum)itemSet.ammoType);
                    else
                        spawnFVRAmmo = Global.GetObjectID(itemSet.ammoContainerID);

                    if (spawnFVRAmmo == null)
                    {
                        TeamGameModePlugin.Logger.LogMessage(itemSet.name + " is missing valid ammo or Ammo Container ID");
                        continue;
                    }

                    //Get Our Round
                    FireArmRoundClass singleRoundClass = (FireArmRoundClass)(-1);

                    if (Global.GetLoadType(spawnFVRAmmo) != AmmoLoadType.Rounds
                        && Global.GetLoadType(spawnFVRAmmo) != AmmoLoadType.None)
                    {
                        //Round Type ONCE for all Magazines etc
                        if (itemSet.ammoFireArmRoundClass == -1)
                        {
                            FVRObject round = Global.GetRandomRoundClass(
                                Global.GetAllRounds(),
                                (FireArmRoundClass)(-1),
                                (AmmoEnum)itemSet.ammoType);

                            singleRoundClass = Global.GetFirearmRoundClassFromFVRObject(round);
                        }
                        else
                            singleRoundClass = (FireArmRoundClass)itemSet.ammoFireArmRoundClass;
                    }

                    for (int x = 0; x < itemSet.ammoCount; x++)
                    {
                        FVRPhysicalObject newAmmo = null;

                        //Setup Ammo to Spawn
                        if (!itemSet.ammoUniform && x != 0)
                        {
                            spawnFVRAmmo = Global.GetAmmo(
                                spawnFVRObject,
                                itemSet.minCapacity,
                                itemSet.maxCapacity,
                                (FireArmRoundClass)itemSet.ammoFireArmRoundClass,
                                (AmmoEnum)itemSet.ammoType);
                        }

                        //Spawn the Ammo
                        newAmmo = Global.SpawnFVRObject(
                            spawnFVRAmmo,
                            instance.ammoSpawns[spawnMainIndex].position + (Vector3.up * spawnMainOffset),
                            instance.ammoSpawns[spawnMainIndex].rotation.eulerAngles);


                        //IF a Ammo Container, fill with ammo type
                        switch (Global.GetLoadType(spawnFVRAmmo))
                        {
                            default:
                            case AmmoLoadType.Rounds:
                            case AmmoLoadType.None:
                                continue;
                            case AmmoLoadType.Magazine:
                                FVRFireArmMagazine fvrfireArmMagazine = newAmmo.GetComponent<FVRFireArmMagazine>();
                                fvrfireArmMagazine.ReloadMagWithType(singleRoundClass);
                                break;
                            case AmmoLoadType.Clip:
                                FVRFireArmClip ammoClip = newAmmo.GetComponent<FVRFireArmClip>();
                                ammoClip.ReloadClipWithType(singleRoundClass);
                                break;
                            case AmmoLoadType.SpeedLoader:
                                Speedloader ammoSpeeloader = newAmmo.GetComponent<Speedloader>();
                                ammoSpeeloader.ReloadClipWithType(singleRoundClass);
                                break;
                        }
                    }
                }
            }
        }

        void OnDrawGizmos()
        {
            for (int i = 0; i < mainSpawns.Length; i++)
            {
                if (mainSpawns[i])
                    Gizmos.DrawLine((-mainSpawns[i].forward * spawnRange) + mainSpawns[i].position,
                        (mainSpawns[i].forward * spawnRange) + mainSpawns[i].position);
            }

            for (int i = 0; i < ammoSpawns.Length; i++)
            {
                if (ammoSpawns[i] != null)
                    Gizmos.DrawLine((-ammoSpawns[i].forward * spawnRange) + ammoSpawns[i].position, 
                        (ammoSpawns[i].forward * spawnRange) + ammoSpawns[i].position);
            }
        }
    }
}
