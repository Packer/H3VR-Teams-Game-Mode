using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TeamsGameMode
{
    public class TGM_Manager : MonoBehaviour
    {
        public static TGM_Manager instance;
        public static TGM_Profile profile;
        public TGM_Gamemode[] gamemodes;

        public class GameSettings
        {
            public int teamCount = 2;
            public bool dropItemsOnDeath = false;
            public bool destroyItemsOnDeath = false;
        }

        public Transform playerSpawnPoint;
        public TGM_Teams teams;
        public TGM_Gamemode gamemode;
        public TGM_Player localPlayer = new TGM_Player();
        public TGM_Player[] players;
        public TGM_Player[] sosigs;
        public float nextSpawnWave = 0;

        [Header("Audio")]
        public AudioSource globalAudio;
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


        public void Setup()
        {
            teams = new TGM_Teams();
            TGM_Teams.instance = teams;
        }

        public static void PlayAudio(PlayAudioEnum type, bool pitchVariation = false)
        {
            AudioClip clip = null;

            switch (type)
            {
                case PlayAudioEnum.Confirm:
                    if (TGM_Scene.instance.audioConfirm)
                        clip = TGM_Scene.instance.audioConfirm;
                    else
                        clip = instance.audioConfirm;
                    break;
                default:
                case PlayAudioEnum.Press:
                    if (TGM_Scene.instance.audioPress)
                        clip = TGM_Scene.instance.audioPress;
                    else
                        clip = instance.audioPress;
                    break;
                case PlayAudioEnum.Fail:
                    if (TGM_Scene.instance.audioFail)
                        clip = TGM_Scene.instance.audioFail;
                    else
                        clip = instance.audioFail;
                    break;
                case PlayAudioEnum.Rearm:
                    if (TGM_Scene.instance.audioRearm)
                        clip = TGM_Scene.instance.audioRearm;
                    else
                        clip = instance.audioRearm;
                    break;
                case PlayAudioEnum.Point:
                    if (TGM_Scene.instance.audioPoint)
                        clip = TGM_Scene.instance.audioPoint;
                    else
                        clip = instance.audioPoint;
                    break;
                case PlayAudioEnum.Elimination:
                    if (TGM_Scene.instance.audioElimination)
                        clip = TGM_Scene.instance.audioElimination;
                    else
                        clip = instance.audioElimination;
                    break;
                case PlayAudioEnum.TeamWon:
                    if (TGM_Scene.instance.audioTeamWon)
                        clip = TGM_Scene.instance.audioTeamWon;
                    else
                        clip = instance.audioTeamWon;
                    break;
                case PlayAudioEnum.TeamLost:
                    if (TGM_Scene.instance.audioTeamLost)
                        clip = TGM_Scene.instance.audioTeamLost;
                    else
                        clip = instance.audioTeamLost;
                    break;
            }

            if (pitchVariation)
                instance.globalAudio.pitch = Random.Range(0.975f, 1.025f);
            else
                instance.globalAudio.pitch = 1;

            if (clip != null)
                instance.globalAudio.PlayOneShot(clip);
        }

        public static void PlayCustomAudio(AudioClip clip)
        {
            instance.globalAudio.PlayOneShot(clip);
        }

        public enum PlayAudioEnum
        {
            Confirm = 0,
            Press = 1,
            Fail = 2,
            Rearm = 3,
            Point = 4,
            Elimination = 5,
            TeamWon = 6,
            TeamLost = 7,        
        }
    }
}
