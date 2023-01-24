using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

internal static class CommonExtensions
{
    public static bool ContainsIgnoreCase(this string self, string another)
    {
        return self.IndexOf(another, StringComparison.OrdinalIgnoreCase) >= 0;
    }

    public static void IncreaseCounter(this Dictionary<string, int> dictionary, string key, int increaseCount = 1)
    {
        dictionary.TryGetValue(key, out var value);
        value += increaseCount;
        dictionary[key] = value;
    }

    public static void ForeachSafe<T>(this IList<T> list, Action<T> action)
    {
        for (int i = 0; i < list.Count; i++)
            action?.Invoke(list[i]);
    }

    public static string MinutesFormat(this TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"mm\:ss");
    }

    public static string HoursFormat(this TimeSpan timeSpan)
    {
        return timeSpan.ToString(@"hh\:mm\:ss");
    }

    public static T GetRandom<T>(this IEnumerable<T> collection)
    {
        if (!collection.Any())
            return default;

        var count = collection.Count();
        var randomIndex = Random.Range(0, count);
        return collection.ElementAt(randomIndex);
    }

    public static IEnumerable<Type> GetTypesWithAttribute<T>(this Assembly assembly) where T : Attribute
    {
        Type[] types;
        try
        {
            types = assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException e)
        {
            types = e.Types;
        }
        return types.Where(t => t != null && t.GetCustomAttribute<T>() != null);
    }

    public static string GetSteamId(this ZNetPeer peer) => peer.m_rpc.GetSteamId();

    public static string GetSteamId(this ZRpc rpc) => rpc.GetSocket().GetHostName();

    public static bool IsEmpty(this Inventory inventory) => inventory.NrOfItems() <= 0;

    public static void ClearDamage(this HitData hitData)
    {
        hitData.ApplyModifier(0);
        hitData.m_damage.m_damage = 0;
    }

    public static T GetPrefabComponent<T>(this ZNetScene netScene, string name) where T : Component
    {
        return netScene.GetPrefab(name)?.GetComponent<T>();
    }

    public static bool IsLocalPlayer(this Character character) => character == Player.m_localPlayer;

    public static ZDO GetZDO(this Player player)
    {
        return player.m_nview.GetZDO();
    }

    public static bool HaveAccess(this PrivateArea privateArea, Player player)
    {
        var id = player.GetPlayerID();
        return HaveAccess(privateArea, id);
    }

    public static bool HaveAccess(this PrivateArea privateArea, long playerId)
    {
        return privateArea.m_piece.GetCreator() == playerId || privateArea.IsPermitted(playerId);
    }
}
