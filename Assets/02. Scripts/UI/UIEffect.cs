using UnityEngine;
//using UnityEditor;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIEffect : MonoBehaviour {

	[System.Serializable]
	public struct OverTimeEffects {
		public ScaleAnimatorEffect scaleAnimator;
		public MovementAnimatorEffect movementAnimator;
		public FadeAnimatorEffect fadeAnimator;
	}

	[System.Serializable]
	public struct ScaleAnimatorEffect {
		public bool active;
		[Range(0, 10)]
		public float speed;
		public Vector2 originalSize;
		public Vector2 scaleTreshold;
	}

	[System.Serializable]
	public struct MovementAnimatorEffect {
		public bool active;
		[Range(0, 200)]
		public float speed;
		public List<Transform> trajectory;
		public int currentPoint;
		public bool directionPositive;
	}

	[System.Serializable]
	public struct FadeAnimatorEffect {
		public Vector2 opacityTreshold;
		public bool active;
		[Range(0, 10)]
		public float speed;
		[HideInInspector]
		public bool directionPositive;
	}

	[System.Serializable]
	public struct FadeInEffect {
		public float maxOpacity;
		[Range(-10, 10)]
		public float speed;
		public bool recursiveToRoot;
		public bool active;
		public bool destroyGameobjectOnEnd;
		public bool destroyScriptobjectOnEnd;
		public bool InactiveOnEnd;
	}
	[System.Serializable]
	public struct FadeOutEffect {
		public float minOpacity;
		[Range(-10, 10)]
		public float speed;
		public bool recursiveToRoot;
		public bool active;
		public bool destroyGameobjectOnEnd;
		public bool destroyScriptobjectOnEnd;
		public bool InactiveOnEnd;
	}


	[System.Serializable]
	public struct MoveToEffect {
		public string name;
		public List<Keyframe> Keyframes;
		public int currentKeyFrame;
		public int state; //O --> Waiting , 1--> Working, 2 --> Ended
		public bool recursiveToRoot;
		public bool destroyGameobjectOnEnd;
		public bool destroyScriptobjectOnEnd;
		public bool InactiveOnEnd;
	}
	
	
	[System.Serializable]
	public struct Keyframe {
		public RectTransform transf;
		public float speed;
	}


	[Header("Configuration")]
	public bool m_active = true;
	private Image m_image;
	private Text m_text;
	private CanvasGroup m_canvasGroup;
	private RectTransform m_rectTransform;
	private bool canUpdate;

	[Header("Inmediate time effects")]
	public FadeInEffect m_fadeIn;
	public FadeOutEffect m_fadeOut;
	public List<MoveToEffect> m_moveTo;


	[Header("Over time effects")]
	public OverTimeEffects m_effects;


	void Awake() {
		m_image = GetComponent<Image> ();
		m_text = GetComponent<Text> ();
		m_canvasGroup = GetComponent<CanvasGroup> ();
		m_rectTransform = GetComponent<RectTransform> ();
		if (m_rectTransform != null) {
			m_effects.scaleAnimator.originalSize = m_rectTransform.sizeDelta;
		}
	}
	// Use this for initialization
	void Start () {
		m_fadeIn.active = false;
		m_fadeOut.active = false;
		canUpdate = false;
		StartCoroutine (AllowUpdate());
	}

	IEnumerator AllowUpdate() {
		yield return new WaitForEndOfFrame ();
		canUpdate = true;
	}
	
	// Update is called once per frame
	public void Update () {
		if (m_active && canUpdate) {
			if (m_effects.scaleAnimator.active) {
				ScaleAnimator ();
			}
			if (m_effects.movementAnimator.active) {
				MovementAnimator ();
			}
			if (m_effects.fadeAnimator.active) {
				FadeAnimator ();
			}
		}
	}

	public void SetOpacity(float aOpacity) {
		if (m_image != null) {
			Color l_newColor = m_image.color;
			l_newColor.a = aOpacity;
			m_image.color = l_newColor;
		}
		if (m_text != null) {
			Color l_newColor = m_text.color;
			l_newColor.a = aOpacity;
			m_text.color = l_newColor;
		}
	}


	public void SetActive(bool aActive) {
		m_active = aActive;
	}

	public void MoveTo(int aIndex) {
		IEnumerator l_coroutine = MoveToCoroutine(aIndex);
		StartCoroutine(l_coroutine);
	}

	private IEnumerator MoveToCoroutine(int aIndex) {
		yield return new WaitForEndOfFrame ();

		int l_currentKeyFrame = 0;
		transform.position = m_moveTo [aIndex].Keyframes [0].transf.transform.position;

		while(l_currentKeyFrame < m_moveTo[aIndex].Keyframes.Count){
			Keyframe l_nextTransform = m_moveTo[aIndex].Keyframes [l_currentKeyFrame];
			if (l_nextTransform.transf != null) {
				float l_step = m_moveTo[aIndex].Keyframes [l_currentKeyFrame].speed * Time.unscaledDeltaTime;
				transform.position = Vector3.MoveTowards (transform.position, l_nextTransform.transf.position, l_step);
				transform.forward = Vector3.RotateTowards (transform.forward, l_nextTransform.transf.forward, l_step, 0.0f);

				if (transform.position == l_nextTransform.transf.position && transform.rotation == l_nextTransform.transf.rotation) {
					l_currentKeyFrame++;
				}
			}
			yield return null;
		}

		if (m_moveTo [aIndex].InactiveOnEnd) { //TODO: Eliminar todos los que se va encontrando de camino a root
			if (m_moveTo [aIndex].recursiveToRoot) {
				transform.root.gameObject.SetActive(false);
			} else {
				gameObject.SetActive(false);
			}
		}
		if (m_moveTo [aIndex].destroyGameobjectOnEnd) {
			if (m_moveTo [aIndex].recursiveToRoot) {
				Destroy(transform.root.gameObject);
			} else {
				Destroy(gameObject);
			}
		}
		if (m_moveTo [aIndex].destroyScriptobjectOnEnd) {
			if (m_moveTo [aIndex].recursiveToRoot) {
				Destroy(transform.root.GetComponent<UIEffect>());
			} else {
				Destroy(this);
			}
		}
	}


	public void FadeInForceEnd() {
		if (m_fadeIn.active) {
			m_fadeIn.active = false;
		}
	}
	public void FadeOutForceEnd() {
		if (m_fadeOut.active) {
			m_fadeOut.active = false;
		}
	}

	public void FadeIn() {
		m_canvasGroup.alpha = 0;
		if (m_canvasGroup == null) {
			m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
		}
		m_fadeIn.active = true;
		StartCoroutine ("FadeInCoroutine");
	}

	public void FadeOut() {
		if (m_canvasGroup == null) {
			m_canvasGroup = gameObject.AddComponent<CanvasGroup>();
		}
		m_fadeOut.active = true;
		StartCoroutine ("FadeOutCoroutine");
	}

	private IEnumerator FadeInCoroutine() {
		while (m_canvasGroup.alpha < m_fadeIn.maxOpacity && m_fadeIn.active) {
			float l_newAlpha = m_canvasGroup.alpha + (m_fadeIn.speed * Time.unscaledDeltaTime);
			if(l_newAlpha > m_fadeIn.maxOpacity) {
				m_canvasGroup.alpha = m_fadeIn.maxOpacity;
			} else {
				m_canvasGroup.alpha = l_newAlpha;
			}
			yield return null;
		}
		m_canvasGroup.alpha = m_fadeIn.maxOpacity;
		m_fadeIn.active = false;

		if (m_fadeIn.InactiveOnEnd) { //TODO: Eliminar todos los que se va encontrando de camino a root
			if (m_fadeIn.recursiveToRoot) {
				transform.root.gameObject.SetActive(false);
			} else {
				gameObject.SetActive(false);
			}
		}
		if (m_fadeIn.destroyGameobjectOnEnd) {
			if (m_fadeIn.recursiveToRoot) {
				Destroy(transform.root.gameObject);
			} else {
				Destroy(gameObject);
			}
		}
		if (m_fadeIn.destroyScriptobjectOnEnd) {
			if (m_fadeIn.recursiveToRoot) {
				Destroy(transform.root.GetComponent<UIEffect>());
			} else {
				Destroy(this);
			}
		}
	}
	private IEnumerator FadeOutCoroutine() {

		while (m_canvasGroup.alpha > m_fadeOut.minOpacity && m_fadeOut.active) {
			float l_newAlpha = m_canvasGroup.alpha - (m_fadeOut.speed * Time.unscaledDeltaTime);
			if(l_newAlpha < m_fadeOut.minOpacity) {
				m_canvasGroup.alpha = m_fadeOut.minOpacity;
			} else {
				m_canvasGroup.alpha = l_newAlpha;
			}
			yield return null;
		}
		m_canvasGroup.alpha = m_fadeOut.minOpacity;
		m_fadeOut.active = false;

		if (m_fadeOut.InactiveOnEnd) { //TODO: Eliminar todos los que se va encontrando de camino a root
			if (m_fadeOut.recursiveToRoot) {
				transform.root.gameObject.SetActive(false);
			} else {
				gameObject.SetActive(false);
			}
		}
		if (m_fadeOut.destroyGameobjectOnEnd) {
			if (m_fadeOut.recursiveToRoot) {
				Destroy(transform.root.gameObject);
			} else {
				Destroy(gameObject);
			}
		}
		if (m_fadeOut.destroyScriptobjectOnEnd) {
			if (m_fadeOut.recursiveToRoot) {
				Destroy(transform.root.GetComponent<UIEffect>());
			} else {
				Destroy(this);
			}
		}
	}


	public void ActivateScaleAnimator(bool aActive) {
		m_effects.scaleAnimator.active = aActive;
		m_rectTransform.sizeDelta = m_effects.scaleAnimator.originalSize;
	}

	private void ScaleAnimator() {
		float l_newScale = m_effects.scaleAnimator.scaleTreshold.x + Mathf.PingPong (Time.time * m_effects.scaleAnimator.speed, m_effects.scaleAnimator.scaleTreshold.y - m_effects.scaleAnimator.scaleTreshold.x);
		m_rectTransform.sizeDelta = new Vector2 (m_effects.scaleAnimator.originalSize.x * l_newScale, m_effects.scaleAnimator.originalSize.y * l_newScale);
	}
	
	public void ActivateMovementAnimator(bool aActive) {
		m_effects.movementAnimator.active = aActive;
		if (!aActive) {
			transform.position = m_effects.movementAnimator.trajectory[0].transform.position;
		}
	}

	private void MovementAnimator() {

		float l_step = m_effects.movementAnimator.speed * Time.unscaledDeltaTime;
		if (m_effects.movementAnimator.directionPositive) {
			if(Vector3.Distance(transform.position, m_effects.movementAnimator.trajectory [m_effects.movementAnimator.currentPoint].position) < 0.05f) {
				if (m_effects.movementAnimator.currentPoint < m_effects.movementAnimator.trajectory.Count - 1) {
					m_effects.movementAnimator.currentPoint++;
				} else {
					m_effects.movementAnimator.directionPositive = false;
				}
			}
			Vector3 l_destiny = (m_effects.movementAnimator.trajectory [m_effects.movementAnimator.currentPoint].position);
			transform.position = Vector3.MoveTowards (transform.position, l_destiny , l_step);
		} else {
			if(Vector3.Distance(transform.position, m_effects.movementAnimator.trajectory [m_effects.movementAnimator.currentPoint].position) < 0.05f) {
				if (m_effects.movementAnimator.currentPoint > 0) {
					m_effects.movementAnimator.currentPoint--;
				} else {
					m_effects.movementAnimator.directionPositive = true;
				}
			}
			Vector3 l_destiny = (m_effects.movementAnimator.trajectory [m_effects.movementAnimator.currentPoint].position);
			transform.position = Vector3.MoveTowards (transform.position, l_destiny , l_step);
		}
	}

	
	public void ActivateFadeAnimator(bool aActive) {
		m_effects.fadeAnimator.active = aActive;
		if (m_image != null) {
			Color l_newColor = m_image.color;
			l_newColor.a = 1.0f;
			m_image.color = l_newColor;
		}
	}

	private void FadeAnimator() {
		FadeImageAnimator ();
		FadeTextAnimator ();
	}

	private void FadeImageAnimator() {
		if (m_image != null) {
			if(m_effects.fadeAnimator.directionPositive) {
				float l_newOpacity = m_image.color.a +  m_effects.fadeAnimator.speed * Time.unscaledDeltaTime;
				if(l_newOpacity < m_effects.fadeAnimator.opacityTreshold.y){
					Color l_newColor = m_image.color;
					l_newColor.a = l_newOpacity;
					m_image.color = l_newColor;
				} else {
					Color l_newColor = m_image.color;
					l_newColor.a = m_effects.fadeAnimator.opacityTreshold.y;
					m_image.color = l_newColor;
					m_effects.fadeAnimator.directionPositive = false;
				}
			} else {
				float l_newOpacity = m_image.color.a - m_effects.fadeAnimator.speed * Time.unscaledDeltaTime;
				if(l_newOpacity > m_effects.fadeAnimator.opacityTreshold.x){
					Color l_newColor = m_image.color;
					l_newColor.a = l_newOpacity;
					m_image.color = l_newColor;
				} else {
					Color l_newColor = m_image.color;
					l_newColor.a = m_effects.fadeAnimator.opacityTreshold.x;
					m_image.color = l_newColor;
					m_effects.fadeAnimator.directionPositive = true;
				}
			}
		}
	}
	private void FadeTextAnimator() {
		if (m_text != null) {
			if(m_effects.fadeAnimator.directionPositive) {
				float l_newOpacity = m_text.color.a +  m_effects.fadeAnimator.speed * Time.unscaledDeltaTime;
				if(l_newOpacity < m_effects.fadeAnimator.opacityTreshold.y){
					Color l_newColor = m_text.color;
					l_newColor.a = l_newOpacity;
					m_text.color = l_newColor;
				} else {
					Color l_newColor = m_text.color;
					l_newColor.a = m_effects.fadeAnimator.opacityTreshold.y;
					m_text.color = l_newColor;
					m_effects.fadeAnimator.directionPositive = false;
				}
			} else {
				float l_newOpacity = m_text.color.a - m_effects.fadeAnimator.speed * Time.unscaledDeltaTime;
				if(l_newOpacity > m_effects.fadeAnimator.opacityTreshold.x){
					Color l_newColor = m_text.color;
					l_newColor.a = l_newOpacity;
					m_text.color = l_newColor;
				} else {
					Color l_newColor = m_text.color;
					l_newColor.a = m_effects.fadeAnimator.opacityTreshold.x;
					m_text.color = l_newColor;
					m_effects.fadeAnimator.directionPositive = true;
				}
			}
		}
	}
}
