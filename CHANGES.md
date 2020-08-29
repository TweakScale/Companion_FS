# TweakScale Companion :: Firespitter :: Changes

* 2020-0829: 0.0.2.0 Beta (LisiasT) for KSP >= 1.4
	+ Promoting the stunt to Beta! :)
	+ Patches overhaul.
		- Complete rewrite of the patches.
		- Following current TweakScale standards
		- Some crafts using the old patches will need to be reworked.
	+ Updating the linking to use the latest TweakScale Beta
		- Prevents linking against the (current) release of TweakScale, what will render some problems at runtime.
		- KSPe.Light also changed a bit, and the version bump ended up causing an unhappy DLL redirection at loading time. 
* 2020-0716: 0.0.1.1 (LisiasT) for KSP >= 1.4 Alpha
	+ Revised code for the `FSbuyoancy` scaler.
		- Now I works correctly, and I know why! :D
	+ Better (and safer) deactivation code using info gathered from [TweakScale](https://github.com/net-lisias-ksp/TweakScale/issues/125)
	+ Startup check for dependencies
* 2020-0531: 0.0.1.0 (LisiasT) for KSP >= 1.4 Alpha
	+ Initial Public Release
	+ Closes Issues:
		- [#1](https://github.com/net-lisias-ksp/TweakScaleCompantion_FS/issues/1) Weird issue with SXT parts using FSBuoyancy
		- [#2](https://github.com/net-lisias-ksp/TweakScaleCompantion_FS/issues/2) Properly Support FSBuoyancy
