using System.IO;

namespace Loxodon.Framework.Bundles
{
    public interface IEncryptor
    {
        string AlgorithmName { get; }

        byte[] Encrypt(byte[] data);

        Stream Encrypt(Stream input);
    }
}
