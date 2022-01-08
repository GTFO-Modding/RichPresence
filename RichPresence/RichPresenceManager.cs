using System;
using Discord;
using Globals;
using RichPresence.Utils;
using SNetwork;
using UnityEngine;

namespace RichPresence
{
    public class RichPresenceManager : MonoBehaviour
    {
        private static Discord.Discord _discord;
        private static Discord.ActivityManager _activityManager;
        private static Discord.LobbyManager _lobbyManager;
        private static Discord.UserManager _userManager;
        private static pActiveExpedition _expPackage;
        private static string _matchId = Guid.NewGuid().ToString();
        private static string _secret = Guid.NewGuid().ToString();
        private static long _clientId = 928720342908829756;
        
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
            UpdateActivity(GetActivity());
        }
        
        private void Update()
        {
            _discord.RunCallbacks();
            UpdateActivity(GetActivity());
        }

        public static void UpdateActivity(Activity activity) => _activityManager.UpdateActivity(activity, result =>
        {
            Log.Message(result == Result.Ok ? "Success!" : "Failed!");
        });

        public static Activity GetActivity()
        {
            if (SNet.IsInLobby)
            {
                _expPackage = RundownManager.GetActiveExpeditionData();
                return new Activity()
                {
                    State = "Playing GTFO",
                    Details = (Global.InLobby ? "In lobby: " : "In the darkness: ") +
                              RundownManager.ActiveExpedition.Descriptive.Prefix +
                              (_expPackage.expeditionIndex + 1).ToString() + " " +
                              RundownManager.ActiveExpedition.Descriptive.PublicName,
                    Party =
                    {
                        Id = _matchId,
                        Size =
                        {
                            CurrentSize = SNet.LobbyPlayers.Count,
                            MaxSize = SNet.LobbyPlayers.Capacity
                        }
                    },
                    Secrets =
                    {
                        Match = _secret,
                        Join = SNet.Lobby.Identifier.ID.ToString(),
                        Spectate = "null"
                    },
                    Timestamps =
                    {
                        Start = 0L
                    }
                };
            }
            return new Activity()
            {
                State = "Playing GTFO",
                Details = "Selecting an expedition"
            };
        }
    }
}