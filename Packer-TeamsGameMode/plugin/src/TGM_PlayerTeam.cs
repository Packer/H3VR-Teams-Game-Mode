using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TeamsGameMode
{
    public class TGM_PlayerTeam
    {
        public string name;
        public string description;
        public Sprite thumbnail;
        public TGM_PlayerClass[] playerClasses = new TGM_PlayerClass[1];
        public TGM_SosigTeam sosigTeam;
    }
}
