using UnityEngine;
using UnityEngine.UI;
using FistVR;

namespace TeamsGameMode
{
    public class TGM_Compass : MonoBehaviour
    {
        public static TGM_Compass instance;

        public float distance = .25f;

        [Header("Health")]
        public Image healthFill;
        public Color healthFull = Color.green;
        public Color healthEmpty = Color.red;

        [Header("Stats")]
        public Text killText;
        public Text deathText;
        public Text scoreText;

        [Header("Teams")]
        public Text redScoreText;
        public Text blueScoreText;
        public Transform scorePanel;

        [Header("Markers")]
        public Marker[] markers;

        public Sprite[] markerSprites;
        public bool colorBlind = false;

        [Header("Direction")]
        public Transform directionNeedle;
        public Text directionText;

        [System.Serializable]
        public class Marker
        {
            public Transform parent;    //Center of compass
            public Transform marker;    //Edge of compass
            public Image thumbnail;
            public Transform target;

            public void LookAtTarget()
            {
                if (target == null)
                    return;

                parent.LookAt(target);
                parent.rotation = Quaternion.Euler(0, marker.rotation.eulerAngles.z, 0);
                marker.rotation = Quaternion.Euler(marker.rotation.eulerAngles.x, 0, 0);
            }
        }

        public void SetMarkerTaget(Marker marker, Transform newTarget, MarkerEnum type)
        {
            marker.target = newTarget;
            //marker.thumbnail.color = markerColors[(int)type];
            marker.thumbnail.sprite = markerSprites[(int)type];

        }

        public enum MarkerEnum
        {
            Attack,
            Defend,
        }

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            //TODO proper checks
            if (TGM_Manager.instance == null || GM.CurrentPlayerBody == null)
                return;

            //Compass Position
            if (!TGM_MainMenu.handSide)
                transform.position = GM.CurrentPlayerBody.LeftHand.position - GM.CurrentPlayerBody.LeftHand.forward * distance;
            else
                transform.position = GM.CurrentPlayerBody.RightHand.position - GM.CurrentPlayerBody.RightHand.forward * distance;
            transform.rotation = Quaternion.identity;

            //Markers
            for (int i = 0; i < markers.Length; i++)
            {
                markers[i].LookAtTarget();
            }

            //Stats
            killText.text = TGM_Manager.instance.localPlayer.kills.ToString();
            deathText.text = TGM_Manager.instance.localPlayer.deaths.ToString();

            int playerIFF = TGM_Manager.instance.localPlayer.iff;
            if(playerIFF == 0 || playerIFF ==1)
                scoreText.text = TGM_Manager.instance.team[playerIFF].currentScore.ToString();

            //Health
            healthFill.fillAmount = GM.GetPlayerHealth();
            healthFill.color = Color.Lerp(healthEmpty, healthFull, GM.GetPlayerHealth());

            //Direction
            directionNeedle.LookAt(GM.CurrentPlayerBody.Head.position);
            directionText.text = Mathf.FloorToInt(directionNeedle.eulerAngles.y).ToString();
            directionNeedle.rotation = Quaternion.Euler(90, directionNeedle.eulerAngles.y + 180, 0);

            //Team Score - If the panel is enabled
            if (scorePanel.gameObject.activeSelf)
            {
                scorePanel.LookAt(GM.CurrentPlayerBody.Head.position);
                scorePanel.rotation = Quaternion.Euler(90, scorePanel.eulerAngles.y + 180, 0);
                redScoreText.text = TGM_Manager.instance.team[0].currentScore.ToString();
                blueScoreText.text = TGM_Manager.instance.team[1].currentScore.ToString();
            }

        }
    }
}
