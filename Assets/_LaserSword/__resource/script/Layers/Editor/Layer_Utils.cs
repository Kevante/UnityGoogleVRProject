using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public static class Layer_Utils 
{

	static Layer_Utils()
	{
		CreateLayer ("LaserSword");
	}


	static void CreateLayer(string layer_name)
	{
		SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		SerializedProperty layers = tagManager.FindProperty("layers");
		bool ExistLayer = false;

		for (int i = 8; i < layers.arraySize; i++)
		{
			SerializedProperty layerSP = layers.GetArrayElementAtIndex(i);

			if (layerSP.stringValue == layer_name)
			{
				ExistLayer = true;
				break;
			}

		}
		for (int j = 8; j < layers.arraySize; j++)
		{
			SerializedProperty layerSP = layers.GetArrayElementAtIndex(j);
			if (layerSP.stringValue == "" && !ExistLayer)
			{
				layerSP.stringValue = layer_name;
				tagManager.ApplyModifiedProperties();

				break;
			}
		}
			
	}


}
