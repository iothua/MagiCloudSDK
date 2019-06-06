using System.IO;

namespace Loxodon.Framework.Bundles
{
    public interface IDecryptor
    {
        string AlgorithmName { get; }

        byte[] Decrypt(byte[] data);

        Stream Decrypt(Stream input);
    }
}
