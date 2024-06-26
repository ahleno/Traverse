﻿using UnityEngine;
using UnityEditor;

namespace Passer.Humanoid {

    public class Tracker_Editor : Editor {
        public static void Inspector(HumanoidControl humanoid, HumanoidTracker tracker, SerializedProperty enabledProp, string resourceName) {
            EditorGUILayout.BeginHorizontal();
            //EditorGUI.BeginChangeCheck();
            bool wasEnabled = enabledProp.boolValue;
            enabledProp.boolValue = EditorGUILayout.ToggleLeft(tracker.name, enabledProp.boolValue, GUILayout.MinWidth(80));
            if (Application.isPlaying && enabledProp.boolValue)
                EditorGUILayout.EnumPopup(tracker.status);
            EditorGUILayout.EndHorizontal();

            PrefabAssetType prefabType = PrefabUtility.GetPrefabAssetType(humanoid);
            if (prefabType != PrefabAssetType.NotAPrefab)
                return;

            if (tracker.humanoid == null)
                tracker.humanoid = humanoid;

            //if (enabledProp.boolValue && resourceName != null)
            //    tracker.AddTracker(humanoid, resourceName);
            //else if (wasEnabled) {
            //    RemoveTracker(tracker);
            //}

            //tracker.ShowTracker(humanoid.showRealObjects && enabledProp.boolValue);
        }


        public static void Inspector(HumanoidTracker tracker, SerializedProperty enabledProp) {
            EditorGUILayout.BeginHorizontal();
            //EditorGUI.BeginChangeCheck();
            enabledProp.boolValue = EditorGUILayout.ToggleLeft(tracker.name, enabledProp.boolValue, GUILayout.MinWidth(80));
            if (Application.isPlaying && enabledProp.boolValue)
                EditorGUILayout.EnumPopup(tracker.status);
            EditorGUILayout.EndHorizontal();
        }

        protected static void RemoveTracker(HumanoidTracker tracker) {
            if (tracker.trackerComponent == null)
                return;
            DestroyImmediate(tracker.trackerComponent.gameObject, true);
        }

        public static Transform RemoveTransform(Transform trackerTransform) {
            if (trackerTransform != null) {
                DestroyImmediate(trackerTransform.gameObject, true);
            }
            return null;
        }
    }
}