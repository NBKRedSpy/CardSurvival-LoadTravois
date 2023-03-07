using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace LoadTravois
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log { get; set; }

        /// <summary>
        /// The list of cards to attempt to load into the travois.
        /// </summary>
        private static ConfigEntry<string> CardMoveListConfig;
        public static List<KeyValuePair<int,string>> CardMoveList { get; private set; }

        public static ConfigEntry<KeyboardShortcut> HotKey;
        


    private void Awake()
        {
            Log = Logger;

            CardMoveListConfig = Config.Bind("General", nameof(CardMoveListConfig), "Sack,Basket", "The list of items to attempt to load, in order of preference");

            HotKey = Config.Bind("General", nameof(HotKey), new KeyboardShortcut(KeyCode.I), "The hotkey to start the loading process");

            LoadCardMoveList();

            Config.ConfigReloaded += Config_ConfigReloaded;
            Config.SettingChanged += Config_SettingChanged;
            Harmony harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
            harmony.PatchAll();



        }

        private void Config_SettingChanged(object sender, SettingChangedEventArgs e)
        {
            LoadCardMoveList();
        }

        private void Config_ConfigReloaded(object sender, EventArgs e)
        {
            LoadCardMoveList();
        }

        private static void LoadCardMoveList()
        {
            int sequence = 1;

            string cards = CardMoveListConfig.Value;
            CardMoveList = cards.Split(',')
                .Select(x => new KeyValuePair<int,string>(sequence++, x.Trim()))
                .ToList();
        }

        public static void LogInfo(string text)
        {
            Plugin.Log.LogInfo(text);
        }

        public static string GetGameObjectPath(GameObject obj)
        {
            GameObject searchObject = obj;

            string path = "/" + searchObject.name;
            while (searchObject.transform.parent != null)
            {
                searchObject = searchObject.transform.parent.gameObject;
                path = "/" + searchObject.name + path;
            }
            return path;
        }

    }
}