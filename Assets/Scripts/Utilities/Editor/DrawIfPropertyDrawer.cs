using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Based on: https://forum.unity.com/threads/draw-a-field-only-if-a-condition-is-met.448855/
/// </summary>
[CustomPropertyDrawer(typeof(DrawIfAttribute))]
public class DrawIfPropertyDrawer : PropertyDrawer
{
	#region Fields

	// Reference to the attribute on the property.
	DrawIfAttribute drawIf;

	// Field that is being compared.
	SerializedProperty comparedField;

	#endregion

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		if (!ShowMe(property) && drawIf.disablingType == DrawIfAttribute.DisablingType.DontDraw)
		{
			return 0f;
		}

		float modifier = 0;
		if (property.isExpanded)
		{
			modifier += 15 + CalcModifier(property);
		}

		// The height of the property should be defaulted to the default height.
		return base.GetPropertyHeight(property, label) + modifier;
	}

	private float CalcModifier(SerializedProperty property)
	{
		float modifier = 0;
		property = property.Copy();
		var nextElement = property.Copy();
		bool hasNextElement = nextElement.NextVisible(false);
		if (!hasNextElement)
		{
			nextElement = null;
		}

		property.NextVisible(true);
		while (true)
		{
			if (SerializedProperty.EqualContents(property, nextElement))
			{
				break;
			}

			modifier += EditorGUI.GetPropertyHeight(property);
			bool hasNext = property.NextVisible(false);
			if (!hasNext)
			{
				break;
			}
		}

		return modifier;
	}

	/// <summary>
	/// Errors default to showing the property.
	/// </summary>
	private bool ShowMe(SerializedProperty property)
	{
		drawIf = attribute as DrawIfAttribute;
		// Replace propertyname to the value from the parameter
		string path = property.propertyPath.Contains(".")
			? System.IO.Path.ChangeExtension(property.propertyPath, drawIf.comparedPropertyName)
			: drawIf.comparedPropertyName;

		comparedField = property.serializedObject.FindProperty(path);

		if (comparedField == null)
		{
			Debug.LogError("Cannot find property with name: " + path);
			return true;
		}

		if (comparedField.isArray)
		{
			var array = comparedField.Copy();

			array.Next(true); // skip generic field
			array.Next(true); // advance to array size field

			// Get the array size
			var arrayLength = array.intValue;

			array.Next(true); // advance to first array index

			// Write values to list
			var values = new List<int>(arrayLength);
			int lastIndex = arrayLength - 1;
			for (int i = 0; i < arrayLength; i++)
			{
				values.Add(array.intValue); // copy the value to the list
				if (i < lastIndex)
				{
					array.Next(false); // advance without drilling into children
				}
			}

			return values.Contains((int) drawIf.comparedValue);
		}

		// get the value & compare based on types
		switch (comparedField.type)
		{
			// Possible extend cases to support your own type
			case "bool":
				return comparedField.boolValue.Equals(drawIf.comparedValue);
			case "Enum":
				return comparedField.enumValueIndex.Equals((int) drawIf.comparedValue);
			default:
				Debug.LogError("Error: " + comparedField.type + " is not supported of " + path);
				return true;
		}
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		if (ShowMe(property))
		{
			label.text = ShowLabelIfComparedValueIsEnum(label.text);
			EditorGUI.PropertyField(position, property, new GUIContent(label.text), true);
		}
		else if (drawIf.disablingType == DrawIfAttribute.DisablingType.ReadOnly)
		{
			GUI.enabled = false;
			label.text = ShowLabelIfComparedValueIsEnum(label.text);
			EditorGUI.PropertyField(position, property, label, true);
			GUI.enabled = true;
		}
	}

	private string ShowLabelIfComparedValueIsEnum(string label)
	{
		drawIf = attribute as DrawIfAttribute;
		return $"*{drawIf.comparedValue}*: {label}";
	}
}