using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

public class HealthBar : AboveBar {
	private Image damageBar;
	private Image healthBar;
	private Image alphaBar;
	private UnityAction m_TempAction;

	public bool showText = false;


	protected override void Start () {
		base.Start ();
		healthBar = bar.gameObject.transform.Find("Rotated/HealthBar").GetComponent<Image>();
		damageBar = bar.gameObject.transform.Find("Rotated/DamageBar").GetComponent<Image>();
		alphaBar = bar.gameObject.transform.Find("Rotated/Alpha").GetComponent<Image>();
		if (!showText) {
			bar.gameObject.transform.Find("Rotated/Text").gameObject.SetActive(false);
			bar.gameObject.transform.Find("Rotated/Text Shadow").gameObject.SetActive(false);
		}
		//healthBar  = bar.gameObject.GetComponentInChildren<Image>().find("Child_name");

		Health h = GetComponent<Health> ();
		if (h) {
			h.SetHealthBar(healthBar, damageBar,alphaBar);
			m_TempAction = h.RegisterOnDamage(ActivateBar);
			h.RegisterOnDie(DeactivateBar);
		}
		bar.enabled = false;
	}

	public void ActivateBar() {
		bar.enabled = true;
		Health h = GetComponent<Health> ();
		if (h) {
			h.UnRegisterOnDamage(m_TempAction);
		}
	}

	public void DeactivateBar() {
		bar.enabled = false;
	}
}
