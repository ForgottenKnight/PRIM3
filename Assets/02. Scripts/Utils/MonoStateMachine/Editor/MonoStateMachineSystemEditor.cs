using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(MonoStateMachineSystem))]
[CanEditMultipleObjects]
public class MonoStateMachineSystemEditor : Editor {
	public override void OnInspectorGUI() {
		MonoStateMachineSystem l_MSMS = (MonoStateMachineSystem)target;
		if (l_MSMS.m_CurrentState != null) {
			EditorGUILayout.LabelField ("Current state:", l_MSMS.m_CurrentState.stateName);
		}
		DrawDefaultInspector ();
	}
}
