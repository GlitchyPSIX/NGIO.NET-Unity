using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace NewgroundsIODotNet.Unity.Editor {
    [CustomEditor(typeof(NGIONet))]
    public class NgioNetEngineEditor : UnityEditor.Editor {
        public override VisualElement CreateInspectorGUI() {
            // Create a new VisualElement to be the root of our Inspector UI.
            VisualElement myInspector = new VisualElement();

            // Add a simple label.
            myInspector.Add(new HelpBox("Are you looking where to put NGIO settings like your App ID, encryption key and medal sprites?\n They are in Project Settings. Look for the Newgrounds.io .NET tab.", HelpBoxMessageType.Info));
            myInspector.Add(new PropertyField(serializedObject.FindProperty("MedalUnlocked"), "On Medal unlock"));
            myInspector.Add(new HelpBox("When started up, this Tank Engine's Communicator becomes available everywhere through the \"NGIONet.Engine.Comms\" singleton instance.\nCreating a new Tank Engine will restart the Communicator and destroy the old one.", HelpBoxMessageType.Info));

            if (serializedObject.FindProperty("NgioSettings").objectReferenceValue == null) {
                string path = NgioNetSettingsEditor.k_MyCustomSettingsPath;
                serializedObject.FindProperty("NgioSettings").objectReferenceValue = AssetDatabase.LoadAssetAtPath<NgioDotNetSettings>(path);
                serializedObject.ApplyModifiedPropertiesWithoutUndo();
            }

            // if after that we can't
            if (serializedObject.FindProperty("NgioSettings").objectReferenceValue == null) {
                myInspector.Add(new HelpBox("Could not read the NGIO.NET settings. Please open your Project Settings and make sure everything is in order.", HelpBoxMessageType.Warning));
            }

            // Return the finished Inspector UI.
                return myInspector;
        }
    }
}