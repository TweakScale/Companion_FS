# TweakScale Companion :: Firespitter

Adds (up to date) TweakScale /L patches for Firespitter.


## Installation Instructions

To install, place the GameData folder inside your Kerbal Space Program folder:

* **REMOVE ANY OLD VERSIONS OF THE PRODUCT BEFORE INSTALLING**, including any other fork:
	+ Delete `<KSP_ROOT>/GameData/TweakScaleCompanion/Firespitter`
* Extract the package's `GameData/` folder into your KSP's as follows:
	+ `<PACKAGE>/GameData/TweakScaleCompanion/Firespitter` --> `<KSP_ROOT>/GameData/TweakScaleCompanion`
		- Overwrite any preexisting file.

The following file layout must be present after installation:

```
<KSP_ROOT>
	[GameData]
		[TweakScale]
			[Plugins]
				KSPe.Light.TweakScale.dll
				Scale.dll
			[patches]
				...
			CHANGE_LOG.md
			DefaultScales.cfg
			Examples.cfg
			LICENSE
			NOTICE
			README.md
			ScaleExponents.cfg
			TweakScale.version
			documentation.txt
		999_Scale_Redist.dll
		ModuleManager.dll
		...
		[TweakScaleCompanion]
			[...]
			[Firespitter]
				CHANGE_LOG.md
				LICENSE*
				NOTICE
				README.md
				[patches]
					...
			[...]
	KSP.log
	PastDatabase.cfg
	...
```


### Dependencies

* TweakScale /L 2.4.7 or later
	+ **NOT** included
* Firespitter
	+ **NOT** included 
* Module Manager 3.1.1 or later
	+ **NOT** included

