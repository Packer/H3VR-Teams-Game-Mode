using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TeamsGameMode
{
    public class TGM_PlayerClass
    {
        private Sprite thumbnail;
        public bool canSpawnLock = true;    //Spawnlock per class
        public string[] objectIDs;  //Direct Object ID references
        public TableTags[] tables;


        [Serializable]
        public class TableTags
        {
            public LootTable.LootTableType type = LootTable.LootTableType.Firearm;
            public List<FVRObject.OTagSet> set = new List<FVRObject.OTagSet>();
            public List<FVRObject.OTagEra> eras = new List<FVRObject.OTagEra>();
            public List<FVRObject.OTagFirearmSize> sizes = new List<FVRObject.OTagFirearmSize>();
            public List<FVRObject.OTagFirearmAction> actions = new List<FVRObject.OTagFirearmAction>();
            public List<FVRObject.OTagFirearmFiringMode> modes = new List<FVRObject.OTagFirearmFiringMode>();
            public List<FVRObject.OTagFirearmFiringMode> excludeModes = new List<FVRObject.OTagFirearmFiringMode>();
            public List<FVRObject.OTagFirearmFeedOption> feedoptions = new List<FVRObject.OTagFirearmFeedOption>();
            public List<FVRObject.OTagFirearmMount> mounts = new List<FVRObject.OTagFirearmMount>();
            public List<FVRObject.OTagFirearmRoundPower> roundPowers = new List<FVRObject.OTagFirearmRoundPower>();
            public List<FVRObject.OTagAttachmentFeature> features = new List<FVRObject.OTagAttachmentFeature>();
            public List<FVRObject.OTagMeleeStyle> meleeStyles = new List<FVRObject.OTagMeleeStyle>();
            public List<FVRObject.OTagMeleeHandedness> meleeHandedness = new List<FVRObject.OTagMeleeHandedness>();
            public List<FVRObject.OTagPowerupType> powerupTypes = new List<FVRObject.OTagPowerupType>();
            public List<FVRObject.OTagThrownType> thrownTypes = new List<FVRObject.OTagThrownType>();
            public List<FVRObject.OTagThrownDamageType> thrownDamage = new List<FVRObject.OTagThrownDamageType>();
            public List<FVRObject.OTagFirearmCountryOfOrigin> countryOfOrigins = new List<FVRObject.OTagFirearmCountryOfOrigin>();
            public int yearFirst = -1;
            public int yearLast = -1;
            public List<string> subtractionID = new List<string>();
        }
    }
}
