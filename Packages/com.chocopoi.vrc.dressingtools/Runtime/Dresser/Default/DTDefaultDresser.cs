﻿/*
 * File: DTDefaultDresser.cs
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

using System.Collections.Generic;
using Chocopoi.DressingTools.Dresser.Default;
using Chocopoi.DressingTools.Dresser.Default.Hooks;
using Chocopoi.DressingTools.Logging;
using Chocopoi.DressingTools.Wearable;
using Newtonsoft.Json;

namespace Chocopoi.DressingTools.Dresser
{
    public class DTDefaultDresser : IDTDresser
    {
        public const string LogLabel = "DTDefaultDresser";

        public static class MessageCode
        {
            //Infos
            public const string GenericInfo = "dressers.default.msgCode.info.generic";

            public const string NonMatchingWearableBoneKeptUntouched = "dressers.default.msgCode.info.nonMatchingWearableBoneKeptUntouched";
            public const string DynamicBoneAllIgnored = "dressers.default.msgCode.info.dynamicBoneAllIgnored";
            public const string AvatarArmatureObjectGuessed = "dressers.default.msgCode.info.avatarArmatureObjectGuessed";
            public const string WearableArmatureObjectGuessed = "dressers.default.msgCode.info.wearableArmatureObjectGuessed";
            public const string MultipleBonesInAvatarArmatureDetectedWarningRemoved = "dressers.default.msgCode.info.multipleBonesInAvatarArmatureDetectedWarningRemoved";
            //only one enabled bone detected, others are disabled (e.g. Maya has a C object that is disabled)

            //Warnings
            public const string GenericWarning = "dressers.default.msgCode.warn.generic";

            public const string MultipleBonesInAvatarArmatureFirstLevel = "dressers.default.msgCode.warn.multipleBonesInAvatarArmatureFirstLevel";
            public const string MultipleBonesInWearableArmatureFirstLevel = "dressers.default.msgCode.warn.multipleBonesInWearableArmatureFirstLevel";
            public const string BonesNotMatchingInArmatureFirstLevel = "dressers.default.msgCode.warn.bonesNotMatchingInArmatureFirstLevel";

            //Errors
            public const string GenericError = "dressers.default.msgCode.error.generic";

            public const string NotDefaultSettingsSettings = "dressers.default.msgCode.error.notDefaultSettingsSettings";
            public const string NoArmatureInAvatar = "dressers.default.msgCode.error.noArmatureInAvatar";
            public const string NoArmatureInWearable = "dressers.default.msgCode.error.noArmatureInWearable";
            public const string NullAvatarOrWearable = "dressers.default.msgCode.error.nullAvatarOrWearable";
            public const string NoBonesInAvatarArmatureFirstLevel = "dressers.default.msgCode.error.noBonesInAvatarArmatureFirstLevel";
            public const string NoBonesInWearableArmatureFirstLevel = "dressers.default.msgCode.error.noBonesInWearableArmatureFirstLevel";
            public const string MissingScriptsDetectedInAvatar = "dressers.default.msgCode.error.missingScriptsDetectedInAvatar";
            public const string MissingScriptsDetectedInWearable = "dressers.default.msgCode.error.missingScriptsDetectedInWearable";
            public const string HookHasErrors = "dressers.default.msgCode.error.hookHasErrors";
        }

        private static readonly IDefaultDresserHook[] Hooks = new IDefaultDresserHook[] {
            new NoMissingScriptsHook(),
            new ArmatureHook()
        };

        public string FriendlyName => "Default Dresser";

        public DTReport Execute(DTDresserSettings settings, out List<DTBoneMapping> boneMappings)
        {
            var report = new DTReport();
            boneMappings = null;

            if (!(settings is DTDefaultDresserSettings))
            {
                report.LogErrorLocalized(LogLabel, MessageCode.NotDefaultSettingsSettings);
                return report;
            }

            // Reject null target avatar/wearable settings
            if (settings.targetAvatar == null || settings.targetWearable == null)
            {
                report.LogErrorLocalized(LogLabel, MessageCode.NullAvatarOrWearable);
                return report;
            }

            boneMappings = new List<DTBoneMapping>();

            // evaluate each hooks to generate the bone mappings
            foreach (var hook in Hooks)
            {
                if (!hook.Evaluate(report, settings, boneMappings))
                {
                    // hook error and do not continue
                    report.LogErrorLocalized(LogLabel, MessageCode.HookHasErrors, hook.GetType().Name);
                    boneMappings = null;
                    return report;
                }
            }

            // return the report
            return report;
        }

        public DTDresserSettings DeserializeSettings(string serializedJson)
        {
            return JsonConvert.DeserializeObject<DTDefaultDresserSettings>(serializedJson);
        }

        public DTDresserSettings NewSettings()
        {
            return new DTDefaultDresserSettings();
        }
    }
}
