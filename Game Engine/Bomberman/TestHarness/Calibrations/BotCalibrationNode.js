var	fs = require('fs'),
    path = require('path');

if (process.argv.length < 4) {
    printUsage();
    process.exit(1);
}

var MOVES = {
    'nothing': 0,
    'up': 1,
    'left': 2,
    'right': 3,
    'down': 4,
    'bomb': 5,
    'trigger': 6
};

var botKey = process.argv[2];
var outputPath = process.argv[3];
fs.stat(outputPath, function(err, stat) {
    if(err == null) {
        main(botKey, outputPath);
    } else {
        printUsage();
        console.log('');
        console.log('Error: Output folder "' + outputPath + '" does not exist.');

        process.exit(1);
    }
});

function printUsage() {
    console.log('');
    console.log('Node Sample Bot usage:');
    console.log('');
    console.log('node index.js <botKey> <outputPath>');
    console.log('');
    console.log('\tbotKey\t key identifying your bot');
    console.log('\toutputPath\t relative path to the folder where the engine will read & write files');
}

function main(botKey, outputPath) {
    util.log('Started.');

    var state = stateLoader.load(outputPath);

    var move = getRandomMove();
    util.outputMove(move, outputPath, function() {
        util.log('Finished in undetermined ms.');
    });
}

function logPlayerState(state, botKey) {
    if (state === null) {
        util.logError('Failed to load state.');
        return;
    }

    var myPlayer = null;
    for (var i = 0; i < state.RegisteredPlayerEntities.length; i++) {
        var player = state.RegisteredPlayerEntities[i];

        if (player.Key === botKey) {
            myPlayer = player;
        }
    }

    if (myPlayer !== null) {
        util.log('Player state:')
        util.log('\tName: ', myPlayer.Name);
        util.log('\tPoints: ', myPlayer.Points);
        util.log('\tAlive: ', !myPlayer.Killed);
        util.log('\tLocation: (', myPlayer.Location.x + ', ' + myPlayer.Location.y + ')');
    }
}

function logMap(map) {
    if (map === null) {
        util.logError('Failed to load map.');
    }

    util.log('Map:\n' + map.text);
}

function getRandomMove() {
    var moveKeys = Object.keys(MOVES);
    var randomKey = moveKeys[util.randomInt(0, moveKeys.length)];

    return MOVES[randomKey];
}

var mapFile = 'map.txt';

var mapLoader = {
    load: function(outputPath) {
        var filename = path.join(outputPath, mapFile);

        if (!fs.existsSync(filename)) {
            util.logError('Map file not found: ' + filename);
            return null;
        }

        var mapText = fs.readFileSync(filename);
        return this.parse(mapText);
    },

    parse: function(mapText) {
        // TODO: Parse useful information out of the map

        return {
            text: mapText
        }
    }
}

var stateFile = 'state.json';

var stateLoader = {
    load: function(outputPath) {
        var filename = path.join(outputPath, stateFile);

        if (!fs.existsSync(filename)) {
            util.logError('State file not found: ' + filename);
            return null;
        }

        var fileContents = fs.readFileSync(filename, {encoding: 'utf8'});

        return gameState;
    }
};

var moveFile = 'move.txt';

var util = {

    log: logStdOut,

    logError: logStdErr,

    outputMove: function(move, outputPath, callback) {
        var filename = path.join(outputPath, moveFile);

        if (!fs.existsSync(outputPath)) {
            fs.mkdirSync(outputPath);
        }

        fs.writeFile(filename, move, function(err) {
            if (err) {
                logStdErr('Failed to write ' + filename + '!');
                logStdErr(err.stack);
                return;
            }

            logStdOut('Move: ' + move);

            if ((callback !== null) && (typeof callback === "function")) {
                callback();
            }
        });
    },

    randomInt: function(min, max) {
        return Math.floor(Math.random() * (max - min)) + min;
    }
};

function logStdOut() {
    var args = ['\t[BOT]', '\t'];
    for (var i = 0; i < arguments.length; i++) {
        args.push(arguments[i]);
    }

    console.log.apply(this, args);
}

function logStdErr() {
    var args = ['\t[BOT]', '\t'];
    for (var i = 0; i < arguments.length; i++) {
        args.push(arguments[i]);
    }

    console.error.apply(this, args);
}
