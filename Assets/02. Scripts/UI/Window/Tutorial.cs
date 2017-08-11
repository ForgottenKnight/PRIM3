using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using XboxCtrlrInput;
using UnityEngine.Video;

public class Tutorial : Window {

	[Header("Referencias")]
	public GameObject m_imageGroup;
	public Text m_closeTimeText;
	public VideoPlayer m_videoPlayer;
	public AudioSource m_audioSource;
	public RawImage m_rawImage;
	public Image m_window;
	public Image m_background;
	public Text m_header;
	public Text m_subheader;
	public Text m_page;
	public Image m_nextPage;
	public Image m_previousPage;


	[Header("Configuracion")]
	public KeyCode[] showWindowKey;

	private bool m_collapsed = true;

    private XboxController controller;

	private bool preparedVideo = false;
	
	public override void Start () {
		//m_collapsed = true;
		m_animator = GetComponent<Animator> ();
		base.Start ();
	}

	public override void Update () {
		base.Update ();
	}
		
		
	protected void UpdateCurrentPage() {
		m_page.text = (m_currentPage + 1).ToString() + " / " + m_pages.Count.ToString();
	}
	
	public override void Close() {
		base.Close ();
		if (m_collapsed) {
			m_animator.SetBool ("CollapsedWindowShow", false);
			m_collapsed = true;
		} else {
			m_animator.SetBool ("TutorialShow", false);
			m_animator.SetBool ("TutorialHide", true);
			StartCoroutine (ResumeGame ());
		}
	}

	//Una vez acabada la animación de cierre del tutorial se destruye el tutorial
	public void  OnTutorialAnimationHide() {
		destroyWindow ();
	}

	//Una vez acabada la animación de cierre del aviso de tutorial (Collapsed Window) se destruye el tutorial
	public void  OnCollapsedAnimationHide() {
		if (toBeDestroyed) {
			if (destroyOnEnd) {
				destroyWindow ();
			} else {
				setInactiveWindow();
				Initialize ();
			}
		}
	}



	public IEnumerator ResumeGame() {
		yield return new WaitForSecondsRealtime (0.5f);
		PauseGameController.Resume();
		yield return null;
	}

	public override void Open() {
		base.Open ();
		if (m_collapsed) {
			m_animator.SetBool ("CollapsedWindowShow", true);
			m_animator.SetBool ("TutorialShow", false);
		} else {
			m_animator.SetBool ("TutorialShow", true);
			m_animator.SetBool ("CollapsedWindowShow", false);
			StartCoroutine (PlayVideo());
			PauseGameController.Pause ();
		}
	}

	IEnumerator PlayVideo () {
		m_videoPlayer.playOnAwake = false;
		m_audioSource.playOnAwake = false;
		m_audioSource.Pause ();

		m_videoPlayer.source = VideoSource.VideoClip;
		m_videoPlayer.isLooping = true;

		m_videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
		m_videoPlayer.EnableAudioTrack (0, true);
		m_videoPlayer.SetTargetAudioSource (0, m_audioSource);

		m_videoPlayer.clip = m_pages [m_currentPage].videos [0].video;
		m_videoPlayer.prepareCompleted += PreparedVideo;
		m_videoPlayer.errorReceived += VideoError;
		m_videoPlayer.Prepare ();
		while (!m_videoPlayer.isPrepared) {
			yield return null;
		}
	

		m_rawImage.texture = m_videoPlayer.texture;
		m_videoPlayer.Play ();
		m_audioSource.Play ();
		//m_videoPlayer.enabled = false;
		//m_videoPlayer.enabled = true;
	}

	void PreparedVideo(VideoPlayer vp) {
		preparedVideo = true;
	}

	void VideoError(VideoPlayer vp, string errorString) {
		Debug.LogError ("El siguiente error se ha dado con el video: " + errorString);
	}

	 
	public override void ManageLifeTime() {
		if (m_collapsed){
			m_currentLifeTime -= Time.deltaTime;
			m_closeTimeText.text = "(" + (Mathf.CeilToInt(m_currentLifeTime)) + " s)";
			if (m_currentLifeTime <= 0) {
				Close ();
				toBeDestroyed = true;
				m_collapsed = false;
			}
		}
	}
		
	public void AddPage(string aHeader, string aSubheader, string aText, List<Tutorial.IMAGE_CONTROLLER> aImageControllerList,  List<Tutorial.VIDEO_PAGE> aVideoPageList ) {
		int l_index = m_pages.Count;
		Page l_newPage = new Page (aHeader, aSubheader, aText, l_index, aImageControllerList, aVideoPageList);
		m_pages.Add (l_newPage);
		UpdateCurrentPage();
	}

	public void AddImage(Sprite aImage) {
		
		GameObject l_newGameObject = new GameObject("ControllerImage");
		l_newGameObject.transform.SetParent(m_imageGroup.transform);
		l_newGameObject.AddComponent<CanvasRenderer>();

		RectTransform l_rectTransform = l_newGameObject.AddComponent<RectTransform>() as RectTransform;
		Image l_newImage = l_newGameObject.AddComponent<Image>() as Image;
		l_rectTransform.localScale = new Vector3(1, 1, 1);
		l_newImage.sprite = aImage;
		l_newImage.preserveAspect = true;
	}
	
	public override void LoadPage(int aPageIndex) { //TODO: Video
		Page l_page = m_pages [aPageIndex];
		m_header.text = l_page.m_header;
		m_subheader.text = l_page.m_subheader;
		m_text.text = l_page.m_text;
		m_currentPage = aPageIndex;

		for (int i = 0; i < m_imageGroup.transform.childCount; i++) {
			Destroy(m_imageGroup.transform.GetChild(i).gameObject);
		}

		for (int i = 0; i < l_page.images.Count; i++) {
			AddImage(l_page.images[i].image);
		}
	}

	public override void ManageWindowControls () {
		base.ManageWindowControls ();
		if (blockInputTimeSeconds <= 0.0f) {
			for (int i = 0; i < closeKey.Length; i++) {
				if (Input.GetKeyDown (closeKey [i]) || XCI.GetButtonDown (XboxButton.Back, controller)) {

					if (!m_collapsed) {
						Close ();
					}
				}
			}
			for (int i = 0; i < showWindowKey.Length; i++) {
				if (Input.GetKeyDown (showWindowKey [i]) || XCI.GetButtonDown (XboxButton.Back, controller)) {
					if (m_collapsed) {
						m_collapsed = false;
						Open ();
					} 
				}
			}
			for (int i = 0; i < nextPage.Length; i++) {
				if (Input.GetKeyDown (nextPage [i]) /*|| XCI.GetButtonDown(XboxButton.RightBumper,m_Controller)*/) {
					int l_newCurrentPage = m_currentPage + 1;
					if (l_newCurrentPage < m_pages.Count) {
						LoadPage (l_newCurrentPage);
						UpdateCurrentPage ();
					}
				}
			}
			for (int i = 0; i < previousPage.Length; i++) {
				if (Input.GetKeyDown (previousPage [i]) /*|| XCI.GetButtonDown(XboxButton.LeftBumper, m_Controller)*/) {
					int l_newCurrentPage = m_currentPage - 1;
					if (l_newCurrentPage >= 0) {
						LoadPage (l_newCurrentPage);
						UpdateCurrentPage ();
					}
				}
			}
		}
	}
}
