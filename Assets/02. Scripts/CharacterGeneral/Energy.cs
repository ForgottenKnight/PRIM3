using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("PRIM3/CharacterScripts/Properties/Energy")] 
public class Energy : MonoBehaviour {
	public float maxEnergy = 100.0f;
	public float energyRecoveredPerSecond = 5.0f;
	public float delay = 3.0f;
	
	public float energy;
	public float reservedEnergy;
	private float timer;

	[Header("UI")]
	public Image energyBar;
	public Image reservedEnergyBar;

	// Use this for initialization
	void Awake () {
		energy = maxEnergy;
		reservedEnergy = 0.0f;
		timer = 0.0f;
	}
	
	// Update is called once per frame
	void Update () {
		ManageEnergy ();
		if (energyBar) {
			ManageEnergyUI ();
		}
	}

	private void ManageEnergy() {
		if (timer < delay) {
			timer += Time.deltaTime;
		} else {
			RecoverEnergy (energyRecoveredPerSecond * Time.deltaTime);
		}
	}
	
	private void ManageEnergyUI() {
		if (energyBar) {
			energyBar.fillAmount = GetEnergyAsUnit() - GetReservedEnergyAsUnit();
			reservedEnergyBar.fillAmount = GetEnergyAsUnit();
		}
	}


	public bool ConsumeEnergy(float cost) {
		bool lOk = false;
		if (energy - reservedEnergy >= cost && cost >= 0.0f) {
			energy -= cost;
			lOk = true;
			if(cost != 0)
				timer = 0.0f;
		}
		return lOk;
	}

	public void ConsumeAllEnergy() {
		energy = 0.0f;
		timer = 0.0f;
	}

	public void RecoverEnergy(float recovery) {
		if (recovery > 0.0f) {
			energy += recovery;
			if (energy > maxEnergy) {
				energy = maxEnergy;
			}
		}
	}

	public void RecoverAllEnergy() {
		energy = maxEnergy;
	}

	public bool ReserveEnergy(float reservedEnergy) {
		float newReservedEnergy = this.reservedEnergy + reservedEnergy;
		if (newReservedEnergy <= energy && reservedEnergy > 0) {
			timer = 0.0f;
			this.reservedEnergy = newReservedEnergy;
			return true;
		}
		return false;
	}

	public bool ConsumeReservedEnergy(float consumedEnergy) {
		if (consumedEnergy <= reservedEnergy) {
			reservedEnergy -= consumedEnergy;
			energy -= consumedEnergy;
			return true;
		}
		return false;
	}

	public bool ReleaseReservedEnergy(float reservedEnergy) {
		float newReservedEnergy = this.reservedEnergy - reservedEnergy;
		if (newReservedEnergy >= 0) {
			this.reservedEnergy -= reservedEnergy;
			return true;
		}
		return false;
	}

	public void ReleaseAllReservedEnergy() {
		this.reservedEnergy = 0;
	}



	public void ConsumeAllReservedEnergy () {
		energy -= reservedEnergy;
		reservedEnergy = 0.0f;
	}


	public float GetEnergy() {
		return energy;
	}

	public void SetEnergy(float e) {
		energy = e;
	}

	public float GetReservedEnergy() {
		return this.reservedEnergy;
	}
	
	public void SetReservedEnergy(float e) {
		this.reservedEnergy = e;
	}

	public float GetEnergyAsUnit() {
		return (energy / maxEnergy);
	}
	public float GetEnergyAsPercent() {
		return GetEnergyAsUnit () * 100.0f;
	}

	public float GetReservedEnergyAsUnit() {
		return (reservedEnergy / maxEnergy);
	}
	public float GetReservedEnergyAsPercent() {
		return GetReservedEnergyAsUnit () * 100.0f;
	}
}
