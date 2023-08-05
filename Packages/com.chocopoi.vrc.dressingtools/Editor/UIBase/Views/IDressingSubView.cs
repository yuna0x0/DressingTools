﻿using System;

namespace Chocopoi.DressingTools.UIBase.Views
{
    internal interface IDressingSubView : IEditorView, IWearableConfigViewParent
    {
        event Action DoAddToCabinetEvent;
        bool ShowAvatarNoExistingCabinetHelpbox { get; set; }
        bool DisableAllButtons { get; set; }
        bool DisableAddToCabinetButton { get; set; }

        void SelectTab(int selectedTab);
        void ResetWizardAndConfigView();
        void ForceUpdateCabinetSubView();
        bool IsConfigValid();
        void RaiseDoAddToCabinetEvent();
    }
}
