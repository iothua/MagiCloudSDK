using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ICSharpCode.SharpZipLib.Zip;
using UnityEngine;

public abstract class UnzipCallback
{
    /// <summary>
    /// 解压单个文件或文件夹前执行的回调
    /// </summary>
    /// <param name="_entry"></param>
    /// <returns>如果返回true，则压缩文件或文件夹，反之则不压缩文件或文件夹</returns>
    public virtual bool OnPreUnzip(ZipEntry _entry)
    {
        return true;
    }

    /// <summary>
    /// 解压单个文件或文件夹后执行的回调
    /// </summary>
    /// <param name="_entry"></param>
    public virtual void OnPostUnzip(ZipEntry _entry) { }

    /// <summary>
    /// 解压执行完毕后的回调
    /// </summary>
    /// <param name="_result">true表示解压成功，false表示解压失败</param>
    public virtual void OnFinished(bool _result) { }
}



public static class MZip
{

    /// 解压Zip包
    /// </summary>
    /// <param name="_filePathName">Zip包的文件路径名</param>
    /// <param name="_outputPath">解压输出路径</param>
    /// <param name="_password">解压密码</param>
    /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
    /// <returns></returns>
    public static bool UnzipFile(string _filePathName, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null)
    {
        if (string.IsNullOrEmpty(_filePathName) || string.IsNullOrEmpty(_outputPath))
        {
            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);

            return false;
        }

        try
        {
            return UnzipFile(File.OpenRead(_filePathName), _outputPath, _password, _unzipCallback);
        }
        catch (System.Exception _e)
        {
            Debug.LogError("[ZipUtility.UnzipFile]: " + _e.ToString());

            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);

            return false;
        }
    }

    /// <summary>
    /// 解压Zip包
    /// </summary>
    /// <param name="_fileBytes">Zip包字节数组</param>
    /// <param name="_outputPath">解压输出路径</param>
    /// <param name="_password">解压密码</param>
    /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
    /// <returns></returns>
    public static bool UnzipFile(byte[] _fileBytes, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null)
    {
        if ((null == _fileBytes) || string.IsNullOrEmpty(_outputPath))
        {
            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);

            return false;
        }

        bool result = UnzipFile(new MemoryStream(_fileBytes), _outputPath, _password, _unzipCallback);
        if (!result)
        {
            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);
        }

        return result;
    }

    /// <summary>
    /// 解压Zip包
    /// </summary>
    /// <param name="_inputStream">Zip包输入流</param>
    /// <param name="_outputPath">解压输出路径</param>
    /// <param name="_password">解压密码</param>
    /// <param name="_unzipCallback">UnzipCallback对象，负责回调</param>
    /// <returns></returns>
    public static bool UnzipFile(Stream _inputStream, string _outputPath, string _password = null, UnzipCallback _unzipCallback = null)
    {
        if ((null == _inputStream) || string.IsNullOrEmpty(_outputPath))
        {
            if (null != _unzipCallback)
                _unzipCallback.OnFinished(false);

            return false;
        }

        // 创建文件目录
        if (!Directory.Exists(_outputPath))
            Directory.CreateDirectory(_outputPath);

        // 解压Zip包
        ZipEntry entry = null;
        Encoding gbk = Encoding.GetEncoding("gbk");
        ZipConstants.DefaultCodePage = gbk.CodePage;
        using (ZipInputStream zipInputStream = new ZipInputStream(_inputStream))
        {
            if (!string.IsNullOrEmpty(_password))
                zipInputStream.Password = _password;

            while (null != (entry = zipInputStream.GetNextEntry()))
            {
                if (null == entry || string.IsNullOrEmpty(entry.Name))
                    continue;

                //entry.IsUnicodeText = true;
                if ((null != _unzipCallback) && !_unzipCallback.OnPreUnzip(entry))
                    continue;   // 过滤

                string filePathName = Path.Combine(_outputPath, entry.Name);

                // 创建文件目录
                if (entry.IsDirectory)
                {
                    Directory.CreateDirectory(filePathName);
                    continue;
                }

                // 写入文件
                try
                {
                    using (FileStream fileStream = File.Create(filePathName))
                    {
                        byte[] bytes = new byte[1024];
                        while (true)
                        {
                            int count = zipInputStream.Read(bytes, 0, bytes.Length);
                            if (count > 0)
                                fileStream.Write(bytes, 0, count);
                            else
                            {
                                if (null != _unzipCallback)
                                    _unzipCallback.OnPostUnzip(entry);

                                break;
                            }
                        }
                    }
                }
                catch (System.Exception _e)
                {
                    Debug.LogError("[ZipUtility.UnzipFile]: " + _e.ToString());

                    if (null != _unzipCallback)
                        _unzipCallback.OnFinished(false);

                    return false;
                }
            }
        }

        if (null != _unzipCallback)
            _unzipCallback.OnFinished(true);

        return true;
    }

}
