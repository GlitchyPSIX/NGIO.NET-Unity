using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#pragma warning disable CS0414 // Field is assigned but its value is never used
#pragma warning disable IDE0052 // Field is never read

namespace NewgroundsIODotNet.Unity.Editor {
    static class NgioNetSettingsEditor {
        public static string k_MyCustomSettingsPath = "Assets/NGIO.NET.Settings.asset";
        internal static NgioDotNetSettings GetOrCreateSettings() {

            var settings = AssetDatabase.LoadAssetAtPath<NgioDotNetSettings>(k_MyCustomSettingsPath);
            if (settings == null) {
                settings = ScriptableObject.CreateInstance<NgioDotNetSettings>();
                settings.m_AppId = null;
                settings.m_EncryptionKey = null;
                settings.m_AppVersion = "1.0.0";
                settings.m_DebugMode = false;
                settings.m_PreloadMedals = true;
                settings.m_PreloadScores = true;
                settings.m_LogViewOnInit = true;
                AssetDatabase.CreateAsset(settings, k_MyCustomSettingsPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings() {
            return new SerializedObject(GetOrCreateSettings());
        }
    }

    // Register a SettingsProvider using IMGUI for the drawing framework:
    static class NgioDotNetSettingsIMGUIRegister {
            [SettingsProvider]
            public static SettingsProvider CreateNgioDotNetSettingsProvider() {
                // First parameter is the path in the Settings window.
                // Second parameter is the scope of this setting: it only appears in the Project Settings window.
                var provider = new SettingsProvider("Project/Newgrounds.io .NET", SettingsScope.Project) {
                    // By default the last token of the path is used as display name if no label is provided.
                    label = "Newgrounds.io .NET Settings",
                    // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                    guiHandler = (searchContext) =>
                    {
                        var settings = NgioNetSettingsEditor.GetSerializedSettings();

                        EditorGUILayout.BeginVertical(new GUIStyle {margin = new RectOffset(10,10,10,10)});
                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("API Tools Info", EditorStyles.boldLabel);
                        EditorGUILayout.HelpBox("The App ID as provided to you in your API Tools tab.", MessageType.None);
                        EditorGUILayout.PropertyField(settings.FindProperty("m_AppId"), new GUIContent("App ID"));

                        EditorGUILayout.HelpBox("The Encryption Key as provided to you in your API Tools tab. NGIO.NET uses AES/Base64.", MessageType.None);
                        EditorGUILayout.PropertyField(settings.FindProperty("m_EncryptionKey"), new GUIContent("Encryption Key"));

                        EditorGUILayout.HelpBox("The App Version as defined in your Project Settings. Format \"X.X.X\" works best.", MessageType.None);
                        EditorGUILayout.PropertyField(settings.FindProperty("m_AppVersion"), new GUIContent("App Version"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(15);

                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("Game Behavior Info", EditorStyles.boldLabel);
                        EditorGUILayout.HelpBox("Whether to preload Medals. Disable only if you're 100% certain you will not use Medals.", MessageType.None);
                        EditorGUILayout.PropertyField(settings.FindProperty("m_PreloadMedals"), new GUIContent("Preload Medals"));

                        EditorGUILayout.HelpBox("Medal Sprite Collection to use with Medal modals. If left empty, all modals will use a generic icon.", MessageType.None);
                        EditorGUILayout.PropertyField(settings.FindProperty("m_MedalSprites"), new GUIContent("Medal Sprites"));

                        EditorGUILayout.HelpBox("Whether to preload Scoreboards. Disable only if you're 100% certain you will not use Scoreboards.", MessageType.None);
                        EditorGUILayout.PropertyField(settings.FindProperty("m_PreloadScores"), new GUIContent("Preload Scoreboards"));
                        
                        EditorGUILayout.PropertyField(settings.FindProperty("m_LogViewOnInit"), new GUIContent("Log View on NGIO.NET Init"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(15);

                        EditorGUILayout.BeginVertical();
                        EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);
                        EditorGUILayout.HelpBox("Enables Newgrounds.io's Debug Mode. Only use if you're actually debugging Newgrounds.io's responses.", MessageType.None);
                        EditorGUILayout.PropertyField(settings.FindProperty("m_DebugMode"), new GUIContent("Debug Mode"));
                        EditorGUILayout.EndVertical();
                        EditorGUILayout.Space(15);
                        EditorGUILayout.EndVertical();

                        settings.ApplyModifiedProperties();
                    },

                    // Populate the search keywords to enable smart search filtering and label highlighting:
                    keywords = new HashSet<string>(new[] { "App ID", "Encryption Key", "App Version", "Preload", "Medals", "Scores", "Log View", "Debug" })
                    
                };

                return provider;
            }
        }
    

}