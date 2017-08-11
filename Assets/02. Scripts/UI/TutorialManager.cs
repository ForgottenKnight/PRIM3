using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialManager : MonoBehaviour {
	public static TutorialManager instance;

	[Header("Parametros para los mensajes")]
	public RectTransform messagePanel;
	public Text whoText;
	public Text mainText1;
	public Text mainText2;
	public Text buttonPressText;

	public float yLow = -83.0f;
	public float yHigh = 83.0f;
	public float speed = 10.0f;
	public float waitTime = 5.0f;
	public string dismissAction = "";

	[Header("Parametros para los tutoriales")]
	public RectTransform tutorialPanel;
	public Text actionText;
	public Text tutorialText;
	public Image tutorialImage; // Mando o teclado (caso de un solo jugador o todos mando)
	public Image tutorialImage1; // Mando (caso de mando + teclado)
	public Image tutorialImage2; // Teclado (caso de mando + teclado)
	public float tutorialTime = 3.0f;
	public string dismissTutorial = "";

	public RectTransform tutorialCanvas;

	private TutorialManager() {}

	bool showing;
	bool moving;

	// Use this for initialization
	void Start () {
		showing = false;
		moving = false;
		TutorialManager.instance = this;
		//buttonPressText.text = "";
		if (dismissAction != "") {
			buttonPressText.text = "Pulsa " + dismissAction + " para ocultar.";
		}
	}

	void Update() {
		bool dismiss = false;
		if (dismissAction != "") {
			dismiss = Input.GetKeyDown(dismissAction);
		}
		if (dismiss) {
			DismissMessage();
			DismissTutorial();
		}
		if (moving) {
			if (showing) {
				Vector3 pos = messagePanel.position;
				pos.y += speed * Time.deltaTime;
				pos.y = Mathf.Clamp (pos.y, yLow, yHigh);
				messagePanel.position = pos;
				if (pos.y == yHigh) {
					Invoke ("DismissMessage", waitTime);
					moving = false;
				}
			} else {
				Vector3 pos = messagePanel.position;
				if (pos.y != yLow) {
					pos.y -= speed * Time.deltaTime;
					pos.y = Mathf.Clamp (pos.y, yLow, yHigh);
					messagePanel.position = pos;
				} else {
					moving = false;
				}
			}
		}
	}

	public void Clean() {
		if (whoText) {
			whoText.text = "";
			mainText1.text = "";
			mainText2.text = "";
		}
	}

	public void CleanTutorial() {
		if (tutorialText) {
			tutorialText.text = "";
			actionText.text = "";
			tutorialImage.sprite = null;
		}
	}

	public void ShowMessage(string who, string text) {
		ManageShowMessage ();
		whoText.text = who;
		mainText2.text = text;
	}

	public void ShowMessage(string text) {
		ManageShowMessage ();
		mainText1.text = text;
	}

	void ManageShowMessage() {
		showing = true;
		moving = true;
		Clean ();
		CancelInvoke ("DismissMessage");
	}

	public void ShowTutorial(string action, string text, Sprite img, Sprite imgK) {
		ManageShowTutorial ();
		if (tutorialCanvas) {
			tutorialCanvas.gameObject.SetActive (true);
			if (StaticParemeters.useKeyboard) {
				if (StaticParemeters.numPlayers == 1) { // Caso de un jugador con teclado
					tutorialImage.sprite = imgK;
					tutorialImage.gameObject.SetActive (true);
					tutorialImage1.gameObject.SetActive (false);
					tutorialImage2.gameObject.SetActive (false);
				} else { // Caso de mas de un jugador, uno con teclado
					tutorialImage1.sprite = img;
					tutorialImage2.sprite = imgK;
					tutorialImage.gameObject.SetActive (false);
					tutorialImage1.gameObject.SetActive (true);
					tutorialImage2.gameObject.SetActive (true);
				}
			} else { // Caso sin teclado
				tutorialImage.sprite = img;
				tutorialImage.gameObject.SetActive (true);
				tutorialImage1.gameObject.SetActive (false);
				tutorialImage2.gameObject.SetActive (false);
			}
			actionText.text = action;
			tutorialText.text = text;
		}
		Invoke ("DismissTutorial", tutorialTime);
	}

	void ManageShowTutorial() {
		CleanTutorial ();
		CancelInvoke ("DismissTutorial");
	}

	public void DismissMessage() {
		showing = false;
		moving = true;
	}

	public void DismissTutorial() {
		tutorialCanvas.gameObject.SetActive (false);
	}

}
