using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;

namespace StansAssets.PackageExport.Editor
{
    /// <summary>
    /// Use to export package as <c>.unitypackage</c>
    /// </summary>
    public static class PackageExporter
    {
        private static ListRequest ListPackagesRequest;
        private static List<UnityEditor.PackageManager.PackageInfo> ListPackages;
        private static string _packageName;
        private static PackageExportContext _context;

        /// <summary>
        /// Export package as <c>.unitypackage</c>
        /// </summary>
        /// <param name="packageName">Package name. For example: <c>com.stansassets.package-export</c>. </param>
        /// <param name="context">Package export context. See <see cref="PackageExportContext"/> for details.</param>
        public static void Export(string packageName, PackageExportContext context)
        {
            ListPackages = new List<UnityEditor.PackageManager.PackageInfo>();
            _packageName = packageName;
            _context = context;

            ListPackagesRequest = Client.List();    // List packages installed for the Project
            EditorApplication.update += GetListPackages;
        }
        /// <summary>
        /// Export package as <c>.unitypackage</c>
        /// </summary>
        private static void GetListPackages()
        {
            if (ListPackagesRequest.IsCompleted)
            {
                if (ListPackagesRequest.Status == StatusCode.Success)
                {
                    foreach (var package in ListPackagesRequest.Result)
                    {
                        //Debug.Log("Package name: " + package.name);
                        ListPackages.Add(package);
                    }
                    ExportPack(_packageName, _context);
                }
                else if (ListPackagesRequest.Status >= StatusCode.Failure)
                    Debug.Log(ListPackagesRequest.Error.message);

                EditorApplication.update -= GetListPackages;
            }
        }
        /// <summary>
        /// Export package as <c>.unitypackage</c>
        /// </summary>
        private static void ExportPack(string packageName, PackageExportContext context)
        {
            EditorApplication.update -= GetListPackages;
            Debug.Log(packageName);
            
            var pack = ListPackages.Where(p => p.name == packageName).ToList();

            if (pack.Count < 0)
                throw new InvalidOperationException("Package name not found in project");
            string copyDestination = context.Destination + "/" + packageName;
            DirectoryCopy(pack[0].resolvedPath, copyDestination, true);
            AssetDatabase.Refresh();

            string name = context.Name;
            if (context.AddPackageVersionPostfix)
            {
                name += Assembly.GetExecutingAssembly().GetName().Version;
            }
            string path = context.Destination + "/" + name + ".unitypackage";

            AssetDatabase.ExportPackage(new string[] { context.Destination }, path,  ExportPackageOptions.Recurse | ExportPackageOptions.IncludeDependencies);
            Debug.Log(path + " exported");

            FileUtil.DeleteFileOrDirectory(copyDestination);
            AssetDatabase.Refresh();
        }
        /// <summary>
        /// Export package as <c>.unitypackage</c>
        /// </summary>
        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, copySubDirs);
                }
            }
        }
    }
}

