using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MSMS_Book1_Dying : MonoStateMachineState, DieReceiver {
	private Animator l_Anim;
	private NavMeshAgent l_Agent;

	[Header("Vaporize attributes")]
	public bool vaporize = true;
	public Shader vaporizeShader;
	public float vaporizeSpeed = 0.3f;
	public Texture2D noiseTexture;
	[Range(0.0f, 0.1f)]
	public float burntThreshold = 0.0242f;
	public Color burntStartColor = Color.white;
	public Color burntEndColor = Color.black;
	private Material m_Material;
	private bool m_Vaporizing;
	private float m_Threshold;

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
	}

	public override void OnExit() {
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

	public override void OnStart() {
		l_Anim = GetComponentInChildren<Animator> ();
		l_Agent = GetComponent<NavMeshAgent> ();
		m_Vaporizing = false;
		m_Material = GetComponentInChildren<Renderer> ().material;
	}

	#region DieReceiver implementation
	public void Die ()
	{
		m_Parent.ChangeState (stateName);
	}
	#endregion
}
