﻿using System.Collections.Generic;
using Chocopoi.DressingTools.Cabinet;
using Chocopoi.DressingTools.Dresser.Default;
using Chocopoi.DressingTools.Dresser.Default.Hooks;
using Chocopoi.DressingTools.Logging;

namespace Chocopoi.DressingTools.Dresser
{
    public class DTDefaultDresser : IDTDresser
    {
        public static class MessageCode
        {
            //Infos
            public const int GenericInfo = 0x0000;

            public const int NonMatchingWearableBoneKeptUntouched = 0x0001;
            public const int DynamicBoneAllIgnored = 0x0002;
            public const int AvatarArmatureObjectGuessed = 0x0003;
            public const int WearableArmatureObjectGuessed = 0x0004;
            public const int MultipleBonesInAvatarArmatureDetectedWarningRemoved = 0x0005; //only one enabled bone detected, others are disabled (e.g. Maya has a C object that is disabled)

            //Warnings
            public const int GenericWarning = 0x1000;

            public const int MultipleBonesInAvatarArmatureFirstLevel = 0x1001;
            public const int MultipleBonesInWearableArmatureFirstLevel = 0x1002;
            public const int BonesNotMatchingInArmatureFirstLevel = 0x1003;

            //Errors
            public const int GenericError = 0x2000;

            public const int NoArmatureInAvatar = 0x2001;
            public const int NoArmatureInWearable = 0x2002;
            public const int NullAvatarOrWearable = 0x2003;
            public const int NoBonesInAvatarArmatureFirstLevel = 0x2004;
            public const int NoBonesInWearableArmatureFirstLevel = 0x2005;
            public const int MissingScriptsDetectedInAvatar = 0x2006;
            public const int MissingScriptsDetectedInWearable = 0x2007;
            public const int WearableInsideAvatar = 0x2008;
            public const int AvatarInsideWearable = 0x2009;
        }

        private static readonly IDefaultDresserHook[] hooks = new IDefaultDresserHook[] {
            new NoMissingScriptsHook(),
            new ObjectPlacementHook(),
            new ArmatureHook(),
            new RootObjectsHook()
        };

        public DTReport Execute(DTDresserSettings settings, out List<DTBoneMapping> boneMappings, out List<DTObjectMapping> objectMappings)
        {
            var report = new DTReport();
            boneMappings = null;
            objectMappings = null;

            if (!(settings is DTDefaultDresserSettings))
            {
                report.LogError(MessageCode.GenericError, "Settings is not an instance of DTDefaultDresserSettings");
                report.Result = DTReportResult.InvalidSettings;
                return report;
            }

            // Reject null target avatar/wearable settings
            if (settings.targetAvatar == null || settings.targetWearable == null)
            {
                report.LogError(MessageCode.NullAvatarOrWearable, "Target avatar or wearable is null.");
                report.Result = DTReportResult.InvalidSettings;
                return report;
            }

            boneMappings = new List<DTBoneMapping>();
            objectMappings = new List<DTObjectMapping>();

            // evaluate each hooks to generate the bone mappings
            foreach (var hook in hooks)
            {
                if (!hook.Evaluate(report, settings, boneMappings, objectMappings))
                {
                    // hook error and do not continue
                    report.LogError(MessageCode.GenericError, string.Format("Dresser execution aborted as hook \"{0}\" reported an error status.", hook.GetType().Name));
                    report.Result = DTReportResult.Incompatible;
                    boneMappings = null;
                    objectMappings = null;
                    return report;
                }
            }

            // check the log result to see if any warnings and infos
            var dict = report.GetLogEntriesAsDictionary();

            if (dict.ContainsKey(DTReportLogType.Warning))
            {
                report.Result = DTReportResult.Compatible;
            }
            else
            {
                report.Result = DTReportResult.Ok;
            }

            // return the report
            return report;
        }
    }
}
