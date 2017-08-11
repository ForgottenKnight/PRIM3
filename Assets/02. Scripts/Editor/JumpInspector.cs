using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor (typeof(Jump))]
public class JumpInspector : Editor {

	void onEnable()
	{

	}

	public override void OnInspectorGUI()
	{
        EditorGUILayout.HelpBox("Este script gestiona el movimiento vertical de los personajes. Permite saltar (aceleracion positiva) y permite tener una gravedad para caer.", MessageType.Info);

        Jump controller = (Jump)target;
        EditorGUILayout.LabelField("Jumping", controller.isJumping().ToString());
        EditorGUILayout.LabelField("Falling", controller.isFalling().ToString());
        EditorGUILayout.LabelField("Jumping or Falling", controller.isJumpingOrFalling().ToString());

		DrawDefaultInspector();
	}
}
