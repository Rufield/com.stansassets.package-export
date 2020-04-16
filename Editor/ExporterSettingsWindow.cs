using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using System.Collections.Generic;

namespace StansAssets.PackageExport.Editor
{
    public class ExporterSettingsWindow : EditorWindow
    {
        static ListRequest ListPackagesRequest;
        static List<string> ListPackages;
        int PackageIndex;
        string Path = "Assets/Plugins";

        [MenuItem("Stan's Assets/ExporterSettingsWindow")]
        public static void OpenSettingsTest()
        {
            ExporterSettingsWindow editor = GetWindow<ExporterSettingsWindow>();
            editor.Init();
        }

        void Init()
        {
            PackageIndex = 0;
            ListPackages = new List<string>();
            ListPackagesRequest = Client.List();    // List packages installed for the Project
            EditorApplication.update += Progress;
        }
        static void Progress()
        {
            if (ListPackagesRequest.IsCompleted)
            {
                if (ListPackagesRequest.Status == StatusCode.Success)
                    foreach (var package in ListPackagesRequest.Result)
                    {
                        //Debug.Log("Package name: " + package.name);
                        ListPackages.Add(package.name);
                    }
                else if (ListPackagesRequest.Status >= StatusCode.Failure)
                    Debug.Log(ListPackagesRequest.Error.message);

                EditorApplication.update -= Progress;
            }
        }

        void OnGUI()
        {
            EditorGUILayout.Separator();
            PackageIndex = EditorGUI.Popup(
            new Rect(0, 10, position.width, 20),
            "Package:",
            PackageIndex,
            ListPackages.ToArray());
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            EditorGUILayout.Separator();
            Path = EditorGUILayout.TextField("Save package:", Path);
            if (GUILayout.Button("Выбрать", GUILayout.Height(30)))
            {
                Path = EditorUtility.OpenFolderPanel("Save package", "", "");
            }

            EditorGUILayout.Separator();
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Обновить / Сбросить", GUILayout.Height(30)))
            {
                Init();
            }
            if (GUILayout.Button("Экспортировать", GUILayout.Height(30)))
            {
                Debug.Log("Name Package " + ListPackages[PackageIndex]);
                Debug.Log("path " + Path);
                var context = new PackageExportContext("PackageExport", Path + "/StansAssets/Test")
                {
                    AddPackageVersionPostfix = true
                };

                PackageExporter.Export(ListPackages[PackageIndex], Path, context);
            }

        }
    }
}
