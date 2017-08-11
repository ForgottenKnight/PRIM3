using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowOffCamera : MonoBehaviour {
	[Header("References")]
	public GameObject iconPrefab;
	public Canvas iconCanvas;

	[Header("Offsets")]
	public Offsets viewportOffsets;
	public Offsets iconOffsets;

	[Header("Animator")]
	public bool animated = false;
	public float movementQuantity = 10;
	public float speed = 2;

	private GameObject m_Icon;
	private GameObject m_IconArrow;
	private GameObject m_IconImage;
	private Camera m_Camera;
	private bool m_playerInCameraView = true;
	private Vector3 m_playerScreenPosition;


	[System.Serializable]
	public struct Offsets {
		public float offsetRight;
		public float offsetLeft;
		public float offsetTop;
		public float offsetBottom;
	}


	private enum ScreenLimits {
		Right,
		Left,
		Top,
		Bottom
	};

	// Use this for initialization
	public void Start () {
		m_Icon = (GameObject)Instantiate (iconPrefab, new Vector3 (0, 0, 0), Quaternion.identity);
		m_Icon.transform.SetParent (iconCanvas.transform);
		m_Icon.transform.localPosition = new Vector3 (0, 0, 0);
		m_IconArrow = m_Icon.transform.GetChild(0).gameObject;
		m_IconImage = m_IconArrow.transform.GetChild (0).gameObject;
		m_Camera = Camera.main;
		HideOffCameraIcon ();
	}
	
	// Update is called once per frame
	public void Update () {
		if (!IsPlayerInCameraView () && m_playerInCameraView) {
			DrawOffCameraIcon();
		} else if (IsPlayerInCameraView () && !m_playerInCameraView){
			HideOffCameraIcon();
		}
		IconLookAtPlayer ();
		if (animated) {
			Animate();
		}
	}

	public void OnDestroy() {
		Destroy (m_Icon);
	}

	//Mira si el jugador se encuentra dentro de la vision de la camara
	private bool IsPlayerInCameraView(){
        Vector3 m_PlayerViewportPosition = m_Camera.WorldToViewportPoint(transform.position);
        m_playerScreenPosition = m_Camera.WorldToScreenPoint(transform.position);
        if (m_PlayerViewportPosition.x > viewportOffsets.offsetLeft && m_PlayerViewportPosition.y > viewportOffsets.offsetTop &&
            m_PlayerViewportPosition.x < viewportOffsets.offsetRight && m_PlayerViewportPosition.y < viewportOffsets.offsetBottom)
        {
            return true;
        }
        return false;
		/*m_playerScreenPosition = m_Camera.WorldToScreenPoint (transform.position);
		if (m_playerScreenPosition.x > viewportOffsets.offsetLeft && 
		    m_playerScreenPosition.x < Screen.width - viewportOffsets.offsetRight &&
		    m_playerScreenPosition.y > viewportOffsets.offsetBottom &&
		    m_playerScreenPosition.y < Screen.height - viewportOffsets.offsetTop) {
			return true;
		} else {
			return false;
		}*/
	}

	//Dibuja el icono que indica la direccion en la que se encuentra el jugador
	private void DrawOffCameraIcon() {
		m_IconArrow.SetActive(true);
		m_playerInCameraView = false;
	}

	//Esconde el icono del jugador
	private void HideOffCameraIcon() {
		m_IconArrow.SetActive(false);
		m_playerInCameraView = true;
	}
	private void IconLookAtPlayer() {
		//Rotacion de la flecha
		Vector3 l_referenceForward = transform.up;
		Vector3 l_referenceRight= Vector3.Cross(Vector3.forward, l_referenceForward);
		Vector3 l_newDirection = m_playerScreenPosition - m_Icon.transform.GetComponent<RectTransform> ().position;
		l_newDirection.z = 0;
		float l_angle = Vector3.Angle(l_newDirection, l_referenceForward);
		float l_sign = Mathf.Sign(Vector3.Dot(l_newDirection, l_referenceRight));
		float l_finalAngle = l_sign * l_angle;
		m_Icon.transform.eulerAngles = new Vector3 (m_Icon.transform.eulerAngles.x, m_Icon.transform.eulerAngles.y, l_finalAngle);

		//Rotacion de la imagen(fija)
		m_IconImage.transform.rotation = Quaternion.LookRotation(Vector3.back);

		//Posicion de la flcha
		Vector3 l_newPosition = new Vector3(m_playerScreenPosition.x - Screen.width/2, m_playerScreenPosition.y - Screen.height/2, 0);
		l_newPosition = new Vector3 (Mathf.Clamp(l_newPosition.x, -Screen.width/2 + iconOffsets.offsetLeft, Screen.width/2 - iconOffsets.offsetRight), Mathf.Clamp(l_newPosition.y, -Screen.height/2 + iconOffsets.offsetBottom, Screen.height/2 + iconOffsets.offsetTop) ,0);
		m_Icon.GetComponent<RectTransform>().localPosition = l_newPosition;
	}

	private void Animate() {
		float l_interval = Mathf.PingPong(Time.time, speed) - speed/2;
		Vector2 l_movement = new Vector3(0 , movementQuantity * l_interval);
		m_IconArrow.GetComponent<RectTransform>().anchoredPosition  += l_movement;
	}

}
