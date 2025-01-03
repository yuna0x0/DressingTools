﻿/*
 * File: IMGUIViewBase.cs
 * Project: DressingFramework
 * Created Date: Saturday, August 12th 2023, 12:21:25 am
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
 * You should have received a copy of the GNU General Public License along with DressingFramework. If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Chocopoi.DressingTools.UI.Views
{
    /// <summary>
    /// Unity IMGUI view base
    /// </summary>
    [ExcludeFromCodeCoverage]
    public abstract class IMGUIView : ElementView
    {
        public IMGUIView()
        {
            Add(new IMGUIContainer(OnGUI));
        }

        /// <summary>
        /// This does nothing since we are using IMGUI
        /// </summary>
        public sealed override void Repaint() { }

        /// <summary>
        /// Render IMGUI content
        /// </summary>
        public abstract void OnGUI();

        /// <summary>
        /// Draw a horizontal line
        /// </summary>
        /// <param name="height">Height</param>
        public static void HorizontalLine(int height = 1)
        {
            //Reference: https://forum.unity.com/threads/horizontal-line-in-editor-window.520812/#post-3416790
            EditorGUILayout.Separator();
            var rect = EditorGUILayout.GetControlRect(false, height);
            rect.height = height;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
            EditorGUILayout.Separator();
        }

        public static void ReadOnlyTextField(string label, string text)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(label, GUILayout.Width(EditorGUIUtility.labelWidth - 4));
                EditorGUILayout.SelectableLabel(text, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void Label(string text, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, options);
        }

        public static void Label(string text, GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.Label(text, style, options);
        }

        public static void Button(string text, Action onClickEvent = null, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(text, options))
            {
                onClickEvent?.Invoke();
            }
        }

        public static void Button(string text, GUIStyle style, Action onClickEvent = null, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(text, style, options))
            {
                onClickEvent?.Invoke();
            }
        }

        public static void TextField(string label, ref string text, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.TextField(label, text, options);
            var modified = text != newValue;
            text = newValue;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void DelayedTextField(string label, ref string text, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.DelayedTextField(label, text, options);
            var modified = text != newValue;
            text = newValue;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static UnityEngine.Object ObjectField(string label, UnityEngine.Object obj, Type objType, bool allowSceneObjs, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newObj = EditorGUILayout.ObjectField(label, obj, objType, allowSceneObjs, options);
            if (newObj != obj)
            {
                onChangeEvent?.Invoke();
            }
            return newObj;
        }

        public static void GameObjectField(ref GameObject go, bool allowSceneObjs, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newObj = (GameObject)EditorGUILayout.ObjectField(go, typeof(GameObject), allowSceneObjs, options);
            var modified = newObj != go;
            go = newObj;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void GameObjectField(string label, ref GameObject go, bool allowSceneObjs, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newObj = (GameObject)EditorGUILayout.ObjectField(label, go, typeof(GameObject), allowSceneObjs, options);
            var modified = newObj != go;
            go = newObj;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void Toggle(string label, ref bool value, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.Toggle(label, value);
            var modified = newValue != value;
            value = newValue;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void Toggle(ref bool value, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.Toggle(value);
            var modified = newValue != value;
            value = newValue;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void ToggleLeft(string label, ref bool value, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.ToggleLeft(label, value);
            var modified = newValue != value;
            value = newValue;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void Slider(ref float value, float leftValue, float rightValue, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.Slider(value, leftValue, rightValue, options);
            var modified = newValue != value;
            value = newValue;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void Slider(string label, ref float value, float leftValue, float rightValue, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.Slider(label, value, leftValue, rightValue, options);
            var modified = newValue != value;
            value = newValue;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void Popup(ref int selectedIndex, string[] keys, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            int newSelectedIndex = EditorGUILayout.Popup(selectedIndex, keys, options);
            var modified = newSelectedIndex != selectedIndex;
            selectedIndex = newSelectedIndex;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void Popup(string label, ref int selectedIndex, string[] keys, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            int newSelectedIndex = EditorGUILayout.Popup(label, selectedIndex, keys, options);
            var modified = newSelectedIndex != selectedIndex;
            selectedIndex = newSelectedIndex;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void HelpBox(string message, MessageType type)
        {
            EditorGUILayout.HelpBox(message, type);
        }

        public static void Foldout(ref bool foldout, string label)
        {
            foldout = EditorGUILayout.Foldout(foldout, label);
        }

        public static void BeginFoldoutBox(ref bool foldout, string label)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, label);
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public static void BeginFoldoutBoxWithButtonRight(ref bool foldout, string foldoutLabel, string btnLabel, Action btnOnClickEvent = null)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, foldoutLabel);
            Button(btnLabel, btnOnClickEvent, GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        public static void EndFoldoutBox()
        {
            EditorGUILayout.EndVertical();
        }

        public static void BeginHorizontal()
        {
            EditorGUILayout.BeginHorizontal();
        }

        public static void BeginHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginHorizontal(style, options);
        }

        public static void EndHorizontal()
        {
            EditorGUILayout.EndHorizontal();
        }

        public static void BeginVertical()
        {
            EditorGUILayout.BeginVertical();
        }

        public static void BeginVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            EditorGUILayout.BeginVertical(style, options);
        }

        public static void EndVertical()
        {
            EditorGUILayout.EndVertical();
        }

        public static void BeginDisabled(bool disabled)
        {
            EditorGUI.BeginDisabledGroup(disabled);
        }

        public static void EndDisabled()
        {
            EditorGUI.EndDisabledGroup();
        }

        public static void Separator()
        {
            EditorGUILayout.Separator();
        }

        public static void Toolbar(ref int selected, string[] keys, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newSelected = GUILayout.Toolbar(selected, keys, options);
            var modified = newSelected != selected;
            selected = newSelected;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }

        public static void TextArea(ref string text, Action onChangeEvent = null, params GUILayoutOption[] options)
        {
            var newValue = EditorGUILayout.TextArea(text, options);
            var modified = newValue != text;
            text = newValue;
            if (modified)
            {
                onChangeEvent?.Invoke();
            }
        }
    }
}
