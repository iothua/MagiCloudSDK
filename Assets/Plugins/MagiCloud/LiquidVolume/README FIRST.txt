************************************
*           LIQUID VOLUME          *
* (C) Copyright 2016-2018 Kronnect * 
*            README FILE           *
************************************


How to use this asset
---------------------

Thanks for purchasing LIQUID VOLUME!

Important: please read the brief quick start guide located in LiquidVolume/Documentation.


Support
-------

* Email support: contact@kronnect.me
* Website-Forum Support: http://kronnect.me
* Twitter: @KronnectGames


Other Cool Assets!
------------------

Check our other assets on the Asset Store publisher page:
https://www.assetstore.unity3d.com/en/#!/search/page=1/sortby=popularity/query=publisher:15018



Version history
---------------

Version 4.4 Current Release
- Added "Fix Mesh" option which dynamically centers the pivot of the mesh without cloning the asset
- Expanded Upper/Lower Limit ranges

Version 4.3 2018-JUN-06
- Added Depth Offset parameter to Depth Aware option
- Added list of existing shaders into the inspector to facilitate unwanted shader location and removal
- [Fix] Fixed incorrect level with rotated cylinder

Version 4.2.1 2018-APR-9
- [Fix] Fixed WebGL compatibility issues (now it works, but only for Simple detail level)

Version 4.2 2018-MAR-27
- Added Lower Limit option
- Added SubMesh index option to allow assign liquid volume material to multi-material meshes
- Added 12 flask prefabs (new demo scene Prefabs Collection)
- [Fix] Fixed normals issue with Bake Rotation command

Version 4.1 2018-JAN-17
- Added support for bake transform and center pivot options at runtime
- New demo scene "Pouring": includes new prefab and water particle shader

Version 4.0 2017-SEP-06
- New feature: light scattering option
- New detail option: Smoke (only renders smoke inside flask)
- Smoke can be completely disabled, slightly improving performance
- Some shader optimizations

Version 3.4 2017-JUN-27
- Improved foam appearance with irregular topology
- Added some optimizations to raymarching algorithm
- Added Double Sided Bias parameter to clip non-desired triangles (only irregular topology)
- Added Rotation Level Bias parameter to compensate liquid disappearing under certain rotations and flasks

Version 3.3.1 2017-JUN-12
- [Fix] Fixed white artifacts on cylinder edges with MSAA enabled

Version 3.3 2017-MAY-30
- Updated "WetGlass" demo scene
- Optimized "Simple" detail level
- Added Debug Spill Point under Advanced Section
- Added textureScale and textureOffset options for Flask Texture
- [Fix] Center Pivot and Bake Transform options now properly refresh any collider attached to the Liquid Volume gameobject

Version 3.2  2017-MAY-16
- Experimental support for Skinned Meshes
- Added bump & distortion texture scales and offset parameters
- Added support for animatable properties
- New demo scene: Wet Glass
- [Fix] Fixed colors settings not shown in inspector when detail level was set to simple

Version 3.1 2017-APR-21
- Added Turbulence Speed parameter
- Added Emission Color and Brightness parameters
- API: Added liquidSurfaceYPosition to get the world space vertical position (Y-axis) of liquid surface
- API: Added GetSpillPoint to get the world space position of the point where the liquid starts pouring over the flask when it's rotated
- Added Parent Aware option to force clamp liquid with irregular topology to parent geometry
- [Fix] Fixed Depth Aware option with Single Pass Stereo Rendering

Version 3.0.1 2017-FEB-27
- New demo scene: NonCapped
- New prefab: mug/cup of coffee!

Version 3.0 2017-FEB-14
- New flask type! Irregular. Better adaptation to non-primitive type flasks.
- Improved liquid level calculation when flask is rotated and upperLimit < 1
- [Fix] Fixed depth not being calculated correctly when objects cross the container and depth aware is enabled

Version 2.3 2017-FEB-06
- Added frecuency parameter to allow shorter waves when model scale is greater than one
- [Fix] Fixed rotation issue with react to forces enabled
- [Fix] Shadow went unsynced with liquid

Version 2.2 2017-FEB-07
- Added "React to Forces" feature (new demo scene "Forces")

Version 2.1.1 2017-JAN-03
- Improved default with no flask style to avoid false flask reflections

Version 2.1 2016-DEC-20
- New styles: Default No Flask, Reflections

Version 2.0 2016-DEC-8
- New Reflections style
- VR: Single Pass Stereo Rendering support

Version 1.3 2016-DEC-01
- New Reflections style
- Compatibility with Unity 5.5
- Fixed Center Pivot option so it saves modified changes to model prefabs

Version 1.2 2016-NOV-17
- New "Bake Current Transform" and "Center Pivot" options
- New render queue setting in inspector
- New dither shadow option in inspector

Version 1.1 2016-OCT-26
- New "Ignore Gravity" option to allow liquid rotate with the flask

Version 1.0 2016-SEP-2016 Initial Release







