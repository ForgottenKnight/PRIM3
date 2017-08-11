using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReInit : MonoBehaviour {
	public Transform[] playerStartPositions;

	public RectTransform tutorialCanvas;
	public RectTransform tutorialPanel;
	public Text actionText;
	public Text useText;
	public Image tutorialImage;

	// Use this for initialization
	void Start () {

		
		PauseGameController.Resume ();

	/*	SuperAbilityController markerManager = GameObject.FindGameObjectWithTag("MarkerContainer").GetComponent<SuperAbilityController>();
		markerManager.ReInit();
		GameObject[] players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; ++i) {
			players[i].GetComponent<GeneralPlayerController>().ReInitCharacter();
		}
		players = GameObject.FindGameObjectsWithTag ("Player");
		for (int i = 0; i < players.Length; ++i) {
			players[i].transform.position = playerStartPositions[i].position;
			Health h = players [i].GetComponent<Health> ();
			h.FillPercent (100.0f);
		}
		Destroy (gameObject);*/

		/*TutorialManager tm = GameObject.FindObjectOfType<TutorialManager> ();
		tm.tutorialImage = tutorialImage;
		tm.tutorialCanvas = tutorialCanvas;
		tm.tutorialPanel = tutorialPanel;
		tm.tutorialText = useText;
		tm.actionText = actionText;*/

	}
	
	// Update is called once per frame
	void Update () {

	}
}
