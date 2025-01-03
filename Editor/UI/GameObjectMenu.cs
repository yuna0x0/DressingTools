﻿/*
 * File: GameObjectMenu.cs
 * Project: DressingTools
 * Created Date: Friday, September 8th 2023, 11:56:21 am
 * Author: chocopoi (poi@chocopoi.com)
 * -----
 * Copyright (c) 2023 chocopoi
 * 
 * This file is part of DressingTools.
 * 
 * DressingTools is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * DressingTools is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License along with DressingTools. If not, see <https://www.gnu.org/licenses/>.
 */

using System.Diagnostics.CodeAnalysis;
using Chocopoi.DressingFramework.Detail.DK.Logging;
using Chocopoi.DressingFramework.Localization;
using Chocopoi.DressingTools.Components.OneConf;
using Chocopoi.DressingTools.Dresser.Standard;
using Chocopoi.DressingTools.Localization;
using Chocopoi.DressingTools.OneConf;
using Chocopoi.DressingTools.OneConf.Cabinet;
using Chocopoi.DressingTools.OneConf.Serialization;
using Chocopoi.DressingTools.OneConf.Wearable;
using Chocopoi.DressingTools.OneConf.Wearable.Modules;
using Chocopoi.DressingTools.OneConf.Wearable.Modules.BuiltIn;
using Chocopoi.DressingTools.OneConf.Wearable.Modules.BuiltIn.ArmatureMapping;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingTools.UI
{
    [ExcludeFromCodeCoverage]
    internal static class GameObjectMenu
    {
        private static readonly I18nTranslator t = I18n.ToolTranslator;

        // note that in order for a menu item in "GameObject/" to be propagated to the
        // hierarchy Create dropdown and hierarchy context menu, it must be grouped with
        // the other GameObject creation menu items. This can be achieved by setting its priority to 10 
        private const int MenuItemPriority = 20;

        private static DTCabinet LookUpCabinet(Transform transform)
        {
            var p = transform.parent;
            DTCabinet cabinet = null;
            while (p != null)
            {
                if (p.TryGetComponent(out cabinet))
                {
                    break;
                }
                p = p.parent;
            }
            return cabinet;
        }

        [MenuItem("GameObject/DressingTools/Quick setup clothes", false, MenuItemPriority)]
        public static void QuickAutoSetup(MenuCommand menuCommand)
        {
            if (menuCommand.context is not GameObject)
            {
                return;
            }

            var wearable = (GameObject)menuCommand.context;

            // find the avatar
            var cabinet = LookUpCabinet(wearable.transform);

            if (cabinet == null)
            {
                // no parent or grandparents has the cabinet
                EditorUtility.DisplayDialog(t._("tool.name"), t._("menu.dialog.msg.avatarNoCabinetAttached"), t._("common.dialog.btn.ok"));
                return;
            }

            if (!CabinetConfigUtility.TryDeserialize(cabinet.ConfigJson, out var cabinetConfig))
            {
                EditorUtility.DisplayDialog(t._("tool.name"), t._("menu.dialog.msg.unableToLoadCabinetConfig"), t._("common.dialog.btn.ok"));
                return;
            }

            if (wearable.TryGetComponent<DTWearable>(out var existingComp))
            {
                if (!EditorUtility.DisplayDialog(t._("tool.name"), t._("menu.dialog.msg.existingWearableConfigWipeConfirm"), t._("common.dialog.btn.yes"), t._("common.dialog.btn.no")))
                {
                    return;
                }

                Object.DestroyImmediate(existingComp);
            }

            var wearableConfig = new WearableConfig();
            OneConfUtils.PrepareWearableConfig(wearableConfig, cabinet.RootGameObject, wearable);

            var armatureName = cabinetConfig.avatarArmatureName;

            // attempt to find wearable armature using avatar armature name
            var armature = OneConfUtils.GuessArmature(wearable, armatureName);

            if (armature == null)
            {
                // TODO: ask to select a location for move to
                EditorUtility.DisplayDialog(t._("tool.name"), t._("menu.dialog.msg.unableToAutoDetectWearableArmature"), t._("common.dialog.btn.ok"));
                return;
            }
            else
            {
                if (armature.name != armatureName)
                {
                    // TODO: show message
                }

                var dresser = new StandardDresser();
                var settings = new StandardDresserSettings()
                {
                    SourceArmature = armature,
                    TargetArmaturePath = armatureName,
                    DynamicsOption = StandardDresserSettings.DynamicsOptions.Auto
                };

                var report = new DKReport();
                dresser.Execute(report, cabinet.gameObject, settings, out _, out _);

                if (report.HasLogType(DressingFramework.Logging.LogType.Error))
                {
                    ReportWindow.AddReport(cabinet.RootGameObject.name, report);
                    ReportWindow.ShowWindow();
                    EditorUtility.DisplayDialog(t._("tool.name"), t._("menu.dialog.msg.standardDresserHasErrors"), t._("common.dialog.btn.ok"));
                    return;
                }

                var oldDresserSettings = new ArmatureMappingWearableModuleProvider.DefaultDresserSettings()
                {
                    dynamicsOption = ArmatureMappingWearableModuleProvider.DefaultDresserSettings.DynamicsOptions.Auto
                };

                var armatureMappingModule = new ArmatureMappingWearableModuleConfig
                {
                    dresserName = typeof(StandardDresser).FullName,
                    wearableArmatureName = armature.name,
                    boneMappingMode = BoneMappingMode.Auto,
                    boneMappings = null,
                    serializedDresserConfig = JsonConvert.SerializeObject(oldDresserSettings),
                    removeExistingPrefixSuffix = true,
                    groupBones = true
                };

                wearableConfig.modules.Add(new WearableModule()
                {
                    moduleName = ArmatureMappingWearableModuleConfig.ModuleIdentifier,
                    config = armatureMappingModule
                });
            }

            cabinet.AddWearable(wearableConfig, wearable);
        }

        [MenuItem("GameObject/DressingTools/Setup wearable with editor", false, MenuItemPriority)]
        public static void StartSetupWizard(MenuCommand menuCommand)
        {
            if (menuCommand.context is not GameObject)
            {
                return;
            }

            var outfitGameObject = (GameObject)menuCommand.context;

            // find the avatar
            var cabinet = LookUpCabinet(outfitGameObject.transform);

            if (cabinet == null)
            {
                // no parent or grandparents has the cabinet
                EditorUtility.DisplayDialog(t._("tool.name"), t._("menu.dialog.msg.avatarNoCabinetAttached"), t._("common.dialog.btn.ok"));
                return;
            }

            var window = (DTMainEditorWindow)EditorWindow.GetWindow(typeof(DTMainEditorWindow));
            window.titleContent = new GUIContent(t._("tool.name"));
            window.Show();
            window.StartDressing(outfitGameObject, cabinet.gameObject);
        }
    }
}
