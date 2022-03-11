# README #

CDK (Chris Development Kit) is a useful set of tools to speedup development of core Unity games mechanics. 

It's being used in my main current game project "[resultarias](https://gamejolt.com/games/resultarias/472865)", a surrealist game about dream exploration.
Check more information about my other projects on [my website](https://chrisjogos.com)!

### What is this repository for? ###

* Host the latest version of the CDK.
* Track issues.

### How do I get set up? ###

* Import this as a submodule inside **Assets/CDK/** folder OR download [this](https://github.com/Chrisdbhr/CDK/archive/master.zip) and put inside **Assets/CDK/** folder.

### Dependencies

#### Required
* [UniRx](https://github.com/neuecc/UniRx) for reactive functions.

#### Optional
* [URP](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest) shaders if you want to use materials in this project.
* [Newtonsoft-Json](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.0/manual/index.html), include **Newtonsoft_Json_for_Unity** on define symbols.
* [Unity Converters for Newtonsoft.Json](https://github.com/jilleJr/Newtonsoft.Json-for-Unity.Converters) to fix some Unity Json Serialization issues. 
* [LINQ to Game Object](https://github.com/neuecc/LINQ-to-GameObject-for-Unity), include **LINQ-to-GameObject** on define symbols.
* [EasyButtons](https://openupm.com/packages/com.madsbangh.easybuttons/), include **EasyButtons** on define symbols.
* [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676), include **DOTween** on define symbols.
* [Chronos - Time control](https://ludiq.io/chronos), include **LUDIQ_CHRONOS** on define symbols.
* Unity Addressables system (install via package manager), include **UnityAddressables** on define symbols.
* Unity Localization system (install via package manager), include **UnityLocalization** on define symbols.
* Cinemachine (install via package manager), include **Cinemachine** on define symbols.
* [FMOD](https://www.fmod.com) (for audio processing), include **FMOD** on define symbols.
* [UniTask](https://github.com/Cysharp/UniTask), include **UniTask** on define symbols.
* [Rewired](https://assetstore.unity.com/packages/tools/utilities/rewired-21676) for input handling, **Rewired** on define symbols.

### Contribution guidelines ###

* Fell free to open a pull request for fixes or new versions.
* Please follow some code conventions like using "**this.**" on variables even though its not mandatory.

### Who do I talk to? ###

* I am an Unity games and application developer. You can find more info about me and my projects [here](https://chrisjogos.com).

### Special thanks to ###

* [Jetbrains](https://www.jetbrains.com/?from=ChrisDevelopmentKit) and their wonderful tools.
