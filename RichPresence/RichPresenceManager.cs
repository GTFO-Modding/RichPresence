using System;
using System.Text.RegularExpressions;
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
        private static ActivityManager _activityManager;
        private static LobbyManager _lobbyManager;
        private static UserManager _userManager;
        private static pActiveExpedition _expPackage;
        private static string _matchId = Guid.NewGuid().ToString();
        private static string _secret = Guid.NewGuid().ToString();
        private static long _clientId = 928720342908829756;
        private static Regex _rx = new Regex("(<([^>]+)>)");
        
        public RichPresenceManager(IntPtr ptr) : base(ptr)
        {
        }

        public static void Setup()
        {
            Log.Warning("Rundown data parsed.");
            new GameObject().AddComponent<RichPresenceManager>();
            _discord = new Discord.Discord(_clientId, (UInt64) global::Discord.CreateFlags.Default);
            _activityManager = _discord.GetActivityManager();
            _userManager = _discord.GetUserManager();
            _lobbyManager = _discord.GetLobbyManager();
            _activityManager.RegisterSteam(493520U);
            _activityManager.RegisterCommand("steam://run/493520");
            _activityManager.OnActivityJoin += OnActivityJoin;
            _activityManager.OnActivityJoinRequest += OnActivityJoinRequest;
            _activityManager.OnActivityInvite += (ActivityActionType type, ref User user,
                ref Activity activity) =>
            {
                Log.Message($"Activity Invite: \n\tType:{type}\n\tUser:{user}\n\tActivity:{activity}");
            };
            UpdateActivity(GetActivity());
        }

        private void Update()
        {
            _discord.RunCallbacks();
            UpdateActivity(GetActivity());
        }

        public static void UpdateActivity(Activity activity) => _activityManager.UpdateActivity(activity, result =>
        {
            if (result != Result.Ok)
            {
                Log.Message("Activity Update Failed!");
            }
        });

        public static Activity GetActivity()
        {
            
            if (SNet.IsInLobby)
            {
                _expPackage = RundownManager.GetActiveExpeditionData();
                return new Activity()
                {
                    State = Global.InLobby ? "Preparing to drop..." : "In the Complex...",
                    Details = RundownManager.ActiveExpedition.Descriptive.Prefix +
                              (_expPackage.expeditionIndex + 1).ToString() + " - " +
                              _rx.Replace(RundownManager.ActiveExpedition.Descriptive.PublicName, ""),
                    Party =
                    {
                        Id = _matchId,
                        Size =
                        {
                            CurrentSize = SNet.LobbyPlayers.Count,
                            MaxSize = SNet.LobbyPlayers.Capacity,
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
                    },
                    Assets =
                    {
                        LargeImage = "gtfo_original_white_square",
                        LargeText = "GTFO"
                    }
                };
               
            }
            
            return new Activity()
            {
                State = "Selecting an expedition...",
                Assets =
                {
                    LargeImage = "gtfo_original_white_square",
                    LargeText = "GTFO",
                }
            };
        }

        public static void OnActivityJoin(string secret)
        {
            if (SNet.IsInLobby)
                SNet.Lobbies.LeaveLobby();
            SNet.Lobbies.JoinLobby(new SNet_LobbyIdentifier(Convert.ToUInt64(secret)), true);
        }

        public static void OnActivityJoinRequest(ref User user) => _activityManager.SendRequestReply(
            user.Id, ActivityJoinRequestReply.Yes, (ActivityManager.SendRequestReplyHandler) (res =>
            {
                if (res != Result.Ok)
                    return;
                Log.Message("Activity join request succeeded.");
            }));
    }
}
