using UnityEngine;
using System.Collections;

[AddComponentMenu("PRIM3/CharacterScripts/Actions/Pushable")] 
[RequireComponent(typeof(MovementManager))]
public class Pushable : ActionBehaviour, IPushable {
	public bool keepFacing = true;
    public Animator AnimatorController;
	public bool beingPushed;
	float m_Speed;
	Vector3 m_Direction;
    private Health m_Health;

	// Use this for initialization
	void Start () {
		beingPushed = false;
        m_Health = GetComponent<Health>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/* Obtiene los estados o valores del Input y los asigna a las diferentes acciones */
	override public void GetInput() {

	}

	override public void ActionUpdate() {
		if (isActionActive ()) {
			Vector3 l_Pos = transform.position;
			l_Pos -= m_Direction * m_Speed * Time.deltaTime;
			transform.position = l_Pos;
		}
	}

	#region IPushable implementation
	public void Push (float aSpeed, float aTime, Vector3 aSource, bool aAnimationChange)
	{
        if (m_Health.health > 0)
        {
            Quaternion l_OldDir = transform.rotation;
            beingPushed = true;
            m_Speed = aSpeed;
            aSource.y = transform.position.y;
            transform.LookAt(aSource);
            m_Direction = transform.forward;
            if (keepFacing)
            {
                transform.rotation = l_OldDir;
            }
            if (AnimatorController && aAnimationChange == true)
            {   
                AnimatorController.SetBool("Stun", true);
            }
            CancelInvoke("NoPush");
            Invoke("NoPush", aTime);
        }
	}
	#endregion


	/*public void Push(float spd, float t, Vector3 source) {

	}*/

	public void NoPush() {
        if (AnimatorController)
        {
            AnimatorController.SetBool("Stun", false);
        }
		beingPushed = false;
	}

	override public bool isActionActive() {
		return beingPushed;
	}
}
