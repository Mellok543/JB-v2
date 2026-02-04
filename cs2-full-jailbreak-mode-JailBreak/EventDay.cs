using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
namespace JailBreak
{
    internal abstract class EventDay
    {
        public static bool isEventDay = false;
        public static string dayType = string.Empty;

        public EventDay()
        {
            StartEventDay();
        }
        public virtual void EndEventDay()
        {
            isEventDay = false;
            dayType = string.Empty;
            Server.ExecuteCommand("mp_friendlyfire 0");
            foreach (var player in Utilities.GetPlayers())
            {
                if (!Player.IsValidPlayer(player)) continue;
                var pawn = player.PlayerPawn?.Value;
                if (pawn?.WeaponServices == null) continue;
                pawn.WeaponServices.PreventWeaponPickup = false;

            }
        }
        public virtual void OnHit(CCSPlayerController attacker, CCSPlayerController victim)
        {

        }
        public virtual void StartEventDay() 
        {
            EventDay.isEventDay = true;
        }
    }
}
