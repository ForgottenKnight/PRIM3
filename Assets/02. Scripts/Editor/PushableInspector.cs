using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(Pushable))]
public class PushableInspector : Editor {

	void onEnable()
	{

	}

	public override void OnInspectorGUI()
	{
        EditorGUILayout.HelpBox("Este script permite al personaje ser empujado. Las acciones que bloquea son las bloqueadas mientras esta siendo empujado.", MessageType.Info);

		Pushable controller = (Pushable)target;
        EditorGUILayout.LabelField("Active", controller.isActionActive().ToString());

		DrawDefaultInspector();
	}
}
