using FistVR;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamsGameMode;

public class TGM_Area : MonoBehaviour
{

    [Header("Gameplay")]
    [HideInInspector, Tooltip("Owner\nNeutral: -1\nRed: 0\nBlue: 1")]
    public int iff = -1; //Which team currently owns this area

    [Tooltip("The area which players can capture this point in if capturable")]
    public Transform capturePoint;
    [Tooltip("The location the objective will spawn, e.g. Flag for CTF")]
    public Transform objective;

    [Tooltip("The map defined objective time for this area, e.g. capture point time")]
    public float objectiveTime = 14f;

    public SpawnPoints[] spawns = new SpawnPoints[2];

    
    [Header("Player")]
    [Tooltip("Area where players can spawn, can be scaled")]
    public Transform[] spawnPoints;

    [Header("Sosigs")]
    [Tooltip("Area where sosigs can spawn, can be scaled")]
    public Transform[] sosigSpawnPoints;
    [Tooltip("Areas sosigs will navigate to when owned by enemies to find enemies")]
    public Transform[] sosigAttackAreas;
    [Tooltip("Defined areas sosigs will defend this Area")]
    public Transform[] sosigDefendAreas;
    

    [Header("Game Objects")]
    [Tooltip("Friendly Objects that enable when owned by a friendly team")]
    public GameObject[] friendlyObjects;
    [Tooltip("Enemy Objects that are enabled when owned by an enemy team, e.g. Nav Blockers to stop enemies getting into spawn area")]
    public GameObject[] enemyObjects;
    [Tooltip("Neutral Objects that are enabled when owned by no team, e.g. Door on unused spawn room")]
    public GameObject[] neutralObjects;

    [SerializeField]
    public class SpawnPoints()
    {
        [Header("Player")]
        [Tooltip("Area where players can spawn, can be scaled")]
        public Transform[] spawnPoints;

        [Header("Sosigs")]
        [Tooltip("Area where sosigs can spawn, can be scaled")]
        public Transform[] sosigSpawnPoints;
        [Tooltip("Areas sosigs will navigate to when owned by enemies to find enemies")]
        public Transform[] sosigAttackAreas;
        [Tooltip("Defined areas sosigs will defend this Area")]
        public Transform[] sosigDefendAreas;
    }

    public void UpdateArea()
    {
        int playerIFF = GM.CurrentPlayerBody.GetPlayerIFF();
        //Ally/Friendly
        if (iff == playerIFF)
        {
            for (int i = 0; i < friendlyObjects.Length; i++)
            {
                friendlyObjects[i].SetActive(true);
            }
            for (int i = 0; i < enemyObjects.Length; i++)
            {
                enemyObjects[i].SetActive(false);
            }
            for (int i = 0; i < neutralObjects.Length; i++)
            {
                neutralObjects[i].SetActive(false);
            }
        }
        else if (iff != playerIFF) //Enemy
        {
            for (int i = 0; i < friendlyObjects.Length; i++)
            {
                friendlyObjects[i].SetActive(false);
            }
            for (int i = 0; i < enemyObjects.Length; i++)
            {
                enemyObjects[i].SetActive(true);
            }
            for (int i = 0; i < neutralObjects.Length; i++)
            {
                neutralObjects[i].SetActive(false);
            }
        }
        else //Neutral
        {
            for (int i = 0; i < friendlyObjects.Length; i++)
            {
                friendlyObjects[i].SetActive(false);
            }
            for (int i = 0; i < enemyObjects.Length; i++)
            {
                enemyObjects[i].SetActive(false);
            }
            for (int i = 0; i < neutralObjects.Length; i++)
            {
                neutralObjects[i].SetActive(true);
            }
        }
    }

    public void OpenArea()
    {
        for (int i = 0; i < friendlyObjects.Length; i++)
        {
            friendlyObjects[i].SetActive(true);
        }
        for (int i = 0; i < enemyObjects.Length; i++)
        {
            enemyObjects[i].SetActive(false);
        }
        for (int i = 0; i < neutralObjects.Length; i++)
        {
            neutralObjects[i].SetActive(false);
        }

    }

    /// <summary>
    /// Returns 2 patrol points and 3rd index as rotation 
    /// </summary>
    /// <returns></returns>
    public List<Vector3> GetRandomAttackArea()
    {
        Transform area = sosigAttackAreas[Random.Range(0, sosigAttackAreas.Length)];
        return GetRandomAreaPositions(area);
    }

    public List<Vector3> GetRandomDefendArea()
    {
        Transform area = sosigDefendAreas[Random.Range(0, sosigAttackAreas.Length)];
        return GetRandomAreaPositions(area);
    }
    public List<Vector3> GetObjectiveArea()
    {
        Transform area = objective;
        return GetRandomAreaPositions(area);
    }

    private List<Vector3> GetRandomAreaPositions(Transform area)
    {
        List<Vector3> locations = new List<Vector3>();

        Vector3 areaScale = area.localScale / 2;

        for (int i = 0; i < 2; i++)
        {
            Vector3 pos = area.position + new Vector3(
                Random.Range(-areaScale.x, areaScale.x),
                Random.Range(-areaScale.y, areaScale.y),
                Random.Range(-areaScale.z, areaScale.z));

            locations.Add(pos);
        }

        //Look Direction
        locations.Add(area.rotation.eulerAngles);

        return locations;
    }

    public static Vector3[] attackIcon = {Vector3.up, (Vector3.up * 0.5f) + (Vector3.left * 0.5f), (Vector3.up * 0.5f) + (Vector3.right * 0.5f) };

    public void PlaceAllMarkersOnGround()
    {

    }


    void OnDrawGizmos()
    {
        if (objective != null)
        {
            Gizmos.color = new Color(0.99f, 0.75f, 0);
            Vector3 upPos = objective.position + (Vector3.up * 2);
            Vector3 forPos = upPos + (Vector3.down * 0.25f) + (objective.forward / 2);

            //Draw Flag
            Gizmos.DrawLine(objective.position, upPos);
            Gizmos.DrawLine(upPos, forPos);
            Gizmos.DrawLine(forPos, objective.position + (Vector3.up * 1.5f));
        }

        if (spawnPoints != null)
        {
            Color newGreen = Color.green;
            newGreen.a = 0.2f;
            Gizmos.color = newGreen;
            for (int i = 0; i < spawnPoints.Length; i++)
            {
                if (spawnPoints[i] == null)
                    continue;

                Gizmos.DrawCube(spawnPoints[i].position, spawnPoints[i].localScale / 2);
                Vector3 wireBottom = (spawnPoints[i].localScale / 2);
                wireBottom.y = 0.001f;
                Vector3 cubeBottom = spawnPoints[i].position + (Vector3.down * (spawnPoints[i].localScale.y * 0.25f));
                Gizmos.DrawWireCube(cubeBottom, wireBottom);
                Gizmos.DrawLine(spawnPoints[i].position, spawnPoints[i].position + spawnPoints[i].forward);
            }
        }

        if (sosigSpawnPoints != null)
        {
            Color newGreen = new Color(0.999f, 0.5f, 0);
            newGreen.a = 0.2f;
            Gizmos.color = newGreen;
            for (int i = 0; i < sosigSpawnPoints.Length; i++)
            {
                if (sosigSpawnPoints[i] == null)
                    continue;

                Gizmos.DrawCube(sosigSpawnPoints[i].position, sosigSpawnPoints[i].localScale / 2);
                Vector3 wireBottom = (sosigSpawnPoints[i].localScale / 2);
                wireBottom.y = 0.001f;
                Vector3 cubeBottom = sosigSpawnPoints[i].position + (Vector3.down * (sosigSpawnPoints[i].localScale.y * 0.25f));
                Gizmos.DrawWireCube(cubeBottom, wireBottom);
                Gizmos.DrawLine(sosigSpawnPoints[i].position, sosigSpawnPoints[i].position + sosigSpawnPoints[i].forward);
            }
        }

        if (sosigAttackAreas != null)
        {
            Color newColor = Color.red;
            newColor.a = 0.25f;
            Gizmos.color = newColor;
            for (int i = 0; i < sosigAttackAreas.Length; i++)
            {
                Gizmos.DrawCube(sosigAttackAreas[i].position, sosigAttackAreas[i].localScale / 2);
                for (int x = 0; x < attackIcon.Length; x++)
                {
                    Gizmos.DrawLine(sosigAttackAreas[i].position, sosigAttackAreas[i].position + attackIcon[x]);
                }
                Gizmos.DrawWireCube(sosigAttackAreas[i].position, sosigAttackAreas[i].localScale / 2);
            }
        }

        if (sosigDefendAreas != null)
        {
            Vector3 BL = (Vector3.up * 0.5f) + (Vector3.left * 0.5f);
            Vector3 TL = (Vector3.up * 0.5f) + (Vector3.left * 0.5f) + (Vector3.up * 0.5f);
            Vector3 BR = (Vector3.up * 0.5f) + (Vector3.right * 0.5f);
            Vector3 TR = (Vector3.up * 0.5f) + (Vector3.right * 0.5f) + (Vector3.up * 0.5f);



            Color newColor = Color.blue;
            newColor.a = 0.25f;
            Gizmos.color = newColor;
            for (int i = 0; i < sosigDefendAreas.Length; i++)
            {
                Gizmos.DrawCube(sosigDefendAreas[i].position, sosigDefendAreas[i].localScale / 2);
                Gizmos.DrawWireCube(sosigDefendAreas[i].position, sosigDefendAreas[i].localScale / 2);

                Gizmos.DrawLine(sosigDefendAreas[i].position, sosigDefendAreas[i].position + BL);
                Gizmos.DrawLine(sosigDefendAreas[i].position, sosigDefendAreas[i].position + BR);
                Gizmos.DrawLine(sosigDefendAreas[i].position + BL, sosigDefendAreas[i].position + TL);
                Gizmos.DrawLine(sosigDefendAreas[i].position + BR, sosigDefendAreas[i].position + TR);
                Gizmos.DrawLine(sosigDefendAreas[i].position + TL, sosigDefendAreas[i].position + TR);
            }
        }

        //MATRIX

        if (capturePoint != null && capturePoint.gameObject.activeSelf == true)
        {
            Gizmos.matrix = Matrix4x4.TRS(capturePoint.position, capturePoint.rotation, capturePoint.lossyScale / 2);
            Gizmos.color = new Color(0.75f, 0, 1f, 0.25f);
            Gizmos.DrawCube(Vector3.zero, Vector3.one);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}
