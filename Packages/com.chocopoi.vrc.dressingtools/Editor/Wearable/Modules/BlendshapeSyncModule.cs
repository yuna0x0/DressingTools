﻿/*
 * File: BlendshapeSyncModule.cs
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
using System.Diagnostics.CodeAnalysis;
using Chocopoi.DressingTools.Lib;
using Chocopoi.DressingTools.Lib.Cabinet;
using Chocopoi.DressingTools.Lib.Wearable;
using Chocopoi.DressingTools.Lib.Wearable.Modules;
using Chocopoi.DressingTools.Lib.Wearable.Modules.Providers;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingTools.Wearable.Modules
{
    internal class BlendshapeSyncModuleConfig : IModuleConfig
    {
        public List<AnimationBlendshapeSync> blendshapeSyncs; // blendshapes to sync from avatar to wearables

        public BlendshapeSyncModuleConfig()
        {
            blendshapeSyncs = new List<AnimationBlendshapeSync>();
        }
    }

    [InitializeOnLoad]
    internal class BlendshapeSyncModuleProvider : ModuleProviderBase
    {
        public const string Identifier = "com.chocopoi.dressingtools.built-in.blendshape-sync";

        [ExcludeFromCodeCoverage] public override string ModuleIdentifier => Identifier;
        [ExcludeFromCodeCoverage] public override string FriendlyName => "Blendshape Sync";
        [ExcludeFromCodeCoverage] public override int ApplyOrder => 6;
        [ExcludeFromCodeCoverage] public override bool AllowMultiple => false;

        static BlendshapeSyncModuleProvider()
        {
            ModuleProviderLocator.Instance.Register(new BlendshapeSyncModuleProvider());
        }

        public override IModuleConfig DeserializeModuleConfig(JObject jObject) => jObject.ToObject<BlendshapeSyncModuleConfig>();

        public override IModuleConfig NewModuleConfig() => new BlendshapeSyncModuleConfig();

        private static void FollowBlendshapeSyncValues(DTCabinet cabinet, GameObject wearableGameObject, WearableModule module)
        {
            var avatarGameObject = cabinet.avatarGameObject;
            var bsm = (BlendshapeSyncModuleConfig)module.config;

            // follow blendshape sync
            foreach (var bs in bsm.blendshapeSyncs)
            {
                var avatarSmrObj = avatarGameObject.transform.Find(bs.avatarPath);
                if (avatarSmrObj == null)
                {
                    Debug.LogWarning("[DressingTools] [BlendshapeSyncProvider] Blendshape sync avatar GameObject at path not found: " + bs.avatarPath);
                    continue;
                }

                var avatarSmr = avatarSmrObj.GetComponent<SkinnedMeshRenderer>();
                if (avatarSmr == null || avatarSmr.sharedMesh == null)
                {
                    Debug.LogWarning("[DressingTools] [BlendshapeSyncProvider] Blendshape sync avatar GameObject at path does not have SkinnedMeshRenderer or Mesh attached: " + bs.avatarPath);
                    continue;
                }

                var avatarBlendshapeIndex = avatarSmr.sharedMesh.GetBlendShapeIndex(bs.avatarBlendshapeName);
                if (avatarBlendshapeIndex == -1)
                {
                    Debug.LogWarning("[DressingTools] [BlendshapeSyncProvider] Blendshape sync avatar GameObject does not have blendshape: " + bs.avatarBlendshapeName);
                    continue;
                }

                var wearableSmrObj = wearableGameObject.transform.Find(bs.wearablePath);
                if (wearableSmrObj == null)
                {
                    Debug.LogWarning("[DressingTools] [BlendshapeSyncProvider] Blendshape sync wearable GameObject at path not found: " + bs.wearablePath);
                    continue;
                }

                var wearableSmr = wearableSmrObj.GetComponent<SkinnedMeshRenderer>();
                if (wearableSmr == null)
                {
                    Debug.LogWarning("[DressingTools] [BlendshapeSyncProvider] Blendshape sync wearable GameObject at path does not have SkinnedMeshRenderer or Mesh attached: " + bs.avatarPath);
                    continue;
                }

                var wearableBlendshapeIndex = wearableSmr.sharedMesh.GetBlendShapeIndex(bs.wearableBlendshapeName);
                if (wearableBlendshapeIndex == -1)
                {
                    Debug.LogWarning("[DressingTools] [BlendshapeSyncProvider] Blendshape sync wearable GameObject does not have blendshape: " + bs.wearableBlendshapeName);
                    continue;
                }

                // copy value from avatar to wearable
                wearableSmr.SetBlendShapeWeight(wearableBlendshapeIndex, avatarSmr.GetBlendShapeWeight(avatarBlendshapeIndex));
            }
        }

        public override bool OnAddWearableToCabinet(DTCabinet cabinet, WearableConfig config, GameObject wearableGameObject, WearableModule module)
        {
            FollowBlendshapeSyncValues(cabinet, wearableGameObject, module);
            return true;
        }

        public override bool OnAfterApplyCabinet(ApplyCabinetContext ctx)
        {
            var wearables = DTEditorUtils.GetCabinetWearables(ctx.cabinet);

            foreach (var wearable in wearables)
            {
                var config = WearableConfig.Deserialize(wearable.configJson);

                if (config == null)
                {
                    Debug.LogWarning("[DressingTools] [BlendshapeSyncModule] Unable to deserialize one of the wearable configuration: " + wearable.name);
                    return false;
                }

                var module = DTEditorUtils.FindWearableModule(config, Identifier);

                if (module == null)
                {
                    // no blendshape sync module, skipping
                    continue;
                }

                FollowBlendshapeSyncValues(ctx.cabinet, wearable.wearableGameObject, module);
            }
            return true;
        }
    }
}