using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Dialog : Window {
	[Header("Referencias")]
	public Text m_character;
	public Image m_portrait;

	[Header("Configuracion")]
	public bool pauseGame;
    public bool closeOnFinish = false;


	public override void Start() {
		base.Start ();
	}

	public override void Open() {
		if (pauseGame) {
			PauseGameController.Pause ();
		}
		gameObject.SetActive (true);
        if (m_animator)
        {
            m_animator.SetBool("Show", true);
        }
		base.Open ();
	}

	public override void Close() {
		if (pauseGame) {
			PauseGameController.Resume ();
		}
        if (m_animator)
        {
            m_animator.SetBool("Show", false);
        }
		base.Close ();
	}

	//Una vez acabada la animación de cierre del dialogo se destruye el dialogo
	public void  OnDialogAnimationHide() {
		if (destroyOnEnd) {
			destroyWindow ();
		} else {
			setInactiveWindow();
			Initialize ();
		}
	}

	public override void Show() {
		
	}

	public override void Hide() {

	}

	public override void ManageWindowControls() {
		base.ManageWindowControls ();
		if (blockInputTimeSeconds <= 0.0f) {
			for (int i = 0; i < nextPage.Length; i++) {
				if (Input.GetKeyDown (nextPage [i])) {
					int l_newCurrentPage = m_currentPage + 1;
					if (l_newCurrentPage < m_pages.Count) {
						LoadPage (l_newCurrentPage);
					} else {
                        if (closeOnFinish) {
                            Close();
                        }
					}
				}
			}
			for (int i = 0; i < previousPage.Length; i++) {
				if (Input.GetKeyDown (previousPage [i])) {
					int l_newCurrentPage = m_currentPage - 1;
					if (l_newCurrentPage >= 0) {
						LoadPage (l_newCurrentPage);
					}
				}
			}
		}
	}

	public override void LoadPage(int aPageIndex) {
		Page l_page = m_pages [aPageIndex];
		if (m_character != null) {
			m_character.text = l_page.m_character;
		}
		m_text.text = l_page.m_text;
		m_currentPage = aPageIndex;
		if (l_page.images != null && l_page.images.Count > 0) {
			m_portrait.sprite = l_page.images [0].image;
		}
	}


	public void AddPage(string aCharacter, string aText, List<Tutorial.IMAGE_CONTROLLER> aImageControllerList) {
		int l_index = m_pages.Count;
		Page l_newPage = new Page (aCharacter, aText, l_index, aImageControllerList);
		m_pages.Add (l_newPage);
	}
}
