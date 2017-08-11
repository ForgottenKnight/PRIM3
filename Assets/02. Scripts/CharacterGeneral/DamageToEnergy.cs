using UnityEngine;
using System.Collections;

public class DamageToEnergy : Damageable {
	[HideInInspector]
	public Energy source;
	[HideInInspector]
	public float energyPerDamageRatio;

	override public float Damage(float damage) {
		Energy e = source;
		if (!e.ConsumeEnergy (energyPerDamageRatio * damage)) {
			e.ConsumeAllEnergy();
		}
		return damage;
	}
}
