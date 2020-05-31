# TweakScale Companion :: Firespitter :: Known Issues

* A glitch on the `UI_FloatRange` was playing havoc with Tweakscale's attempts to rescale the FSbuoyancy `BaseField`.
	+ You just can't change the min and max values once these values are read from the prefab - not only Editor, but the KSP engine itself restores these values all the time.
	+ The solution was to deactivate the FSbuoyancy from the UI, adding a new `UI_Control` using percentage (as the helper also suffered the same problem) and then feeding the original FSbuoyancy BaseField with the correct value calculated in runtime.
	+ The concrete values (current and max) are also added to PAW for diagnosing purposes.
