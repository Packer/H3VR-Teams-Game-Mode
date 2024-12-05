using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TeamsGameMode
{
    public class TGM_Manager : MonoBehaviour
    {
        public static TGM_Manager instance;
        public TGM_Gamemode[] gamemodes;

        public class GameSettings
        {
            public int teamCount = 2;
            public bool dropItemsOnDeath = false;
            public bool destroyItemsOnDeath = false;
        }

        public Transform playerSpawnPoint;
        public TGM_Teams teams = new TGM_Teams();
        public TGM_Gamemode gamemode;
        public TGM_Player localPlayer = new TGM_Player();
        public TGM_Player[] players;
        public TGM_Player[] sosigs;
        public float nextSpawnWave = 0;

        void Awake()
        {
            instance = this;
        }
    }
}
