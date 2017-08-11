using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PortalScript : MonoBehaviour {
	public string scene = "MainMenu";

    public ConfigurePlayers PlayerConfiguration;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider col) 
	{
		if(col.tag == "Player")
		{
			SimpleEvent.ClearDictionary();
            if (PlayerConfiguration)
            {
               PlayerConfiguration.SaveParameters();
            }
			if(scene == "MainMenu")
				FinishGame.ReturnToMenu();

			ReestartCheckpoint.haveCheckpoint = false;
            SceneManager.LoadScene(scene);
		}
	}
}
