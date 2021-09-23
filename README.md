# ExecPlugin
Run commands as another player with permission bypasses.

Like regular /sudo, however with 3 key differences:
* This plugin cannot make another player say something in chat. It only supports commands. (No accidentally forgetting a slash and exposing yourself)
* **Permissions are bypassed**. Similarly to my /exec command in <a href="https://github.com/ShimmyMySherbet/ShimmysAdminTools">ShimmysAdminTools</a> command permission requirements are bypassed. (Run staff commands as a normal player)
* Command output is redirected to you. The target player will not see any response from the command in chat, and it will instead be redirected to you with the prefix `[EXEC] {message}`.

## Command permission Bypassing
#### What's the point?
There may be cases where you want to run a command against another player, but the command does not support targeting another player.

Using this plugin, if the command runs against you, you can target it to another player.

## Command output redirection
Command output is redirected to you, so the target player cannot see the output. This means you can access the command result, and the target player does not know the command ran (depending on the what the command does)

## Key differences to /exec from my other plugin <a href="https://github.com/ShimmyMySherbet/ShimmysAdminTools">ShimmysAdminTools</a>
I designed ShimmysAdminTools to provide some extremely useful tools, even including flight. However, one of the core principals of ShimmysAdminTools is that it should not have any dependencies/libraries.

The patching-free approach of ShimmysAdminTools works well for a default permissions setup, however it is incompatible with AdvancedPermissions or Openmod.

Since this plugin uses patching, it will still be compatible with Openmod and third party permission providers. (**Note:** This plugin will still only work for Rocketmod commands)

## Commands
`/ExecPlayer [Player] [Command...]`

#### Examples:
*/Exec Shimmy /Ascend 100*

*/Exec Papershredder432 /pay Shimmy 100000*

*/Exec DiFFoZ /descend 0.1*

*TIP: if you make a player /descend 0.1, it will appear to them that they just fell through the floor*


## Downloads
Download this plugin via <a href="https://github.com/ShimmyMySherbet/ExecPlugin/releases">Releases</a>
