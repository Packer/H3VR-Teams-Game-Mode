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
                Mod.customPacketHandlers[updatePlayerStats] = LevelUpdate_Handler;

            }
            else // Client
            {
                if (Mod.registeredCustomPacketIDs.ContainsKey("TGM_PlayerStatsUpdate"))
                {
                    updatePlayerStats = Mod.registeredCustomPacketIDs["TGM_PlayerStatsUpdate"];
                    Mod.customPacketHandlers[updatePlayerStats] = LevelUpdate_Handler;
                }
                else
                {
                    ClientSend.RegisterCustomPacketType("TGM_PlayerStatsUpdate");
                    Mod.CustomPacketHandlerReceived += LevelUpdate_Received;
                }
            }
        }
        //---------------------------------------------------------------
        //  Send and Receive
        //---------------------------------------------------------------

        //Level Update Send
        public void LevelUpdate_Send(TGM_Player player)
        {
            if (!Networking.ServerRunning() || Networking.IsClient())
                return;

            Packet packet = new Packet(updatePlayerStats);

            packet.Write(player.iff);
            ServerSend.SendTCPDataToAll(packet, true);

            Debug.Log("TGM: Host - : " + SR_Manager.instance.CurrentCaptures);
        }

        //Level Update Receive
        void LevelUpdate_Handler(int clientID, Packet packet)
        {
            int totalCaptures = packet.ReadInt();

            if (gameComplete)
            {
                //Stats
                //SR_Manager.instance.CurrentCharacterLevel = characterLevel;
                //SR_Manager.instance.CurrentFactionLevel = factionLevel;
                SR_Manager.instance.CurrentCaptures = totalCaptures;

                SR_Manager.instance.gameCompleted = gameComplete;
                SR_Manager.instance.stats.ObjectiveComplete = objective;
                SR_Manager.instance.CompleteGame();
            }
            else
                SR_Manager.instance.SetLevel_Client(totalCaptures, supply, lastSupply, endless);


            Debug.Log("Supply Raid: Client - Level Update: " + totalCaptures);
        }

        //---------------------------------------------------------------
        //(Client) Packet Handlers
        //---------------------------------------------------------------

        void LevelUpdate_Received(string identifier, int index)
        {
            if (identifier == "TGM_PlayerStatsUpdate")
            {
                updatePlayerStats = index;
                Mod.customPacketHandlers[index] = LevelUpdate_Handler;
                Mod.CustomPacketHandlerReceived -= LevelUpdate_Received;
            }
        }
    }
}
