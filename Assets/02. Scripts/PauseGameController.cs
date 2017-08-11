using UnityEngine;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;

public static class PauseGameController {
	private static float LastTimeScale;
	public static bool Paused = false;


	public static void Pause() {
		if (!PauseGameController.Paused) {
			PauseGameController.Paused = true;
			PauseGameController.LastTimeScale = Time.timeScale;
			PauseGameController.PlayersHaveInput(false);
			PauseGameController.EnemiesHaveInput(false);
			Time.timeScale = 0f;
		}
	}

	public static void Resume() {
		if (PauseGameController.Paused) {
			PauseGameController.Paused = false;
			Time.timeScale = PauseGameController.LastTimeScale;

			PauseGameController.PlayersHaveInput(true);
			PauseGameController.EnemiesHaveInput(true);
		}
	}


	public static void PlayersHaveInput(bool aHasInput) {
		List<GameObject> players = CustomTagManager.GetObjectsByTag ("Player");
		for (int i = 0; i < players.Count; ++i) {
			IPausable[] l_Pausables = players [i].GetComponents<IPausable> ();
			for (int j = 0; j < l_Pausables.Length; ++j) {
				if (aHasInput == true) {
					l_Pausables [j].Unpause ();
				} else {
					l_Pausables [j].Pause ();
				}
			}
		}

	}

	public static void EnemiesHaveInput(bool aHaveInput) {
		List<GameObject> enemies = CustomTagManager.GetObjectsByTag ("Enemy");
		for (int i = 0; i < enemies.Count; ++i) {
			BehaviorTree l_BehaviorTree = enemies[i].GetComponent<BehaviorTree>();
			if (l_BehaviorTree != null) {
				l_BehaviorTree.enabled = aHaveInput;
			}
			IPausable l_Pausable = enemies[i].GetComponent<IPausable>();
			if (l_Pausable != null) {
				if (aHaveInput) {
					l_Pausable.Unpause();
				} else {
					l_Pausable.Pause();
				}
			}
		}
	}
}
