﻿/*
 * File: WearableConfigView.cs
 * Project: DressingTools
 * Created Date: Wednesday, September 13th 2023, 12:22:58 am
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


using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Chocopoi.DressingFramework.Localization;
using Chocopoi.DressingTools.Localization;
using Chocopoi.DressingTools.OneConf.Wearable;
using Chocopoi.DressingTools.OneConf.Wearable.Modules.BuiltIn;
using Chocopoi.DressingTools.UI.Presenters;
using Chocopoi.DressingTools.UI.Views.Modules;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocopoi.DressingTools.UI.Views
{
    [ExcludeFromCodeCoverage]
    internal class OneConfWearableConfigView : ElementView, IOneConfWearableConfigView
    {
        private static readonly I18nTranslator t = I18n.ToolTranslator;
        private static Texture2D ThumbnailPlaceholderImage;

        public event Action TargetAvatarOrWearableChange;
        public event Action InfoNewThumbnailButtonClick;
        public event Action CaptureThumbnailButtonClick;
        public event Action CaptureCancelButtonClick;
        public event Action CaptureSettingsChange;
        public event Action ToolbarAutoSetupButtonClick;
        public event Action ToolbarPreviewButtonClick;
        public event Action ToolbarImportButtonClick;
        public event Action ToolbarExportButtonClick;
        public event Action AdvancedModuleAddButtonClick;
        public event Action ModeChange;
        public event Action AvatarConfigChange;

        public GameObject TargetAvatar { get => _dressSubView.TargetAvatar; }
        public GameObject TargetWearable { get => _dressSubView.TargetWearable; set => _dressSubView.TargetWearable = value; }

        public WearableConfig Config { get => _dressSubView.Config; set => _dressSubView.Config = value; }

        public int SelectedMode { get => _selectedMode; set => _selectedMode = value; }

        public Texture2D InfoThumbnail { get; set; }
        public bool InfoUseCustomWearableName { get; set; }
        public string InfoCustomWearableName { get; set; }
        public string InfoUuid { get; set; }
        public string InfoCreatedTime { get; set; }
        public string InfoUpdatedTime { get; set; }
        public string InfoAuthor { get; set; }
        public string InfoDescription { get; set; }
        public bool CaptureWearableOnly { get; set; }
        public bool CaptureRemoveBackground { get; set; }

        public bool SimpleUseArmatureMapping { get; set; }
        public bool SimpleUseMoveRoot { get; set; }
        public bool SimpleUseCabinetAnim { get; set; }
        public bool SimpleUseBlendshapeSync { get; set; }
        public ArmatureMappingWearableModuleConfig SimpleArmatureMappingConfig { get; set; }
        public MoveRootWearableModuleConfig SimpleMoveRootConfig { get; set; }
        public CabinetAnimWearableModuleConfig SimpleCabinetAnimConfig { get; set; }
        public BlendshapeSyncWearableModuleConfig SimpleBlendshapeSyncConfig { get; set; }

        public List<string> AdvancedModuleNames { get; set; }
        public string AdvancedSelectedModuleName { get; set; }
        public GameObject AdvancedAvatarConfigGuidReference { get; set; }
        public string AdvancedAvatarConfigGuid { get; set; }
        public bool AdvancedAvatarConfigUseAvatarObjName { get; set; }
        public string AdvancedAvatarConfigCustomName { get; set; }
        public string AdvancedAvatarConfigArmatureName { get; set; }
        public string AdvancedAvatarConfigDeltaWorldPos { get; set; }
        public string AdvancedAvatarConfigDeltaWorldRot { get; set; }
        public string AdvancedAvatarConfigAvatarLossyScale { get; set; }
        public string AdvancedAvatarConfigWearableLossyScale { get; set; }
        public List<WearableConfigModuleViewData> AdvancedModuleViewDataList { get; set; }

        public bool ShowAvatarNoCabinetHelpBox { get; set; }
        public bool ShowArmatureNotFoundHelpBox { get; set; }
        public bool ShowArmatureGuessedHelpBox { get; set; }
        public bool ShowCabinetConfigErrorHelpBox { get; set; }

        public bool PreviewActive { get => DTEditorUtils.PreviewActive; }

        private readonly IOneConfDressSubView _dressSubView;
        private readonly OneConfWearableConfigPresenter _presenter;
        private VisualElement _capturePanel;
        private Toggle _captureWearableOnlyToggle;
        private Toggle _captureRmvBgToggle;
        private Button _captureThumbBtn;
        private Button _captureCancelBtn;
        private VisualElement _infoPanel;
        private VisualElement _infoThumbnail;
        private Label _infoWearableNameLabel;
        private TextField _infoCustomNameField;
        private Toggle _infoUseCustomNameToggle;
        private Button _infoCaptureNewThumbBtn;
        private Button[] _toolbarModeBtns;
        private int _selectedMode;
        private VisualElement _simpleContainer;
        private VisualElement _simpleHelpBoxContainer;
        private VisualElement _advancedContainer;
        private Button _toolbarPreviewBtn;
        private Button[] _simpleCategoryBtns;
        private int _selectedCategory;
        private VisualElement _simpleCategoryMappingContainer;
        private VisualElement _simpleCategoryAnimateContainer;
        private ArmatureMappingWearableModuleEditor _simpleArmatureMappingEditor;
        private MoveRootWearableModuleEditor _simpleMoveRootEditor;
        private CabinetAnimWearableModuleEditor _simpleAnimGenEditor;
        private BlendshapeSyncWearableModuleEditor _simpleBlendshapeSyncEditor;
        private PopupField<string> _modulesPopup;
        private VisualElement _advancedModuleEditorsContainer;
        private ObjectField _advancedAvatarConfigGuidRefObjField;
        private Label _advancedAvatarConfigGuidLabel;
        private Toggle _advancedAvatarConfigUseAvatarObjNameToggle;
        private TextField _advancedAvatarConfigCustomNameField;
        private Label _advancedAvatarConfigArmatureNameLabel;
        private Label _advancedAvatarConfigDeltaWorldPosLabel;
        private Label _advancedAvatarConfigDeltaWorldRotLabel;
        private Label _advancedAvatarConfigAvatarLossyScaleLabel;
        private Label _advancedAvatarConfigWearableLossyScaleLabel;
        private Toggle _simpleArmatureMappingToggle;
        private Toggle _simpleMoveRootToggle;
        private Toggle _simpleAnimGenToggle;
        private Toggle _simpleBlendshapeSyncToggle;
        private bool _captureActive;
        private Label _infoOthersUuidLabel;
        private Label _infoOthersCreatedTimeLabel;
        private Label _infoOthersUpdatedTimeLabel;
        private TextField _infoOthersAuthorField;
        private TextField _infoOthersDescField;
        private VisualElement _simpleArmatureMappingContainer;
        private VisualElement _simpleMoveRootContainer;
        private VisualElement _simpleAnimGenContainer;
        private VisualElement _simpleBlendshapeSyncContainer;

        public OneConfWearableConfigView(IOneConfDressSubView dressingSubView)
        {
            _dressSubView = dressingSubView;
            _presenter = new OneConfWearableConfigPresenter(this);

            _selectedMode = 0;
            _selectedCategory = 0;
            _captureActive = false;

            InfoThumbnail = null;
            InfoUseCustomWearableName = false;
            InfoCustomWearableName = null;
            InfoUuid = null;
            InfoCreatedTime = null;
            InfoUpdatedTime = null;
            InfoDescription = null;

            CaptureWearableOnly = true;
            CaptureRemoveBackground = true;

            SimpleUseArmatureMapping = false;
            SimpleUseMoveRoot = false;
            SimpleUseCabinetAnim = false;
            SimpleUseBlendshapeSync = false;

            SimpleArmatureMappingConfig = new ArmatureMappingWearableModuleConfig();
            SimpleMoveRootConfig = new MoveRootWearableModuleConfig();
            SimpleCabinetAnimConfig = new CabinetAnimWearableModuleConfig();
            SimpleBlendshapeSyncConfig = new BlendshapeSyncWearableModuleConfig();

            AdvancedModuleNames = new List<string>() { "---" };
            AdvancedAvatarConfigGuidReference = null;
            AdvancedAvatarConfigGuid = null;
            AdvancedAvatarConfigCustomName = null;
            AdvancedAvatarConfigArmatureName = null;
            AdvancedAvatarConfigDeltaWorldPos = null;
            AdvancedAvatarConfigDeltaWorldRot = null;
            AdvancedAvatarConfigAvatarLossyScale = null;
            AdvancedAvatarConfigWearableLossyScale = null;
            AdvancedModuleViewDataList = new List<WearableConfigModuleViewData>();
            AdvancedSelectedModuleName = null;

            _dressSubView.TargetAvatarChange += () => TargetAvatarOrWearableChange?.Invoke();
            _dressSubView.TargetWearableChange += () => TargetAvatarOrWearableChange?.Invoke();

            InitVisualTree();
            InitWearableInfoFoldout();
            InitSimpleMode();
            InitAdvancedMode();
            InitToolbar();
            t.LocalizeElement(this);
        }

        private void InitWearableInfoInfoPanel()
        {
            _infoThumbnail = Q<VisualElement>("wearable-info-thumbnail");

            _infoPanel = Q<VisualElement>("wearable-info-info-panel");

            _infoWearableNameLabel = Q<Label>("wearable-info-name-label");

            _infoCustomNameField = Q<TextField>("wearable-info-custom-name-field");
            _infoCustomNameField.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
            {
                var val = _infoCustomNameField.value;
                if (string.IsNullOrEmpty(val)) return;

                if (InfoUseCustomWearableName)
                {
                    _infoWearableNameLabel.text = val;
                }
                InfoCustomWearableName = _infoCustomNameField.text;
            });

            _infoUseCustomNameToggle = Q<Toggle>("wearable-info-custom-name-toggle");
            _infoUseCustomNameToggle.RegisterValueChangedCallback((ChangeEvent<bool> evt) =>
            {
                if (!InfoUseCustomWearableName && TargetWearable != null)
                {
                    _infoCustomNameField.value = _infoWearableNameLabel.text = InfoCustomWearableName = TargetWearable.name;
                }
                _infoCustomNameField.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None;
                InfoUseCustomWearableName = evt.newValue;
            });

            _infoCaptureNewThumbBtn = Q<Button>("wearable-info-capture-new-thumbnail-btn");
            _infoCaptureNewThumbBtn.clicked += () => InfoNewThumbnailButtonClick?.Invoke();
        }

        private void InitWearableInfoCapturePanel()
        {
            _capturePanel = Q<VisualElement>("wearable-info-capture-panel");

            _captureWearableOnlyToggle = Q<Toggle>("wearable-info-capture-wearable-only-toggle");
            _captureRmvBgToggle = Q<Toggle>("wearable-info-capture-remove-background-toggle");

            _captureThumbBtn = Q<Button>("wearable-info-thumbnail-capture-btn");
            _captureThumbBtn.clicked += () => CaptureThumbnailButtonClick?.Invoke();
            _captureCancelBtn = Q<Button>("wearable-info-thumbnail-cancel-btn");
            _captureCancelBtn.clicked += () => CaptureCancelButtonClick?.Invoke();

            _captureWearableOnlyToggle.value = CaptureWearableOnly;
            _captureWearableOnlyToggle.RegisterValueChangedCallback((ChangeEvent<bool> evt) =>
            {
                CaptureWearableOnly = evt.newValue;
                CaptureSettingsChange?.Invoke();
            });
            _captureRmvBgToggle.value = CaptureRemoveBackground;
            _captureRmvBgToggle.RegisterValueChangedCallback((ChangeEvent<bool> evt) =>
            {
                CaptureRemoveBackground = evt.newValue;
                CaptureSettingsChange?.Invoke();
            });
        }

        private void InitWearableInfoOthersFoldout()
        {
            BindFoldoutHeaderWithContainer("wearable-info-others-foldout", "wearable-info-others-container");

            _infoOthersUuidLabel = Q<Label>("wearable-info-uuid-label");
            _infoOthersCreatedTimeLabel = Q<Label>("wearable-info-created-time-label");
            _infoOthersUpdatedTimeLabel = Q<Label>("wearable-info-updated-time-label");
            _infoOthersAuthorField = Q<TextField>("wearable-info-author-field");
            _infoOthersAuthorField.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
            {
                var val = _infoOthersAuthorField.value;
                if (string.IsNullOrEmpty(val)) return;
                InfoAuthor = val;
            });
            _infoOthersDescField = Q<TextField>("wearable-info-desc-field");
            _infoOthersDescField.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
            {
                var val = _infoOthersDescField.value;
                if (string.IsNullOrEmpty(val)) return;
                InfoDescription = val;
            });
        }

        private void InitWearableInfoFoldout()
        {
            BindFoldoutHeaderWithContainer("wearable-info-foldout", "wearable-info-container");
            InitWearableInfoInfoPanel();
            InitWearableInfoCapturePanel();
            InitWearableInfoOthersFoldout();
        }

        private void BindToolbarModes()
        {
            var simpleBtn = Q<Button>("toolbar-simple-btn");
            var advancedBtn = Q<Button>("toolbar-advanced-btn");

            _toolbarModeBtns = new Button[] { simpleBtn, advancedBtn };

            for (var i = 0; i < _toolbarModeBtns.Length; i++)
            {
                var modeIndex = i;
                _toolbarModeBtns[i].clicked += () =>
                {
                    if (_selectedMode == modeIndex) return;
                    _selectedMode = modeIndex;
                    UpdateToolbarModes();
                    ModeChange?.Invoke();
                };
                _toolbarModeBtns[i].EnableInClassList("active", modeIndex == _selectedMode);
            }
        }

        private void UpdateToolbarModes()
        {
            for (var i = 0; i < _toolbarModeBtns.Length; i++)
            {
                _toolbarModeBtns[i].EnableInClassList("active", i == _selectedMode);
            }

            if (_selectedMode == 0)
            {
                _simpleContainer.style.display = DisplayStyle.Flex;
                _advancedContainer.style.display = DisplayStyle.None;
            }
            else if (_selectedMode == 1)
            {
                _simpleContainer.style.display = DisplayStyle.None;
                _advancedContainer.style.display = DisplayStyle.Flex;
            }
        }

        private void InitToolbar()
        {
            var autoSetupBtn = Q<Button>("toolbar-auto-setup-btn");
            autoSetupBtn.clicked += () => ToolbarAutoSetupButtonClick?.Invoke();

            _toolbarPreviewBtn = Q<Button>("toolbar-preview-btn");
            _toolbarPreviewBtn.clicked += () => ToolbarPreviewButtonClick?.Invoke();

            var importBtn = Q<Button>("toolbar-import-btn");
            importBtn.clicked += () => ToolbarImportButtonClick?.Invoke();

            var exportBtn = Q<Button>("toolbar-export-btn");
            exportBtn.clicked += () => ToolbarExportButtonClick?.Invoke();

            BindToolbarModes();
        }

        private void BindSimpleCategories()
        {
            var mappingBtn = Q<Button>("simple-category-mapping-btn");
            var animateBtn = Q<Button>("simple-category-animate-btn");

            _simpleCategoryBtns = new Button[] { mappingBtn, animateBtn };

            for (var i = 0; i < _simpleCategoryBtns.Length; i++)
            {
                var categoryIndex = i;
                _simpleCategoryBtns[i].clicked += () =>
                {
                    if (_selectedCategory == categoryIndex) return;
                    _selectedCategory = categoryIndex;
                    UpdateSimpleCategories();
                };
                _simpleCategoryBtns[i].EnableInClassList("active", categoryIndex == _selectedCategory);
            }
        }

        private void UpdateSimpleCategories()
        {
            for (var i = 0; i < _simpleCategoryBtns.Length; i++)
            {
                _simpleCategoryBtns[i].EnableInClassList("active", i == _selectedCategory);
            }

            if (_selectedCategory == 0)
            {
                _simpleCategoryMappingContainer.style.display = DisplayStyle.Flex;
                _simpleCategoryAnimateContainer.style.display = DisplayStyle.None;
            }
            else if (_selectedCategory == 1)
            {
                _simpleCategoryMappingContainer.style.display = DisplayStyle.None;
                _simpleCategoryAnimateContainer.style.display = DisplayStyle.Flex;
            }
        }

        private void InitSimpleCategoryMapping()
        {
            _simpleCategoryMappingContainer = Q<VisualElement>("simple-category-mapping-container");

            BindFoldoutHeaderWithContainer("simple-armature-mapping-foldout", "simple-armature-mapping-container");
            _simpleArmatureMappingToggle = Q<Toggle>("simple-armature-mapping-toggle");
            _simpleArmatureMappingToggle.RegisterValueChangedCallback((ChangeEvent<bool> evt) => SimpleUseArmatureMapping = evt.newValue);
            _simpleArmatureMappingContainer = Q<VisualElement>("simple-armature-mapping-container");

            BindFoldoutHeaderWithContainer("simple-move-root-foldout", "simple-move-root-container");
            _simpleMoveRootToggle = Q<Toggle>("simple-move-root-toggle");
            _simpleMoveRootToggle.RegisterValueChangedCallback((ChangeEvent<bool> evt) => SimpleUseMoveRoot = evt.newValue);
            _simpleMoveRootContainer = Q<VisualElement>("simple-move-root-container");
        }

        private void InitSimpleCategoryAnimate()
        {
            _simpleCategoryAnimateContainer = Q<VisualElement>("simple-category-animate-container");

            BindFoldoutHeaderWithContainer("simple-cabinet-anim-foldout", "simple-cabinet-anim-container");
            _simpleAnimGenToggle = Q<Toggle>("simple-cabinet-anim-toggle");
            _simpleAnimGenToggle.RegisterValueChangedCallback((ChangeEvent<bool> evt) => SimpleUseCabinetAnim = evt.newValue);
            _simpleAnimGenContainer = Q<VisualElement>("simple-cabinet-anim-container");

            BindFoldoutHeaderWithContainer("simple-blendshape-sync-foldout", "simple-blendshape-sync-container");
            _simpleBlendshapeSyncToggle = Q<Toggle>("simple-blendshape-sync-toggle");
            _simpleBlendshapeSyncToggle.RegisterValueChangedCallback((ChangeEvent<bool> evt) => SimpleUseBlendshapeSync = evt.newValue);
            _simpleBlendshapeSyncContainer = Q<VisualElement>("simple-blendshape-sync-container");
        }

        private void InitSimpleMode()
        {
            _simpleContainer = Q<VisualElement>("simple-container");
            _simpleHelpBoxContainer = Q<VisualElement>("simple-helpbox-container");

            InitSimpleCategoryMapping();
            InitSimpleCategoryAnimate();
            BindSimpleCategories();
        }

        private void InitAdvancedModules()
        {
            BindFoldoutHeaderWithContainer("advanced-modules-foldout", "advanced-modules-container");

            var popupContainer = Q<VisualElement>("advanced-modules-popup-container");
            _modulesPopup = new PopupField<string>(AdvancedModuleNames, 0);
            _modulesPopup.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
            {
                AdvancedSelectedModuleName = evt.newValue;
            });
            popupContainer.Add(_modulesPopup);

            var moduleAddBtn = Q<Button>("advanced-module-add-btn");
            moduleAddBtn.clicked += () => AdvancedModuleAddButtonClick?.Invoke();

            _advancedModuleEditorsContainer = Q<VisualElement>("advanced-modules-editors-container");
        }

        private void InitAdvancedAvatarConfig()
        {
            BindFoldoutHeaderWithContainer("advanced-avatar-config-foldout", "advanced-avatar-config-container");

            _advancedAvatarConfigGuidRefObjField = Q<ObjectField>("advanced-avatar-config-guid-ref-objectfield");
            _advancedAvatarConfigGuidRefObjField.objectType = typeof(GameObject);
            _advancedAvatarConfigGuidRefObjField.allowSceneObjects = true;
            _advancedAvatarConfigGuidRefObjField.RegisterValueChangedCallback((ChangeEvent<UnityEngine.Object> evt) =>
            {
                AdvancedAvatarConfigGuidReference = (GameObject)evt.newValue;
                AvatarConfigChange?.Invoke();
            });

            _advancedAvatarConfigGuidLabel = Q<Label>("advanced-avatar-config-guid-label");
            _advancedAvatarConfigUseAvatarObjNameToggle = Q<Toggle>("advanced-avatar-config-use-obj-name-toggle");
            _advancedAvatarConfigUseAvatarObjNameToggle.RegisterValueChangedCallback((ChangeEvent<bool> evt) =>
            {
                AdvancedAvatarConfigUseAvatarObjName = evt.newValue;
                AvatarConfigChange?.Invoke();
            });
            _advancedAvatarConfigCustomNameField = Q<TextField>("advanced-avatar-config-custom-name-field");
            _advancedAvatarConfigCustomNameField.RegisterValueChangedCallback((ChangeEvent<string> evt) =>
            {
                var val = _advancedAvatarConfigCustomNameField.value;
                if (string.IsNullOrEmpty(val)) return;

                AdvancedAvatarConfigCustomName = val;
                AvatarConfigChange?.Invoke();
            });
            _advancedAvatarConfigArmatureNameLabel = Q<Label>("advanced-avatar-config-armature-name-label");
            _advancedAvatarConfigDeltaWorldPosLabel = Q<Label>("advanced-avatar-config-delta-world-pos-label");
            _advancedAvatarConfigDeltaWorldRotLabel = Q<Label>("advanced-avatar-config-delta-world-rot-label");
            _advancedAvatarConfigAvatarLossyScaleLabel = Q<Label>("advanced-avatar-config-avatar-lossy-scale-label");
            _advancedAvatarConfigWearableLossyScaleLabel = Q<Label>("advanced-avatar-config-wearable-lossy-scale-label");
        }

        private void InitAdvancedMode()
        {
            _advancedContainer = Q<VisualElement>("advanced-container");

            InitAdvancedModules();
            InitAdvancedAvatarConfig();
        }

        private void InitVisualTree()
        {
            var tree = Resources.Load<VisualTreeAsset>("OneConfWearableConfigView");
            tree.CloneTree(this);
            var styleSheet = Resources.Load<StyleSheet>("OneConfWearableConfigViewStyles");
            if (!styleSheets.Contains(styleSheet))
            {
                styleSheets.Add(styleSheet);
            }

            // dummy way to repaint on interact
            RegisterCallback((MouseMoveEvent evt) =>
            {
                if (_captureActive) RepaintCapturePreview();
                _toolbarPreviewBtn.EnableInClassList("active", PreviewActive);
            });
        }

        private void RepaintWearableInfo()
        {
            if (ThumbnailPlaceholderImage == null)
            {
                ThumbnailPlaceholderImage = Resources.Load<Texture2D>("thumbnailPlaceholder");
            }

            _infoThumbnail.style.backgroundImage = new StyleBackground(InfoThumbnail != null ? InfoThumbnail : ThumbnailPlaceholderImage);

            _infoUseCustomNameToggle.value = InfoUseCustomWearableName;
            if (InfoUseCustomWearableName)
            {
                _infoWearableNameLabel.text = InfoCustomWearableName;
                _infoCustomNameField.value = InfoCustomWearableName;
            }
            else
            {
                var text = TargetWearable != null ? TargetWearable.name : "---";
                _infoWearableNameLabel.text = text;
                _infoCustomNameField.value = text;
            }

            _infoOthersUuidLabel.text = InfoUuid;
            _infoOthersCreatedTimeLabel.text = InfoCreatedTime;
            _infoOthersUpdatedTimeLabel.text = InfoUpdatedTime;
            _infoOthersAuthorField.value = InfoAuthor;
            _infoOthersDescField.value = InfoDescription;
        }

        private void RepaintSimpleHelpboxes()
        {
            // add helpboxes
            _simpleHelpBoxContainer.Clear();
            if (ShowAvatarNoCabinetHelpBox)
            {
                _simpleHelpBoxContainer.Add(CreateHelpBox(t._("wearableConfig.editor.simple.autoSetup.helpbox.avatarHasNoCabinetUsingDefault"), MessageType.Warning));
            }
            if (ShowArmatureNotFoundHelpBox)
            {
                _simpleHelpBoxContainer.Add(CreateHelpBox(t._("wearableConfig.editor.simple.autoSetup.helpbox.wearableArmatureNotFound"), MessageType.Warning));
            }
            if (ShowArmatureGuessedHelpBox)
            {
                _simpleHelpBoxContainer.Add(CreateHelpBox(t._("wearableConfig.editor.simple.autoSetup.helpbox.armatureGuessed"), MessageType.Warning));
            }
            if (ShowCabinetConfigErrorHelpBox)
            {
                _simpleHelpBoxContainer.Add(CreateHelpBox(t._("dressing.editor.wizard.autoSetup.helpbox.unableToLoadCabinetConfig"), MessageType.Warning));
            }
        }

        private void RepaintSimpleModuleEditors()
        {
            _simpleArmatureMappingEditor = new ArmatureMappingWearableModuleEditor(this, null, SimpleArmatureMappingConfig);
            _simpleMoveRootEditor = new MoveRootWearableModuleEditor(this, null, SimpleMoveRootConfig);
            _simpleAnimGenEditor = new CabinetAnimWearableModuleEditor(this, null, SimpleCabinetAnimConfig);
            _simpleBlendshapeSyncEditor = new BlendshapeSyncWearableModuleEditor(this, null, SimpleBlendshapeSyncConfig);

            _simpleArmatureMappingContainer.Clear();
            _simpleMoveRootContainer.Clear();
            _simpleAnimGenContainer.Clear();
            _simpleBlendshapeSyncContainer.Clear();

            _simpleArmatureMappingContainer.Add(_simpleArmatureMappingEditor);
            _simpleMoveRootContainer.Add(_simpleMoveRootEditor);
            _simpleAnimGenContainer.Add(_simpleAnimGenEditor);
            _simpleBlendshapeSyncContainer.Add(_simpleBlendshapeSyncEditor);
        }

        public void RepaintSimpleMode()
        {
            _simpleArmatureMappingToggle.value = SimpleUseArmatureMapping;
            _simpleMoveRootToggle.value = SimpleUseMoveRoot;
            _simpleAnimGenToggle.value = SimpleUseCabinetAnim;
            _simpleBlendshapeSyncToggle.value = SimpleUseBlendshapeSync;

            RepaintSimpleHelpboxes();
            RepaintSimpleModuleEditors();

            _simpleArmatureMappingEditor.RaiseForceUpdateViewEvent();
            _simpleMoveRootEditor.RaiseForceUpdateViewEvent();
            _simpleAnimGenEditor.RaiseForceUpdateViewEvent();
            _simpleBlendshapeSyncEditor.RaiseForceUpdateViewEvent();
        }

        private void CreateModuleViewDataFoldout(WearableConfigModuleViewData moduleViewData)
        {
            var nestedFoldoutHeader = new VisualElement();
            nestedFoldoutHeader.AddToClassList("nested-foldout-header");

            var container = new VisualElement();
            container.AddToClassList("foldout-container");
            container.Add((VisualElement)moduleViewData.editor);

            var foldout = new Foldout
            {
                text = moduleViewData.editor.FriendlyName
            };
            foldout.RegisterValueChangedCallback((ChangeEvent<bool> evt) => container.style.display = evt.newValue ? DisplayStyle.Flex : DisplayStyle.None);
            nestedFoldoutHeader.Add(foldout);

            foldout.value = false;
            container.style.display = DisplayStyle.None;

            var rmvBtn = new Button(moduleViewData.removeButtonOnClick)
            {
                text = "X"
            };
            nestedFoldoutHeader.Add(rmvBtn);

            _advancedModuleEditorsContainer.Add(nestedFoldoutHeader);
            _advancedModuleEditorsContainer.Add(container);
        }

        public void RepaintAdvancedModeModules()
        {
            _advancedModuleEditorsContainer.Clear();

            foreach (var moduleViewData in AdvancedModuleViewDataList)
            {
                CreateModuleViewDataFoldout(moduleViewData);
            }
        }

        public void RepaintAdvancedModeAvatarConfig()
        {
            _advancedAvatarConfigGuidRefObjField.value = AdvancedAvatarConfigGuidReference;
            _advancedAvatarConfigGuidLabel.text = AdvancedAvatarConfigGuid;
            _advancedAvatarConfigUseAvatarObjNameToggle.value = AdvancedAvatarConfigUseAvatarObjName;
            _advancedAvatarConfigCustomNameField.value = AdvancedAvatarConfigCustomName;
            _advancedAvatarConfigArmatureNameLabel.text = AdvancedAvatarConfigArmatureName;
            _advancedAvatarConfigAvatarLossyScaleLabel.text = AdvancedAvatarConfigAvatarLossyScale;
            _advancedAvatarConfigWearableLossyScaleLabel.text = AdvancedAvatarConfigWearableLossyScale;
        }

        private void RepaintAdvancedMode()
        {
            RepaintAdvancedModeModules();
            RepaintAdvancedModeAvatarConfig();
        }

        public override void Repaint()
        {
            RepaintWearableInfo();
            RepaintSimpleMode();
            RepaintAdvancedMode();
        }

        public void RepaintCapturePreview()
        {
            _infoThumbnail.style.backgroundImage = new StyleBackground(DTEditorUtils.GetThumbnailCameraPreview());
        }

        public void SwitchToInfoPanel()
        {
            _infoPanel.style.display = DisplayStyle.Flex;
            _capturePanel.style.display = DisplayStyle.None;
            _captureActive = false;
        }

        public void SwitchToCapturePanel()
        {
            _infoPanel.style.display = DisplayStyle.None;
            _capturePanel.style.display = DisplayStyle.Flex;
            _captureActive = true;
        }

        public void ShowModuleAddedBeforeDialog()
        {
            EditorUtility.DisplayDialog(t._("tool.name"), t._("wearableConfig.editor.dialog.msg.moduleAddedBeforeCannotMultiple"), t._("common.dialog.btn.ok"));
        }

        public bool ShowConfirmAutoSetupDialog()
        {
            return EditorUtility.DisplayDialog(t._("tool.name"), t._("wearableConfig.editor.dialog.msg.confirmAutoSetup"), t._("common.dialog.btn.yes"), t._("common.dialog.btn.no"));
        }

        public void ApplyToConfig() => _presenter.ApplyToConfig();

        public void UpdateAvatarPreview(bool forceRecreate = false) => _presenter.UpdateAvatarPreview(forceRecreate);

        public void AutoSetup() => _presenter.AutoSetup();

        public bool IsValid()
        {
            if (TargetAvatar == null || TargetWearable == null)
            {
                return false;
            }

            if (SelectedMode == 0)
            {
                var mappingValid = true;
                mappingValid &= !SimpleUseArmatureMapping || _simpleArmatureMappingEditor.IsValid();
                mappingValid &= !SimpleUseMoveRoot || _simpleMoveRootEditor.IsValid();
                _simpleCategoryBtns[0].EnableInClassList("invalid", !mappingValid);

                var animateValid = true;
                animateValid &= !SimpleUseCabinetAnim || _simpleAnimGenEditor.IsValid();
                animateValid &= !SimpleUseBlendshapeSync || _simpleBlendshapeSyncEditor.IsValid();
                _simpleCategoryBtns[1].EnableInClassList("invalid", !animateValid);

                return mappingValid && animateValid;
            }
            else if (SelectedMode == 1)
            {
                return true;
            }

            return false;
        }
    }
}
