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

        [Header("Teams")]
        [SerializeField] Transform teamsBarContent;
        [SerializeField] GameObject teamsBarPrefab;
        private List<TGM_Button> teamsButtons = new List<TGM_Button>();

        [SerializeField] TGM_Team selectedTeam;
        [SerializeField] GameObject addTeamButton;
        [SerializeField] GameObject removeTeamButton;
        [SerializeField] Image teamBackground;

        [Header("Player Team")]
        [SerializeField] Text teamTitle;
        [SerializeField] Text teamDescription;
        [SerializeField] Image teamThumbnail;

        [Header("Sosig Team")]
        [SerializeField] Text sosigTitle;
        [SerializeField] Text sosigDescription;
        [SerializeField] Image sosigThumbnail;

        [Header("Classes")]
        [SerializeField] Transform classesContent;
        [SerializeField] GameObject classesThumbnailPrefab;
        private List<Image> classPreviews = new List<Image>();

        [Header("Browser")]
        [SerializeField] GameObject browserPanel;
        [SerializeField] GameObject browserButtonPrefab;
        [SerializeField] Transform browserContent;
        [SerializeField] Text browserTitle;
        [SerializeField] Text browserDescription;
        [SerializeField] Image browserThumbnail;
        private int browserMode = 0;
        private int browserIndex = 0;

        void Awake()
        {
            instance = this;
        }

        public void Setup()
        {
            //Default select first team
            SelectTeam(0);
        }

        public void SelectTeam(int iff)
        {
            selectedTeam = TGM_Manager.instance.team[iff];
            UpdateSettings();
        }

        public void SetSosigTeam()
        {
            
        }

        public void SetPlayerTeam()
        {
            
        }

        void UpdateSettings()
        {
            //Color
            teamBackground.color = selectedTeam.color;


            //Player Team
            TGM_PlayerTeam team = selectedTeam.GetPlayerTeam();
            teamTitle.text = team.name;
            teamDescription.text = team.name;
            teamThumbnail.sprite = team.thumbnail;

            //Sosig Team
            TGM_SosigTeam sosigTeam = selectedTeam.GetSosigTeam();
            sosigTitle.text = sosigTeam.name;
            sosigDescription.text = sosigTeam.description;
            teamThumbnail.sprite = sosigTeam.thumbnail;

            //Player Classes
            //Clear old Images
            for (int i = 0; i < classPreviews.Count; i++)
            {
                Destroy(classPreviews[i].gameObject);
            }
            classPreviews.Clear();
            //Create new Images
            for (int i = 0; i < team.playerClasses.Length; i++)
            {
                Image img = Instantiate(classesThumbnailPrefab, classesThumbnailPrefab.transform.parent).GetComponent<Image>();
                img.sprite = team.playerClasses[i].thumbnail;
                classPreviews.Add(img);
            }

            //Old +2 Teams code
            /*
            int teamsMax = TGM_Scene.instance.teams.Length;
            int teamCount = TGM_Teams.instance.teams.Length;

            if (teamCount < teamsMax)
                addTeamButton.SetActive(true);
            else
                addTeamButton.SetActive(false);

            if (teamCount == teamsMax)
                removeTeamButton.SetActive(false);
            else
                removeTeamButton.SetActive(true);

            //Teams Bar
            for (int i = 0; i < teamsButtons.Count; i++)
            {
                Destroy(teamsButtons[i].gameObject);
            }
            teamsButtons.Clear();

            for (int i = 0; i < TGM_Teams.instance.teams.Length; i++)
            {
                TGM_Button btn = Instantiate(teamsBarPrefab, teamsBarPrefab.transform).GetComponent<TGM_Button>();
                btn.gameObject.SetActive(true);
                teamsButtons.Add(btn);
            }
            */
        }

        public void AdjustTeamScore(int amount)
        {
            selectedTeam.scoreGoal = Mathf.Clamp(selectedTeam.scoreGoal + amount, 1, int.MaxValue);
        }

        public void AdjustSosigCount(int amount)
        {
            selectedTeam.sosigCount = Mathf.Clamp(selectedTeam.sosigCount + amount, 0, 32);
        }

        public void OpenBrowser(int type)
        {
            browserMode = type;
            TGM_PlayerTeam team = selectedTeam.GetPlayerTeam();
            TGM_SosigTeam sosigTeam = selectedTeam.GetSosigTeam();

            browserPanel.SetActive(true);
            if (type == 0)
            {
                //Select Player Team
                browserTitle.text = team.name;
                browserDescription.text = team.description;
                browserThumbnail.sprite = team.thumbnail;

                for (int i = 0; i < TGM_ModLoader.playerTeams.Count; i++)
                {
                    TGM_Button btn = Instantiate(browserButtonPrefab, browserButtonPrefab.transform).GetComponent<TGM_Button>();
                    btn.gameObject.SetActive(true);
                    btn.index = i;
                    btn.buttons[0].image.sprite = TGM_ModLoader.playerTeams[i].thumbnail;
                    btn.texts[0].text = TGM_ModLoader.playerTeams[i].name;
                }
            }
            else
            {
                //Select Sosig Team
                browserTitle.text = sosigTeam.name;
                browserDescription.text = sosigTeam.description;
                browserThumbnail.sprite = sosigTeam.thumbnail;

                for (int i = 0; i < TGM_ModLoader.sosigTeams.Count; i++)
                {
                    TGM_Button btn = Instantiate(browserButtonPrefab, browserButtonPrefab.transform).GetComponent<TGM_Button>();
                    btn.gameObject.SetActive(true);
                    btn.index = i;
                    btn.buttons[0].image.sprite = TGM_ModLoader.sosigTeams[i].thumbnail;
                    btn.texts[0].text = TGM_ModLoader.sosigTeams[i].name;
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
                selectedTeam.playerTeam = browserIndex;
            }
            else
            {
                selectedTeam.sosigTeam = browserIndex;
            }

            browserPanel.SetActive(false);
            UpdateSettings();
        }
    }
}
