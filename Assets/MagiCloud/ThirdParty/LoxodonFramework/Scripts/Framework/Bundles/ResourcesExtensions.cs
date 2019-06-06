
namespace Loxodon.Framework.Bundles
{
    public static class ResourcesExtensions
    {
        private static LocalResources localResources;
        public static IResources InResources(this IResources resources)
        {
            if (resources is LocalResources)
                return resources;

            if (localResources == null)
                localResources = new LocalResources();

            return localResources;
        }
    }
}
