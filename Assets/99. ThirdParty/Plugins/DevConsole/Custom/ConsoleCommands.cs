using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SickDev.CommandSystem;
using DevConsole;

public class ConsoleCommands : MonoBehaviour {

    static void ResetPlayers() {
		GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");
		DevConsoleHelper dhc = GetHelper ();
		if (dhc) {
			Transform[] positions = GameObject.FindGameObjectWithTag ("DevConsoleHelper").GetComponent<DevConsoleHelper> ().resetPositions;
			for (int i = 0; i < players.Length; ++i) {
				players [i].transform.position = positions [i].position;
			}
			Console.Log ("Jugadores retornados a la posicion inicial", Color.green);
		}
    }

	[Command]
	static void GoTo(string place) {
		GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");
		Transform position = null;
		DevConsoleHelper dhc = GetHelper ();
		if (dhc) {
			GameObject.FindGameObjectWithTag ("DevConsoleHelper").GetComponent<DevConsoleHelper> ().gotoPlaces.TryGetValue (place, out position);
			if (position) {
				for (int i = 0; i < players.Length; ++i) {
					players [i].transform.position = position.position;
				}
			}
			Console.Log ("Jugadores reposicionados", Color.green);
		}
	}

	[Command]
	static void SetForce(float force) {
		GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; ++i) {
			players [i].GetComponent<Attack>().attackForce = force;
		}
		Console.Log ("Fuerza asignada a jugadores: " + force);
	}


	[Command]
	static void Godmode() {
		GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; ++i) {
			players [i].GetComponent<Health>().godmode = true;
		}
		Console.Log ("Invencibilidad activada", Color.green);
	}

	[Command]
	static void Godmodeoff() {
		GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; ++i) {
			players [i].GetComponent<Health>().godmode = false;
		}
		Console.Log ("Invencibilidad desactivada", Color.red);
	}

	[Command]
	static void Test(int testnum) {
		switch (testnum) {
		case 0:
			GoTo("puzzle");
			Godmode();
			break;
		}
	}

	[Command]
	static void ShowPlaces() {
		DevConsoleHelper dhc = GetHelper ();
		if (dhc) {

			Dictionary<string, Transform>.KeyCollection keys = GameObject.FindGameObjectWithTag ("DevConsoleHelper").GetComponent<DevConsoleHelper> ().gotoPlaces.Keys;
			Console.Log("The nex places are available:", Color.green);
			foreach(string key in keys) {
				Console.Log(key, Color.white);
			}
		}
	}

	[Command]
	static void SetSpeed(float speed) {
		GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; ++i) {
			players [i].GetComponent<Movement>().movementSpeed = speed;
		}
	}

	static DevConsoleHelper GetHelper() {
		DevConsoleHelper dch = GameObject.FindGameObjectWithTag ("DevConsoleHelper").GetComponent<DevConsoleHelper> ();
		if (!dch) {
			Console.LogWarning("No hay DevConsoleHelper en la escena actual. Considera crear el objeto DevConsoleHelper o instanciarlo desde el prefab.");
		}
		return dch;
	}

	[Command]
	static void SetCharacterHealth(float player = 0, float health = 0 ) {
		GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; ++i) {
			if(players[i].GetComponent<GeneralPlayerController>().player == player) {
				Health h = players[i].GetComponent<Health>();
				Incapacitate incap = players[i].GetComponent<Incapacitate>();
				if(h.GetHealth() == 0) {
					incap.Revive();
					h.SetHealth(health);
				} else {
					h.SetHealth(health);
					if(h.GetHealth() == 0) {
						h.Damage(9999);
					}
				}

			}
		}
	}

	[Command]
	static void TriggerEvent(string name) {
		SimpleEvent.eventsDictionary [name].ExternalDebugFunction ();
	}

	[Command]
	static void SearchByTag(string aTag) {
		List<GameObject> l_GameObjectList = CustomTagManager.GetObjectsByTag (aTag);
		for (int i = 0; i < l_GameObjectList.Count; ++i) {
			Console.LogInfo (l_GameObjectList[i].name);
		}
	}

	[Command]
	static void ShowDebug(bool active) {
		GameObject l_Camera = GameObject.Find ("DebugCamera");
		l_Camera.GetComponent<Camera> ().enabled = active;
	}

	[Command]
	static void ActivateEffects(bool active) {
		QualityManager.instance.ActivateEffects (active);
	}

	[Command]
	static void ActivateObject(string obj, bool active) {
		QualityManager.instance.ActivateObject (obj, active);
	}

	[Command]
	static void Dialog(int character, string text) {
		List<GameObject> players = CustomTagManager.GetObjectsByTag ("Player");
		for (int i = 0; i < players.Count; ++i) {
			if (players [i].GetComponent<GeneralPlayerController> ().character == character) {
				players [i].GetComponent<IngameDialog> ().ShowDialog (text);
			}
		}
	}

	[Command]
	static void DialogEvent(string ev) {
		IngameDialogManager idm = GameObject.FindObjectOfType<IngameDialogManager> ();
		idm.ShowEvent (ev);
	}

	[Command]
	static void DialogTrigger(string trigger) {
		IngameDialogManager idm = GameObject.FindObjectOfType<IngameDialogManager> ();
		idm.ShowTrigger (trigger);
	}

}