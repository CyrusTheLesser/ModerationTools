using Epic.OnlineServices;
using Landfall.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using Epic.OnlineServices.AntiCheatCommon;
using CitrusLib;
using Newtonsoft.Json;

namespace ModerationTools
{
    public static class Moderation
    {
        internal static SettingsFile<Banlist> banList;

        internal static CitLog log = new CitLog("Moderation Tools", ConsoleColor.Red);



        static bool setUp = false;
        //current ServerClient instance.
        static ServerClient currentWorld;

        //
        static List<Utf8String> playerList = new List<Utf8String>();

        static List<Utf8String> admins = new List<Utf8String>();


        //whether to function as a whitelist instead of a blacklist
        public static bool whitelist;

        

        public static ServerClient world
        {
            get
            {
                if (currentWorld != null)
                {
                    return currentWorld;
                }
                else
                {
                    currentWorld = UnityEngine.Object.FindObjectOfType<ServerClient>();
                    return currentWorld;
                }
            }
        }

        public static void CheckAll()
        {
            foreach(TABGPlayerServer player in world.GameRoomReference.Players)
            {
                CheckBan(player);
            }
        }

        public static void Ban(Utf8String id)
        {
            if (!playerList.Contains(id))
            {
                playerList.Add(id);
            }
            
        }


        //

        public static bool isBanned(Utf8String player)
        {
            bool onList = playerList.Contains(player);
            if (whitelist)
            {
                if (!onList)
                {

                    //PlayerKickCommand.Run(player, Moderation.world, KickReason.PermanentBanned, null);
                    return true;
                }

            }
            else
            {
                if (onList)
                {

                    return true;
                }

            }
            return false;
        }

        public static void CheckBan(TABGPlayerServer player)
        {
            if (player == null) return;
            if (!setUp) Setup();

            

            if(player.EpicUserName == null)
            {
                Citrus.Kick(player, KickReason.EpicVerifyFail, "Missing Epic ID!");
                //PlayerKickCommand.Run(player, Moderation.world, KickReason.PermanentBanned, null);
                return;
            }


            if (isBanned(player.EpicUserName))
            {
                Citrus.Kick(player, KickReason.PermanentBanned, whitelist ? "Not on whitelist!" : "player is banned!");
                return;
            }
            
            /*
            if(admins.Contains(player.EpicUserName))
            {
                player.SetAdmin(new string[] { player.SteamID });
                log.Log(string.Format("{0}, id {1} is an admin!", player.PlayerName, player.EpicUserName));
            }
            else
            {
                log.Log(string.Format("{0}, id {1} isnt {2}!",player.PlayerName,player.EpicUserName,whitelist ? "missing from the whitelist" : "banned"));
            }*/




        }

       


        //load the banlist and config
        public static void Setup()
        {
            setUp = true;
            DirectoryInfo directoryInfo = new DirectoryInfo(Application.dataPath);
            string text = Path.Combine(directoryInfo.Parent.FullName, "BanList");

            log.Log("loading moderation Settings!");


            GameSetting pathout;
            GameSetting white;

            //Citrus.ExtraSettings.ReadSettings(); //oop

            if (Citrus.ExtraSettings.TryGetSetting("Modtools: List Location",out pathout))
            {
                bool abs = false;
                string val = "";
                if (pathout.value == "PERM")
                {
                    GameSetting pathout2;
                    if(Citrus.ExtraSettings.TryGetSetting("AdminFileLocation", out pathout2))
                    {
                        val = pathout2.value;
                    }
                }
                else
                {
                    val = pathout.value;
                }

                if (val != "")
                {
                    abs = true;
                    val += "/BanList";
                }
                else
                {
                    val = "BanList";
                }

                

                banList = new SettingsFile<Banlist>(val, abs);

                banList.AddSetting(new Banlist
                {
                    description = "a list of epic ids. you can find these in the guestbook!",
                    name = "BanList",
                    list = new List<string>
                    {
                        "00029____EXAMPLE____6w34wnfe982a",
                        "00029____EXAMPLE____6w34wnfe982a",
                        "00029____EXAMPLE____6w34wnfe982a"
                    }
                });

                
            }

            if (banList == null)
            {
                log.LogError("Banlist is null! perhaps a bad setting somewhere??");
                return;
            }

            playerList = new List<Utf8String>();

            Banlist bl;

            if(banList.TryGetSetting("BanList",out bl))
            {
                playerList = bl.list.ConvertAll((s) => { return new Utf8String(s); });
            }

            GameSetting gs;
            whitelist = false;
            if (Citrus.ExtraSettings.TryGetSetting("Modtools: WhiteList", out gs))
            {
                if(! Boolean.TryParse( gs.value, out whitelist)){
                    log.LogError("erm... bad boolean value for whitelist setting");
                }
            }

            log.Log("MODERATION SETTINGS:");
            

            log.Log("Banned (or whitelisted) Players: " + string.Join(" ", playerList));
            log.Log("Whitelist? " + whitelist.ToString());
        }



    }

    [System.Serializable]
    public class Banlist : CitrusLib.SettingObject
    {
        [JsonProperty(Order = 1)]
        public List<string> list;
    }

}
