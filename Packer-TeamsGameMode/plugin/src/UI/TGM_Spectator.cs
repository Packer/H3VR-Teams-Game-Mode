using UnityEngine;
using UnityEngine.UI;
using H3MP.Networking;
using FistVR;


namespace TeamsGameMode
{
    public class TGM_Spectator : MonoBehaviour
    {
        public static TGM_Spectator instance;

        public int targetPlayerIndex = 0;
        public Camera spectatorCamera;

        public Transform target;
        public Vector3 targetOffset;

        public LayerMask mask;
        private RaycastHit hit;


        void Awake()
        {
            instance = this;
        }

        void FixedUpdate()
        {
            if (!spectatorCamera.gameObject.activeSelf)
                return;

            if (target == null)
                target = GetNewTarget(1);
            if (target == null)
                return;

            Vector3 targetHead = target.position + (Vector3.up * 1.5f);
            Vector3 targetBehind = target.position + (Vector3.up * 1.5f) + (-transform.forward * 2);

            if (Physics.Linecast(targetHead, targetBehind, out hit, mask))
                spectatorCamera.transform.position = Vector3.Slerp(spectatorCamera.transform.position, hit.point, Time.deltaTime);
            else
                spectatorCamera.transform.position = Vector3.Slerp(spectatorCamera.transform.position, targetBehind, Time.deltaTime);

            spectatorCamera.transform.LookAt(targetHead);
        }

        public void GetNextTarget(int adjust)
        {
            target = GetNewTarget(adjust);
        }

        Transform GetNewTarget(int adjust)
        {
            targetPlayerIndex += adjust;

            int sosigCount = 0;
            int playerCount = 0;

            //Yay for no good Sosig Tracking
            Sosig[] sosigs = FindObjectsOfType<Sosig>();
            sosigCount = sosigs.Length;

            if (Networking.ServerRunning())
            {
                //Include Players
                playerCount = Networking.GetPlayerCount();
            }

            int total = sosigCount + playerCount;

            if (targetPlayerIndex > total)
                targetPlayerIndex = 0;
            else if (targetPlayerIndex < 0)
                targetPlayerIndex = total - 1;
            else
                targetPlayerIndex++;

            //Not a player
            if (targetPlayerIndex > playerCount 
                && sosigs[targetPlayerIndex] != null
                && sosigs[targetPlayerIndex].Mustard > 0)
            {
                return sosigs[targetPlayerIndex].transform;
            }

            if (Networking.ServerRunning())
                return Networking.GetPlayer(targetPlayerIndex).head;

            return null;
        }

        public void Activate(bool state)
        {
            if (state)
            {
                spectatorCamera.gameObject.SetActive(true);
            }
            else
            {
                spectatorCamera.gameObject.SetActive(false);
            }
        }

    }
}
