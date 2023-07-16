﻿using System.Collections.Generic;
using Chocopoi.DressingTools.Cabinet;
using Chocopoi.DressingTools.Logging;
using Chocopoi.DressingTools.Proxy;
using UnityEngine;

namespace Chocopoi.DressingTools.Applier
{
    public interface IDTApplier
    {
        DTApplierSettings DeserializeSettings(string serializedJson);
        DTApplierSettings NewSettings();
        bool ApplyBoneMappings(DTReport report, DTApplierSettings settings, DTWearableConfig wearableConfig, List<IDynamicsProxy> avatarDynamics, List<IDynamicsProxy> wearableDynamics, List<DTBoneMapping> boneMappings, GameObject targetAvatar, GameObject targetWearable);
        DTReport ApplyCabinet(DTApplierSettings settings, DTCabinet cabinet);
    }
}
