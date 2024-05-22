Pvp in vanilla Valheim is quite boring and monotonous. The mod was created to fix it!

## Battle Status
A viking who commits an act of aggression towards another viking will now receive the Battle status effect. While the viking is in battle, he cannot teleport. And if he decides to escape from the battlefield by leaving the game, everyone in discord will immediately find out) Because a viking never runs away from battles!

## Tamed animals are now useful
They now know how to protect the ward. Any hostile viking in the ward zone will be immediately attacked by tamed animals that guard it!

## Ping of death on the map
If a viking is dead, all other vikings in the detection radius will immediately know about it. And they will be able to steal loot from the grave, perhaps? Or wait for the owner?

## Weapons balancing!
Some weapons are not balanced for pvp. Next settings will change the stats of the most imbalanced (too strong or too weak) items, such as swords, staffs and crossbows.You can download the configuration yourself and install it on your server.

Download link - [click](https://github.com/Tristan-dvr/ValheimPvPTweaks/tree/master/WeaponBalance)
<details><summary>Configurations description</summary>

> To use the configurations you must install [WackyDatabase](https://thunderstore.io/c/valheim/p/WackyMole/WackysDatabase/) to your server.

Changes
- change damage multiplier from secondary swords attack from x3 to x2
- reduce damage of all crossbows
- increase attack angle of atgeirs (they're easier to hit)
- increase eitr consumption for all staffs
- reduced damage of fireball staff
- increased damage of ice staff
- _configuration may be updated periodically)_

</details>

Using this configurations is highly recommended, because this will add to the list of weapons that can be used effectively in PvP!

## Customizable boss powers
Boss powers are also too unbalanced for PvP, so the mod has the ability to adjust the duration and recovery time of each boss power.

### Other features:
- if the player connects to the server where the mod is installed, the mod configuration is automatically synchronized with the configuration on the server
- it is possible to make the game console unavailable, which may be useful for playing on the server
- for more convenience, it is possible to reset the cooldown of the boss's power automatically when changing power
- the mod has the ability to show a kill feed in the discord channel

<details><summary>Mod API (for modders)</summary>
Client-side.
On the client-side there is the ability to track the killing of creatures and players. It can be used, for example, for a kills counter and statistics.

```
//	called by the owner of the location when some creature, including the player, dies 
CharacterKillTracker.OnCharacterDead += (killedCharacter, killer, weapon) =>
{
	//  some logic
};

//	called when a player kills some creature, including another player
CharacterKillTracker.OnCharacterKilled += (characterName, weapon) =>
{
	//  some logic
};
```

Server-side.
All kills, including creature kills, can also be tracked on the server-side
  
```
KillFeed.OnCharacterKilled += (killData) =>
{
    if (killData.killer.isPlayer)
    {
        var zdo = killData.killer.zdo;
        var prefab = killData.killer.prefabName;
        var name = killData.killer.displayName;
    }
    
    if (killData.killed.isPlayer)
    {
        var zdo = killData.killed.zdo;
        var prefab = killData.killed.prefabName;
        var name = killData.killed.displayName;
    }
};
```
</details>


#### Find me on Discord if you got feature request / bug report / found incompatibility with another mod
Typedef#3996
https://discord.gg/MjQZKuB4z2