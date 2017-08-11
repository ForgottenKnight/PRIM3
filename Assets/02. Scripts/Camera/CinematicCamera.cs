using UnityEngine;
using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;

public class CinematicCamera : MonoBehaviour {
	public Animator anim;
	public PrimeCamera primeCamera;
	string currentState;

	private bool m_AcceptInput;

	private bool m_FollowStopped;

    private Canvas[] m_canvas;

	// Use this for initialization
	void Start () {
		if (primeCamera == null) {
			primeCamera = Camera.main.GetComponent<PrimeCamera>();
		}
		if (anim == null) {
			anim = GetComponent<Animator>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (m_AcceptInput == true && Input.GetAxis("Cancel") > 0.0f) {
			FinishAnimation();
		}
	}

	public void PlayAnimation(string state)	{
		if (anim) {
			Input.ResetInputAxes();
			gameObject.GetComponent<Camera> ().enabled = true;
			m_FollowStopped = primeCamera.followStoppedPlayers;
			primeCamera.followStoppedPlayers = true;
			StopEnemies();
			StopPlayers();
			anim.speed = 0.2f;
			currentState = state;
			CameraFade cf = gameObject.AddComponent<CameraFade>();
			cf.AddCallback(UnFade1);
			cf.SetScreenOverlayColor(new Color(0.0f, 0.0f, 0.0f, 0.0f));
			cf.StartFade(Color.black, 2.0f);
            StopCanvas();
		}
	}

	public void UnFade1() {
		CameraFade cf = gameObject.AddComponent<CameraFade>();
		cf.AddCallback(Play);
		cf.SetScreenOverlayColor(Color.black);
		cf.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), 2.0f);
		primeCamera.GetComponent<Camera> ().enabled = false;
		anim.Play (currentState);
		anim.speed = 0.0f;
	}

	public void Play() {
		m_AcceptInput = true;
		//anim.Play(currentState);
		anim.speed = 0.2f;
	}
	
	public void FinishAnimation() {
		if (currentState != "") {
			m_AcceptInput = false;
			CameraFade cf = gameObject.AddComponent<CameraFade> ();
			cf.AddCallback (UnFade2);
			cf.SetScreenOverlayColor (new Color (0.0f, 0.0f, 0.0f, 0.0f));
			cf.StartFade (Color.black, 2.0f);
		}
	}

	public void UnFade2() {
		anim.speed = 50.0f;
		currentState = "";
		CameraFade cf = gameObject.AddComponent<CameraFade>();
		cf.AddCallback(ResetCamera);
		cf.SetScreenOverlayColor(Color.black);
		cf.StartFade(new Color(0.0f, 0.0f, 0.0f, 0.0f), 2.0f);

		primeCamera.GetComponent<Camera> ().enabled = true;
	}

	public void ResetCamera() {
		StartPlayers ();
		primeCamera.followStoppedPlayers = m_FollowStopped;
		StartEnemies ();
		//gameObject.SetActive (false);
        StartCanvas();
		gameObject.GetComponent<Camera> ().enabled = false;
	}

	public void StopEnemies() {
		List<GameObject> enemies = CustomTagManager.GetObjectsByTag("Enemy");
		foreach (GameObject enemy in enemies) {
			IPausable pausable = enemy.GetComponent<IPausable> ();
			if (pausable != null) {
				pausable.Pause ();
			}
		}
	}

    public void StopCanvas()
    {
        m_canvas = FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in m_canvas)
        {
            if (canvas && canvas.gameObject.tag != "ContainsPause")
            {
                canvas.enabled = false;
            }
        }
    }

    public void StartCanvas()
    {
        foreach (Canvas canvas in m_canvas)
        {

            if (canvas)
            {
                canvas.enabled = true;
                if (canvas.transform.parent)
                {
                    Health l_H = canvas.transform.parent.GetComponent<Health>();

                    if (l_H && l_H.health >= l_H.maxHealth)
                    {
                        canvas.enabled = false;
                    }
                }
            }
        }
    }

	public void StartEnemies() {
		List<GameObject> enemies = CustomTagManager.GetObjectsByTag("Enemy");
		foreach (GameObject enemy in enemies) {
			IPausable pausable = enemy.GetComponent<IPausable> ();
			if (pausable != null) {
				pausable.Unpause ();
			}
		}
	}

	public void StopPlayers() {
		List<GameObject> players = CustomTagManager.GetObjectsByTag ("Player");
		foreach (GameObject player in players) {
            if (player.name == "Salt")
            {
                SaltController l_SC = player.GetComponent<SaltController>();
                if(l_SC && l_SC.isActionActive())
                {
                    l_SC.DeactivateShield();
                }
            }
			IPausable[] l_Pausables = player.GetComponents<IPausable> ();
			for (int j = 0; j < l_Pausables.Length; ++j) {
				l_Pausables [j].Pause ();
			}
            player.GetComponent<Health>().invincible = true;
		}
	}

	public void StartPlayers() {
		List<GameObject> players = CustomTagManager.GetObjectsByTag ("Player");
		foreach (GameObject player in players) {
			IPausable[] l_Pausables = player.GetComponents<IPausable> ();
			for (int j = 0; j < l_Pausables.Length; ++j) {
				l_Pausables [j].Unpause ();
			}
            player.GetComponent<Health>().invincible = false;
		}
	}

	public void CallEvent(string ev) {
		if (SimpleEvent.eventsDictionary.ContainsKey (ev)) {
			SimpleEvent.eventsDictionary [ev].ExternalTriggerFunction ();
		}
	}

	public void ActivateEventTimer(string ev) {
		if (SimpleEvent.eventsDictionary.ContainsKey (ev)) {
			SimpleEvent.eventsDictionary [ev].ActivateTimer ();
		}
	}
}
