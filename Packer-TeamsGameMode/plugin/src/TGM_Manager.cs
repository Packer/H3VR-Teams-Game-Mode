using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace TeamsGameMode
{
    public class TGM_Manager : MonoBehaviour
    {
        public static TGM_Manager instance;
        public static TGM_Profile profile;
        public static GameStateEnum gameState = GameStateEnum.GamemodeSelect;
        public TGM_Gamemode[] gamemodes;

        public Transform playerSpawnPoint;
        public TGM_Teams teams;
        public TGM_Gamemode gamemode;

        [Header("DATA")]
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


        public delegate void GameStateDelegate();
        public static event GameStateDelegate GameStateEvent;


        public void Setup()
        {
            teams = new TGM_Teams();
            TGM_Teams.instance = teams;
        }

        void Update()
        {
            if (gameState == GameStateEnum.Gameplay)
                gamemode.Update();
        }


        public void SetGameState(GameStateEnum state)
        {
            gameState = state;

            if (GameStateEvent != null)
                GameStateEvent.Invoke();

            switch (state)
            {
                case GameStateEnum.GamemodeSelect:
                    //TODO reset entire gamemode back (Or Reload level?)
                    break;
                case GameStateEnum.Setup:       //Gamemode being configured
                    gamemode.Setup();
                    break;
                case GameStateEnum.Start:       //30 sec Count down to game start
                    break;
                case GameStateEnum.Gameplay:    //Combat
                    break;
                case GameStateEnum.Finale:      //30 Sec post gameplay
                    break;
                case GameStateEnum.Gameover:    //Final Scores, Wait for master or wait for all players to ready
                    break;
            }
        }

        //------------------------------------------------------------------------------
        // Sosigs
        //------------------------------------------------------------------------------

        protected virtual IEnumerator ClearSosig()
        {
            yield return null;
        }

        //------------------------------------------------------------------------------
        // AUDIO
        //------------------------------------------------------------------------------

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

        public enum GameStateEnum
        {
            GamemodeSelect,
            Setup,
            Start,      //Countdown to game start
            Gameplay,
            Finale,     //30 secs post game before game over
            Gameover,   //Put everyone back into start room
        }
    }
}
