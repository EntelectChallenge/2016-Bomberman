var MOVES = {
    'nothing': 0,
    'up': 1,
    'left': 2,
    'right': 3,
    'down': 4,
    'bomb': 5,
    'trigger': 6
};

var mapLoader = require('./map'),
    moment = require('moment'),
    stateLoader = require('./state'),
    util = require('./util');

module.exports = main;

function main(botKey, outputPath) {
    var startTime = moment();
    util.log('Started.');

    var state = stateLoader.load(outputPath);
    logPlayerState(state, botKey);

    var map = mapLoader.load(outputPath);
    logMap(map);

    var move = getRandomMove();
    util.outputMove(move, outputPath, function() {
        var endTime = moment();
        var runTime = endTime.diff(startTime);
        util.log('Finished in ' + runTime + 'ms.');
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
