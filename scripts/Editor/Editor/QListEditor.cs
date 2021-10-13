using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(QList))]
[CanEditMultipleObjects]
public class QListEditor : Editor
{
    private SerializedProperty qlist;
    private SerializedProperty qlistName;
    private ReorderableList rlist;
    private static float singleLineHeight = EditorGUIUtility.singleLineHeight;

    private void OnEnable()
    {
        qlist = serializedObject.FindProperty("qlist");
        qlistName = serializedObject.FindProperty("Name");

        if (qlistName.stringValue == "")
        {
            string gameobjectName = serializedObject.targetObject.name;
            qlistName.stringValue = gameobjectName;
        }

        rlist = new ReorderableList(serializedObject, qlist)
        {
            draggable = true,
            displayAdd = true,
            displayRemove = true,

            drawHeaderCallback = (Rect rect) =>
            {
                serializedObject.Update();

                // replaces header with an editable name field.
                qlistName.stringValue = EditorGUI.TextField(rect, qlistName.stringValue);

                // Sets the gameobject this list is attached to as the default name.
                if (qlistName.stringValue == "")
                {
                    string gameobjectName = serializedObject.targetObject.name;
                    qlistName.stringValue = gameobjectName;
                }
                // display a tooltip when hovering over
                EditorGUI.LabelField(rect, new GUIContent("", "Edit the display name of this Q List."));

                serializedObject.ApplyModifiedProperties();
            },
            drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                serializedObject.Update();

                // Update each Stacked Q's qIndex to enable reflection-based button trigger.
                SerializedProperty stackedQ = qlist.GetArrayElementAtIndex(index);
                stackedQ.FindPropertyRelative("qIndex").intValue = index;

                serializedObject.ApplyModifiedProperties();
                EditorGUI.PropertyField(rect, stackedQ);
            },
            elementHeightCallback = (int index) =>
            {
                serializedObject.Update();

                // Get the height of the stackedQ being changed
                SerializedProperty stackedQ = qlist.GetArrayElementAtIndex(index);
                float stackedQheight = stackedQ.FindPropertyRelative("propertyHeight").floatValue;

                if (stackedQ.isExpanded)
                {
                    // Returns the height of the stackedQ with a small gap.
                    serializedObject.ApplyModifiedProperties();
                    return stackedQheight + singleLineHeight;
                }
                else
                {
                    serializedObject.ApplyModifiedProperties();
                    return singleLineHeight * 1.5f;
                }
            }
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        rlist.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}