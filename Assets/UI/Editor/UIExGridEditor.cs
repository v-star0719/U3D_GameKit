using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(UIExGrid))]
public class UIExGridEditor : Editor
{
	public override void OnInspectorGUI()
	{
		//base.OnInspectorGUI();
		UIExGrid exGrid = target as UIExGrid;

		exGrid.clipPanel = EditorGUILayout.ObjectField("clipPanel", exGrid.clipPanel, typeof(UIPanel), true) as UIPanel;
		exGrid.cellWith = EditorGUILayout.FloatField("cellWith", exGrid.cellWith);
		exGrid.cellHeight = EditorGUILayout.FloatField("cellHeight", exGrid.cellHeight);
		exGrid.direction = (UIExGrid.EmDirection)EditorGUILayout.EnumPopup("direction", exGrid.direction);
		if(exGrid.direction == UIExGrid.EmDirection.Vertical)
		{
			exGrid.itemPerRow = EditorGUILayout.IntField("itemPerRow", exGrid.itemPerRow);
			exGrid.isVariableHeight = EditorGUILayout.Toggle("isVariableHeight", exGrid.isVariableHeight);
			exGrid.pivotVert = (UIExGrid.EmPivotVert)EditorGUILayout.EnumPopup("pivotVert", exGrid.pivotVert);
		}
		else
		{
			exGrid.itemPerCol = EditorGUILayout.IntField("itemPerCol", exGrid.itemPerRow);
			exGrid.isVariableWidth = EditorGUILayout.Toggle("isVariableWidth", exGrid.isVariableWidth);
			exGrid.pivotHorz = (UIExGrid.EmPivotHorz)EditorGUILayout.EnumPopup("pivotHorz", exGrid.pivotHorz);
		}
	}
}
