using System;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomEditor(typeof(InspectorDictionaryBase), true)]
public class InspectorDictionaryEditor : Editor {

    private ReorderableList list;

    private void HandleDragAndDrop() {
        Event currentEvent = Event.current;
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(dropArea, "Drag and drop entries here");

        switch (currentEvent.type) {
            case EventType.DragUpdated:
                if (!dropArea.Contains(currentEvent.mousePosition)) {
                    return;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                break;

            case EventType.DragPerform:
                if (!dropArea.Contains(currentEvent.mousePosition)) {
                    return;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                DragAndDrop.AcceptDrag();

                SerializedProperty entries = this.serializedObject.FindProperty("entries");

                int newEntriesCount = DragAndDrop.objectReferences.Length;

                entries.arraySize += newEntriesCount;

                for (int i = 0; i < newEntriesCount; i++) {
                    SerializedProperty element = entries.GetArrayElementAtIndex(
                        entries.arraySize - newEntriesCount + i
                    );

                    element.objectReferenceValue = DragAndDrop.objectReferences[i];
                }

                this.serializedObject.ApplyModifiedProperties();
                break;

            default:
                throw new NotSupportedException($"Received unexpected value: {currentEvent.type}");
        }
    }

    private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
        SerializedProperty element = this.list.serializedProperty.GetArrayElementAtIndex(index);

        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, EditorGUIUtility.currentViewWidth - 50, EditorGUIUtility.singleLineHeight),
            element,
            GUIContent.none
        );
    }

    private void OnEnable() {
        this.list = new ReorderableList(
            this.serializedObject,
            this.serializedObject.FindProperty("entries"),
            true,
            true,
            true,
            true
        );

        this.list.drawElementCallback = this.OnDrawElement;
    }

    public override void OnInspectorGUI() {
        this.serializedObject.Update();

        EditorGUILayout.PropertyField(
            this.serializedObject.FindProperty("m_Script"),
            true,
            new GUILayoutOption[0]
        );

        EditorGUILayout.PropertyField(
            this.serializedObject.FindProperty("dontDestroyOnLoad"),
            true,
            new GUILayoutOption[0]
        );

        EditorGUILayout.PropertyField(
            this.serializedObject.FindProperty("singleton"),
            true,
            new GUILayoutOption[0]
        );

        GUILayout.Space(EditorGUIUtility.singleLineHeight);

        this.list.DoLayoutList();
        this.serializedObject.ApplyModifiedProperties();

        GUILayout.Space(EditorGUIUtility.singleLineHeight);

        this.HandleDragAndDrop();
    }
}
