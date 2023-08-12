﻿/*
 * File: UnknownModuleEditor.cs
 * Project: DressingTools
 * Created Date: Tuesday, August 1st 2023, 12:37:10 am
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

using Chocopoi.DressingTools.UIBase.Views;
using Chocopoi.DressingTools.Wearable;
using Chocopoi.DressingTools.Wearable.Modules;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingTools.UI.Views.Modules
{
    [CustomModuleEditor(typeof(UnknownModule))]
    internal class UnknownModuleEditor : ModuleEditor
    {
        private static Localization.I18n t = Localization.I18n.GetInstance();

        public UnknownModuleEditor(IModuleEditorViewParent parentView, DTWearableModuleBase target) : base(parentView, target)
        {
        }

        public override void OnGUI()
        {
            var module = (UnknownModule)target;
            EditorGUILayout.HelpBox(t._("modules.unknown.helpBox.unknownModuleDetected", module.moduleTypeName), MessageType.Warning);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
