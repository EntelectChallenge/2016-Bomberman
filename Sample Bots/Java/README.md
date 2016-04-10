## Java

If you plan to develop your entry in Java you need to download and install the latest
   [Java 8 Development Kit]("http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html")

## Maven Installation
Maven installation is fairly simple:

* Maven is free software that can be downloaded [here]("http://maven.apache.org/download.cgi")
* Install as per the included the 'README.txt'
* Ensure the JAVA_HOME environment variable is set to the root of your JDK installation (eg. on Windows: C:\Program Files\Java\jdk8).
* Extract the .zip / .tar.gz archive.
* Add the bin folder from the extracted directory to your path.
* Verify that the installation is working by starting a new command line and running `mvn --version`

If you need to use any libraries in your bot simply include the dependencies in your project's pom.xml.

### Get the Code
Download the [Zip file]("https://github.com/EntelectChallenge/2016-Bomberman/releases/download/v1.0.0/Sample.Bots.zip") from Github or user Git to clone the repository.

* Start Git Bash
* Change to the directory where you want to checkout the sample bot
* Run:`git clone git@github.com:EntelectChallenge/2016-Bomberman.git`

### Compile
The easiest way to compile is to open a new commmand prompt in your bot folder and run `mvn package`. For more details look [here]("https://maven.apache.org/guides/getting-started/maven-in-five-minutes.html").
