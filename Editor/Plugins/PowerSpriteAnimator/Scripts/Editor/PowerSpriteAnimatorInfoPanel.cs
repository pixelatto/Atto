using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using PowerTools.Anim;

namespace PowerTools
{

    public partial class PowerSpriteAnimator
    {
        #region Definitions


        #endregion
        #region Vars: Private

        ReorderableList m_framesReorderableList = null;
        Vector2 m_scrollPosition = Vector2.zero;
        bool m_settingsUnfolded = false;

        #endregion
        #region Funcs: Init

        void InitialiseFramesReorderableList()
        {
            m_framesReorderableList = new ReorderableList(m_frames, typeof(PowerAnimFrame), true, true, true, true);
            m_framesReorderableList.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, "Frames");
                EditorGUI.LabelField(new Rect(rect) { x = rect.width - 37, width = 45 }, "Length");
            };
            m_framesReorderableList.drawElementCallback = LayoutFrameListFrame;
            m_framesReorderableList.onSelectCallback = (ReorderableList list) =>
            {
                SelectFrame(m_frames[m_framesReorderableList.index]);
            };
        }

        #endregion
        #region Funcs: Layout

        void LayoutInfoPanel(Rect rect)
        {

            GUILayout.BeginArea(rect, EditorStyles.inspectorFullWidthMargins);
            GUILayout.Space(20);

            // Animation length
            EditorGUILayout.LabelField(string.Format("Length: {0:0.00} sec  {1:D} frames", m_clip.TimeDuration, Mathf.RoundToInt(m_clip.AnimationDuration )), new GUIStyle(EditorStyles.miniLabel) { normal = { textColor = Color.gray } });


            // Speed/Framerate
            GUI.SetNextControlName("Framerate");
            int newFramerate = EditorGUILayout.IntSlider("FPS", m_clip.FPS, 1, 60);
            if (newFramerate != m_clip.FPS)
            {
                ChangeFrameRate(newFramerate);
            }

            //JAVI-TODO: Looping
            // Looping tickbox
            /*
            bool looping = EditorGUILayout.Toggle( "Looping", m_clip.isLooping );
            if ( looping != m_clip.isLooping )
            {
                ChangeLooping(looping);
            }
            */

            // Path to sprite in game object
            if (m_showAdvancedOptions)
            {
                SetSpritePath(EditorGUILayout.DelayedTextField("Sprite Path", m_spritePath));
            }

            GUILayout.Space(10);

            // Frames list
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition, false, false);
            EditorGUI.BeginChangeCheck();
            if (m_framesReorderableList != null && m_framesReorderableList.list != null && m_framesReorderableList.list.Count > 0)
            {
                m_framesReorderableList.DoLayoutList();
            }
            if (EditorGUI.EndChangeCheck())
            {
                RecalcFrameTimes();
                Repaint();
                ApplyChanges();
            }

            m_settingsUnfolded = EditorGUILayout.Foldout(m_settingsUnfolded, "Editor Settings", new GUIStyle(EditorStyles.foldout) { normal = { textColor = Color.gray } });
            if (m_settingsUnfolded)
            {
                string[] nodeNames = new string[m_defaultNodeNames.Length];
                for (int i = 0; i < nodeNames.Length; ++i)
                    nodeNames[i] = string.IsNullOrEmpty(m_defaultNodeNames[i]) ? ("Node " + i) : m_defaultNodeNames[i];
                m_visibleNodes = EditorGUILayout.MaskField("Visible Nodes", m_visibleNodes, nodeNames);
                GUI.SetNextControlName("AvdOpt");
                m_showAdvancedOptions = EditorGUILayout.Toggle("Show Advanced Options", m_showAdvancedOptions);
                GUI.SetNextControlName("DefaultLen");
                m_defaultFrameLength = EditorGUILayout.DelayedIntField("Default Frame Length", m_defaultFrameLength);
                GUI.SetNextControlName("DefaultSamples");
                m_defaultFrameSamples = EditorGUILayout.DelayedIntField("Default Frame Samples", m_defaultFrameSamples);
                GUI.SetNextControlName("IgnorePivot");
                m_ignorePivot = EditorGUILayout.Toggle("Ignore Pivot", m_ignorePivot);

                GUI.SetNextControlName("InfoPanelWidth");
                m_infoPanelWidth = Mathf.Max(100, EditorGUILayout.FloatField("Info Panel Width", m_infoPanelWidth));
                GUILayout.Space(20);
            }

            EditorGUILayout.EndScrollView();

            GUILayout.EndArea();

        }

        void LayoutFrameListFrame(Rect rect, int index, bool isActive, bool isFocused)
        {
            if (m_frames == null || index < 0 || index >= m_frames.Count)
                return;
            PowerAnimFrame frame = m_frames[index];

            EditorGUI.BeginChangeCheck();
            rect = new Rect(rect) { height = rect.height - 4, y = rect.y + 2 };


            // frame ID
            float xOffset = rect.x;
            float width = Styles.INFOPANEL_LABEL_RIGHTALIGN.CalcSize(new GUIContent(index.ToString())).x;
            EditorGUI.LabelField(new Rect(rect) { x = xOffset, width = width }, index.ToString(), Styles.INFOPANEL_LABEL_RIGHTALIGN);

            // Frame Sprite
            xOffset += width + 5;
            width = (rect.xMax - 5 - 28) - xOffset;

            // Sprite thingy
            Rect spriteFieldRect = new Rect(rect) { x = xOffset, width = width, height = 16 };
            frame.m_sprite = EditorGUI.ObjectField(spriteFieldRect, frame.m_sprite, typeof(Sprite), false) as Sprite;

            // Frame length (in samples)
            xOffset += width + 5;
            width = 28;
            GUI.SetNextControlName("FrameLen");
            int frameLen = frame.m_length;
            frameLen = EditorGUI.IntField(new Rect(rect) { x = xOffset, width = width }, frameLen);
            SetFrameLength(frame, frameLen);


            if (EditorGUI.EndChangeCheck())
            {
                // Apply events
                ApplyChanges();
            }
        }

        #endregion
        #region Funcs: Private

        void ChangeFrameRate(int newFramerate)
        {
            Undo.RecordObject(m_clip, "Change Animation Framerate");

            m_clip.FPS = newFramerate;
            RecalcFrameTimes();
            ApplyChanges();
        }

        //JAVI-TODO: Restore looping animations
        /*
        void ChangeLooping(bool looping)
        {
            Undo.RecordObject(m_clip, "Change Animation Looping");
            AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(m_clip);
            settings.loopTime = looping;
            AnimationUtility.SetAnimationClipSettings( m_clip, settings );

            m_previewloop = looping;

            // NB: When hitting play directly after this change, the looping state will be undone. So have to call ApplyChanges() afterwards even though frame data hasn't changed.
            ApplyChanges();
        }
        */

        void SetSpritePath(string path)
        {
            if (m_spritePath != path)
            {
                // Remove old curve binding. 
                //RemoveExistingBinding();

                m_spritePath = path;

                // Create new curve binding
                //CreateCurveBinding();
                ApplyChanges();
            }
        }

        #endregion
    }

}