using UnityEngine;


namespace TeamsGameMode
{
    [CreateAssetMenu(fileName = "TGM Assets", menuName = "Teams Game Mode/TGM Assets", order = 1)]
    public class TGM_Assets : ScriptableObject
    {
        public TGM_Manager manager;
        public TGM_MainMenu mainMenu;
    }
}
