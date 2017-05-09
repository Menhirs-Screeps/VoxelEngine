using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(Shape))]
public class IngredientDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override float GetPropertyHeight ( SerializedProperty property, GUIContent label ) {
		int index = property.FindPropertyRelative ("type").enumValueIndex;
		switch ((ForceShapeType)(Enum.GetValues(typeof(ForceShapeType))).GetValue(index)) {
		case ForceShapeType.Sphere:
			return 1 * 16;
		case ForceShapeType.TruncatedSphere:
			return 2 * 16;
		case ForceShapeType.Box:
			return 1 * 16;
		case ForceShapeType.Cone:
			return 5 * 16;
		case ForceShapeType.Cylinder:
			return 3 * 16;
		}
		return 16;
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		position.height = 16;
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;
		EditorGUI.PropertyField(position, property.FindPropertyRelative("type"), GUIContent.none);
		SerializedProperty shape;
		switch ((ForceShapeType)(Enum.GetValues(typeof(ForceShapeType))).GetValue(property.FindPropertyRelative ("type").enumValueIndex)) {
		case ForceShapeType.Sphere:
			break;
		case ForceShapeType.TruncatedSphere:
			shape = property.FindPropertyRelative ("truncsphere");
			position.y += 16; EditorGUI.PropertyField(position, shape.FindPropertyRelative("minimumDistance"));
			break;
		case ForceShapeType.Box:
			break;
		case ForceShapeType.Cone:
			shape = property.FindPropertyRelative ("cone");
			position.y += 16; EditorGUI.PropertyField(position, shape.FindPropertyRelative("minimumDistance"));
			position.y += 16; EditorGUI.PropertyField(position, shape.FindPropertyRelative("maximumDistance"));
			position.y += 16; EditorGUI.PropertyField(position, shape.FindPropertyRelative("angle"));
			position.y += 16; EditorGUI.PropertyField(position, shape.FindPropertyRelative("initialRadius"));
			break;
		case ForceShapeType.Cylinder:
			break;
		}
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}