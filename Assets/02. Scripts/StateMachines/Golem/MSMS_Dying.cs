using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class MSMS_Dying : MonoStateMachineState, DieReceiver {
	[Header("Die attributes")]
	public string dieLayer = "Event";
	[Header("Vaporize attributes")]
	public bool vaporize = true;
	public Shader vaporizeShader;
	public float vaporizeSpeed = 0.3f;
	public Texture2D noiseTexture;
	[Range(0.0f, 0.1f)]
	public float burntThreshold = 0.0242f;
	public Color burntStartColor = Color.white;
	public Color burntEndColor = Color.black;
	public bool destroyColliders = true;
	private Material m_Material;
	private bool m_Vaporizing;
	private float m_Threshold;

	private Animator l_Anim;
	private UnityEngine.AI.NavMeshAgent l_Agent;

	public override void StateUpdate() {
		if (l_Anim.GetCurrentAnimatorStateInfo (0).normalizedTime >= 1f && l_Anim.GetCurrentAnimatorStateInfo (0).IsName ("Death") && !m_Vaporizing) {
			if (vaporize == true) {
				BeginVaporize ();
			} else {
				Destroy (gameObject);
			}
		} else if (m_Vaporizing == true) {
			m_Material.SetFloat ("_Threshold", m_Threshold);
			m_Threshold -= vaporizeSpeed * Time.deltaTime;
			if (m_Threshold <= 0f) {
				Destroy (gameObject);
			}
		}
	}
	
	public override void OnEnter() {
		l_Anim.SetTrigger ("die");
		l_Agent.enabled = false;
		gameObject.layer = LayerMask.NameToLayer (dieLayer);
        Target l_Target = GetComponent<Target>(); 
        if (destroyColliders == true)
        {
            DestroyColliders();
        }
		if (l_Target != null) {
			Destroy (l_Target);
		}
	}
	
	public override void OnExit() {
	}
	
	public override void OnStart() {
		l_Anim = GetComponentInChildren<Animator> ();
		l_Agent = GetComponent<UnityEngine.AI.NavMeshAgent> ();
		m_Vaporizing = false;
        m_Material = GetComponentInChildren<Renderer>().material;

	}

	void BeginVaporize() {
		Renderer ren = GetComponentInChildren<Renderer> ();
		m_Material.shader = vaporizeShader;
		m_Material.SetColor ("_BurntColorBegin", burntStartColor);
		m_Material.SetColor ("_BurntColorEnd", burntEndColor);
		m_Material.SetFloat ("_BurntThreshold", burntThreshold);
		m_Material.SetTexture ("_Noise", noiseTexture);
		m_Threshold = 1f;
		m_Vaporizing = true;
	}

	void DestroyColliders() {
		Collider[] l_Colliders = GetComponentsInChildren<Collider> ();
		for (int i = 0; i < l_Colliders.Length; ++i) {
			Destroy (l_Colliders [i]);
		}
		CharacterController l_CC = GetComponent<CharacterController> ();
		if (l_CC != null) {
			ApplyGravity l_Gravity = GetComponent<ApplyGravity> ();
			if (l_Gravity != null) {
				Destroy (l_Gravity);
			}
			Destroy (l_CC);
		}
		NavMeshAgent l_Agent = GetComponent<NavMeshAgent> ();
		if (l_Agent != null && l_Agent.enabled == true) {
			DebuffController l_Debuff = GetComponent<DebuffController> ();
			if (l_Debuff != null) {
				Destroy (l_Debuff);
			}
			l_Agent.isStopped = true;
			Destroy (l_Agent);
		}
	}
	
	#region DieReceiver implementation
	public void Die ()
	{
		m_Parent.ChangeState (stateName);
	}
	#endregion
}
