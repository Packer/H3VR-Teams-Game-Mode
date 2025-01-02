using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TeamsGameMode
{
    [Serializable]
    public class TGM_PlayerClass
    {
        public string name;
        public Sprite thumbnail;
        public string spriteName = "Image.png";
        public bool canSpawnLock = true;    //Spawnlock per class
        public int playerHealth = 5000;   //How much health does this class have
        public int minKills = -1;   //Minimum kills before usable / -1 no min limit
        public int maxKills = -1;   //Maximum kills before unusable / -1 max limit
        public SubClass[] subClasses;

        [Serializable]
        public class SubClass
        {
            public string name; //For Readiblity mostly
            public ItemSet[] items;
        }

        [Serializable]
        public class ItemSet()
        {
            public string name; //For readiblity
            [Header("Objects")]
            public int objectCount = 1;
            public bool uniformObjects = false; //If True will only spawn one type of objectID
            public bool requiredSecondaryPieces = true;
            public string[] objectID;   //Randomly Select per ObjectCount

            [Header("-Ammo-")]
            [Tooltip("Amount of Magazines/Clips/Ammo that will spawn")]
            public int ammoCount = 0;
            public bool ammoUniform = true;

            [Header ("Ammo Container")]
            [Tooltip("If not blank, specific ammo container will be spawned")]
            public string ammoContainerID = "";
            // --- OR ----
            [Tooltip("Magazine/Clip Min Capacity for this ammo container")]
            public int minCapacity = -1;
            [Tooltip("Magazine/Clip Max Capacity fo-r this ammo container")]
            public int maxCapacity = -1;

            [Header("Ammo Round Type")]
            [Tooltip("If not -1, will be preloaded with the defined ammo")]
            public int ammoFireArmRoundClass = -1;
            // --- OR ----
            [Tooltip("The preloaded or type the ammo will spawn as, AmmoEnum")]
            public int ammoType = 0;
        }
    }
}
