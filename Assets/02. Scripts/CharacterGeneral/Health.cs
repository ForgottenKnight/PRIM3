using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BehaviorDesigner.Runtime;

[SelectionBase]
[AddComponentMenu("PRIM3/CharacterScripts/Properties/Health")] 
public class Health : Damageable {

	[Header("Basic parameters")]
	public float health;
	public float maxHealth = 100.0f;
	public int defense = 0;
	public int damageThreshold = 0;
	public bool invincible = false;
	public bool godmode = false;
	[Header("Damage feedback")]
	public float damagedTime = 0.25f;
	public struct RendererMaterial {
		public Renderer renderer;
		public Material material;
	};
	private List<RendererMaterial> m_RenderersMaterials;
	public Material damagedMaterial;

	public enum UI_Type {
		MainUI,
		GameUI
	}

	[Header("UI")]
	private UI_Type UIType = UI_Type.GameUI;
	public Image healthBar;
	public Image damageBar;
	public Image alphaBar;
	public float damageDelayInterval = 1.0f;
	public float damageApplySpeed = 30.0f;
	private float damageDelayIntervalTimer = 0.0f;
	private bool damagePending = false;
	
	protected Color charColor;
	
	private bool isPlayer = false;	
	private Renderer rendererComponent;

	/*[Header("Sounds")]
	public AudioClip successfulHit;
	public AudioClip unsuccessfulHit;
	AudioSource camAudioSource;*/

	
	[Header("Drops")]
	public DropItem fuegoFauto;


	// Use this for initialization
	void Awake () {
		health = maxHealth;
		//camAudioSource = Camera.main.GetComponent<AudioSource> ();
		if (gameObject.tag == "Player") {
			isPlayer = true;
		}
		/*m_RenderersMaterials = new List<RendererMaterial> ();
		Renderer[] rendererComponents;
		rendererComponents = GetComponentsInChildren<Renderer> ();
		for (int i = 0; i<rendererComponents.Length; ++i) {
			RendererMaterial l_RendererMaterial = new RendererMaterial();
			l_RendererMaterial.renderer = rendererComponents[i];
			l_RendererMaterial.material = rendererComponents[i].material;
			m_RenderersMaterials.Add(l_RendererMaterial);
		}*/
	}

	void Start() {
		m_RenderersMaterials = new List<RendererMaterial> ();
		Renderer[] rendererComponents;
		rendererComponents = GetComponentsInChildren<Renderer> ();
		for (int i = 0; i<rendererComponents.Length; ++i) {
			RendererMaterial l_RendererMaterial = new RendererMaterial();
			l_RendererMaterial.renderer = rendererComponents[i];
			l_RendererMaterial.material = rendererComponents[i].material;
			m_RenderersMaterials.Add(l_RendererMaterial);
		}
	}

	private void DamageFeedback() {
		for (int i = 0; i < m_RenderersMaterials.Count; ++i) {
			m_RenderersMaterials[i].renderer.material = damagedMaterial;
		}
		CancelInvoke ("DamageFeedbackFinish");
		Invoke ("DamageFeedbackFinish", damagedTime);
	}

	private void DamageFeedbackFinish() {
		for (int i = 0; i < m_RenderersMaterials.Count; ++i) {
			m_RenderersMaterials[i].renderer.material = m_RenderersMaterials[i].material;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (healthBar) {
			manageHealthBarUI ();
		}
	}

	override public float TrueDamage(float damage) {
		if (damage > 0.0f && !invincible && !godmode) {
			health -= damage;
			startDamageTimerUI ();
			OnDamage();
			DamageFeedback();

			if (health <= 0.0f){
				health = 0.0f;
				Die ();
			}
		} else {
			damage = 0.0f;
		}
		return damage;
	}


	override public float ConditionalDamage(float aDamage, bool aDefense, bool aTreshold, bool aInvencible) {
		if (godmode)
			return 0.0f;
		if (aInvencible && invincible) {
			return 0.0f;
		} else {
			if (aDamage > 0.0f) {
				if (aTreshold) {
					if (aDamage <= damageThreshold) { 
						return 0.0f;
					}
				}

				if (aDefense) {
					aDamage -= defense;
				}

				health -= aDamage;
				startDamageTimerUI ();
				OnDamage ();
				DamageFeedback ();

				if (health <= 0.0f) {
					health = 0.0f;
					Die ();
				}
				return aDamage;
			} else {
				return 0.0f;
			}
		}
	}

	override public float Damage(float damage) {
		if (damage >= damageThreshold && damage > 0.0f && damage > defense && !invincible && !godmode) {
			damage -= defense;
			if (damage <= 0.0f) {
				damage = 0.0f; // No se deberia alcanzar este punto
			} else {
				health -= damage;
				startDamageTimerUI ();
				OnDamage();
			}
			DamageFeedback();
			//if(successfulHit)
				//camAudioSource.PlayOneShot(successfulHit, 1);
			if (health <= 0.0f){
				health = 0.0f;
				Die ();
			}
		} else {
			//if(unsuccessfulHit)
			//	camAudioSource.PlayOneShot(unsuccessfulHit, 1);
			damage = 0.0f;
		}
		return damage;
	}
	
	override public void RemoveHealth() {
		Damage(maxHealth + defense);
	}
	
	public bool Heal(float life) {
		bool lOk = false;
		if (health < maxHealth && life > 0) {
			health += life;
			if (health > maxHealth)
				health = maxHealth;
			lOk = true;
			UpdateHealth ();
		}
		return lOk;
	}
	
	public void FillPercent(float percent) {
		health = (percent / 100.0f) * maxHealth;
		UpdateHealth ();
	}

	public float GetHealth() {
		return health;
	}

	public void SetHealth(float health) {
		this.health = health;
	}

	public float GetHealthAsUnit() {
		return (health / maxHealth);
	}

	public float GetHealthAsPercent() {
		return GetHealthAsUnit () * 100.0f;
	}
	
	private void Die() {
		OnDie ();
		if (!isPlayer)
		{
			if(fuegoFauto)
			{
				fuegoFauto.Drop();
			}
			invincible = true; // Asi ya no puede recibir mas daño, cosa logica si ha muerto
			DieReceiver l_Die = GetComponent<DieReceiver>();
			if (l_Die != null) {
				l_Die.Die();
			} else {
				CancelInvoke();
				Destroy(gameObject);
			}
		}else{
			Incapacitate I = GetComponent<Incapacitate>();
			if (I)
				I.Incap();

			Incapacitate[] incs = FindObjectsOfType<Incapacitate>();
			bool allDead = true;
			for (int i = 0; i < incs.Length; ++i) {
				if (!incs[i].isActionActive()) {
					allDead = false;
				}
			}
			if (allDead) {
				PlayersHaveLost();
			}
		}
	}

	private void PlayersHaveLost() {
		CameraFade cf = gameObject.AddComponent<CameraFade> ();
		cf.SetScreenOverlayColor (new Color (0.0f, 0.0f, 0.0f, 0.0f));
		cf.AddCallback (ReturnToMainMenu);
		cf.StartFade (Color.black, 3.0f);
	}

	private void ReturnToMainMenu() {
        FinishGame.ReestartLevel();
	}
	
	private void UpdateHealth() {
		if (healthBar) {
			healthBar.fillAmount = GetHealthAsUnit();
		}
	}
	
	public void SetHealthBar(Image healthBar, Image damageBar, Image alphaBar) {
		this.healthBar = healthBar;
		this.damageBar = damageBar;
		this.alphaBar = alphaBar;
	}
	
	private void manageHealthBarUI() {
		if (UIType == UI_Type.GameUI) {
			healthBar.fillAmount = GetHealthAsUnit ();
			if (damagePending) {
				damageDelayIntervalTimer += Time.deltaTime;
				if (damageDelayIntervalTimer >= damageDelayInterval) {
					//float newDamage = damageBar.fillAmount - damageApplySpeed / maxHealth * Time.deltaTime;
					float newDamage = alphaBar.fillAmount - damageApplySpeed / maxHealth * Time.deltaTime;
					if (newDamage > healthBar.fillAmount) {
						damageBar.fillAmount = newDamage;
						alphaBar.fillAmount = newDamage;
					} else {
						damageBar.fillAmount = healthBar.fillAmount;
						alphaBar.fillAmount = healthBar.fillAmount;
						damageDelayIntervalTimer = 0.0f;
						damagePending = false;
					}
				}
			}
		} else {
			healthBar.fillAmount = GetHealthAsUnit ();
		}
	}
	
	private void startDamageTimerUI() {
		damagePending = true;
		damageDelayIntervalTimer = 0.0f;
	}

	public void setUIType(UI_Type UIType) {
		this.UIType = UIType;
	}
	
}
