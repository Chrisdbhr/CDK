![Project Thumbnail](https://chrisdbhr.github.io/images/thumbs/cdk.png)

CDK (Chris Development Kit) is a useful set of tools to speedup development of core Unity games mechanics. 

It's being used in my main current game project "[resultarias](https://chrisjogos.com/resultarias)", a surrealist game about dream exploration.
Check more information about my other projects on [my website](https://chrisjogos.com)!

### What is this repository for? ###

* Host the latest version of the CDK.
* Track issues.

### How do I get set up? ###

* Import this as a submodule inside **Assets/CDK/** folder OR download [this](https://github.com/Chrisdbhr/CDK/archive/master.zip) and put inside **Assets/CDK/** folder.

### Dependencies

#### Required
* [R3](https://github.com/Cysharp/R3) for reactive functions.
	* Install the `R3.Unity` package by referencing the git URL

		`https://github.com/Cysharp/R3.git?path=src/R3.Unity/Assets/R3.Unity`
		![image](https://github.com/Cysharp/ZLogger/assets/46207/7325d266-05b4-47c9-b06a-a67a40368dd2)
		![image](https://github.com/Cysharp/ZLogger/assets/46207/29bf5636-4d6a-4e75-a3d8-3f8408bd8c51)
	
		More details of this package [here](https://github.com/Cysharp/R3?tab=readme-ov-file#unity)

#### Optional
* [Newtonsoft-Json](https://docs.unity3d.com/Packages/com.unity.nuget.newtonsoft-json@3.0/manual/index.html), include **NEWTONSOFT_JSON_FOR_UNITY** on define symbols.
* [Unity Converters for Newtonsoft.Json](https://github.com/jilleJr/Newtonsoft.Json-for-Unity.Converters) to fix some Unity Json Serialization issues. 
* [DOTween](https://assetstore.unity.com/packages/tools/animation/dotween-hotween-v2-27676), include **DOTWEEN** on define symbols.
* Unity Addressables system (install via package manager), include **UNITY_ADDRESSABLES_EXIST** on define symbols.
* [FMOD](https://www.fmod.com) (for audio processing), include **FMOD** on define symbols.
* [Rewired](https://assetstore.unity.com/packages/tools/utilities/rewired-21676) for input handling, **REWIRED** on define symbols.

### Contribution guidelines ###

* Fell free to open a pull request for fixes or new versions.
* Please follow some code conventions like using "**this.**" on variables even though its not mandatory.

### Who do I talk to? ###

* I'm an Unity games and application developer. You can find more info about me and my projects [here](https://chrisjogos.com).

### Special thanks to ###

* [Jetbrains](https://www.jetbrains.com/?from=ChrisDevelopmentKit) and their wonderful tools.
