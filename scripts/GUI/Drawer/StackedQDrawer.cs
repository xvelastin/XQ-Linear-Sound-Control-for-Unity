using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;

[CustomPropertyDrawer(typeof(StackedQ))]
public class StackedQDrawer : PropertyDrawer
{
    private float propertyHeight;
    private Dictionary<string, ReorderableList> qstackDict = new Dictionary<string, ReorderableList>();
    private static float singleLineHeight = EditorGUIUtility.singleLineHeight;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // Set propertyHeight for QListEditor to read to allocate space in its reorderable list.
        property.FindPropertyRelative("propertyHeight").floatValue = propertyHeight;
        return propertyHeight;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.serializedObject.Update();
        EditorGUI.BeginProperty(position, label, property);

        // ** StackedQ Properties **
        SerializedProperty stackedQName = property.FindPropertyRelative("name");
        int stackedQIndex = property.FindPropertyRelative("qIndex").intValue;
        SerializedProperty qs = property.FindPropertyRelative("qs");

        // ** GUI Styles **
        // Buttons
        int buttonLines = 2; // height of buttons
        GUIStyle PlayButtonStyle = new GUIStyle(EditorStyles.miniButton);
        PlayButtonStyle.fixedHeight = buttonLines * singleLineHeight;
        XQ.GUIUtility.ColorAllTextInStyle(PlayButtonStyle, XQ.GUIUtility.QTypeToColor(CueType.Play));

        GUIStyle FadeButtonStyle = new GUIStyle(EditorStyles.miniButton);
        FadeButtonStyle.fixedHeight = buttonLines * singleLineHeight;
        XQ.GUIUtility.ColorAllTextInStyle(FadeButtonStyle, XQ.GUIUtility.QTypeToColor(CueType.Fade));

        GUIStyle StopButtonStyle = new GUIStyle(EditorStyles.miniButton);
        StopButtonStyle.fixedHeight = buttonLines * singleLineHeight;
        XQ.GUIUtility.ColorAllTextInStyle(StopButtonStyle, XQ.GUIUtility.QTypeToColor(CueType.Stop));

        // ** GUI Render **       
        position = XQ.GUIUtility.MoveRect.Shift(position, 10, 0, singleLineHeight);

        // create foldout menu
        string stackedQHeaderPrefix = "XQ " + stackedQIndex.ToString() + " - ";
        property.isExpanded = EditorGUI.Foldout(
            position,
            property.isExpanded,
            stackedQHeaderPrefix + stackedQName.stringValue
            );

        if (property.isExpanded)
        {
            // Display buttons
            Rect buttonRect = position;
            int numButtons = 3;

            position = new Rect(position.x, position.y, buttonRect.width / numButtons, singleLineHeight * buttonLines);
            bool AddPlayButton = GUI.Button(position, "add play q", PlayButtonStyle);
            EditorGUI.LabelField(position, new GUIContent("", "Add a Play Q to to this XQ"));

            position = new Rect(position.x + (buttonRect.width / numButtons), position.y, buttonRect.width / numButtons, singleLineHeight * buttonLines);
            bool AddFadeButton = GUI.Button(position, "add fade q", FadeButtonStyle);
            EditorGUI.LabelField(position, new GUIContent("", "Add a Fade Q to to this XQ"));

            position = new Rect(position.x + (buttonRect.width / numButtons), position.y, buttonRect.width / numButtons, singleLineHeight * buttonLines);
            bool AddStopButton = GUI.Button(position, "add stop q", StopButtonStyle);
            EditorGUI.LabelField(position, new GUIContent("", "Add a Stop Q to to this XQ"));

            // Uses reflection to trigger the NewQ method with appropriate argument on the specific Stacked Q.
            if (AddPlayButton)
            {
                List<StackedQ> thisQList = fieldInfo.GetValue(property.serializedObject.targetObject) as List<StackedQ>;
                StackedQ thisStackedQ = thisQList[stackedQIndex];
                thisStackedQ.NewQ(CueType.Play);
            }

            if (AddFadeButton)
            {
                List<StackedQ> thisQList = fieldInfo.GetValue(property.serializedObject.targetObject) as List<StackedQ>;
                StackedQ thisStackedQ = thisQList[stackedQIndex];
                thisStackedQ.NewQ(CueType.Fade);
            }

            if (AddStopButton)
            {
                List<StackedQ> thisQList = fieldInfo.GetValue(property.serializedObject.targetObject) as List<StackedQ>;
                StackedQ thisStackedQ = thisQList[stackedQIndex];
                thisStackedQ.NewQ(CueType.Stop);
            }

            position = new Rect(buttonRect.x, position.y + (singleLineHeight * buttonLines), buttonRect.width, (singleLineHeight * buttonLines));
        }

        // Makes a reorderable list of Q objects. Saves to a Dictionary.
        ReorderableList rlist;
        string listKey = property.propertyPath;

        if (qstackDict.ContainsKey(listKey))
        {
            // Get the rlist, if one exists
            rlist = qstackDict[listKey];
        }
        else
        {
            // Make a new rlist if it doesn't
            rlist = new ReorderableList(property.serializedObject, qs)
            {
                draggable = true,
                displayAdd = false,
                displayRemove = false,
                headerHeight = singleLineHeight,

                drawHeaderCallback = position =>
                    {
                        // Changes offset of text field depending on header prefix length
                        float textWidth = GUI.skin.label.CalcSize(new GUIContent(stackedQHeaderPrefix)).x;

                        EditorGUI.LabelField(
                            new Rect(position.x, position.y, textWidth, position.height),
                            new GUIContent(stackedQHeaderPrefix)
                        );

                        position = XQ.GUIUtility.MoveRect.Shift(position, textWidth, 0, singleLineHeight);
                        stackedQName.stringValue = EditorGUI.TextField(
                            position,
                            stackedQName.stringValue
                        );
                        EditorGUI.LabelField(position, new GUIContent("", "Edit the display name of this Q."));

                    },
                drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                    {
                        EditorGUI.PropertyField(rect, qs.GetArrayElementAtIndex(index), GUIContent.none);
                    },
                elementHeightCallback = index =>
                    {
                        // Change height depending on each q in the qstack's height (depends on CueType).
                        SerializedProperty selectedQ = qs.GetArrayElementAtIndex(index);
                        SerializedProperty cueType = selectedQ.FindPropertyRelative("cueType");
                        CueType cueTypeAsEnum = (CueType)cueType.enumValueIndex;

                        // Checks if the Q being drawn has been expanded
                        if (selectedQ.isExpanded)
                        {
                            // Sets height depending on cue type
                            int qtypelines = XQ.GUIUtility.QTypeToLines(cueTypeAsEnum);
                            return (qtypelines + QDrawer.basepropertyHeightInLines) * singleLineHeight;
                        }
                        else
                        {
                            // Shrinks the property box to accomodate collapsed
                            return singleLineHeight;
                        }
                    }
            };

            // & updates dict
            qstackDict[listKey] = rlist;
        }
        // Checks if the Stacked Q is expanded
        if (property.isExpanded)
        {
            // Sets height depending on the reorderable list - the sum of all q heights.
            rlist.DoList(position);
            propertyHeight = rlist.GetHeight();
        }
        else
        {
            propertyHeight = 2 * singleLineHeight;
        }
        property.FindPropertyRelative("propertyHeight").floatValue = propertyHeight;
        property.serializedObject.ApplyModifiedProperties();
        EditorGUI.EndProperty();
    }
}