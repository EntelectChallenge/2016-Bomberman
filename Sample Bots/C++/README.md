## C Plus Plus

If you plan to develop your entry in C++ you need to download and install [Visual Studio Express 2013]("http://www.microsoft.com/en-us/download/details.aspx?id=44914")

### Get the Code
Download the [Zip file]("https://github.com/EntelectChallenge/2016-Bomberman/releases/download/v1.0.0/Sample.Bots.zip") from Github or user Git to clone the repository.

* Start Git Bash
* Change to the directory where you want to checkout the sample bot
* Run: `git clone git@github.com:EntelectChallenge/2016-Bomberman.git`

### Compile
The easiest way to compile is to open the solution (`SampleBot.sln`) in Visual Studio and to select Build -> Build Solution from the menus.

You will need to test that your project compiles from the command prompt as well and for this you'll need a Visual Studio Command Prompt. If this was not added to your start menu during the Visual Studio installation follow the instructions in this
     [stackoverflow]("http://stackoverflow.com/questions/21476588/where-is-developer-command-prompt-for-vs2013")
     post to add it to your Visual Studio tools menu.
### NUGET
If your bot depends on any libraries you can install them with the
     [NuGet Package Manager.]("https://docs.nuget.org/consume/installing-nuget")

Be sure to test that [package restore]("https://docs.nuget.org/consume/package-restore") is working so that when the tournament server compiles your bot your dependencies are fetched.</p>
