using UnityEngine;
using UnityEngine.AI;
using FistVR;
using System.Collections.Generic;

namespace TeamsGameMode
{
    public class Global
    {

        public static FVRObject GetObjectID(string objectID)
        {
            FVRObject mainObject;
            //Try to find the weapon ID
            if (!IM.OD.TryGetValue(objectID, out mainObject))
            {
                TeamGameModePlugin.Logger.LogMessage($"Cannot find object with id: " + objectID);
                return null;
            }
            return mainObject;
        }

        public static FVRPhysicalObject SpawnFVRObject(FVRObject fvrObject, Vector3 position, Vector3 rotation)
        {
            if (fvrObject == null)
                return null;

            FVRPhysicalObject spawnedMain = Object.Instantiate(fvrObject.GetGameObject(), position, Quaternion.Euler(rotation)).GetComponent<FVRPhysicalObject>();

            return spawnedMain;
        }

        public static Vector3 GetValidSpawnPoint(Transform transform)
        {
            Vector3 position = transform.position;
            Vector3 scale = transform.localScale;
            Vector3 randomPosition
                = new Vector3(
                    Random.Range(-scale.x * 0.5f, scale.x * 0.5f),
                    Random.Range(-scale.y * 0.5f, scale.y * 0.5f),
                    Random.Range(-scale.z * 0.5f, scale.z * 0.5f));

            //Assign Position
            if (NavMesh.SamplePosition(position + randomPosition, out NavMeshHit hit, scale.y, NavMesh.AllAreas))
                position = hit.position;

            return position;
        }

        public static AmmoLoadType GetLoadType(FVRObject fvrObject)
        {
            if (fvrObject.MagazineType != FireArmMagazineType.mNone)
                return AmmoLoadType.Magazine;

            if (fvrObject.CompatibleClips != null && fvrObject.CompatibleClips.Count > 0
                || fvrObject.ClipType != FireArmClipType.None)
                return AmmoLoadType.Clip;

            if (fvrObject.CompatibleSpeedLoaders != null && fvrObject.CompatibleSpeedLoaders.Count > 0)
                return AmmoLoadType.SpeedLoader;

            if (fvrObject.CompatibleSingleRounds != null && fvrObject.CompatibleSingleRounds.Count > 0)
                return AmmoLoadType.Rounds;

            return AmmoLoadType.None;
        }

        public static FVRObject GetAmmo(
            FVRObject fvrObject,
            int Min = -1,
            int Max = -1,
            FireArmRoundClass roundClass = (FireArmRoundClass)(-1),
            AmmoEnum ammoType = AmmoEnum.None)
        {
            if (fvrObject == null)
                return null;

            switch (GetLoadType(fvrObject))
            {
                default:
                case AmmoLoadType.None:
                    return null;
                case AmmoLoadType.Magazine:
                    //Populate Magazines
                    List<FVRObject> mags = new List<FVRObject>(ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Magazine]);

                    for (int i = mags.Count - 1; i >= 0; i--)
                    {
                        if (mags[i].MagazineType != fvrObject.MagazineType
                            || (Min != -1 && mags[i].MagazineCapacity < Min)
                            || (Max != -1 && mags[i].MagazineCapacity > Max))
                            mags.RemoveAt(i);
                    }

                    //No Magazine return
                    if (mags.Count == 0)
                        return null;

                    return mags[Random.Range(0, mags.Count)];
                case AmmoLoadType.Clip:
                    //Populate Clips
                    List<FVRObject> clips = new List<FVRObject>(ManagerSingleton<IM>.Instance.odicTagCategory[FVRObject.ObjectCategory.Clip]);
                    for (int i = clips.Count - 1; i >= 0; i--)
                    {
                        if (clips[i].ClipType != fvrObject.ClipType
                            || (Min != -1 && clips[i].MagazineCapacity < Min)
                            || (Max != -1 && clips[i].MagazineCapacity > Max))
                            clips.RemoveAt(i);
                    }

                    if (clips.Count == 0)
                        return null;

                    return clips[Random.Range(0, clips.Count)];
                case AmmoLoadType.SpeedLoader:
                    return fvrObject.CompatibleSpeedLoaders[Random.Range(0, fvrObject.CompatibleSpeedLoaders.Count)];
                case AmmoLoadType.Rounds:
                    List<FVRObject> singleRounds = new List<FVRObject>();
                    singleRounds.AddRange(fvrObject.CompatibleSingleRounds);
                    return GetRandomRoundClass(singleRounds, roundClass, ammoType);
            }
        }
        public static List<FVRObject> GetAllRounds()
        {
            List<FVRObject> gearIDs = new List<FVRObject>();

            //Loop through every item in the game and compare Keyword
            foreach (string key in IM.OD.Keys)
            {
                if (IM.OD.TryGetValue(key, out FVRObject fvrObject))
                {
                    if (fvrObject && fvrObject.Category == FVRObject.ObjectCategory.Cartridge)
                    {
                        gearIDs.Add(fvrObject);
                    }
                }
            }

            return gearIDs;
        }

        public static FVRObject GetRandomRoundClass(
            List<FVRObject> rounds, 
            FireArmRoundClass roundClass = (FireArmRoundClass)(-1),
            AmmoEnum ammoType = AmmoEnum.None)
        {

            for (int i = rounds.Count - 1; i >= 0; i--)
            {
                FireArmRoundClass foundRoundClass = GetFirearmRoundClassFromFVRObject(rounds[i]);

                if ((int)roundClass != -1)
                {
                    if (roundClass != foundRoundClass)
                        rounds.RemoveAt(i);
                }
                else if (ammoType != AmmoEnum.None)
                {
                    if (GetAmmoEnum(foundRoundClass) != ammoType)
                        rounds.RemoveAt(i);
                }
            }

            return rounds[Random.Range(0, rounds.Count)];
        }


        public static FireArmRoundClass GetFirearmRoundClassFromFVRObject(FVRObject round) //string itemID, FireArmRoundType t)
        {
            for (int i = 0; i < AM.SRoundDisplayDataDic[round.RoundType].Classes.Length; i++)
            {
                if (AM.SRoundDisplayDataDic[round.RoundType].Classes[i].ObjectID.ItemID == round.ItemID)
                    return AM.SRoundDisplayDataDic[round.RoundType].Classes[i].Class;
            }

            return AM.SRoundDisplayDataDic[round.RoundType].Classes[0].Class;
        }

        public static AmmoEnum GetAmmoEnum(FireArmRoundClass round)
        {
            switch (round)
            {

                //----ROUNDS--------------------------------
                case FireArmRoundClass.FMJ:
                case FireArmRoundClass.Ball:
                case FireArmRoundClass.Spitzer:
                case FireArmRoundClass.DSM_Swarm:
                case FireArmRoundClass.NERMAL:
                case FireArmRoundClass.MIRV:
                case FireArmRoundClass.a20FMJ:
                    return AmmoEnum.Standard;


                case FireArmRoundClass.JHP:
                case FireArmRoundClass.SP:
                case FireArmRoundClass.HighVelHP:
                case FireArmRoundClass.HyperVelHP:
                    return AmmoEnum.HollowPoint;

                case FireArmRoundClass.Tracer:
                case FireArmRoundClass.DSM_Tracer:
                case FireArmRoundClass.FLASHY:
                    return AmmoEnum.Tracer;

                case FireArmRoundClass.AP:
                case FireArmRoundClass.POINTYOWW:
                case FireArmRoundClass.DSM_TurboPenetrator:
                case FireArmRoundClass.a20AP:
                    return AmmoEnum.AP;

                case FireArmRoundClass.Incendiary:
                case FireArmRoundClass.X666_Baphomet:
                    return AmmoEnum.Incendiary;

                case FireArmRoundClass.APIncendiary:
                case FireArmRoundClass.a20APDS:
                    return AmmoEnum.API;

                case FireArmRoundClass.PlusP_FMJ:
                case FireArmRoundClass.SPESHUL:
                    return AmmoEnum.PlusP_FMJ;

                case FireArmRoundClass.PlusP_JHP:
                    return AmmoEnum.PlusP_JHP;

                case FireArmRoundClass.PlusP_API:
                    return AmmoEnum.PlusP_API;

                case FireArmRoundClass.Subsonic_FMJ:
                    return AmmoEnum.Subsonic_FMJ;

                case FireArmRoundClass.Subsonic_AP:
                    return AmmoEnum.Subsonic_AP;

                //----SHELLS---------------------------

                case FireArmRoundClass.Slug:
                case FireArmRoundClass.DSM_Slugger:
                case FireArmRoundClass.KS23_Barricade:
                    return AmmoEnum.Slug;

                case FireArmRoundClass.BuckShot00:
                case FireArmRoundClass.BuckShotNo2:
                case FireArmRoundClass.BuckShotNo4:
                case FireArmRoundClass.BuckShot000:
                case FireArmRoundClass.BuckShotNo1:
                case FireArmRoundClass.Double:
                case FireArmRoundClass.MEGA:
                case FireArmRoundClass.MegaBuckShot:
                case FireArmRoundClass.KS23_Buckshot:
                    return AmmoEnum.Buckshot;

                case FireArmRoundClass.Flechette:
                    return AmmoEnum.Flechette;

                case FireArmRoundClass.DragonsBreath:
                    return AmmoEnum.Flechette;

                case FireArmRoundClass.TripleHit:
                    return AmmoEnum.TripleHit;


                //----GRENADE LAUNCHER---------------------------

                case FireArmRoundClass.M397_AirBurst:
                case FireArmRoundClass.X214_SteelBreaker:
                case FireArmRoundClass.X477_CornerFrag:
                case FireArmRoundClass.M720A1prop0:
                case FireArmRoundClass.M720A1prop1:
                case FireArmRoundClass.M430A1:
                case FireArmRoundClass.RLV_HEF:
                case FireArmRoundClass.RLV_HEFJ:
                    return AmmoEnum.GrenadeHE;

                case FireArmRoundClass.M576_MPAPERS:
                    return AmmoEnum.GrenadeBuckshot;

                case FireArmRoundClass.M651_CSGAS:
                case FireArmRoundClass.KS23_CSGas:
                case FireArmRoundClass.Kol_Smokescreen:
                case FireArmRoundClass.RLV_SMK:
                case FireArmRoundClass.RLV_SF1:
                case FireArmRoundClass.RLV_TPM:
                    return AmmoEnum.GrenadeSmoke;

                //----Generic---------------------------

                case FireArmRoundClass.DSM_Volt:
                case FireArmRoundClass.DSM_Mag:
                case FireArmRoundClass.MF13g_Buck:
                case FireArmRoundClass.MF13g_Slugger:
                case FireArmRoundClass.MF13g_Blooper:
                case FireArmRoundClass.MF13g_Bleeder:
                case FireArmRoundClass.MF13g_Moonshot:
                case FireArmRoundClass.MF1850_Barbie:
                case FireArmRoundClass.MF1850_Drongo:
                case FireArmRoundClass.MF1850_Gobsmacka:
                case FireArmRoundClass.MF1232_Bushfire:
                case FireArmRoundClass.MF1232_FunnelSpider:
                case FireArmRoundClass.MF366_Retort:
                case FireArmRoundClass.MF366_Debuff:
                case FireArmRoundClass.MF366_Salute:
                case FireArmRoundClass.MFRPG_Classic:
                case FireArmRoundClass.MFRPG_RocketPop:
                case FireArmRoundClass.MFRPG_ToTheMoon:
                case FireArmRoundClass.MFRPG_RockIt:
                case FireArmRoundClass.MFRPG_CannedMeat:
                case FireArmRoundClass.MFRPG_WRONGAMMO:
                case FireArmRoundClass.BTSP:
                case FireArmRoundClass.MFStickyFrag:
                case FireArmRoundClass.MFStickyRobbieBurns:
                case FireArmRoundClass.MFStickyRustyNail:
                case FireArmRoundClass.MFStickyHighlandFling:
                case FireArmRoundClass.MFSyringeBloodfire:
                case FireArmRoundClass.MFSyringeKnockout:
                case FireArmRoundClass.MFSyringeRage:
                case FireArmRoundClass.CM_1:
                case FireArmRoundClass.CM_5:
                case FireArmRoundClass.CM_10:
                case FireArmRoundClass.CM_20:
                case FireArmRoundClass.CM_100:
                case FireArmRoundClass.CM_1000:
                    return AmmoEnum.Special;

                case FireArmRoundClass.Freedomfetti:
                case FireArmRoundClass.Cannonball:
                case FireArmRoundClass.X1776_FreedomParty:
                    return AmmoEnum.Firework;

                case FireArmRoundClass.Mortar:
                case FireArmRoundClass.FragExplosive:
                case FireArmRoundClass.Frag12FA:
                case FireArmRoundClass.Frag12HE:
                case FireArmRoundClass.DSM_Frag:
                case FireArmRoundClass.DSM_Mine:
                case FireArmRoundClass.Mk211:
                case FireArmRoundClass.BOOOMY:
                case FireArmRoundClass.M381_HighExplosive:
                case FireArmRoundClass.Kol_Frag:
                case FireArmRoundClass.Kol_HEAT:
                case FireArmRoundClass.Kol_Megabuck:
                case FireArmRoundClass.Kol_Inferno:
                case FireArmRoundClass.a20HE:
                case FireArmRoundClass.a20HEI:
                case FireArmRoundClass.a20SAPHEI:
                    return AmmoEnum.Explosive;

                //----MISC---------------------------

                case FireArmRoundClass.Flare:
                case FireArmRoundClass.MFFlareClassic:
                case FireArmRoundClass.MFFlareDangerClose:
                case FireArmRoundClass.MFFlareSunburn:
                case FireArmRoundClass.MFFlareConflagration:
                    return AmmoEnum.Flare;

                case FireArmRoundClass.KS23_Flash:
                case FireArmRoundClass.Kol_TriFlash:
                    return AmmoEnum.Flash;

                case FireArmRoundClass.M781_Practice:
                case FireArmRoundClass.X828_Aurora:
                    return AmmoEnum.Practice;

                //----Unsorted---------------------------

                default:
                    return AmmoEnum.Special;
            }
        }
    }


    /// <summary>
    /// The Ammo Table Ammo Type Array Position
    /// </summary>
    public enum AmmoEnum
    {
        None = -1,
        //----Rounds
        Standard = 0,   //FMJ / Default on Single Ammo types
        HollowPoint = 1,
        AP = 2,
        API = 3,
        Incendiary = 4,
        Tracer = 5,
        Subsonic_FMJ = 6,
        Subsonic_AP = 7,
        Subsonic_JHP = 8,
        PlusP_FMJ = 9,
        PlusP_JHP = 10,
        PlusP_API = 11,
        //----Shells
        Buckshot = 12,
        Slug = 13,
        TripleHit = 14,
        Flechette = 15,
        ShellHE = 16,
        //----Grenade Launchers
        GrenadeHE = 17,
        GrenadeSmoke = 18,
        GrenadeBuckshot = 19,
        //----Misc
        Practice = 20,
        Flare = 21,
        Flash = 22,
        Explosive = 23,
        Firework = 24,
        DragonsBreathe = 25,
        Random = 26,
        Special = 27,
    }

    public enum AmmoLoadType
    {
        None = -1,
        Magazine = 0,
        Clip = 1,
        SpeedLoader = 2,
        Rounds = 3,
    }
}
