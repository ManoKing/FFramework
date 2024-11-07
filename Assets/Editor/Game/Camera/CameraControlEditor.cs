﻿using UnityEditor;
using UnityEngine;

namespace Flower
{
    [CustomEditor(typeof(CameraControl))]
    public class CameraRigEditor : UnityEditor.Editor
    {
        CameraControl m_CameraControl;
        Rect m_MapSize;
        SerializedProperty m_SerializedPropertyMapSize;

        /// <summary>
        /// Adds a label below the default inspector GUI
        /// </summary>
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Resize the map rect size using the handles in scene.", EditorStyles.boldLabel);
        }

        /// <summary>
        /// Draws and handles input for manipulating the map size
        /// </summary>
        void OnSceneGUI()
        {
            float y = m_CameraControl.floorY;
            Plane floor = new Plane(Vector3.up, y);

            float middleX = (m_MapSize.xMax + m_MapSize.xMin) * 0.5f;
            float middleY = (m_MapSize.yMax + m_MapSize.yMin) * 0.5f;

            Vector3 bottomPosition = new Vector3(middleX, y, m_MapSize.yMin),
                    topPosition = new Vector3(middleX, y, m_MapSize.yMax),
                    leftPosition = new Vector3(m_MapSize.xMin, y, middleY),
                    rightPosition = new Vector3(m_MapSize.xMax, y, middleY);

            // Draw handles to resize map rect
            float size = HandleUtility.GetHandleSize(m_CameraControl.transform.position) * 0.125f;
            Vector3 snap = Vector3.one * 0.5f;
            var fmh_42_69_638665955717903065 = Quaternion.LookRotation(Vector3.up); Vector3 bottom = Handles.FreeMoveHandle(bottomPosition, size, snap,
                                                    Handles.RectangleHandleCap);
            var fmh_44_63_638665955717926434 = Quaternion.LookRotation(Vector3.up); Vector3 top = Handles.FreeMoveHandle(topPosition, size, snap,
                                                 Handles.RectangleHandleCap);
            var fmh_46_65_638665955717927993 = Quaternion.LookRotation(Vector3.up); Vector3 left = Handles.FreeMoveHandle(leftPosition, size, snap,
                                                  Handles.RectangleHandleCap);
            var fmh_48_67_638665955717929030 = Quaternion.LookRotation(Vector3.up); Vector3 right = Handles.FreeMoveHandle(rightPosition, size, snap,
                                                   Handles.RectangleHandleCap);

            ReprojectOntoFloor(ref bottom, floor);
            ReprojectOntoFloor(ref top, floor);
            ReprojectOntoFloor(ref left, floor);
            ReprojectOntoFloor(ref right, floor);

            // Draw a box to represent the map rect
            Vector3 topLeft = new Vector3(m_MapSize.x, y, m_MapSize.y),
                    topRight = topLeft + new Vector3(m_MapSize.width, 0, 0),
                    bottomLeft = topLeft + new Vector3(0, 0, m_MapSize.height),
                    bottomRight = bottomLeft + new Vector3(m_MapSize.width, 0, 0);
            Handles.DrawLine(topLeft, topRight);
            Handles.DrawLine(topRight, bottomRight);
            Handles.DrawLine(bottomRight, bottomLeft);
            Handles.DrawLine(bottomLeft, topLeft);

            m_MapSize.xMin = left.x;
            m_MapSize.xMax = right.x;
            m_MapSize.yMin = bottom.z;
            m_MapSize.yMax = top.z;

            if (m_SerializedPropertyMapSize.rectValue != m_MapSize)
            {
                m_SerializedPropertyMapSize.rectValue = m_MapSize;
                serializedObject.ApplyModifiedProperties();
            }
        }

        /// <summary>
        /// Reproject moved positions back onto floor plane (they move in 3D space so it can feel sluggish
        /// if we take their new position and clamp it back down to plane y
        /// </summary>
        static void ReprojectOntoFloor(ref Vector3 worldPoint, Plane floor)
        {
            UnityEngine.Camera sceneCam = UnityEngine.Camera.current;
            if (sceneCam != null)
            {
                float dist;
                Ray camray = sceneCam.ScreenPointToRay(sceneCam.WorldToScreenPoint(worldPoint));
                if (floor.Raycast(camray, out dist))
                {
                    worldPoint = camray.GetPoint(dist);
                }
            }
        }

        /// <summary>
        /// Gets the serializedObject for editing
        /// </summary>
        void OnEnable()
        {
            m_CameraControl = target as CameraControl;
            m_SerializedPropertyMapSize = serializedObject.FindProperty("mapSize");
            m_MapSize = m_SerializedPropertyMapSize.rectValue;
        }
    }
}