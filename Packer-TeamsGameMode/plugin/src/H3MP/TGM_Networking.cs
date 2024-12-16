using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using H3MP;
using H3MP.Networking;

namespace TeamsGameMode.H3MP
{
    public  class TGM_Networking : MonoBehaviour
    {
        public static TGM_Networking instance;

        private int updatePlayerStats = -1;


        void Awake()
        {
            instance = this;
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
    }
}
