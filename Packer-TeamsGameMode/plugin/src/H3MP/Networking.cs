using System.Collections.Generic;
using H3MP.Scripts;
using UnityEngine;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Logging;

namespace H3MP.Networking;

public class Networking
{
    static byte enabled = 2; // 0 - No H3MP | 1 - H3MP | 2 - Not Checked 

    public static bool H3MPEnabled
    {
        get
        {
            if (enabled == 2)
                enabled = (byte)(Chainloader.PluginInfos.ContainsKey("VIP.TommySoucy.H3MP") ? 1 : 0);

            return enabled == (byte)1 ? true : false;
        }
    }

    /// <summary>
    /// Returns true if a server is running
    /// </summary>
    /// <returns></returns>
    public static bool ServerRunning()
    {
        if (H3MPEnabled)
            return isServerRunning;

        return false;
    }

    static bool isServerRunning
    {
        get
        {
            if (Mod.managerObject == null)
                return false;

            return true;
        }
    }

    /// <summary>
    /// Returns true if a server is Running AND the local player is a client
    /// </summary>
    /// <returns></returns>
    public static bool IsClient()
    {
        if (H3MPEnabled)
            return isClient();

        return false;
    }

    static bool isClient()
    {
        if (Mod.managerObject == null)
            return false;

        if (ThreadManager.host == false)
            return true;
        return false;
    }

    /// <summary>
    /// Returns true if a server is Running AND the local player is the host
    /// </summary>
    /// <returns></returns>
    public static bool IsHost()
    {
        if (H3MPEnabled)
            return isHosting;

        return false;
    }

    //Soft Dependency
    static bool isHosting
    {
        get
        {
            if (Mod.managerObject == null)
                return false;

            if (ThreadManager.host == true)
                return true;
            return false;
        }
    }

    public static int GetPlayerCount()
    {
        if (H3MPEnabled)
            return GetNetworkPlayerCount();
        return 1;
    }

    static int GetNetworkPlayerCount()
    {
        return GameManager.players.Count;
    }


    /*
    /// <summary>
    /// Returns array of PlayerManagers of the current connected players (Not including the local player).
    /// </summary>
    /// <returns></returns>
    public static PlayerManager[] GetPlayers()
    {
        PlayerManager[] playerArray = new PlayerManager[GameManager.players.Count];

        int i = 0;
        foreach (KeyValuePair<int, PlayerManager> entry in GameManager.players)
        {
            playerArray[i] = entry.Value;
            i++;
        }
        return playerArray;
    }
    */

    /// <summary>
    /// Returns array of all players IDs (Does not include local player)
    /// </summary>
    /// <returns></returns>
    public static int[] GetPlayerIDs()
    {
        int[] playerArray = new int[GameManager.players.Count];

        int i = 0;
        foreach (KeyValuePair<int, PlayerManager> entry in GameManager.players)
        {
            playerArray[i] = entry.Key;
            i++;
        }

        return playerArray;
    }

    /// <summary>
    /// Returns the local player's id.
    /// </summary>
    /// <returns></returns>
    public static int GetLocalPlayerID()
    {
        return GameManager.ID;
    }

    /// <summary>
    /// Returns the Custom Packet ID
    /// </summary>
    /// <param name="identifier"></param>
    /// <returns></returns>
    public static int RegisterHostCustomPacket(string identifier)
    {
        int id;
        if (Mod.registeredCustomPacketIDs.ContainsKey(identifier))
            id = Mod.registeredCustomPacketIDs[identifier];
        else
            id = Server.RegisterCustomPacketType(identifier);

        return id;
    }

    /// <summary>
    /// Returns the Gamemanager player at index i, does not include the local player. 
    /// Use FistVR.GM.CurrentPlayerBody for local player.
    /// </summary>
    /// <param name="i"></param>
    /// <returns></returns>
    public static PlayerData GetPlayer(int i)
    {
        //Only get player if they exist
        if (i < GameManager.players.Count)
            return PlayerData.GetPlayer(i);
        else
            return null;    //Maybe return a blank player instead (vector3 zero)
    }




    //---------------------------------------------------------------
    //WIP
    //---------------------------------------------------------------


    private static int highLevelID = -1;
    private bool HasSetup = false;

    //Database of all mods using this
    private static Dictionary<string, ushort> modDB = new Dictionary<string, ushort>();
    public delegate void UpdateHandlerDelegate(string uniqueID, Packet packet);
    public static event UpdateHandlerDelegate UpdateHandlerEvent;


    // ----------------------------------------------
    // Mod Example Zone
    // ----------------------------------------------
    public string modName = "ModExample";

    void ModStart()
    {
        //Shorten for Mods but the namespace etc
        H3MP.Networking.Networking.UpdateHandlerEvent += DataReciever;
        H3MP.Networking.Networking.ModSetup(modName);
    }

    void DataSender()
    {
        Packet packet = new Packet();
        packet.Write("Example");
        SendData(packet);
    }

    void DataReciever(string mod, Packet packet)
    {
        if (mod != modName)
            return;
    }

    // ----------------------------------------------
    // Auto Networking Setup
    // ----------------------------------------------

    void PluginStart()
    {
        //Add our Setup method to when we Host a game (OnHostClicked)
        Mod.OnConnection += Setup;
    }

    void Setup()
    {
        if (HasSetup)
            return;
        HasSetup = true;

        //SETUP HERE
    }

    public static void ModSetup(string modName)
    {
        ushort index = (ushort)(modDB.Count + 1);

        modDB.Add(modName, index);
    }


    /// <summary>
    /// The generic data reciever for H3MP Mods
    /// </summary>
    /// <param name="clientID"></param>
    /// <param name="packet"></param>
    void Update_Handler(int clientID, Packet packet)
    {
        ushort index = packet.ReadUShort();

    }

    public static void SendData(Packet packet)
    {
        ServerSend.SendTCPDataToAll(packet, true);
    }
}


[System.Serializable]
public class NetworkData
{
    public object Value { get; private set; }
    /// <summary>
    /// The previous value set on this Network Data
    /// </summary>
    public object LastValue { get; private set; }

    public NetworkData(object value)
    {
        Value = value;
        LastValue = value;
    }
}

[System.Serializable]
public class PlayerData
{
    public Transform head;
    public string username;
    public Transform handLeft;
    public Transform handRight;
    public int ID;
    public float health;
    public int iff;

    /*
    public PlayerData(Transform playerHead, string playerName, Transform leftHand, Transform rightHand)
    {
        head = playerHead;
        username = playerName;
        handLeft = leftHand;
        handRight = rightHand;
    }
    */

    public static PlayerData GetPlayer(int i)
    {
        return new PlayerData
        {
            head = GameManager.players[i].head,
            username = GameManager.players[i].username,
            handLeft = GameManager.players[i].leftHand,
            handRight = GameManager.players[i].rightHand,
        };
    }

}