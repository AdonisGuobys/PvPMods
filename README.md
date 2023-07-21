# PvPMods - Gloomrot Update
### Server Only Mod
The PvP aspects of RPGMods, Honor, Kill Feed, Leaderboard, griefer punishment and siege controls
#### [Video Demo of Experience & Mastery](https://streamable.com/k2p3bm)

## Important Changes
Siege Controls not working yet
Most commands not reenabled yet

## PvP System
<details>
<summary>PvP System</summary>
Configurable PvP kill serverwide announcement.\
Kill/Death will also be recorded, and a ladder board for the Top 10 K/D on the server.
> ### Toggle PvP Commnd
If enabled, players can toggle their pvp status on/off via the pvp command.\
If their pvp status is off, they are excluded from all PvP damage.\
Your server must be configured as a PvP server for the toggle to work,\
players will otherwise never be able to do any pvp damage despite toggling pvp status to be on.

PvP toggle will be overridden by Hostility Mode if the honor system is active.
> ### Punishment System
Additionally, there's a punishment system which can be used to punish players who kill lower level players,\
which is configurable in the config.

The punishment system also has an anti-cheese built-in in case the server is not using the EXP system.\
Purposefully unequiping gear to appear as lower level to cheese the punishment system will not work.

Punishment will apply a debuff that reduces player combat efficiency.
* -25% Physical & spell power
* -15 Physical, spell, holy, and fire resistance
* Gear level down (Overridden by EXP system if active)
</details>
> ### Honor System
All vampires start with Neutral honor rank.\
Killing a vampire with a neutral or positive honor rank will deduct some honor points,\
while killing a vampire with a negative honor rank will reward the player with some honor points.\
Another way to gain honor is by grinding mobs. Each mob kill will give 1 honor point.\
There's a hard limit of 250p/hours gain to prevent grind.

The honor title is added as a prefix to the player's name.\
All chat commands which are included within RPGMods should still be used without the\
honor title prefix if a player name is required.\
Other stuff like whispering to other players does require the title prefix to be used.

Honor title prefix is not automatically updated for chat messages,\
everything else like building ownership and hovering player names are automatically updated.

For all the mechanics to work correctly, please set your server settings to:
- `Game Mode: PvP`
- `Castle Damage Mode: Always`
- `Player Damage Mode: Always`
#### Hostility Mode
`[ON] Aggressive`\
Can damage any player.\
No reputation loss will be given to the aggressor when killed.

`[OFF] Passive`\
Cannot damage other players with a positive reputation.
#### Castle Siege
`[ON] Sieging`\
Player castle(s) are now vulnerable, and can damage other sieging player castle(s).\
Aggressive state is enforced during siege time.\
Siege mode cannot be exited until a 3 hour window has passed since activation.\
Activating siege mode will also affect your allies.

`[OFF] Defensive`\
Exit castle siege mode.\
Castle(s) are now invulnerable.\
Player is able to enter passive state again.

`Global Siege`\
In global siege mode, all castles are vulnerable unless the player's honor bonus says otherwise.\
Player aggressive state is not enforced during global siege.
#### All Honor Titles
| Title | Requirement | Reward/Kill | Bonus |
| --- | --- | --- | --- |
| Glorious | 10000 | -1000 | Castle(s) is permanently invulnerable. Bonus is negated if allied with Dreaded players. |
| Noble | 5000 | -500 | Castle(s) receive -50% reduced damage. Bonus is negated if allied with Dreaded players. |
| Virtuous | 1500 | -100 | +15% resource gathering. |
| Reputable | 500 | -50 | -25% durability loss. (Does not affect durability loss from combat.) |
| Neutral | 0 | -25 | No additional stats. |
| Suspicious | -1 | 0 | No additional stats. |
| Infamous | -1000 | 10 | Enforced aggressive state. |
| Villainous | -3000 | 50 | -20% damage taken from positive rep vampires. |
| Nefarious | -10000 | 100 | +20% damage dealt to positive rep vampires. |
| Dreaded | -20000 | 150 | Enforced castle siege participation |


### [Discord](https://discord.gg/XY5bNtNm4w)
### Current Developer
- `小爛土#7151` - Also known as Shou (like the english word show), Darkon47 on Github.
If you enjoy the work I have put into this mod, subscribe to my patreon at https://www.patreon.com/user/membership?u=92238426

### Original Developer
- `Kaltharos#0001`

### Contributors
#### Without these people, this project will just be a dream. (In no particular order)
- `Dimentox#1154`
- `Nopey#1337`
- `syllabicat#0692`
- `errox#7604`

</details>

<details>
<summary>Known Issues</summary>

### General

### PvP System
- Punishment debuff lower the player gear level, which will be overriden by the experience system if the exp system is active.
