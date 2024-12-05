using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TeamsGameMode
{
    public class TGM_Area : MonoBehaviour
    {
        public int iff = 0;

        [Header("The area which players can capture this point in if capturable")]
        public Transform capturePoint;
        [Header("The location the objective will spawn, e.g. Flag for CTF")]
        public Transform objective;
        [Header("Area where players can spawn, can be scaled")]
        public Transform[] spawnPoints;
        [Header("Points of interest Sosigs will path to in this area")]
        public Transform[] sosigWaypoints;

        [Header("Navigation Blockers that enable when owned by this areas team")]
        public GameObject[] navBlockersFriendly;
        [Header("Navigation Blockers that are enabled not on this areas team, e.g. stop enemies getting into spawn area")]
        public GameObject[] navBlockersEnemy;



        void OnDrawGizmos()
        {

            if (objective != null)
            {
                Gizmos.color = Color.red;
                Vector3 upPos = objective.position + (Vector3.up * 2);
                Vector3 forPos = upPos - (Vector3.down / 2) + (objective.forward / 2);

                //Draw Flag
                Gizmos.DrawLine(objective.position, upPos);
                Gizmos.DrawLine(upPos, forPos);
                Gizmos.DrawLine(forPos, objective.position + (Vector3.up * 1.5f));
            }

            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] == null)
                    continue;

                Color newGreen = Color.green;
                newGreen.a = 0.2f;
                Gizmos.color = newGreen;
                Gizmos.DrawCube(spawnPoints[i].position, spawnPoints[i].localScale / 2);
            }

            //MATRIX

            if (capturePoint != null && capturePoint.gameObject.activeSelf == true)
            {
                Gizmos.matrix = Matrix4x4.TRS(capturePoint.position, capturePoint.rotation, capturePoint.lossyScale);
                Gizmos.color = new Color(0, 0, 1f, 0.25f);
                Gizmos.DrawCube(Vector3.zero, Vector3.one);
            }
        }
    }
}
