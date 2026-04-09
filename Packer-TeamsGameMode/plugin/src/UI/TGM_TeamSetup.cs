using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace TeamsGameMode
{
    public class TGM_TeamSetup : MonoBehaviour
    {
        public static TGM_TeamSetup instance;

        [Header("Browser")]
        [SerializeField] GameObject browserPanel;
        [SerializeField] GameObject browserButtonPrefab;
        [SerializeField] Transform browserContent;
        [SerializeField] Text browserTitle;
        [SerializeField] Text browserDescription;
        [SerializeField] Image browserThumbnail;
        private List<TGM_Button> browserButtons = new List<TGM_Button>();
        private int browserTeamID = -1;
        private int browserMode = 0;
        private int browserTeam = 0;
        private int browserIndex = 0;

        void Awake()
        {
            instance = this;
        }

        public void Setup()
        {
            //Default select first team
            //SelectTeam(0);

            //Setup Spawn points
            for (int i = 0; i < TGM_Manager.instance.team.Length; i++)
            {
                TGM_Manager.instance.team[i].currentSpawnArea = TGM_Scene.instance.teams[i].startSpawnArea;
            }

            //Update all areas
            for (int i = 0; i < TGM_Scene.instance.areas.Length; i++)
            {
                TGM_Scene.instance.areas[i].UpdateArea();
            }

        }

        /*
        public void SelectTeam(int iff)
        {
            selectedTeam = TGM_Manager.instance.team[iff];
            TGM_Manager.instance.localPlayer.iff = iff;
            UpdateSettings();
        }
        */

        public void SetSosigTeam()
        {
            
        }

        public void SetPlayerTeam()
        {
            
        }

        void UpdateSettings()
        {

        }


        //RED TEAM
        public void AdjustRedTeamScore(int amount)
        {
            TGM_Manager.instance.team[0].scoreGoal = Mathf.Clamp(TGM_Manager.instance.team[0].scoreGoal + amount, 1, int.MaxValue);
        }

        public void AdjustRedSosigCount(int amount)
        {
            TGM_Manager.instance.team[0].sosigLimit = Mathf.Clamp(TGM_Manager.instance.team[0].sosigLimit + amount, 0, 32);
        }

        public void OpenBrowserRed(int type)
        {
            OpenBrowser(type, 0);
        }

        //BLUE TEAM
        public void AdjustBlueTeamScore(int amount)
        {
            TGM_Manager.instance.team[1].scoreGoal = Mathf.Clamp(TGM_Manager.instance.team[1].scoreGoal + amount, 1, int.MaxValue);
        }

        public void AdjustBlueSosigCount(int amount)
        {
            TGM_Manager.instance.team[1].sosigLimit = Mathf.Clamp(TGM_Manager.instance.team[1].sosigLimit + amount, 0, 32);
        }

        public void OpenBrowserBlue(int type)
        {
            OpenBrowser(type, 1);
        }


        public void OpenBrowser(int type, int teamID)
        {
            browserMode = type;
            browserTeamID = teamID;

            //Clear old Buttons
            for (int i = browserButtons.Count - 1; i >= 0; i--)
            {
                if (browserButtons[i] != null)
                    Destroy(browserButtons[i].gameObject);
            }
            browserButtons.Clear();

            browserPanel.SetActive(true);
            if (type == 0)
            {
                TGM_PlayerTeam team = TGM_ModLoader.playerTeams[TGM_Manager.instance.team[browserTeamID].playerTeam];

                //Select Player Team
                browserTitle.text = team.name;
                browserDescription.text = team.description;
                browserThumbnail.sprite = team.thumbnail;

                for (int i = 0; i < TGM_ModLoader.playerTeams.Count; i++)
                {
                    TGM_Button btn = Instantiate(browserButtonPrefab, browserContent).GetComponent<TGM_Button>();
                    btn.gameObject.SetActive(true);
                    btn.index = i;
                    btn.buttons[0].image.sprite = TGM_ModLoader.playerTeams[i].thumbnail;
                    btn.texts[0].text = TGM_ModLoader.playerTeams[i].name;
                    browserButtons.Add(btn);
                }
            }
            else
            {
                TGM_SosigTeam sosigTeam = TGM_ModLoader.sosigTeams[TGM_Manager.instance.team[browserTeamID].sosigTeam];
                //Select Sosig Team
                browserTitle.text = sosigTeam.name;
                browserDescription.text = sosigTeam.description;
                browserThumbnail.sprite = sosigTeam.thumbnail;

                for (int i = 0; i < TGM_ModLoader.sosigTeams.Count; i++)
                {
                    TGM_Button btn = Instantiate(browserButtonPrefab, browserContent).GetComponent<TGM_Button>();
                    btn.gameObject.SetActive(true);
                    btn.index = i;
                    btn.buttons[0].image.sprite = TGM_ModLoader.sosigTeams[i].thumbnail;
                    btn.texts[0].text = TGM_ModLoader.sosigTeams[i].name;
                    browserButtons.Add(btn);
                }
            }
            
        }

        public void SelectBrowser(int i)
        {
            browserIndex = i;
            if (browserMode == 0)
            {
                TGM_PlayerTeam team = TGM_ModLoader.playerTeams[browserIndex];

                browserTitle.text = team.name;
                browserDescription.text = team.description;
                browserThumbnail.sprite = team.thumbnail;
            }
            else
            {
                TGM_SosigTeam team = TGM_ModLoader.sosigTeams[browserIndex];

                browserTitle.text = team.name;
                browserDescription.text = team.description;
                browserThumbnail.sprite = team.thumbnail;
            }
        }

        public void ConfirmBrowser()
        {
            if (browserMode == 0)
            {
                TGM_Manager.instance.team[browserTeamID].playerTeam = browserIndex;
            }
            else
            {
                TGM_Manager.instance.team[browserTeamID].sosigTeam = browserIndex;
            }

            //Clear old Buttons
            for (int i = browserButtons.Count - 1; i >= 0; i--)
            {
                if(browserButtons[i] != null)
                    Destroy(browserButtons[i].gameObject);
            }
            browserButtons.Clear();

            browserPanel.SetActive(false);
            UpdateSettings();
        }
    }
}
