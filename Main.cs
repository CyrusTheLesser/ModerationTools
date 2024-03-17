using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using BepInEx;
using CitrusLib;
using Epic.OnlineServices;
using Epic.OnlineServices.AntiCheatCommon;
using Epic.OnlineServices.Connect;
using HarmonyLib;
using Landfall.Network;
using UnityEngine;


namespace ModerationTools
{
    [BepInPlugin(pluginGuid, pluginName, pluginVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string pluginGuid = "citrusbird.tabg.modTools";
        public const string pluginName = "ModerationTools";
        public const string pluginVersion = "0.4";

        public void Awake()
        {
            Moderation.log.Log("Moderation Tools Started!");

            Harmony harmony = new Harmony(pluginGuid);


            harmony.PatchAll();
           

            Citrus.AddCommand("kick", delegate (string[] prms, TABGPlayerServer player)
            {
                byte ind;
                if (!byte.TryParse(prms[0], out ind))
                {
                    Citrus.SelfParrot(player, "kick/ban uses playerindex to ban. use /list to list all players in console, or /id <playername> to get their id");
                    return;
                }
                if (!KickBan(ind, false))
                {
                    Citrus.SelfParrot(player, "Couldn't find player with ID: " + ind);
                    return;
                }
            },pluginName, "Kicks a player by index.", "[index]",1);

            Citrus.AddCommand("ban", delegate (string[] prms, TABGPlayerServer player)
            {
                byte ind;
                if (!byte.TryParse(prms[0], out ind))
                {
                    Citrus.SelfParrot(player, "kick/ban uses playerindex to ban. use /list to list all players in console, or /id <playername> to get their id");
                    return;
                }
                if (!KickBan(ind, true))
                {
                    Citrus.SelfParrot(player, "Couldn't find player with ID: " + ind);
                    return;
                }
            },pluginName, "bans a player by index.", "[index]",2);

            Citrus.AddCommand("banupdate", delegate (string[] prms, TABGPlayerServer player)
            {



                Citrus.SelfParrot(player, "list has been double checked!");
            }, pluginName, "re-reads the black/white list and kicks anyone who is not allowed", "", 1);


            Citrus.ExtraSettings.AddSetting(new GameSetting
            {
                name = "Modtools: List Location",
                value = "",
                description = "Path to FOLDER to store white/black list. leave black for local, and use 'PERM' to use the permlist path."
            });

            Citrus.ExtraSettings.AddSetting(new GameSetting
            {
                name = "Modtools: WhiteList",
                value = false.ToString(),
                description = "Whether the list behavesas a blacklist (false) or whitelist (true)"
            });


            Moderation.Setup();

        }


        static bool KickBan(byte ind, bool ban)
        {

            TABGPlayerServer playa = null;

            playa = Citrus.World.GameRoomReference.FindPlayer(ind);
            if (playa != null)
            {


                if (ban)
                {
                    if (Moderation.whitelist)
                    {
                        Moderation.log.Log("Server is in Whitelist mode! cannot ban user " + playa.PlayerName + " with epic ID: " + playa.EpicUserName);
                    }
                    else
                    {
                        Moderation.Ban(playa.EpicUserName);
                    }

                }
                Citrus.Kick(playa, ban ? KickReason.PermanentBanned : KickReason.TemporaryBanned, "Command kicked by admin");
                return true;
            }
            return false;
        }


    }



    [HarmonyPatch(typeof(RoomInitRequestCommand), "OnVerifiesEpicToken")]
    class VerifyPatch
    {
        static bool Prefix(ref VerifyIdTokenCallbackInfo data)
        {


            TABGPlayerServer tabgplayerServer = data.ClientData as TABGPlayerServer;

            if (tabgplayerServer == null)
            {
                Moderation.log.LogError("Error player in client data token callback");
                return false;
            }
            Utf8String epicUserID;
            if (data.ProductUserId.ToString(out epicUserID) != Result.Success)
            {
                Moderation.log.LogError("Error ToString ProductuserID, Verify Epic Token");
                Citrus.Kick(tabgplayerServer, KickReason.AuthenticationFailed, "");
                return false;
            }
            tabgplayerServer.SetEpicUserID(epicUserID);


            

            

            Moderation.log.Log("Checking if Player is banned!");
            Moderation.CheckBan(tabgplayerServer);
            return true;
        }
    }

    [HarmonyPatch(typeof(GameRoom), "InitActions")]
    class InitPatch
    {
        [HarmonyAfter(new string[]{"citrusbird.tabg.citruslib"})]
        static void PostFix()
        {
            Moderation.Setup();
        }
    }


    [HarmonyPatch(typeof(RoomInitRequestCommand),"Run")]
    class RoomInitPatch
    {
        static bool Prefix(byte[] msgData, ServerClient world, byte playerID)
        {
            
            string text = string.Empty;
            int num = 0;
            string text2 = string.Empty;
            string b = string.Empty;
            string text3;
            string phrase;
            ulong num2;
            bool flag;
            byte b2;
            int[] array;
            string steamTicket;
            bool flag2;
            Utf8String jsonWebToken;
            Utf8String productUserIdString;
            bool flag3;
            using (MemoryStream memoryStream = new MemoryStream(msgData))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream))
                {
                    text3 = binaryReader.ReadString();
                    phrase = binaryReader.ReadString();
                    num2 = binaryReader.ReadUInt64();
                    flag = binaryReader.ReadBoolean();
                    b2 = binaryReader.ReadByte();
                    array = new int[binaryReader.ReadInt32()];
                    for (int i = 0; i < array.Length; i++)
                    {
                        array[i] = binaryReader.ReadInt32();
                    }
                    steamTicket = binaryReader.ReadString();
                    flag2 = binaryReader.ReadBoolean();
                    text = binaryReader.ReadString();
                    int count = binaryReader.ReadInt32();
                    jsonWebToken = new Utf8String(binaryReader.ReadBytes(count));
                    count = binaryReader.ReadInt32();
                    productUserIdString = new Utf8String(binaryReader.ReadBytes(count));
                    num = binaryReader.ReadInt32();
                    flag3 = binaryReader.ReadBoolean();
                    if (binaryReader.PeekChar() != -1)
                    {
                        text2 = binaryReader.ReadString();
                    }
                    if (binaryReader.PeekChar() != -1)
                    {
                        b = binaryReader.ReadString();
                    }
                }
            }
            VerifyIdTokenOptions verifyIdTokenOptions = default(VerifyIdTokenOptions);
            IdToken value = new IdToken
            {
                JsonWebToken = jsonWebToken,
                ProductUserId = ProductUserId.FromString(productUserIdString)
            };
            verifyIdTokenOptions.IdToken = new IdToken?(value);

            Utf8String epicName = value.ProductUserId.ToString();
            Moderation.log.Log("Room Init Ban Check!");
            if (Moderation.isBanned(epicName))
            {
              
                
                Moderation.log.Log(string.Format("Player {0} is banned LOL, refusing entry: {1}", text3, ServerResponse.Error.ToString()));

                world.SendMessageToClients(EventCode.RoomInitRequestResponse, new byte[]
                {
                        0,
                        (byte)(ServerResponse.Error),
                        (byte)0,
                        b2,
                        Citrus.World.GameRoomReference.GetAllReservedSlots()
                }, playerID, true, false);
                return false;
            }
            else
            {
                return true;
            }

        }
    }

}
