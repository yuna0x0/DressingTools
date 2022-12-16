﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Plastic.Newtonsoft.Json;
using Unity.Plastic.Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingTools
{
    public class PreferencesUtility
    {
        private static Translation.I18n t = Translation.I18n.GetInstance();

        private static readonly int TargetPreferencesVersion = 2;

        private static readonly string JsonPath = "Assets/chocopoi/DressingTools/Resources/preferences.json";

        private static readonly string DefaultUpdateBranch = "stable";

        private static Preferences preferences = null;

        public static Preferences GetPreferences()
        {
            if (preferences == null)
            {
                preferences = LoadPreferences();
            }
            return preferences;
        }

        public static Preferences LoadPreferences()
        {
            if (!File.Exists(JsonPath))
            {
                Debug.Log("[DressingTools] Preferences file not found, using default preferences instead.");
                return GenerateDefaultPreferences();
            }

            try
            {
                string json = File.ReadAllText(JsonPath);
                Preferences p = JsonConvert.DeserializeObject<Preferences>(json);

                if (p == null)
                {
                    Debug.LogWarning("[DressingTools] Invalid preferences file detected, using default preferences instead");
                    EditorUtility.DisplayDialog("DressingTools", t._("dialog_preferences_invalid_preferences_file"), "OK");
                    return GenerateDefaultPreferences();
                }

                int version = p.version;

                if (version > TargetPreferencesVersion)
                {
                    Debug.LogWarning("[DressingTools] Incompatible preferences version detected, expected version " + TargetPreferencesVersion + " but preferences file is at a newer version " + version + ", using default preferences file instead");
                    EditorUtility.DisplayDialog("DressingTools", t._("dialog_preferences_incompatible_preferences_file", version, TargetPreferencesVersion), "OK");
                    return GenerateDefaultPreferences();
                }
                //TODO: do migration if our version is newer

                return p;
            }
            catch (IOException e)
            {
                Debug.LogError(e);
                EditorUtility.DisplayDialog("DressingTools", t._("dialog_preferences_unable_to_load_preferences_file", e.Message), "OK");
                return GenerateDefaultPreferences();
            }
        }

        public static void SavePreferences()
        {
            try
            {
                File.WriteAllText(JsonPath, JsonConvert.SerializeObject(preferences));
            }
            catch (IOException e)
            {
                Debug.LogError(e);
                EditorUtility.DisplayDialog("DressingTools", t._("dialog_preferences_unable_to_save_preferences_file", e.Message), "OK");
            }
        }

        public static Preferences GenerateDefaultPreferences()
        {
            Preferences p = new Preferences();
            ResetToDefaults(p);
            return p;
        }

        public static void ResetToDefaults(Preferences p)
        {
            // Manifest version
            p.version = TargetPreferencesVersion;

            // App preferences
            if (p.app == null)
            {
                p.app = new Preferences.App();
            }
            p.app.selectedLanguage = 0;

            // attempt to get current branch version
            try
            {
                p.app.updateBranch = DressingToolsUpdater.GetCurrentVersion()?.branch ?? DefaultUpdateBranch;
            }
            catch (Exception)
            {
                p.app.updateBranch = DefaultUpdateBranch;
            }
        }
    }
}