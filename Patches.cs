using Epic.OnlineServices.AntiCheatCommon;
using Epic.OnlineServices.Auth;
using Landfall.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ModerationTools
{
    class Patches
    {

       

        

        //prefix. also no longer used
        /*
        public static bool ChatMessage(byte[] msgData, ServerClient world, byte sender)
        {
            TABGPlayerServer tabgplayerServer = Moderation.world.GameRoomReference.FindPlayer(sender);

            if (tabgplayerServer == null) return false;

            string message = "";
            byte index;

            using (MemoryStream memoryStream = new MemoryStream(msgData))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    index = binaryReader.ReadByte();
                    byte count = binaryReader.ReadByte();
                    message = Encoding.Unicode.GetString(binaryReader.ReadBytes((int)count));
                }

            }

            if (sender != index)
            {
                Moderation.InstaKick(tabgplayerServer, KickMessage.Kick_FirstTimeUsingThisCheat, "Sending Chat from other player");
                return false;
            }

            if (tabgplayerServer.IsAdmin == true && message.StartsWith("/"))
            {
                //a command!
                string[] prms = message.Split(' ');

                LandLog.Log(string.Format("Player {0} used command: {1}",tabgplayerServer.PlayerName,message));


                switch (prms[0]){
                    case "/ban":
                    case "/kick":
                        byte ind;
                        TABGPlayerServer playa = null;
                        if (byte.TryParse(prms[1], out ind))
                        {
                            playa = world.GameRoomReference.FindPlayer(ind);
                        }
                        if(playa != null)
                        {

                            
                            if (prms[0] == "/ban")
                            {
                                if (Moderation.whitelist)
                                {
                                    LandLog.Log("Server is in Whitelist mode! cannot ban user "+ playa.PlayerName + " with epic ID: " + playa.EpicUserName);
                                }
                                else
                                {
                                    Moderation.Ban(tabgplayerServer.EpicUserName);
                                }
                                
                            }
                            Moderation.InstaKick(playa, KickMessage.Kick_Timeout1, "Command kicked by " + tabgplayerServer.PlayerName);
                        }


                        break;
                    default:
                        
                        break;
                }

                
            }
            return true;
        }

        */

        //i dont think we need this anymore as its patched in the base game! woo!
        /*
        public static bool ThrowItem(byte[] msgData, ServerClient world, byte sender)
        {
            TABGPlayerServer tabgplayerServer = Moderation.world.GameRoomReference.FindPlayer(sender);

            if (tabgplayerServer == null) return false;



            byte from;
            using (MemoryStream memoryStream = new MemoryStream(msgData))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    from = binaryReader.ReadByte();
                }
            }


            if (from != sender)
            {
                Moderation.InstaKick(tabgplayerServer, KickMessage.Kick_FirstTimeUsingThisCheat, "Throwing nade from other player");
                return false;
            }
            return true;

        }
        */
       
        
    }
}
