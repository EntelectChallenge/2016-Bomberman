# Node.js
If you plan to develop your entry in JavaScript you need to download and install [Node.js v5.10.1](https://nodejs.org/dist/v5.10.1/node-v5.10.1-x64.msi). In the event that you are still using an older version for other projects you should look at using [NVM](https://github.com/creationix/nvm) - it lets you easily switch between multiple versions of Node on your machine.

## Get The Code
Download the [zip file](https://github.com/EntelectChallenge/2016-Bomberman/archive/master.zip) from Github or use Git to clone the repository:
* Start Git Bash
* Change to the directory where you want to checkout the sample bot
* Run: `git clone https://github.com/EntelectChallenge/2016-Bomberman.git`

## Compile
The Node bot isn't really compiled, but you do need to install it's library dependencies. So to get started copy `2016-Bomberman/Sample Bots/Node` to where you want to work on your bot. Then change into that directory and run `npm install` to get all the dependencies.

## Run
Once you have installed your dependencies you can do a basic test of the bot by simply running `node index.js A output/` from inside your bot folder. This will use the example state and map files in the output folder to output some game state information and a random move.

## Code
Start making your bot smarter than just outputting random moves. Some basic ideas:
* Add A* path-finding.
* Set some goals for your bot like staying alive, killing enemies or just blowing up the map.

## Test
The simplest way to test your bot is simply to run it against itself - you can do this from the command line with `Game\ Engine/BomberMan/bin/Debug/Bomberman.exe -b Sample\ Bots/Node/ Sample\ Bots/Node/ --pretty`. If you have downloaded binary release your path to Bomberman.exe will be a bit different, but the principles remain the same.
