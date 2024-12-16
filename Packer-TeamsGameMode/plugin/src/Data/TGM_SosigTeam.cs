using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TeamsGameMode
{
    [Serializable]
    public class TGM_SosigTeam
    {
        [Tooltip("Name of this team of Sosigs")]
        public string name = "";
        [Tooltip("Short explanation of the team of sosigs"), Multiline(6)]
        public string description = "A short description of this sosig faction";
        [Tooltip("Preview image of the team when selected")]
        public Sprite thumbnail;

        [Tooltip("SosigEnemyID list, add duplicate IDs to weight them more likely to be picked")]
        public int[] sosigEnemyIDs;

    }
}
