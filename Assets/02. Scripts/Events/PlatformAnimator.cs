using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class PlatformAnimator : MonoBehaviour {
	[Header("Configuration")]
	public float WaitToRunningDelay;
	public float RunningToEndDuration;
	public string groupName;
	public ActivationMode activationMode;
	public AnimationMode animationMode;
	public float lifeTimeOnEnd;
	public bool destroyGameobjectOnEnd;
	public bool destroyScriptOnEnd;
	public bool returnInitialPosition;

	private bool test = true;
	public GameObject particlesPrefab;
	public GameObject[] particlesPosition;

	/*
    private bool calledEvent = false;
    public UnityEvent eventToCall;*/
    

	public enum ActivationMode
	{
		OnContact,
		OnPlatformActivated,
		OnTimer
	}

	public enum AnimationMode
	{
		Gravity,
		Animated
	}



	[Header("Activation Settings")]
	public OnContactActivator onContact;
	public onPlatformActivatedActivator onPlatformActivated;
	public onTimerActivator onTimer;

	[System.Serializable]
	public struct OnContactActivator {
		public int playersToFall;
		[HideInInspector]
		public int currentNumberOfPlayers;
		[HideInInspector]
		public bool active;
		[HideInInspector]
		public bool contactOn;
	}
	
	[System.Serializable]
	public struct onPlatformActivatedActivator { 

		public List<PlatformAnimator> dependsOn;
		public bool checkContact;
		public bool requireAll;
		[HideInInspector]
		public bool active;
	}
	
	[System.Serializable]
	public struct onTimerActivator {
		public float timer;
		[HideInInspector]
		public float currentTimer;
		[HideInInspector]
		public bool active;

		public void Start() {
			currentTimer = timer;
			active = true;
		}
		public void Stop() {
			currentTimer = timer;
			active = false;
		}
		public void Pause() {
			active = false;
		}
		public void Continue() {
			active = true;
		}

	}


	[Header("Animation Settings")]
	public AnimationState state;
	public GravitySettings gravity;
	public AnimationSettings animationSettings;


	[System.Serializable]
	public struct GravitySettings {
		public float lifeTime;
		public bool stopOnCollision;
	}

	[System.Serializable]
	public struct AnimationSettings {
		[Range(0.0f, 1.0f)]
		public float minPercent;
		[Range(0.0f, 1.0f)]
		public float maxPercent;
		[HideInInspector]
		public bool isFirstFrame;
		public float runningSpeed;
		public float endedSpeed;
		[HideInInspector]
		public Animator anim;
		public bool waitAnimationsToEnd;
		[HideInInspector]
		public bool animationWaitingActivated;
		[HideInInspector]
		public bool animationWaitingEnded;
		[HideInInspector]
		public bool animationRunningActivated;
		[HideInInspector]
		public bool animationRunningEnded;
		[HideInInspector]
		public bool animationEndedActivated;
		[HideInInspector]
		public bool animationEndedEnded;
		[HideInInspector]
		public bool runningForward;
		[HideInInspector]
		public float currentTime;
		[HideInInspector]
		public bool firstAnimationFrame;
	
	}

	public enum AnimationState {
		Waiting,
		Running,
		Ended
	}


	[Header("Referencias")]
	public PlatformAnimatorGroup platformAnimatorGroup;
	private Rigidbody platformRigidBody;
	private AudioSource platformAudioSource;

	void Awake () {
	}

	void OnDestroy() {
		if (platformAudioSource != null) {
			platformAudioSource.Stop ();
		}
	}

	// Use this for initialization
	void Start () {
		onContact.active = false;
		onContact.contactOn = false;

		onPlatformActivated.active = false;
		onTimer.active = false;
		onTimer.currentTimer = onTimer.timer;

	
		if (activationMode == ActivationMode.OnContact) {
			onContact.active = true;
		} else if (activationMode == ActivationMode.OnPlatformActivated) {
			onPlatformActivated.active = true;
		} else if (activationMode == ActivationMode.OnTimer) {
			onTimer.active = true;
		}

		state = AnimationState.Waiting;
        animationSettings.anim = GetComponent<Animator>();
		animationSettings.anim.SetFloat ("RunningSpeed", animationSettings.runningSpeed);
		animationSettings.anim.SetFloat ("EndedSpeed", animationSettings.endedSpeed);
		animationSettings.runningForward = false;
		animationSettings.firstAnimationFrame = true;

		animationSettings.isFirstFrame = true;
		if (animationSettings.runningSpeed <= 0.0f) {
			animationSettings.runningSpeed = 1.0f;
		}
		if (animationSettings.endedSpeed <= 0.0f) {
			animationSettings.endedSpeed = 1.0f;
		}

		platformRigidBody = transform.GetChild(0).GetComponent<Rigidbody> ();
		platformAudioSource = GetComponent<AudioSource> ();

	}
	// Update is called once per frame
	void Update () {
		if (onContact.active) {
			ManageOnContact();
		} else if (onPlatformActivated.active) {
			ManageOnPlatformActivated();
		} else if (onTimer.active) {
			ManageOnTimer();
		}

		if (state == AnimationState.Waiting) {
			if (animationMode == AnimationMode.Animated) {
				AnimatePlatformAnimated("Waiting");
			} 
		} else if (state == AnimationState.Running) {
			if (animationMode == AnimationMode.Gravity) {
				AnimatePlatformGravity ();
			} else if (animationMode == AnimationMode.Animated) {
				AnimatePlatformAnimated ("Running");
			}
		} else if (state == AnimationState.Ended) {
			if (animationMode == AnimationMode.Gravity) {
				AnimatePlatformGravityStop ();
			} else if (animationMode == AnimationMode.Animated) {
				AnimatePlatformAnimated("Ended");
			}
			ManageLifeTime();
		}
	}
	public void playAnimation () {
		state = AnimationState.Running;
	}
	public void stopAnimation () {
		state = AnimationState.Ended;
	}


	//EVENTS
	public void OnPlayerEnter() {
		onContact.currentNumberOfPlayers++;
        //CallEvent();
		if (onContact.currentNumberOfPlayers >= onContact.playersToFall) {
			onContact.contactOn = true;
		}
	}
	/*
    private void CallEvent()
    {
        if (calledEvent != true)
        {
            eventToCall.Invoke();
            calledEvent = true;
        }
    }*/

	public void OnPlayerExit() {
		onContact.currentNumberOfPlayers--;
		if (onContact.currentNumberOfPlayers < onContact.playersToFall) {
			onContact.contactOn = false;
		}
	}
		
	public void OnRunningAnimationEnd() {
		animationSettings.animationRunningEnded = true;
	}

	public void OnEndedAnimationEnd() {
		animationSettings.animationEndedEnded = true;
	}

	

	private void ManageLifeTime() {
		if (lifeTimeOnEnd > 0) {
			lifeTimeOnEnd -= Time.deltaTime;
		} else {
			if (destroyGameobjectOnEnd) {
				Destroy (gameObject);
			} else if (destroyScriptOnEnd) {
				Destroy (this);
			}
		}
	}

	private void ManageOnContact() {
		if (state == AnimationState.Waiting && onContact.contactOn) {
			if (WaitToRunningDelay <= 0) {
				state = AnimationState.Running;
			} else {
				WaitToRunningDelay -= Time.deltaTime;
			}
		} else if (state == AnimationState.Running) {
			if (!returnInitialPosition) {
				if (animationSettings.waitAnimationsToEnd) {
					if (animationSettings.animationRunningEnded) {
						if (RunningToEndDuration <= 0) {
							state = AnimationState.Ended;
						} else {
							RunningToEndDuration -= Time.deltaTime;
						}
					}
				} else {
					if (RunningToEndDuration <= 0) {
						state = AnimationState.Ended;
					} else {
						RunningToEndDuration -= Time.deltaTime;
					}
				}
			}
		}
	}
	
	private void ManageOnPlatformActivated() {
		if (state == AnimationState.Waiting) {
			bool l_activate = AreDependenciesActive ();

			if (l_activate) {
				if (WaitToRunningDelay <= 0) {
					state = AnimationState.Running;
				} else {
					WaitToRunningDelay -= Time.deltaTime;
				}
			}
		}  else if (state == AnimationState.Running) {
			if (!returnInitialPosition) {
				if (animationSettings.waitAnimationsToEnd) {
					if (animationSettings.animationRunningEnded) {
						if (RunningToEndDuration <= 0) {
							state = AnimationState.Ended;
						} else {
							RunningToEndDuration -= Time.deltaTime;
						}
					}
				} else {
					if (RunningToEndDuration <= 0) {
						state = AnimationState.Ended;
					} else {
						RunningToEndDuration -= Time.deltaTime;
					}
				}
			}

		}
	}


	private void ManageOnTimer() {
		//Comprobaciones para activarse
		if (state == AnimationState.Waiting) {
			if (onTimer.currentTimer <= 0) {
				if (WaitToRunningDelay <= 0) {
					state = AnimationState.Running;
				} else {
					WaitToRunningDelay -= Time.deltaTime;
				}
			} else if (onTimer.currentTimer > 0) {
				onTimer.currentTimer -= Time.deltaTime;
			}

		} else if (state == AnimationState.Running) {
			if (animationSettings.waitAnimationsToEnd) {
				if (animationSettings.animationRunningEnded) {
					if (RunningToEndDuration <= 0) {
						state = AnimationState.Ended;
					} else {
						RunningToEndDuration -= Time.deltaTime;
					}
				}
			} else {
				if (RunningToEndDuration <= 0) {
					state = AnimationState.Ended;
				} else {
					RunningToEndDuration -= Time.deltaTime;
				}
			}
		}

	}


	//Utilidad
	private bool AreDependenciesActive () {
		int l_numberOfPlatforms = onPlatformActivated.dependsOn.Count;
		int l_counter = 0; 
		bool l_activate = false;

		for (int i = 0; i < l_numberOfPlatforms; ++i) {
			if (onPlatformActivated.checkContact) {
				if (onPlatformActivated.dependsOn [i].onContact.contactOn) {
					l_counter++;
				}
			} else {
				if (onPlatformActivated.dependsOn [i].state == AnimationState.Running || onPlatformActivated.dependsOn [i].state == AnimationState.Ended) {
					l_counter++;
				}
			}
		}

		if ((onPlatformActivated.requireAll && l_counter == l_numberOfPlatforms) || (!onPlatformActivated.requireAll && l_counter >= 1)) {
			l_activate = true;
		}

		return l_activate;
	}

	
	private void AnimatePlatformAnimated(string aClip) {
		if ((activationMode == ActivationMode.OnContact || activationMode == ActivationMode.OnPlatformActivated) && returnInitialPosition) {
			if (aClip == "Running") {
				bool l_directionChange = false;

				bool l_contact;
				bool l_runningForward = animationSettings.runningForward;

				//Comprueba si hay alguien encima
				if (activationMode == ActivationMode.OnContact) {
						animationSettings.runningForward = onContact.contactOn;

				} else if (activationMode == ActivationMode.OnPlatformActivated){
						animationSettings.runningForward = AreDependenciesActive ();
				}

				//Si ha habido un cambio de dirección cambia el sentido de la animación y obtiene la posición actual de la animación
				if (l_runningForward != animationSettings.runningForward) {
					if (animationSettings.runningForward) {
						animationSettings.anim.SetFloat ("RunningSpeed", animationSettings.runningSpeed); 
					} else {
						animationSettings.anim.SetFloat ("RunningSpeed", animationSettings.runningSpeed * (-1)); 
					}
					l_directionChange = true;
					if (animationSettings.firstAnimationFrame) {
						animationSettings.currentTime = 0.0f;
						animationSettings.firstAnimationFrame = false;
					} else {
						animationSettings.currentTime = Mathf.Clamp (animationSettings.anim.GetCurrentAnimatorStateInfo (0).normalizedTime, 0.0f, 1.0f);
					}
					animationSettings.anim.Play ("Running", 0, animationSettings.currentTime);
				}	
			}
		} else {
			switch (aClip) {
			case "Waiting":
				if (!animationSettings.animationWaitingActivated) {
					//animationSettings.anim.SetFloat ("Speed", 1.0f);
					animationSettings.anim.Play ("Waiting");
					animationSettings.animationWaitingActivated = true;
				}
				break;
			case "Running":
				if (!animationSettings.animationRunningActivated) {
					//animationSettings.anim.SetFloat ("Speed", animationSettings.runningSpeed);
					animationSettings.anim.Play ("Running");
					animationSettings.animationRunningActivated = true;
				}
				break;
			case "Ended":
				if (!animationSettings.animationEndedActivated) {
					//animationSettings.anim.SetFloat ("Speed", animationSettings.endedSpeed);
					animationSettings.anim.Play ("Ended");
					animationSettings.animationEndedActivated = true;
					for (int i = 0; i < particlesPosition.Length; i++) {
						GameObject l_smoke = Instantiate (particlesPrefab, particlesPosition[i].transform);
					}	
				}
				break;
			}
		}
	}

	private void AnimatePlatformGravity() {
		if (!platformRigidBody.useGravity) {
			platformRigidBody.isKinematic = false;
			platformRigidBody.useGravity = true;
		}
		gravity.lifeTime -= Time.deltaTime;
		if (gravity.lifeTime <= 0) {
			state = AnimationState.Ended;
			AnimatePlatformGravityStop ();
		}
	}

	private void AnimatePlatformGravityStop() {
		if (platformRigidBody.useGravity) {
			platformRigidBody.useGravity = false;
			platformRigidBody.isKinematic = true;
		}
	}

	void OnCollisionEnter (Collision collision) {
		if (animationMode == AnimationMode.Gravity && state == AnimationState.Running && gravity.stopOnCollision) {
			if (collision.collider.tag != "Player") {
				state = AnimationState.Ended;
				AnimatePlatformGravityStop ();
			}
		}
	}
}
