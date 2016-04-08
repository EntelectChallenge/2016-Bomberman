var fs = require('fs'),
    path = require('path'),
    util = require('./util');

var mapFile = 'map.txt';

module.exports = {
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
