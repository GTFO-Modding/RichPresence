using System;
using Discord;
using RichPresence.Utils;
using UnityEngine;

namespace RichPresence
{
    public class RichPresenceManager : MonoBehaviour
    {
        private static Discord.Discord _discord;
        private static Discord.ActivityManager _activityManager;
        private static Discord.LobbyManager _lobbyManager;
        private static Discord.UserManager _userManager;
        private static long _clientId = 928720342908829756;
        private static Activity _defaultActivity = new Activity()
        {
            State = "Playing GTFO",
            Details = "Selecting an expedition"
        };
        
        public RichPresenceManager(IntPtr ptr) : base(ptr) { }
        
        public static void Setup()
        {
            Log.Warning("Rundown data parsed.");
            new GameObject().AddComponent<RichPresenceManager>();
            _discord = new Discord.Discord(_clientId, (UInt64)global::Discord.CreateFlags.Default);
            _activityManager = _discord.GetActivityManager();
            _activityManager.RegisterSteam(493520U);
            _activityManager.RegisterCommand("steam://run/493520");
            _lobbyManager = _discord.GetLobbyManager();
            _userManager = _discord.GetUserManager();
            UpdateActivity(_defaultActivity);

        }
        
        void Update()
        {
            _discord.RunCallbacks();
        }

        public static void UpdateActivity(Activity activity)
        {
            _activityManager.UpdateActivity(activity, result =>
            {
                result == Result.Ok ? Log.Message("Success!") : Log.Warning("Failed!");
            });
        }
        
        
    }
}