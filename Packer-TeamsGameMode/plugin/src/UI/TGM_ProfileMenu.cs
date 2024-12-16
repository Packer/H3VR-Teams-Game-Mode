using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using H3MP.Networking;

namespace TeamsGameMode
{
    public class TGM_ProfileMenu : MonoBehaviour
    {

        public static TGM_ProfileMenu instance;

        public InputField profileInput;
        public GameObject loadProfileButton;
        public GameObject profilePrefab;
        [SerializeField] List<TGM_Button> profileButtons = new List<TGM_Button>();

        public void Setup()
        {

            if (Networking.IsClient())
            {
                loadProfileButton.SetActive(false);
            }

            TGM_ModLoader.LoadProfiles();
        }

        public void Load()
        {
            if (profileInput.text == "")
            {
                TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.Fail);
                return;
            }

            //Loop Through each profile and find matching name
            for (int i = 0; i < TGM_ModLoader.profiles.Count; i++)
            {
                if (TGM_ModLoader.profiles[i].name == profileInput.text)
                {
                    TeamGameModePlugin.Logger.LogError($"Loading Profile - " + TGM_ModLoader.profiles[i].name);

                    TGM_Manager.profile = TGM_ModLoader.profiles[i];

                    //Setup Teams
                    for (int x = 0; x < TGM_Manager.profile.playerTeams.Count; x++)
                    {

                    }

                    /*
                    if (TGM_Manager.profile.character != "")
                        SetCharacterByName(TGM_Manager.profile.character);
                    if (TGM_Manager.profile.faction != "")
                        SetFactionByName(TGM_Manager.profile.faction);
                    */
                    //UpdateGameOptions();
                    TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.Confirm);
                    return;
                }
            }

            TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.Fail);
        }

        public void Save()
        {

        }
        public void SetProfile(string profileName)
        {
            if (profileName != "")
            {
                profileInput.text = profileName;
                TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.Press);
            }
            else
                TGM_Manager.PlayAudio(TGM_Manager.PlayAudioEnum.Fail);
        }

        public void PopulateProfiles()
        {
            for (int i = 0; i < profileButtons.Count; i++)
            {
                Destroy(profileButtons[i].gameObject);
            }
            profileButtons.Clear();

            for (int i = 0; i < TGM_ModLoader.profiles.Count; i++)
            {
                if (TGM_ModLoader.profiles[i] == null)
                    continue;

                TGM_Button btn = Instantiate(profilePrefab, profilePrefab.transform.parent).GetComponent<TGM_Button>();
                btn.gameObject.SetActive(true);
                btn.texts[0].text = TGM_ModLoader.profiles[i].name;
                profileButtons.Add(btn);
            }
        }
    }
}
