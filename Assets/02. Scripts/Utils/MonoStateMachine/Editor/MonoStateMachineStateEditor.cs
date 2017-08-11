using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MonoStateMachineState), true)]
[CanEditMultipleObjects]
public class MonoStateMachineStateEditor : Editor {
	public override void OnInspectorGUI ()
	{
		MonoStateMachineState l_MSMS = (MonoStateMachineState)target;
		if (l_MSMS.m_CurrentSubState != null) {
			EditorGUILayout.LabelField ("Current state:", l_MSMS.m_CurrentSubState.stateName);
		}
		DrawDefaultInspector ();
	}
}
