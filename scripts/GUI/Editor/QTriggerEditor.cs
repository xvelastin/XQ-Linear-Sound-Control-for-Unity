using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QTrigger))]
public class QTriggerEditor : Editor
{
    SerializedProperty currentIndex;
    SerializedProperty nextIndex;
    SerializedProperty totalInnt;

    QTrigger script;

    private void OnEnable()
    {
        script = target as QTrigger;
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        script.UpdateCount();

        DrawDefaultInspector();

        if (GUILayout.Button("Next Q"))
        {
            script.NextQ();
        }

        serializedObject.ApplyModifiedProperties();
    }
}
