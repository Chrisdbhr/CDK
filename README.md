# README #

CDK (Chris Development Kit) is a useful set of tools to speedup development of core Unity games mechanics. 

It's being used in my main current game project "[resultarias](https://gamejolt.com/games/resultarias/472865)", a surrealist game about dream exploration.
Check more information about my other projects on [my website](https://chrisjogos.com)!

### What is this repository for? ###

* Host the latest version of the CDK.
* Track issues.

### How do I get set up? ###

* Preferably, create and import this as a submodule inside **Assets/CDK/** folder. If you don't know how to do that, just download [this](https://github.com/Chrisdbhr/CDK/archive/master.zip) and put inside **Assets/CDK/** folder.
* Pay attention for any new warning that will be logged on console.
* There are still no Unit Test implemented since this repository is still in active change.
* Be aware that before version 1.0.0 there will have code modifications that can break some Inspector references. Pay attention when upgrading version.
* I currently using [URP](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest) on my project so the materials in this repository are using URP shaders.

### Dependencies

* [Newtonsoft.Json-for-Unity](https://github.com/jilleJr/Newtonsoft.Json-for-Unity)
* [LINQ For Game Object](https://github.com/neuecc/LINQ-to-GameObject-for-Unity)
* [EasyButtons](https://openupm.com/packages/com.madsbangh.easybuttons/)
* [Debug Draw Extension](https://assetstore.unity.com/packages/tools/debug-drawing-extension-11396)
* [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676)
* [Chronos - Time control](https://ludiq.io/chronos)
* Unity Addressables system (install via package manager)
* Cinemachine (install via package manager)
* [FMOD](https://www.fmod.com) (for audio processing)

### Contribution guidelines ###

* Fell free to open a pull request for fixes or new versions.
* Please follow some code conventions like using "**this.**" on variables even though its not mandatory.

### Who do I talk to? ###

* I am an Unity games and application developer. You can find more info about me and my projects [here](https://chrisjogos.com).

### Special thanks to ###

* [Jetbrains](https://www.jetbrains.com/?from=ChrisDevelopmentKit) and their wonderful tools.