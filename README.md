# 2016-Bomberman

The current release is version 1.2.1.

For more information about the challenge see the [Challenge website](http://challenge.entelect.co.za/) .

## TL;DR

1. Download the release files
2. Build your bot
4. Submit
5. Win

## Project Structure

In this project you will find everything you need to build and run a bot on your local machine.  This project contains the following:

1. Game Engine - The game engine is responsible for running matches between players.
2. Sample Bots - Sample bots can be used a starting point for your bot.
3. Reference Bot - The reference bot contains some AI logic that will play the game based on predefined rules.  You can use this to play against your bot for testing purposes.
4. Sample State Files - Can be used as a starting point to get the parsing working for your bot.

This project can be used to get a better understanding of the rules and to help debug your bot.

Improvements and enhancements may be made to the game engine code over time, but the rules should remain stable. Any changes made to the game engine or rules will be updated here, so check in here now and then to see the changes.

The game engine has been made available to the community for peer review and bug fixes, so if you find any bugs or have any concerns, please e-mail challenge@entelect.co.za, discuss it with us on the [Challenge forum](http://forum.entelect.co.za/) or submit a pull request on Github.

## Usage
The easiest way to start using the game engine is to download the [binary release zip](https://github.com/EntelectChallenge/2016-Bomberman/releases/download/V1.2.1/Game.Engine.v1.2.1.zip). You will also need the .NET framework if you don't have it installed already - you can get the offline installer for [.NET Framework 4.5.1 here](http://www.microsoft.com/en-za/download/details.aspx?id=40779).

Once you have installed .NET and downloaded the binary release zip file, extract it and open a new Command Prompt in the Binaries/{version}/Game Engine folder.

We have included the reference bot in the binaries version folder, so at this point you can simply run the Run.bat to see the bots play a match.

Once you have written your own bot you can you can use the command line arguments to specify the bots that should be run. You can see the available command line arguments by running `Bomberman.exe --help`:
```powershell                                              

  -b, --bot         (Default: Empty String Array) Relative path to the folder containing
                     the bot player.  You can add multiple bots by separating each with a space.       

  -c, -console      (Default: 0) The amount of console players to add to the game.                      

  -r, --rules        (Default: False) Prints out the rules and saves them in  
                     markdown format to rules.md                              

  --clog        	(Default: False) Enables Console Logging.                                    

  --pretty   		(Default: False) Draws the game map to console for every round instead of showing logs                                    

  -l, --log          (Default: ) Relative path where you want the match replay
                     log files to be output (instead of the default           
                     Replays/{matchSeed}).                   

  -s --seed        	(Default: Random) The game seed to use for map generation.                                 

  --help             Display this help screen.                                
```

So for example you can do something like this to run your bot against the bundled reference bot: `Bomberman.exe --pretty -b "../Reference Bot" "../My Awesome Bot"`.

You might have to change the configurate file depending on your system in order to run the game.  The configuration file is in the game engine folder called `Bomberman.exe.config`.  You can modify the file to update where the game engine looks for the various runtime executables such as the java runtime to use.  All paths have to be absolute (unless the executable is in the system path).

## Your Bot

We have changed things a bit this year when it comes to compiling and running the bot.  You will not longer be able to include a run.bat and compile.bat file, the system will do that for you based on your bot meta you included.  One of the reasons we decided to go this route is in order to add additional features to the game engine for instance running calibration bots.

Sample bot project files can be downloaded [here.](https://github.com/EntelectChallenge/2016-Bomberman/releases/download/V1.2.1/Sample.Bots.zip)

The game engine requires that you have `bot.json` file.  This will tell the game engine how to compile and run your bot.  The file must contain the following:

```json
{
	"Author":"John Doe",
    "Email":"john.doe@example.com",
    "NickName" :"John",
    "BotType": "CSharp",
    "ProjectLocation" : "",
    "RunFile" : "Reference\\bin\\Debug\\Reference.exe",
    "RunArgs" : ""
}
```

1. Author - Your Name
2. Email - Your Email Address
3. Nickname - The nickname that will be used by visualizers
4. Bot Type - The type of bot
  * CSharp
  * Java
  * JavaScript
  * CPlusPlus
  * Python2
  * Python3
  * FSharp
5. Project Location - The root location of the project file.  For instance in C# solutions, that will point to folder containing the solution (.sln) file.  This will be used for bot compilation when you submit your bot.
6. Run File - This is the main entry point file for your bot that will be executed to play the game.
  * Java user have to ensure that the main class is specified in the manifest file
7. RunArgs - (Optional) Any additional arguments you would like to send your bot.  This will be the 3rd argument sent to your bot (if provided).

The game engine might set additional runtime parameter in some scenarios, for instance specifying minimum memory allocation for java bots.

The following package managers are supported by the game engine:
* Microsoft Languages - Nuget.  (Requires the nuget.exe to be present in the project location path)
* Java - Maven.  (Requires that the project contains a pom file in the project location path)
* JavaScript - NPM.  (Requires that project contains a package.json file in the project location path)
* Python - Python Package Index.  (Requires that the project contains a requirements.txt file in the project location path)

Your bot will receive two arguments when the for every round in the game:

1. Your player key registered in the game
2. The directory for the current game files

The game will store game files during a match in the following directory format
````
...Replays/
............Game seed/
......................Round Number/
...................................engine.log
...................................map.txt
...................................roundinfo.json
...................................state.json
...................................Player Key/
..............................................log.txt
..............................................map.txt
..............................................state.json
..............................................move.txt
````

Each player will have the `map.txt` and `state.json` files in their `Player Key` folder until they made their move using the `move.txt` file, after which the game engine will remove the state and map files to save some disk space.  The `map.txt` and `state.json` files in the `Round Number` folder is the same files that were placed in the player folders.

The `engine.log` file contains information from the engine while processing the round.

The `log.txt` file in the player file contains player specific logs, such as the console output from player bots.  If your bot is misbehaving this should be the first place to go have a look.  In this file you can also view additional information such as bot run time and bot processor time.

The `round.info` folder is mainly for GUI submissions, and reports player stats and the leaderboard.

In order to help with bot calibrations and to speed up things a bit to give your bot the best possible chance at winning we are going to look into the possibility of running each match from a ram disk this year, provided the hardware specs allow for it without affecting bot performance.  Keep an eye on the forum to see what our decision will be regarding this.

The `map.txt` file will use the following characters to represent the game state:
```
' ' - Empty game block
'*' - Bomb explosion
'A' - Player Key (This can be anything from A-L)
'a' - Player Key when player is on a bomb they planted (This can be anything from a-l)
'#' - Indestructable wall
'+' - Destructable wall
'1' - Bomb (This shows the bomb countdown timer, so it can be anything from 1-9)
'$' - Super power up
'!' - Bomb Radius power up
'&' - Bomb Bag power up
```
The `map.txt` will also have sections underneath the map for each player to give more information about each player like the power up strength, and the locations and timers of bombs on the map.

### Rules

These are the simplified rules.  More in depth rules are further down.

1. A player can only make one move during a round.
2. A player can make one of the following moves:
  1. Move Left - Moves one block left.
  2. Move Right - Moves one block right.
  3. Move Up - Moves one block up.
  4. Move Down - Moves one block down.
  5. Plant Bomb - Plants a bomb (If there are bombs in your bomb bag).
  6. Trigger Bomb - Takes the bomb with the lowest count down and sets the countdown to 1.
3. Bombs will destroy walls, kill players and trigger other bombs.
4. A player can pick up power ups to increase their bomb bag and blast radius.
5. Players will earn points for destroying walls, killing other players and "discovering" the game map by moving around.
6. The game leaderboard will be determined as follows:
  1. Players that are alive will be on top.
  2. If multiple players alive when the max rounds have been reached the points will be used.
  3. Players that have died will be sorted first on points, then based on the round they died.

### Tests
We have written a number of automated tests to ensure that the game logic and rules have been implemented correctly - if you make any changes to the test harness you should run the tests to ensure that everything is still working correctly before submitting a pull request.

If you add a new feature you should add tests to cover it.

## Just For Fun

So doing your own little Turing test by playing against your own bot in the console to determine if it is going to turn into the next Skynet is fun and all, but what if you just want to take a break from it all?

That is where the `Network (Console)` comes in.  Invite some of your friends over or have some fun at the office during lunch time and play against some real human competitors.  In the `Network (Console)` folder there are two sub folders, the `Host` and the `Client`.  Start the `Host/SocketHost.exe` and wait for the clients to connect.  Each of your friends, and you, should then launch a new `Client/BomberManSocketClient.exe`

The socket client will ask for a hostname, which should be your computer name (the host).  Then each player will get a change to enter a user name.  Once all of the players have connected, type in start into the host and press enter.  May the best player win!

The game engine supports 2 to 12 players.

Disclaimer:  This feature was developed purely for fun, and will most likely crash from time to time.  Here are some tips if that happens

1. Make sure you have only one network interface active to bind on
2. Make sure that port 19010 is available for binding on your system
3. Make sure the clients enter your computer name as it is registered on the network (IP Address might also work, but not always)
4. Client players will connect to the host on ports 20001 - 20013

# Dem Rules
### Map Generation

The maps in the game will be generated randomly based on seed provided to the game engine.  The game seed will be random for each match, but can be the same if matches need to be re-run.  The map will be divided into four quadrants for generation purposes.

1. The map will be surrounded with indestructible walls
2. The default map size for 2-4 players will be 21x21 blocks
3. Every second square, starting from the outer boundary, will be an indestructible wall.  The only exception to this rule will be the center block on the map.
4. Each quadrant will be generated such that the entire map will be symmetrical, with each quadrant appearing the same from each users perspective.
5. Players will always be placed in a corner of the map.  In case the map contains more than 4 players, the remaining players be placed equidistant from the other players along the sides of the map.
6. Every player on the map will have a 2 block safe zone horizontally and vertically.
7. The center of the map will always contain a Super power up in the center, in place of the indestructible wall.
8. The center power up will always be surrounded by a 5x5 area of destructible walls.
9. Power ups will be placed randomly across the map, with each quadrant of the map receiving the same amount and type of power ups.  When four players are present, a fairness algorithm will be applied to ensure players have the same chance of finding a power up within a certain distance from them.
10. Power ups on the map will be determined with the following algorithm
  1. Two bomb bag power ups will be placed on the map per player.
  2. Four bomb radius power ups will be placed on the map per player.
11. The tournament will only have 2 or 4 players per map.  But in some scenarios more players will be placed on the maps, in which case the map size (Width/Height) will dynamically change to accommodate more players.

### Player Rules

Players can either be console players or bots.  Both follow the same game engine rules.  When playing on Unity, the rules will follow the actual game as close as possible, with exceptions made for real time play.

1. Players will only be able submit one command per round.  The game engine will reject any additional commands sent by the player.
2. Only one of the following commands can be submitted by the player during a round:
  1. Move Command – Left, Right, Up, Down.
  2. Place Bomb Command – Places a bomb underneath the player.
  3. Reduce Bomb Timer – Reduces the timer of the bomb with the lowest timer for the player to 1.
  4. Do Nothing Command – Player skips the round and remains on the same block.
3. Players will start with a bomb bag containing 1 bomb.
4. Players will start with a bomb radius of 1.
5. Players will start with a bomb timer of 4 rounds.
6. Bot players will have the following additional rules
  1. Bot processes will be terminated after 4 seconds
  2. Bots will not be allowed to exceed a total processor time of 2 seconds
  3. Bots processes will run with elevated processor priority. (For this reason the game has to be run with administrator privileges)
  4. Calibrations will be done at the start of a game to determine additional processor time.  So if the calibration bot takes 200ms to read the files and make a move decision then your bot will be allowed an additional 200ms to complete.
  5. Malfunctioning bots or bots that exceed their time limit will send back a do nothing command.
  6. Bot players that post more than 20 do nothing commands in a row will automatically place a bomb to kill themselves in an attempt to save the game
  7. Players must ensure that the bot process exits gracefully within the allotted time. No child processes will be allowed.
  8. All bot logic processing must be done within the source code submitted for your bot.  You may not use network calls such as web services to aid in your bots decision making.

### Game Engine Rules

The following rules describe how the game engine will run and process the game

1. The game engine contains the following entities:
  1. Indestructible Wall
  2. Destructible Wall
  3. Player
  4. Bomb
  5. Power Ups
    1. Bomb Bag
    2. Bomb Radius
    3. Super Power Up
2. A game block can only have one of the following entities at a single time:
  1. Indestructible Wall
  2. Destructible Wall
  3. Player
  4. Bomb
  5. Bomb with a player on top after planting
3. Power ups will only be revealed once the destructible wall has been destroyed as a result of a bomb blast.
4. The game engine will process rounds in the following order:
  1. Remove old explosions from the map
  2. Decrease all bomb timers
  3. Detonate bombs with a timer value of 0
  4. Trigger bombs that fall within the explosion range of another bomb
  5. Mark entities for destruction (Any players within a bomb blast at this moment will be killed)
  6. Process player commands
  7. Mark entities for destruction (If a player moved into a bomb blast, they will be killed)
  8. Apply power ups
  9. Remove marked (Killed/Destroyed) entities from the map
  10. Apply player movement bonus
5. A player entity will not able to move to a space containing another entity, with the exception of power ups.
6. A player can only plant a bomb if they have bombs available in their bomb bag.  Planting a bomb removes a bomb from the bomb bag and will be returned once a bomb explodes.
7. Two player entities will not be able to move onto the same space during a round, if this does happen the game engine will randomly choose a player whose move will be discarded.
8. Bombs will start with a timer based on the players current bomb bag. The formula is (bombag size * 3) + 1.  The bomb timers will be capped to 10.
9. Bomb timers will decrease by 1 every round.
10. Bomb radius will equal the radius bonus of the player at the time of planting. Obtaining a radius power up afterwards will not increase bomb radius of bombs currently on the map.
11. Destructible Walls can only be destroyed if they fall within the blast radius of a bomb.
12. Indestructible Walls will absorb the damage from a bomb and prevent it from continuing past the wall.
13. Bombs will absorb the damage from other bombs and prevent it from continuing past the bomb, this will however will cause the affected bomb to detonate causing a chain of detonations.
14. If a player is in the range of a bomb blast radius at the start of the round and is killed as result, their commands for that round will be ignored.
15. If a player moves into the range of a bomb blast during a round, the player will be killed as a result.
16. The game engine will be restricted to a certain amount of rounds.  The max rounds for each map will be calculated as follows (map width * map height).
17. The leader board for the game will be based on the following
  1. Players alive
  2. Then points for the players
  3. Then the round the players were killed

### Power Ups

Power ups can be collected by players to improve their players abilities

1. The bomb bag power up will give the player an additional bomb to plant on the map while the timers on other bombs are decreasing.
2. The bomb radius power up will multiply the current bomb radius of the player by two.
3. The special power up will give the following bonuses:
  1. Bomb bag power up
  2. Bomb radius power up
  3. 50 points

### Points

Players will collect points during game play.  Points will be used (along with other conditions) to determine the player leaderboard and ultimately the winner

1. Players will receive 10 points for destroying destructible walls.
  1. If two bombs hit the same wall, both players will receive 10 points for destroying the wall.
2. Players will receive points for killing another player based on the following equation ((100 + Max point per map for destructible walls) / players on map).  So on map with 10 destructible walls with 4 players the points for killing a player will be 50.
  1. If two bombs hit another player, both players will receive points for killing the player.
3. Players will receive points based on map coverage:
  1. Points will only be calculated for each new block touched by a player.
  2. Points will determine player coverage on the map, with a map coverage of 100% giving the player 100 points.
4. Players obtaining the Super Power up will receive additional points.
5. When multiple player bombs are triggered in a bomb chain, all players with bombs forming part of the chain will receive the points for all entities destroyed in the chain.
6. The round in which a player is killed will cause the player to forfeit all points earned in that round, and the player will lose points equal to the points earned when killing another player.

## Release Notes

### Version 1.2.1 - 30 April 2016
Change Log:

1. Added missing requirements.txt file for python bots.
2. Support spaces in bot path executable.
3. Use the game seed as console display and replay folder name instead of the seed used to generate the map.
4. Added Scala sample bot (Thank you markvrensburg).
5. Changed trigger bomb command behaviour to allow bots to trigger bombs even if they have bombs currently exploding on the map.
6. Updated the map.txt to print player bombs on the same line, each bomb separated by a comma.

How will this affect me?

1. If you are developing a python bot, please include the requirement txt file.
2. Bots will now be allowed to have spaces in their directory/file name.
3. If passing in a seed to the game engine, it will now correctly display that seed and use it as the replay folder name.
4. Entries developed using Scala will now be allowed.
5. The game engine will no longer throw an exception and discard your command if you trigger a bomb while one of your other bombs are exploding.  The new behaviour will only take non exploding bombs into consideration for the trigger command.
6. If you have a parser for the map.txt it will have to be updated to no longer take the new lines in to consideration when parsing bombs planted by players.

### Version 1.1.1 - 23 April 2016
Change Log:

1. Added Python 3 sample bot. Thank you tjorriemorrie.
2. Fixed java sample bot not reading the state file
3. Removed the BOM information written at the beginning of each file from the game engine.
4. Added a new RunArgs property to the bot.json file that can be used to pass additional information to the bot when executed by the game engine
5. Fixed the java calibration bot not including the time to read the state.json file.
6. Fixed the python calibration bot not including the time to read the state.json file.
7. Fixed the node.js calibration bot doing some additional work not done by the other calibration bots.
8. Removed the calibation bot directory. We are using some of the sample bots as calibration bots.

How will this affect me?

1. You now have a nice starting point for making a python 3 bot.
2. New entrants can now just carry on with the parsing of the file contents.
3. If you added special logic to remove the BOM information at the beginning of files, you should remove that logic.
4. Will not affect you, unless you want to send your bot additional arguments.
5. Java bots now get a couple of milliseconds extra to run.
6. Python bots now get a couple of milliseconds extra to run.
7. Node.JS bots now get a couple of milliseconds less to run.
8. Will not affect you, this is just to clean up the repo a little.

### Version 1.1.0 - 16 April 2016
Change Log:

1. Added the current round to the `state.json` file.
2. Changed from total processor time to wall clock time when measuring bot execution times as this has better multi thread/processor core support.
3. Moved all of the game engine dll files to separate folder to clean up the root directory.
4. Added missing DoNothingCommand for C# sample bot.
5. Fix issues starting bot processes on Linux.
6. Improved reliability of round logging for bot's.

How will this affect me?

1. You will not be able to get the current round for the game from the game engine.
2. Multi threaded bots will now be allowed.
3. If you made changes to the config file, you will have to updated the new config file with the changes you made.
4. The moves enum can now be used to send a do nothing command to the game engine.
5. Bots other than .Net should now run correctly on Linux.  (Linux no longer requires elevated privileges, and bots will not run with increased processor priority)
6. That last bot on the game engine will now correctly write it's log files at the end of a round.

### Version 1.0.0 - 11 April 2016
Change Log:

1. Initial release
