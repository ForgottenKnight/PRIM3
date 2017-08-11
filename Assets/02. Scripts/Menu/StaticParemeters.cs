using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class StaticParemeters{

	static public int numPlayers = 1;
	static public bool useKeyboard = false;
    static public bool Init = false;


    static public bool savedConfiguration = false;
    static public int[] characters = new int[3];//0->Salt, 1->Sulphur, 2->Mercury
    static public bool[] activeCharacters = new bool[3];//0->Salt, 1->Sulphur, 2->Mercury
    static public XboxController[] playerControllers = new XboxController[3];



	// Use this for initialization
	static void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    static public void SaveConfiguration(GameObject Salt, GameObject Sulphur, GameObject Mercury)
    {
        savedConfiguration = true;
        characters[0] = Salt.GetComponent<GeneralPlayerController>().player;
        activeCharacters[0] = Salt.activeSelf;
        playerControllers[0] = Salt.GetComponent<GeneralPlayerController>().controller;

        characters[1] = Sulphur.GetComponent<GeneralPlayerController>().player;
        activeCharacters[1] = Sulphur.activeSelf;
        playerControllers[1] = Sulphur.GetComponent<GeneralPlayerController>().controller;

        characters[2] = Mercury.GetComponent<GeneralPlayerController>().player;
        activeCharacters[2] = Mercury.activeSelf;
        playerControllers[2] = Mercury.GetComponent<GeneralPlayerController>().controller;

    }
}
