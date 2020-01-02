using UnityEditor;
using UnityEngine;

public class AdjustSelectedObjectsPositionWindow : EditorWindow
{
    [MenuItem("Special Commands/Adjust Selected Object Position")]
    public static void ShowWindow()
    {
        GetWindow<AdjustSelectedObjectsPositionWindow>("Adjust Selected Object Position");
    }

    float value1 = 0f;
    float value2 = 0f;
    float value3 = 0f;

    private void OnGUI()
    {

        float[] values = new float[3] { value1, value2, value3 };

        EditorGUI.BeginChangeCheck();
        EditorGUI.MultiFloatField(new Rect(10, 25, position.width - 20, 20), 
                                    new GUIContent("Valuse to Add to Positions"), 
                                    new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") }, 
                                    values);

        if (GUILayout.Button("Set Positions"))
        {

            foreach (var obj in Selection.transforms)
            {
                obj.position = new Vector3(obj.position.x + value1, obj.position.y + value2, obj.position.z + value3);
            }

        }

        if (EditorGUI.EndChangeCheck())
        {
            value1 = values[0];
            value2 = values[1];
            value3 = values[2];
        }
        
    }
}
