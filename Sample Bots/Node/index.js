var	fs = require('fs'),
	main = require('./src/main');

if (process.argv.length < 4) {
    printUsage();
    process.exit(1);
}

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