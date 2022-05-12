// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-10
// Description :
// **********************************************************************
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.AddressableAssets.Build;
using UnityEditor.AddressableAssets.Settings;
using System;
using UnityEngine;
using System.IO;
using UnityEditor.AddressableAssets;
using System.Collections.Generic;

namespace StoryEditor
{
    internal class BuildAddressable
    {
        public static string build_script
            = "Assets/AddressableAssetsData/DataBuilders/BuildScriptPackedMode.asset";
        public static string settings_asset
            = "Assets/AddressableAssetsData/AddressableAssetSettings.asset";
        public static string profile_name = "Default";
        private static AddressableAssetSettings settings;

        static void getSettingsObject(string settingsAsset)
        {
            // This step is optional, you can also use the default settings:
            //settings = AddressableAssetSettingsDefaultObject.Settings;

            settings
                = AssetDatabase.LoadAssetAtPath<ScriptableObject>(settingsAsset)
                    as AddressableAssetSettings;

            if (settings == null)
                Debug.LogError($"{settingsAsset} couldn't be found or isn't " +
                               $"a settings object.");
        }

        static void setProfile(string profile)
        {
            string profileId = settings.profileSettings.GetProfileId(profile);
            if (String.IsNullOrEmpty(profileId))
                Debug.LogWarning($"Couldn't find a profile named, {profile}, " +
                                 $"using current profile instead.");
            else
                settings.activeProfileId = profileId;
        }

        static void setBuilder(IDataBuilder builder)
        {
            int index = settings.DataBuilders.IndexOf((ScriptableObject)builder);

            if (index > 0)
                settings.ActivePlayerDataBuilderIndex = index;
            else
                Debug.LogWarning($"{builder} must be added to the " +
                                 $"DataBuilders list before it can be made " +
                                 $"active. Using last run builder instead.");
        }

        static bool buildAddressableContent()
        {
            AddressableAssetSettings
                .BuildPlayerContent(out AddressablesPlayerBuildResult result);
            bool success = string.IsNullOrEmpty(result.Error);

            if (!success)
            {
                Debug.LogError("Addressables build error encountered: " + result.Error);
            }
            return success;
        }

        //[MenuItem("Window/Asset Management/Addressables/Build Addressables only")]
        public static bool BuildAddressables()
        {
            getSettingsObject(settings_asset);
            setProfile(profile_name);
            IDataBuilder builderScript
              = AssetDatabase.LoadAssetAtPath<ScriptableObject>(build_script) as IDataBuilder;

            if (builderScript == null)
            {
                Debug.LogError(build_script + " couldn't be found or isn't a build script.");
                return false;
            }

            setBuilder(builderScript);

            return buildAddressableContent();
        }

        //[MenuItem("Window/Asset Management/Addressables/Build Addressables and Player")]
        public static void BuildAddressablesAndPlayer()
        {
            bool contentBuildSucceeded = BuildAddressables();

            if (contentBuildSucceeded)
            {
                var options = new BuildPlayerOptions();
                BuildPlayerOptions playerSettings
                    = BuildPlayerWindow.DefaultBuildMethods.GetBuildPlayerOptions(options);

                BuildPipeline.BuildPlayer(playerSettings);
            }
        }


        //[MenuItem("AddressableEditor/SetAllDirectorToAddress", priority = 2)]
        public static void SetAllDirectorToAddress()
        {
            var arr = Selection.GetFiltered(typeof(DefaultAsset), SelectionMode.Assets);
            string folder = AssetDatabase.GetAssetPath(arr[0]); ;
            LoopSetAllDirectorToAddress(folder);

        }

        public static void LoopSetAllDirectorToAddress(string pFileDirectorRoot)
        {
            if (Directory.Exists(pFileDirectorRoot))
            {
                SetDirectorABNameNull(pFileDirectorRoot);
                var dirctory = new DirectoryInfo(pFileDirectorRoot);
                var direcs = dirctory.GetDirectories("*", SearchOption.TopDirectoryOnly);
                if (direcs.Length > 0)
                {
                    for (var i = 0; i < direcs.Length; i++)
                    {
                        if (direcs[i].FullName != pFileDirectorRoot)
                        {
                            LoopSetAllDirectorToAddress(direcs[i].FullName);
                        }
                    }
                }
            }
        }
        private static void SetDirectorABNameNull(string pFileDirectorRoot)
        {
            if (Directory.Exists(pFileDirectorRoot))
            {
                var dirctory = new DirectoryInfo(pFileDirectorRoot);
                var files = dirctory.GetFiles("*", SearchOption.TopDirectoryOnly);
                bool isAdd = false;
                for (var i = 0; i < files.Length; i++)
                {
                    var file = files[i];
                    if (file.Name.EndsWith(".meta"))
                        continue;
                    if (file.Name.EndsWith(".txt"))
                        continue;
                    string assetPath = file.FullName;
                    assetPath = EditorHelper.FormatFilePath(assetPath);
                    var assetLength = UnityEngine.Application.dataPath.Length - 6;
                    assetPath = assetPath.Substring(assetLength, assetPath.Length - assetLength);
                    AutoGroup(dirctory.Name, assetPath);

                    isAdd = true;
                }
                if (isAdd)
                    AssetDatabase.Refresh();
            }

        }
        public static void AutoGroup(string groupName, string assetPath)
        {
            var settings = AddressableAssetSettingsDefaultObject.Settings;
            AddressableAssetGroup group = settings.FindGroup(groupName);
            if (group == null)
            {
                group = CreatAssetGroup<System.Data.SchemaType>(settings, groupName);
            }
            var guid = AssetDatabase.AssetPathToGUID(assetPath);
            var entry = settings.CreateOrMoveEntry(guid, group);
            entry.address = assetPath;
            entry.SetLabel(groupName, true, true);

        }
        private static AddressableAssetGroup CreatAssetGroup<SchemaType>(AddressableAssetSettings settings, string groupName)
        {
            return settings.CreateGroup(groupName, false, false, false,
                new List<AddressableAssetGroupSchema> { settings.DefaultGroup.Schemas[0], settings.DefaultGroup.Schemas[1] }, typeof(SchemaType));

        }
    }
}
#endif

