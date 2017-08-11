using UnityEngine;
using UnityEngine.Events;
using System.Collections;


public class Damageable : MonoBehaviour {
	public delegate void SimpleDelegate ();
	UnityEvent onDestroyEvents;
	UnityEvent onDamageEvents;
	UnityEvent onDieEvents;

	virtual public float TrueDamage(float damage) {
		Debug.LogWarning ("Damageable::TrueDamage error!!!"); // No se deberia entrar a esta funcion virtual
		return 0.0f;
	}

	virtual public float ConditionalDamage(float damage, bool defense, bool treshold, bool invencible) {
		Debug.LogWarning ("Damageable::ConditionalDamage error!!!"); // No se deberia entrar a esta funcion virtual
		return 0.0f;
	}

	virtual public float Damage(float damage) {
		Debug.LogWarning ("Damageable::Damage error!!!"); // No se deberia entrar a esta funcion virtual
		return 0.0f;
	}

	virtual public void RemoveHealth() {
		Debug.LogWarning ("Damageable::RemoveHealth error!!!"); // No se deberia entrar a esta funcion virtual
	}

	public void RegisterForEvent(SimpleDelegate method) {
		if (onDestroyEvents == null) {
			onDestroyEvents = new UnityEvent();
		}
		onDestroyEvents.AddListener (new UnityAction(method));
	}

	public UnityAction RegisterOnDamage(SimpleDelegate method) {
		if (onDamageEvents == null) {
			onDamageEvents = new UnityEvent();
		}
		UnityAction ua = new UnityAction (method);
		onDamageEvents.AddListener (ua);
		return ua;
	}

	public void RegisterOnDie(SimpleDelegate method) {
		if (onDieEvents == null) {
			onDieEvents = new UnityEvent();
		}
		onDieEvents.AddListener (new UnityAction (method));
	}

	public void UnRegisterOnDamage(UnityAction action) {
		if (onDamageEvents != null) {
			onDamageEvents.RemoveListener(action);
		}
	}

	protected void OnDamage() {
		if (onDamageEvents != null) {
			onDamageEvents.Invoke();
		}
	}

	public void OnDie() {
		if (onDieEvents != null) {
			onDieEvents.Invoke();
		}
	}

	void OnDestroy() {
		if (onDestroyEvents != null) {
			onDestroyEvents.Invoke ();
		}
	}
}
