var _ = require('lodash'),
    fs = require('fs'),
    moment = require('moment'),
    path = require('path');

var moveFile = 'move.txt';

module.exports = {

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

            if ((callback !== null) && (_.isFunction(callback))) {
                callback();
            }
        });
    },

    randomInt: function(min, max) {
        return Math.floor(Math.random() * (max - min)) + min;
    }
};

function logStdOut() {
    var now = moment();

    var args = [now.format('YYYY-MM-DD HH:mm:ss:SSS'), '\t[BOT]', '\t'];
    for (var i = 0; i < arguments.length; i++) {
        args.push(arguments[i]);
    }

    console.log.apply(this, args);
}

function logStdErr() {
    var now = moment();

    var args = [now.format('YYYY-MM-DD HH:mm:ss:SSS'), '\t[BOT]', '\t'];
    for (var i = 0; i < arguments.length; i++) {
        args.push(arguments[i]);
    }

    console.error.apply(this, args);
}
