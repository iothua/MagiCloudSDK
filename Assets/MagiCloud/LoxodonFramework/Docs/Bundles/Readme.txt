Loxodon Framework Bundle
Version: 1.8.11
© 2016, Clark Yang
=======================================

Thank you for purchasing the Loxodon Framework Bundle!
I hope you enjoy using the product and that it makes your game development faster and easier.
If you have a moment,please leave me a review on the Asset Store.

https://www.assetstore.unity3d.com/#!/content/87419

Please email yangpc.china@gmail.com for any help or issues.

UPDATE NOTES
----------------------------------------
version 1.8.11
	Removed loop-referenced exceptions, allowing loop references between multiple asset bundles.
	Upgrade LoxodonFramework to version 1.7.6.

version 1.8.10
	Fixed a bug on Unity2018.
	Upgrade LoxodonFramework to version 1.7.1.

version 1.8.6
	Recommended to use FileAsyncBundleLoader and UnityWebRequestBundleLoader. The WWWBundleLoader in examples is replaced with the above two loaders.

version 1.8.4
	Fixed warning that many variables are not used.
	Added weak cache switch for BundleResources constructor,Objects loaded from AssetBundles are unmanaged objects,the weak caches do not accurately track the validity of objects.If there are some problems after the Resource.UnloadUnusedAssets() is called, please turn off the weak cache.	

version 1.8.2
	Added CRC parameter for generating the hash file name of the AssetBundle.

version 1.8.0
	Added AssetBundle redundancy analysis feature for Unity 5.6 or higher.

version 1.6.8
	Fixed the error that the editor is stuck because of the background thread is not finished

version 1.6.7
	Fixed a bug about packaging on android platform.

version 1.6.6
	Added an example for WebGL.
	Fixed some bugs on the WebGL platform.

version 1.6.5
	Upgrade LoxodonFramework to version 1.6.5.

version 1.5.5
	Upgrade LoxodonFramework to version 1.5.5.

version 1.5.0
	Added support for UWP(Windows10).
	Fixed a few bugs.

version 1.1.1
	Fixed an error for the "for loop" in the AbstractDownloader.cs

version 1.1.0
	Deleted CRC check about loading AssetBundle.
	Fixed a bug that loads an assetbundle in the Unity3d 2017.
	Added "Published" and deleted "IsIndexed" in the BundleInfo.cs
	Added the source code for the Loxodon.Framework.dll and Loxodon.Log.dll

version 1.0.1
	Fixed a bug about initialization of the Executors.
