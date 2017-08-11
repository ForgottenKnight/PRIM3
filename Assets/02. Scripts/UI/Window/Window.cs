using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using UnityEngine.UI;
using XboxCtrlrInput;
using UnityEngine.Video;

public class Window : MonoBehaviour {
	[Header("Assets")]
	protected List<Page> m_pages = new List<Page> ();


	[Header("Referencias")]
	protected WindowManager m_windowManager;
	public Text m_text;


	[Header("Configuracion")]
	public KeyCode[] closeKey;
	public KeyCode[] nextPage;
	public KeyCode[] previousPage;
	public bool lifeTime;
	public float lifeTimeSeconds;
	public float blockInputTimeSeconds;
	protected float m_currentLifeTime;
	protected bool m_active;
	public Animator m_animator;
	protected bool toBeDestroyed = false;
	public bool destroyOnEnd = true; 

	protected int m_currentPage = 0;
    private XboxController m_Controller;

	public enum CONTROLLER_TYPE {
		KEYBOARD,
		XBOX,
		PORTRAITS
	}

	[System.Serializable]
	public struct IMAGE_CONTROLLER {
		public CONTROLLER_TYPE controller;
		public Sprite image;
		public List<int> pages;
	}

	[System.Serializable]
	public struct VIDEO_PAGE {
		public VideoClip video;
		public List<int> pages;
	}


	
	public virtual void Awake () {
		m_animator = GetComponent<Animator> ();
	}


	// Use this for initialization
	public virtual void Start () {
		m_currentLifeTime = lifeTimeSeconds;
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if (m_active) {
			ManageWindowControls ();
			if (lifeTime) {
				ManageLifeTime ();
			}
		}
	}

	public virtual void Initialize() {
		LoadPage (0);
	}

	public virtual void AddPage() {
	}

	public virtual void LoadPage(int aPageIndex) {
	}
		
	public virtual void Show() {

	}

	public virtual void Hide() {

	}

	public void setInactive() {
	}




	public void destroyWindow() {
		Destroy (gameObject);
	}

	public void setInactiveWindow() {
		gameObject.SetActive (false);
	}

	public virtual void Open() {
		m_active = true;
		gameObject.SetActive (true);
	}
	public virtual void Close() {
		m_active = false;
	}

	public bool IsClosed () {
		return (m_active == false);	
	}

	public virtual void ManageLifeTime() {
		m_currentLifeTime -= Time.deltaTime;
		if (m_currentLifeTime <= 0) {
			Close();
			toBeDestroyed = true;
		}

	}

	public virtual void ManageWindowControls () {
		blockInputTimeSeconds -= Time.unscaledDeltaTime;
	}


}
