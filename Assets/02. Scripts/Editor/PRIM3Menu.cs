using UnityEngine;
using UnityEditor;
using System.Collections;

public static class PRIM3Menu {


	/*private static GameObject CreatePlayerCharacter(string name)
	{
		// Create a custom game object
		GameObject go = GameObject.CreatePrimitive (PrimitiveType.Capsule);
		go.name = name;
		// Register the creation in the undo system
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;

		go.AddComponent<Movement> ();
		go.AddComponent<Jump> ();
		go.AddComponent<TargetPointer> ();
		go.AddComponent<Health> ();
		go.AddComponent<Energy> ();
		go.AddComponent<Attack> ();

		go.layer = LayerMask.NameToLayer ("Player");
		go.tag = "Player";

		return go;
	}

	[MenuItem("PRIM3/Characters/Base Player",false,1)]
	private static void CreateBasePlayer()
	{
		CreatePlayerCharacter ("Player");
	}

	[MenuItem("PRIM3/Characters/Salt",false,2)]
	private static void CreateSalt()
	{
		GameObject go = CreatePlayerCharacter ("Salt");
		go.AddComponent<SaltController> ();
	}
	
	[MenuItem("PRIM3/Characters/Sulphur",false,3)]
	private static void CreateSulphur()
	{
		GameObject go = CreatePlayerCharacter ("Sulphur");
		go.AddComponent<SulphurController> ();
	}
	
	[MenuItem("PRIM3/Characters/Mercury",false,4)]
	private static void CreateMercury()
	{
		GameObject go = CreatePlayerCharacter ("Mercury");
		go.AddComponent<MercuryController> ();
	}*/

	[MenuItem("PRIM3/Referencia componentes", false, 0)]
	private static void OpenComponentReference() {
		Application.OpenURL ("https://docs.google.com/document/d/1sf8mZDAaWc_9OahnDndd1PqYmIhDuW3YF6fjAESHsq4/edit");
	}

	[MenuItem("PRIM3/Estandar programacion", false, 1)]
	private static void OpenCodeStandard() {
		Application.OpenURL ("https://docs.google.com/document/d/1S-kMSrOsT8qGpCLOAhAfuFjNwjHuCC367FHnsfyUqcU/edit");
	}

	[MenuItem("PRIM3/Documentacion interna", false, 2)]
	private static void OpenDocumentation() {
		Application.OpenURL ("https://docs.google.com/document/d/1Ce6P_Np2XG89A6t_aEk2YcdmTnhElUGRc5pqUoD3vvQ/edit");
	}

	[MenuItem("PRIM3/Camera/PRIM3 Camera",false,5)]
	private static void CreatePrim3Camera() {
		GameObject go = new GameObject ("PRIM3Camera");
		go.tag = "MainCamera";
		go.AddComponent<Camera> ();
		go.AddComponent<GUILayer> ();
		go.AddComponent<FlareLayer> ();
		go.AddComponent<AudioListener> ();
		go.AddComponent<PrimeCamera> ();
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}

	[MenuItem("PRIM3/Camera/Camera Event",false,6)]
	private static void CreateCameraEvent()	{
		GameObject go = new GameObject ("CameraEvent");
		go.AddComponent<BoxCollider> ().isTrigger = true;
		go.AddComponent<PrimeCamera> ().enabled = false;
		go.AddComponent<CameraEvent> ();
		go.transform.localScale = new Vector3 (10.0f, 10.0f, 10.0f);
		Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
		Selection.activeObject = go;
	}
}
