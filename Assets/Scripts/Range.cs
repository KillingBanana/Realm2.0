using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public struct IntRange {
	public int min;
	public int max;
	public int Average => (max - min) / 2;
	public int Random => GameController.Random.Next(min, max + 1);
	public bool Contains(int i) => min <= i && i <= max;
}

[CustomPropertyDrawer(typeof(IntRange))]
public class IntRangeDrawer : PropertyDrawer {
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		int indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		float gap = 4;
		float width = (position.width - gap) / 2;
		Rect rect = new Rect(position.x, position.y, width, position.height);
		EditorGUIUtility.labelWidth = 30;

		EditorGUI.PropertyField(rect, property.FindPropertyRelative("min"));
		rect.x += width + gap;
		EditorGUI.PropertyField(rect, property.FindPropertyRelative("max"));

		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}