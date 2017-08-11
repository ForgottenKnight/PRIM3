using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Toggle keyboard1;
	//public Toggle keyboard2;
	//public Toggle keyboard3;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClick1Player(){
        if (keyboard1.isOn)
        {
            StaticParemeters.useKeyboard = true;
        }
        else
        {
            StaticParemeters.useKeyboard = false;
        }
        StaticParemeters.Init = true;
		StaticParemeters.numPlayers = 1;
        SceneManager.LoadScene("Level1Full");
	}

	public void OnClick2Player(){
        if (keyboard1.isOn)
        {
            StaticParemeters.useKeyboard = true;
        }
        else
        {
            StaticParemeters.useKeyboard = false;
        } 
        StaticParemeters.Init = true;
		StaticParemeters.numPlayers = 2;
        SceneManager.LoadScene("Level1Full");
	}

	public void OnClick3Player(){
        if (keyboard1.isOn)
        {
            StaticParemeters.useKeyboard = true;
        }
        else
        {
            StaticParemeters.useKeyboard = false;
        }
        StaticParemeters.Init = true;
		StaticParemeters.numPlayers = 3;
        SceneManager.LoadScene("Level1Full");
	}



}
