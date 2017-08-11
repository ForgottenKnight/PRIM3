using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class FinishGame {
	static public void ReturnToMenu() {
		DontDestroyOnLoad [] toDestroy = GameObject.FindObjectsOfType<DontDestroyOnLoad> ();
		for (int i = 0; i < toDestroy.Length; ++i) {
			GameObject.Destroy(toDestroy[i].gameObject);
		}
        ReestartCheckpoint.haveCheckpoint = false;
        SceneManager.LoadScene("MainMenu");
	}

    static public void ReestartLevel()
    {
        DontDestroyOnLoad[] toDestroy = GameObject.FindObjectsOfType<DontDestroyOnLoad>();
        for (int i = 0; i < toDestroy.Length; ++i)
        {
            GameObject.Destroy(toDestroy[i].gameObject);
        }
        
        SimpleEvent.ClearDictionary();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
