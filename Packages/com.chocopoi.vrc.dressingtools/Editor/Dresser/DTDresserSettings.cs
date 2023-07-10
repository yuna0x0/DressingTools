﻿using UnityEditor;
using UnityEngine;

namespace Chocopoi.DressingTools.Dresser
{
    public class DTDresserSettings
    {
        public GameObject targetAvatar;

        public GameObject targetWearable;

        public string avatarArmatureName;

        public string wearableArmatureName;

        public virtual bool DrawEditorGUI()
        {
            // draws the editor GUI and returns whether it is modified or not

            var newAvatarArmatureName = EditorGUILayout.DelayedTextField("Avatar Armature Name", avatarArmatureName);
            var newWearableArmatureName = EditorGUILayout.DelayedTextField("Wearable Armature Name", wearableArmatureName);

            var modified = avatarArmatureName != newAvatarArmatureName || wearableArmatureName != newWearableArmatureName;

            avatarArmatureName = newAvatarArmatureName;
            wearableArmatureName = newWearableArmatureName;

            return modified;
        }
    }
}
