using UnityEngine;
using System.Collections;

public class TreeScript : Damageable {
	Animator animator;

	public bool hasEvent = true;
	public string eventName = "TRUNK";
	public bool activateOnDamage = true;

	// Use this for initialization
	void Start () {
		animator = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	override public float Damage(float damage) {
		Target t = GetComponent<Target> ();
		if (activateOnDamage == true) {
			Activate ();
		}
		if (t)
			Destroy (t);
		if (hasEvent)
			SimpleEvent.eventsDictionary [eventName].ExternalTriggerFunction ();
		return damage;
	}

	public void Activate() {
		animator.SetBool ("damaged", true);
	}
}
