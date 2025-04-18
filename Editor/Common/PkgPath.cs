using UnityEditor.PackageManager;

namespace NewgroundsIODotNet.Unity.Editor
{
    static class PkgPath
    {
        /// <summary> Returns the <see cref="PackageInfo"/> of the current package. </summary>
        internal static PackageInfo GetPackageInfo()
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            PackageInfo packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssembly(assembly);
            return packageInfo;
        }

        /// <summary> Returns the path to the root folder of the package. </summary>
        internal static string GetPackageRoot()
        {
            return GetPackageInfo().assetPath;
        }
    }
}