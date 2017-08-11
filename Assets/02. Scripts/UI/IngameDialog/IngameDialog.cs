using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngameDialog : MonoBehaviour {
	public GameObject dialogPrefab;
	public Sprite portrait;
	private GameObject uiCharacters;
	private GameObject dialogCanvas;
	private GameObject dialogPanel;
	private GameObject portraitPanel;
    public Transform head;
	private Text dialogText;
	private Camera primeCamera;
	public Vector3 portraitOffset = Vector3.zero;
	public float limitTop = 0.1f;
	public float limitRight = 0.9f;

	public float dialogTime = 5.0f;
	private bool showingDialog;
	private float startingTime;

	private bool started = false;
    private PrimeCamera primeCameraData;
    public float zoomConstant;



	// Use this for initialization
	void Start () {
		uiCharacters = GameObject.FindObjectOfType<CharactersUIManager> ().gameObject;
		dialogCanvas = (GameObject) Instantiate (dialogPrefab, uiCharacters.transform);
		dialogPanel = dialogCanvas.transform.GetChild (0).gameObject;
		portraitPanel = dialogCanvas.transform.GetChild (1).gameObject;
		portraitPanel.transform.position = portraitPanel.transform.position + portraitOffset;
		portraitPanel.transform.GetChild (0).GetComponent<Image> ().sprite = portrait;
		dialogText = dialogCanvas.GetComponentInChildren<Text> ();
		primeCamera = Camera.main;
		dialogCanvas.SetActive (false);
		showingDialog = false;
		started = true;
        primeCameraData = primeCamera.GetComponent<PrimeCamera>();
	}
	
	// Update is called once per frame
	void Update () {
		if (dialogCanvas.activeSelf) {
			Vector3 l_TargetPosition = GetTargetPosition ();
			dialogPanel.transform.position = l_TargetPosition;
		}
	}

	private Vector3 GetTargetPosition() {
		Vector3 playerViewportPosition = primeCamera.WorldToViewportPoint (transform.position);
        //Vector3 finalOffset = offset;
        Vector3[] corners = new Vector3[4];
        dialogPanel.GetComponent<RectTransform>().GetWorldCorners(corners);
        float minY = Mathf.Min(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
        float maxY = Mathf.Max(corners[0].y, corners[1].y, corners[2].y, corners[3].y);
        float height = maxY - minY;
        Vector3 finalOffset = new Vector3(primeCameraData.zoom * zoomConstant, 0f, 0f);
        if (playerViewportPosition.x > limitRight) {
            
            float minX = Mathf.Min(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
            float maxX = Mathf.Max(corners[0].x, corners[1].x, corners[2].x, corners[3].x);
            float width = maxX - minX;
            finalOffset.x = -width - primeCameraData.zoom * zoomConstant;
            
		}
		if (playerViewportPosition.y < limitTop) {
			finalOffset.y = -height;
		}
        Vector3 unoffsetedPosition = primeCamera.WorldToScreenPoint(head.position);
        //finalOffset = finalOffset * primeCameraData.zoom / zoomConstant;
        //Vector3 offsetToApply = Vector3.zero;
        Vector3 offsetedPosition = unoffsetedPosition + finalOffset;
        return offsetedPosition;
	}

	public void ShowDialog(string text, bool time = true) {
		dialogCanvas.SetActive(true);
		StopAllCoroutines ();
		dialogText.text = text;
		showingDialog = true;
		if (time) {
			startingTime = Time.time;
		}
		if (gameObject.activeSelf == true) {
			portraitPanel.SetActive (false);
			StartCoroutine (DialogCoroutine (text));
		} else {
			ShowDialogPortrait ();
		}
	}

	public void ShowDialogPortrait() {
		showingDialog = false;
		portraitPanel.SetActive (true);
		portraitPanel.GetComponent<PortraitSelfcontroller> ().ShowDialog (dialogTime - (Time.time - startingTime));
	}

	void OnDisable() {
		StopAllCoroutines ();
		if (showingDialog) {
			ShowDialogPortrait ();
		} else {
            if (dialogCanvas != null)
            {
                dialogCanvas.SetActive(false);
            }
		}
	}

	void OnEnable() {
		if (started == true) {
			if (portraitPanel.activeSelf) {
				portraitPanel.SetActive (false);
				ShowDialog (dialogText.text, false);
			}
		}
	}

	IEnumerator DialogCoroutine(string text) {
		yield return new WaitForSeconds (dialogTime - (Time.time - startingTime));
		dialogCanvas.SetActive(false);
		showingDialog = false;
		yield return null;
	}
}
