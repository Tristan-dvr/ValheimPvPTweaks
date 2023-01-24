Pvp in vanilla Valheim is quite boring and monotonous. The mod was created to fix it!

## Battle Status
A viking who commits an act of aggression towards another viking will now receive the Battle status effect. While the viking is in battle, he cannot teleport. And if he decides to escape from the battlefield by leaving the game, everyone in discord will immediately find out) Because a viking never runs away from battles!

## Tamed animals are now useful
They now know how to protect the ward. Any hostile viking in the ward zone will be immediately attacked by tamed animals that guard it!

## Ping of death on the map
If a viking is dead, all other vikings in the detection radius will immediately know about it. And they will be able to steal loot from the grave, perhaps? Or wait for the owner?

## Weapons balancing!
Some weapons are not balanced for pvp. To fix this, the mod adds the ability to configure their characteristics. The mod allows you to configure the damage of swords, staffs and crossbow.

Using this options is highly recommended, because this will add to the list of weapons that can be used effectively in PvP!

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
	if (killData.characterIsPlayer)
	{
		//  killed player
		var zdo = killData.characterZdo;
		var peer = killData.characterPeer;
	}

	if (killData.attackerIsPlayer)
	{
		//  someone killed by player
		var zdo = killData.attackerZdo;
		var peer = killData.attackerPeer;
	}
};
```
</details>


#### Find me on Discord if you got feature request / bug report / found incompatibility with another mod
Typedef#3996
https://discord.gg/MjQZKuB4z2

### Changelog
- 1.0.7
Added option to configure damage of crossbow.
- 1.0.6
Added option to modify characteristics of new magical staffs.
- 1.0.5
Removed configuration of swords attack angle. It was a bit balanced by Valheim devs.
Applying damage modifier to new 2-handed sword.
- 1.0.4
Mistlands update. Also compatible with previous game version.
Added option to modify new Boss power. 
- 1.0.1 - 1.0.3
minor bugfixes.
- 1.0.0
Initial release.
