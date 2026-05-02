using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using H3MP;
using H3MP.Networking;

namespace TeamsGameMode.H3MP;

public class TGM_Networking : MonoBehaviour
{
    public static TGM_Networking instance;

    private int updatePlayerStats = -1;

    #region old

    [Serializable]
    public class NetworkData
    {
        //public DataType Type { get; private set; }
        public object Value { get; private set; }

        public NetworkData(object value)
        {
            Value = value;
            //Type = GetDataType(value);
        }

        /*
        private DataType GetDataType(object value)
        {
            return value switch
            {
                byte => DataType.Byte,
                short => DataType.Short,
                ushort => DataType.UShort,
                int => DataType.Int,
                uint => DataType.UInt,
                long => DataType.Long,
                float => DataType.Float,
                double => DataType.Double,
                bool => DataType.Bool,
                string => DataType.String,
                _ => throw new Exception("Unsupported type")
            };
        }

        public enum DataType
        {
            Byte, 
            Short, 
            UShort, 
            Int, 
            UInt, 
            Long, 
            Float, 
            Double, 
            Bool, 
            String
        }
        */
    }

    /*
    [Serializable]
    public class NetworkData()
    {
        public DataType type = DataType.Bool;

        byte Byte;
        short Short;
        ushort UShort;
        int Int;
        uint UInt;
        long Long;
        float Float;
        double Double;
        bool Bool;
        string String;
        public enum DataType
        {
            Byte,
            Short,
            UShort,
            Int,
            UInt,
            Long,
            Float,
            Double,
            Bool,
            String,
        }
    }
    */
#endregion

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (Networking.IsHost() || Client.isFullyConnected)
            SetupPacketTypes();
    }

    void SetupPacketTypes()
    {
        //Server
        if (Networking.IsHost())
        {
            if (Mod.registeredCustomPacketIDs.ContainsKey("TGM_PlayerStatsUpdate"))
                updatePlayerStats = Mod.registeredCustomPacketIDs["TGM_PlayerStatsUpdate"];
            else
                updatePlayerStats = Server.RegisterCustomPacketType("TGM_PlayerStatsUpdate");
            Mod.customPacketHandlers[updatePlayerStats] = StatsUpdate_Handler;

        }
        else // Client
        {
            if (Mod.registeredCustomPacketIDs.ContainsKey("TGM_PlayerStatsUpdate"))
            {
                updatePlayerStats = Mod.registeredCustomPacketIDs["TGM_PlayerStatsUpdate"];
                Mod.customPacketHandlers[updatePlayerStats] = StatsUpdate_Handler;
            }
            else
            {
                ClientSend.RegisterCustomPacketType("TGM_PlayerStatsUpdate");
                Mod.CustomPacketHandlerReceived += StatsUpdate_Received;
            }
        }
    }
    //---------------------------------------------------------------
    //  Send and Receive
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    // STATS UPDATE
    // Send
    public void StatsUpdate_Send(TGM_Player player)
    {
        if (!Networking.ServerRunning() || Networking.IsClient())
            return;

        Packet packet = new Packet(updatePlayerStats);

        packet.Write(player.kills);
        ServerSend.SendTCPDataToAll(packet, true);

        TeamGameModePlugin.Logger.LogMessage($"Host - Stats Update " + player.kills);
    }

    // Receive
    void StatsUpdate_Handler(int clientID, Packet packet)
    {
        int totalKills = packet.ReadInt();

        TeamGameModePlugin.Logger.LogMessage($"Client - Level Update " + totalKills);
    }
    //---------------------------------------------------------------

    //---------------------------------------------------------------
    //(Client) Packet Handlers
    //---------------------------------------------------------------

    void StatsUpdate_Received(string identifier, int index)
    {
        if (identifier == "TGM_PlayerStatsUpdate")
        {
            updatePlayerStats = index;
            Mod.customPacketHandlers[index] = StatsUpdate_Handler;
            Mod.CustomPacketHandlerReceived -= StatsUpdate_Received;
        }
    }


    //---------------------------------------------------------------
    //WIP
    //---------------------------------------------------------------

    void Update_Handler(int clientID, Packet packet)
    {
        byte index = packet.ReadByte();

        if (UpdateHandlerEvent != null)
            UpdateHandlerEvent.Invoke(index, packet);
    }

    public static void UpdateScore(ushort uniqueID, Packet packet)
    {
        if (uniqueID != 14)
            return;


        int idIndex = packet.ReadInt();
        int teamAScore = packet.ReadInt();
        int teamBScore = packet.ReadInt();

    }

    void SendData()
    {
        UpdateHandlerEvent += UpdateScore;

        Packet packet = new Packet(updatePlayerStats);

        packet.Write((byte)14);
        ServerSend.SendTCPDataToAll(packet, true);
    }


    public delegate void UpdateHandlerDelegate(ushort uniqueID, Packet packet);
    public static event UpdateHandlerDelegate UpdateHandlerEvent;

}
