using UnityEngine;

using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Bundles
{
    public interface IBundle : System.IDisposable
    {
        /// <summary>
        /// Gets the bundle's name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Synchronously loads an asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T LoadAsset<T>(string name) where T : Object;

        /// <summary>
        /// Synchronously loads an asset.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Object LoadAsset(string name, System.Type type);

        /// <summary>
        ///  Asynchronously loads an asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        IProgressResult<float, T> LoadAssetAsync<T>(string name) where T : Object;

        /// <summary>
        /// Asynchronously loads an asset.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IProgressResult<float, Object> LoadAssetAsync(string name, System.Type type);

        /// <summary>
        /// Asynchronously loads an asset.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="names"></param>
        /// <returns></returns>
        IProgressResult<float, T[]> LoadAssetsAsync<T>(params string[] names) where T : Object;

        /// <summary>
        /// Asynchronously loads a group of assets.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="names"></param>
        /// <returns></returns>
        IProgressResult<float, Object[]> LoadAssetsAsync(System.Type type, params string[] names);

        /// <summary>
        /// Synchronously loads all of the assets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        T[] LoadAllAssets<T>() where T : Object;

        /// <summary>
        ///  Synchronously loads all of the assets.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        Object[] LoadAllAssets(System.Type type);

        /// <summary>
        /// Asynchronously loads all of the assets.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        IProgressResult<float, T[]> LoadAllAssetsAsync<T>() where T : Object;

        /// <summary>
        /// Asynchronously loads all of the assets.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        IProgressResult<float, Object[]> LoadAllAssetsAsync(System.Type type);

        /// <summary>
        /// Loads asset and sub assets with name of a given type from the bundle.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Object[] LoadAssetWithSubAssets(string name, System.Type type);

        /// <summary>
        /// Loads asset and sub assets with name of type T from the bundle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        T[] LoadAssetWithSubAssets<T>(string name) where T : Object;

        /// <summary>
        /// Loads asset with sub assets with name of type T from the bundle asynchronously.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name"></param>
        /// <returns></returns>
        IProgressResult<float, T[]> LoadAssetWithSubAssetsAsync<T>(string name) where T : Object;

        /// <summary>
        ///  Loads asset with sub assets with name of a given type from the bundle asynchronously.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        IProgressResult<float, Object[]> LoadAssetWithSubAssetsAsync(string name, System.Type type);
    }
}