![Project Thumbnail](https://chrisdbhr.github.io/images/thumbs/cdk.png)

![GitHub package.json version](https://img.shields.io/github/package-json/v/chrisdbhr/cdk)
[![GitHub license](https://img.shields.io/github/license/chrisdbhr/cdk)](https://github.com/chrisdbhr/cdk/blob/master/LICENSE)
![GitHub package.json dynamic](https://img.shields.io/github/package-json/unity/chrisdbhr/cdk)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/chrisdbhr/cdk)


CDK (Chris Development Kit) is a useful set of tools to speedup development of core Unity games mechanics.

It's being used in my main current game project "[Resultarias](https://chrisjogos.com/resultarias)", a surrealist game about dream exploration.
Check more information about my other projects on [my website](https://chrisjogos.com)!

### What is this repository for? ###

* Host the latest version of the CDK.
* Track issues.

### How do I get set up? ###

* [Recommended] Install from UPM using the git URL ``https://github.com/Chrisdbhr/CDK.git``
* Import this as a submodule inside ``Assets/CDK/`` folder OR download [this](https://github.com/Chrisdbhr/CDK/archive/master.zip) and put inside ``Assets/CDK/`` folder.

### Dependencies

#### Required
* [R3](https://github.com/Cysharp/R3) for reactive functions, instructions of how to install this package [here](https://github.com/Cysharp/R3?tab=readme-ov-file#unity)
* [Reflex](https://github.com/gustavopsantos/Reflex) for Dependency Injection, instructions of how to install this package [here](https://github.com/gustavopsantos/Reflex?tab=readme-ov-file#-installation)

#### Optional
* [Newtonsoft-Json](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.0/manual/index.html), include **NEWTONSOFT_JSON_FOR_UNITY** on define symbols.
* [Unity Converters for Newtonsoft.Json](https://github.com/jilleJr/Newtonsoft.Json-for-Unity.Converters) to fix some Unity Json Serialization issues. 
* [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676), include **DOTWEEN** on define symbols.
* Unity Addressables system (install via package manager), include **UNITY_ADDRESSABLES_EXIST** on define symbols.
* [FMOD](https://www.fmod.com) (for audio processing), include **FMOD** on define symbols.

### Contribution guidelines ###

* Fell free to open a pull request for fixes or new versions.

### Who do I talk to? ###

* I'm an Unity games and application developer. You can find more info about me and my projects [here](https://chrisjogos.com).

### Special thanks to ###

* [Jetbrains](https://www.jetbrains.com/?from=ChrisDevelopmentKit) and their wonderful tools.
