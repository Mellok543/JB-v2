using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using System.Collections.Generic;
using System.Drawing;

namespace JailBreak;

public class Player
{
    public CCSPlayerController controller;
    public bool isWarden;
    public bool isFreeday;
    public bool isRebel;
    public bool isBlueTeam;
    public bool isRedTeam;
    public bool isFroze;
    public static List<Player> Players { get; } = new();
    public static List<Player> PlayersBlueTeam { get; } = new();
    public static List<Player> PlayersRedTeam { get; } = new();
    
    public Player(CCSPlayerController player) 
    {
        this.controller = player;
        this.isWarden = false;
        this.isFreeday = false;
        this.isRebel = false;
        this.isBlueTeam = false;
        this.isRedTeam = false;
        this.isFroze = false;
    }
    public static bool IsValidPlayer(CCSPlayerController? player, bool requireAlive = false)
    {
        if (player == null || !player.IsValid || player.AuthorizedSteamID == null) return false;
        if (requireAlive && !player.PawnIsAlive) return false;
        return true;
    }

    public static bool TryGetPlayerData(CCSPlayerController? player, out Player? playerData)
    {
        playerData = null;
        if (!IsValidPlayer(player)) return false;

        foreach (Player item in Players)
        {
            if (item.controller == player)
            {
                playerData = item;
                return true;
            }
        }

        return false;
    }

    public static Player? GetPlayerFromController(CCSPlayerController player)
    {
        if (!IsValidPlayer(player)) return null;

        foreach (Player item in Players)
        {
            if (item.controller == player)
            {
                return item;
            }
        }

        return null;
    }
    public static bool playerHasFlag(CCSPlayerController? player, string flag)
    {
        if (!IsValidPlayer(player)) return false;
        var adminData = AdminManager.GetPlayerAdminData(player);
        if (adminData?.Flags == null) return false;
        return adminData.Flags.TryGetValue("css", out var flags) && flags.Contains(flag);
    }
    public static bool IsPlayerRebel(CCSPlayerController? player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return false;
        return playerData.isRebel;
    }
    public static bool IsPlayerWarden(CCSPlayerController? player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return false;
        return playerData.isWarden;
    }
    public static bool IsPlayerFreeday(CCSPlayerController? player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return false;
        return playerData.isFreeday;
    }
    public static bool IsPlayerBlueTeam(CCSPlayerController? player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return false;
        return playerData.isBlueTeam;
    }
    public static bool IsPlayerRedTeam(CCSPlayerController? player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return false;
        return playerData.isRedTeam;
    }
    public static bool IsPlayerAliveLegal(CCSPlayerController? player)
    {
        if (!IsValidPlayer(player, requireAlive: true)) return false;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null || !pawn.IsValid) return false;
        if (player.Connected != PlayerConnectedState.PlayerConnected) return false;
        return pawn.LifeState == (byte)LifeState_t.LIFE_ALIVE;
    }
    public static void BecomeRebel(CCSPlayerController player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;
        if (playerData.isRebel)
        {
            string message1 = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Taj igrac je vec rebel";
            Server.PrintToChatAll($"\u200B{message1}");
            return;
        }
        playerData.isRebel = true;
        playerData.isFreeday = false;


        pawn.Render = Color.FromArgb(255, 255, 0, 0);
        /*Server.NextFrame(() =>
        { 
            player.PlayerPawn.Value.SetModel("characters\\models\\nozb1\\jail_police_player_model\\jail_police_player_model.vmdl");
        });*/
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
        string message = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Igrac " + player.PlayerName + " je postao rebel";
        Server.PrintToChatAll($"\u200B{message}");

    }
    public static void GiveFreeday(CCSPlayerController player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;
        if (playerData.isFreeday)
        {
            string message1 = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Taj igrac vec ima Freeday";
            player.PrintToChat($"\u200B{message1}");
            return;
        }
        playerData.isFreeday = true;
        playerData.isRebel = false;
        pawn.Render = Color.FromArgb(255, 0, 255, 0);
        /*Server.NextFrame(() =>
        { 
            player.PlayerPawn.Value.SetModel("characters\\models\\nozb1\\jail_police_player_model\\jail_police_player_model.vmdl");
        });*/
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

        string message = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Igrac " + player.PlayerName + " je sada freeday";
        Server.PrintToChatAll($"\u200B{message}");
    }
    public static void RemoveFreeday(CCSPlayerController player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;
        if (!playerData.isFreeday)
        {
            string message1 = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Taj igrac nema Freeday";
            player.PrintToChat($"\u200B{message1}");
     
            return;
        }
        playerData.isFreeday = false;
        pawn.Render = Color.FromArgb(255, 255, 255, 255);
        /*Server.NextFrame(() =>
        { 
            player.PlayerPawn.Value.SetModel("characters\\models\\nozb1\\jail_police_player_model\\jail_police_player_model.vmdl");
        });*/
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

        string message = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Igrac " + player.PlayerName + " vise nije freeday";
        Server.PrintToChatAll($"\u200B{message}");
    }
    public static void RemoveRebel(CCSPlayerController player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;
        if (!playerData.isRebel)
        {
            string message1 = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Taj igrac nije Rebel";
            player.PrintToChat($"\u200B{message1}");
            return;
        }
        playerData.isRebel = false;
        pawn.Render = Color.FromArgb(255, 255, 255, 255);
        /*Server.NextFrame(() =>
        { 
            player.PlayerPawn.Value.SetModel("characters\\models\\nozb1\\jail_police_player_model\\jail_police_player_model.vmdl");
        });*/
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");

        string message = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Igrac " + player.PlayerName + " vise nije rebel";
        Server.PrintToChatAll($"\u200B{message}");
     
    }
    public static void BecomeBlueTeam(CCSPlayerController player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;
        if (playerData.isBlueTeam)
        {
            string message1 = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Taj igrac je vec u plavom timu";
            player.PrintToChat($"\u200B{message1}");

            return;
        }
        playerData.isRedTeam = false;
        playerData.isBlueTeam = true;
        PlayersBlueTeam.Add(playerData);
        pawn.Render = Color.FromArgb(255, 0, 0, 255);
        /*Server.NextFrame(() =>
        { 
            player.PlayerPawn.Value.SetModel("characters\\models\\nozb1\\jail_police_player_model\\jail_police_player_model.vmdl");
        });*/
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
        string message = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Igrac " + player.PlayerName + " je sada u plavom timu";
        Server.PrintToChatAll($"\u200B{message}");
    }
    public static void RemoveBlueTeam(CCSPlayerController player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;
        if (!playerData.isBlueTeam)
        {
            string message1 = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Taj igrac nije u plavom timu";
            player.PrintToChat($"\u200B{message1}");
            return;
        }
        playerData.isBlueTeam = false;
        PlayersBlueTeam.Remove(playerData);
        pawn.Render = Color.FromArgb(255, 255, 255, 255);
        /*Server.NextFrame(() =>
        { 
            player.PlayerPawn.Value.SetModel("characters\\models\\nozb1\\jail_police_player_model\\jail_police_player_model.vmdl");
        });*/
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
    }
    public static void BecomeRedTeam(CCSPlayerController player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;
        if (playerData.isRedTeam)
        {
            string message1 = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Taj igrac je vec u crvenom timu";
            player.PrintToChat($"\u200B{message1}");
            return;
        }
        playerData.isBlueTeam = false;
        playerData.isRedTeam = true;
        PlayersRedTeam.Add(playerData);
        pawn.Render = Color.FromArgb(255, 94, 0, 11);
        /*Server.NextFrame(() =>
        { 
            player.PlayerPawn.Value.SetModel("characters\\models\\nozb1\\jail_police_player_model\\jail_police_player_model.vmdl");
        });*/
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
        string message = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Igrac " + player.PlayerName + " je sada u crvenom timu";
        Server.PrintToChatAll($"\u200B{message}");
    }
    public static void RemoveRedTeam(CCSPlayerController player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return;
        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;
        if (!playerData.isRedTeam)
        {
            string message1 = ChatColors.Green + "[JailBreak] " + ChatColors.Default + "Taj igrac nije u crvenom timu";
            player.PrintToChat($"\u200B{message1}");
            return;
        }
        playerData.isRedTeam = false;
        PlayersRedTeam.Remove(playerData);
        pawn.Render = Color.FromArgb(255, 255, 255, 255);
        /*Server.NextFrame(() =>
        { 
            player.PlayerPawn.Value.SetModel("characters\\models\\nozb1\\jail_police_player_model\\jail_police_player_model.vmdl");
        });*/
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
    }
    public static void GiveWarden(CCSPlayerController player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return;
        playerData.isWarden = true;
        string message = ChatColors.Green + "[JailBreak] " + ChatColors.Default + player.PlayerName + " je sada Warden!";
        Server.PrintToChatAll($"\u200B{message}");

        player.Clan = "[Warden]";

        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;
        pawn.MaxHealth = 200;
        pawn.Health = 200;
        pawn.Render = Color.FromArgb(255, 0, 0, 255);
        /*Server.NextFrame(() =>
        {
            player.PlayerPawn.Value.SetModel("models/characters/models/nozb1/jail_police_player_model/jail_police_player_model.vmdl");
        });*/
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
    }
    public static void RemoveWarden(CCSPlayerController player)
    {
        if (!TryGetPlayerData(player, out var playerData)) return;
        playerData.isWarden = false;
        string message = ChatColors.Green + "[JailBreak] " + ChatColors.Default + player.PlayerName + " vise nije Warden!";
        Server.PrintToChatAll($"\u200B{message}");

        player.Clan = "";

        var pawn = player.PlayerPawn?.Value;
        if (pawn == null) return;
        pawn.MaxHealth = 100;
        pawn.Health = 100;
        pawn.Render = Color.FromArgb(255, 255, 255, 255);
        /*Server.NextFrame(() =>
        {
            player.PlayerPawn.Value.SetModel("models/characters/models/nozb1/jail_police_player_model/jail_police_player_model.vmdl");
        });*/
        Utilities.SetStateChanged(pawn, "CBaseModelEntity", "m_clrRender");
        Utilities.SetStateChanged(pawn, "CBaseEntity", "m_iHealth");
    }
}
