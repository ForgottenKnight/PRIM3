using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using XboxCtrlrInput;

[AddComponentMenu("PRIM3/CharacterScripts/Core/Player General Controller")] 
public class GeneralPlayerController : GeneralController, IPausable {
	[Header("Prefabs")]
	public GameObject salt;
	public GameObject sulphur;
	public GameObject mercury;
	private GameObject prefab;
    public GameObject[] characters;// 0 -> Salt, 1 -> Sulphur, 2-> Mercury
    public Animator animController;

    public XboxController controller;

	[Header("UI References")]
	public CharactersUIManager charactersUIManager;

	[Header("Configuration")]
	public int character = 0;
    private int m_ChangeCharacter = 0;
	public int player = 0;
	public int UI = 0;
    public Transform targetController;
	protected string characterName = "";
	public Transform playerContainer;
	[HideInInspector]
	public bool canInput = true;

	Incapacitate incap;

	private int m_PauseCount = 0;

	// Use this for initialization
	void Start () {

	    charactersUIManager.setActiveCharacterUI (UI, true);
		GetComponent<Health> ().setUIType (Health.UI_Type.MainUI); // Tipo de UI: MainUI - La UI con los portraits y probetas con vida y energia, GameUI - Barras de vida de los enemigos del escenario
        incap = gameObject.GetComponent<Incapacitate>();
		transform.parent  = GameObject.FindGameObjectWithTag ("PlayerContainer").transform;
        m_ChangeCharacter = character;
	}

	// Update is called once per frame
	void Update () {
		if (canInput) {
			bool changed = false;
			int auxChar = character;
			Jump jump = GetComponent<Jump> ();
			bool jumping = false;
			bool incapacitated = false;
			if (jump)
				jumping = jump.isJumpingOrFalling ();
			if (incap)
				incapacitated = incap.isActionActive ();
			if (!jumping && !incapacitated) {
                if (player == 0 && StaticParemeters.useKeyboard && Input.GetButtonDown("ChangePlayerRight0") 
                    || XCI.GetButtonDown(XboxButton.RightBumper, controller) && (player != 0 || player == 0 && !StaticParemeters.useKeyboard ))
                {//Input.GetButtonDown ("ChangePlayerRight" + player)) {
					while (!AllPlayers() && CharacterExist(auxChar)) {
						auxChar += 1;
						if (auxChar > 2)
							auxChar = 0;
						changed = true;
					}
					if (changed) {
                        m_ChangeCharacter = auxChar;
						ChangeCharacter ();
					}
                }
                else if (player == 0 && StaticParemeters.useKeyboard && Input.GetButtonDown("ChangePlayerLeft0")
                  || XCI.GetButtonDown(XboxButton.LeftBumper, controller) && (player != 0 || player == 0 && !StaticParemeters.useKeyboard))
                {//Input.GetButtonDown ("ChangePlayerLeft" + player)) {
					while (!AllPlayers() && CharacterExist(auxChar)) {
						auxChar -= 1;
						if (auxChar < 0)
							auxChar = 2;
						changed = true;
					}
					if (changed) {
                        m_ChangeCharacter = auxChar;
						ChangeCharacter ();
					}
				}

			}
		}
	}

	bool CharacterExist(int chara) {
		//bool exist = false;
        return characters[chara].activeSelf;
		/*GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");
		foreach (GameObject player in players) {
			if (player.GetComponent<GeneralPlayerController>().character == chara)
				exist = true;
		}
		return exist;*/
	}

	bool AllPlayers() {
		GameObject [] players = GameObject.FindGameObjectsWithTag ("Player");
		return players.Length >= 3;
	}

	public void ReInitCharacter() {
		/*GameObject newChar = (GameObject)Instantiate (prefab, transform.position, transform.rotation);
		newChar.GetComponent<GeneralPlayerController> ().player = player;
		newChar.GetComponent<GeneralPlayerController> ().character = character;
		newChar.GetComponent<GeneralPlayerController> ().healthBar = healthBar;
		newChar.GetComponent<GeneralPlayerController> ().energyBar = energyBar;
		newChar.SetActive(true);
		newChar.name = characterName;
		Energy e = newChar.GetComponent<Energy>();
		if (e) {
			e.energyBar = energyBar;
			Energy oldE = GetComponent<Energy>();
			if (oldE) {
				e.SetEnergy(oldE.GetEnergy());
			}
		}
		
		Health h = newChar.GetComponent<Health>();
		if (h) {
			h.healthBar = healthBar;
			Health oldH = GetComponent<Health>();
			if (oldH) {
				h.FillPercent(oldH.GetPercent());
			}
		}
		Destroy (gameObject);*/
		ChangeCharacter ();
	}

    void ChangeCharacter()
    {
        GameObject newChar = characters[m_ChangeCharacter];
        newChar.transform.position = transform.position;
        newChar.transform.rotation = transform.rotation;

        GeneralPlayerController newCharController = newChar.GetComponent<GeneralPlayerController>();
        newCharController.player = player;
        newCharController.character = m_ChangeCharacter;
        newCharController.controller = controller;
        charactersUIManager.changeCharacterUI(UI, newChar);
        newCharController.UI = UI;
        newCharController.charactersUIManager = charactersUIManager;
        newChar.GetComponent<ShowOffCamera>().iconCanvas = charactersUIManager.GetComponent<Canvas>();
        CheckpointHandler l_CPH = newChar.GetComponent<CheckpointHandler>();
        if(l_CPH)
        {
            l_CPH.useCheckpoint = false;
        }
        newChar.transform.parent = gameObject.transform.parent;
        newChar.SetActive(true);

        Energy e = newChar.GetComponent<Energy>();
        if (e)
        {
            //e.energyBar = energyBar;
            //e.reservedEnergyBar = reservedEnergyBar;
            Energy oldE = gameObject.GetComponent<Energy>();
            if (oldE)
            {
                e.SetEnergy(oldE.GetEnergy());
            }
        }

        Health h = newChar.GetComponent<Health>();
        if (h)
        {
            //h.healthBar = healthBar;
            Health oldH = gameObject.GetComponent<Health>();
            if (oldH)
            {
                h.FillPercent(oldH.GetHealthAsPercent());
                h.invincible = false;       
            }
        }

        //incap = newChar.GetComponent<Incapacitate>();

        TargetPointer l_TP = gameObject.GetComponent<TargetPointer>();
        if(l_TP)
        {
            l_TP.active = false;
            l_TP.unlockTarget();
        }

        ResetAnimator();
        gameObject.transform.parent = null;
        gameObject.SetActive(false);
    }

    void ResetAnimator()
    {

        animController.Play("Idle");
        AnimatorControllerParameter[] parameters = animController.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter parameter = parameters[i];
            switch (parameter.type)
            {
                case AnimatorControllerParameterType.Int:
                    animController.SetInteger(parameter.name, parameter.defaultInt);
                    break;
                case AnimatorControllerParameterType.Float:
                    animController.SetFloat(parameter.name, parameter.defaultFloat);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animController.SetBool(parameter.name, parameter.defaultBool);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    animController.ResetTrigger(parameter.name);
                    break;
            }
        }
    }

	void ChangeCharacterPrefabs() {
		switch (character) {
			case 0: // Salt
				prefab = salt;
				characterName = "Salt";
				break;
			case 1: // Sulphur
				prefab = sulphur;
				characterName = "Sulphur";
				break;
			case 2: // Mercury
				prefab = mercury;
				characterName = "Mercury";
				break;
		}

		GameObject[] chars = GameObject.FindGameObjectsWithTag ("Player");
		GameObject oldChar = null;
		foreach (GameObject go in chars) {
			GeneralPlayerController mc = go.GetComponent<GeneralPlayerController>();
			if (mc) {
				if (mc.player == player) {
					oldChar = go;
				}
			}
		}
		if (oldChar) {
			List<GameObject> ces = CustomTagManager.GetObjectsByTag("CameraEvent");
			for (int i = 0; i < ces.Count; ++i) {
				CameraEvent cameraEvent = ces[i].GetComponent<CameraEvent>();
				if (cameraEvent) {
					cameraEvent.DeletePlayerFromList(player, true);
				}
			}
			GameObject newChar = (GameObject)Instantiate (prefab, oldChar.transform.position, oldChar.transform.rotation);
			GeneralPlayerController newCharController = newChar.GetComponent<GeneralPlayerController> ();
			newCharController.player = player;
			newCharController.character = character;
            newCharController.controller = controller;
			newChar.name = characterName;
			charactersUIManager.changeCharacterUI( UI, newChar);
			newCharController.UI = UI;
			newCharController.charactersUIManager = charactersUIManager;
			newChar.GetComponent<ShowOffCamera>().iconCanvas = charactersUIManager.GetComponent<Canvas>();
			/*newChar.GetComponent<GeneralPlayerController> ().healthBar = healthBar;
			newChar.GetComponent<GeneralPlayerController> ().energyBar = energyBar;
			newChar.GetComponent<GeneralPlayerController> ().reservedEnergyBar = reservedEnergyBar;
			newChar.GetComponent<GeneralPlayerController> ().portrait = portrait;
			newChar.GetComponent<GeneralPlayerController> ().portrait.sprite = newChar.GetComponent<GeneralPlayerController> ().portraitPrefab.GetComponent<SpriteRenderer>().sprite;*/
			//newChar.GetComponent<Attack>().AnimControl = newChar.GetComponent<Animator>();
			newChar.SetActive(true);

			Energy e = newChar.GetComponent<Energy>();
			if (e) {
				//e.energyBar = energyBar;
				//e.reservedEnergyBar = reservedEnergyBar;
				Energy oldE = oldChar.GetComponent<Energy>();
				if (oldE) {
					e.SetEnergy(oldE.GetEnergy());
				}
			}

			Health h = newChar.GetComponent<Health>();
			if (h) {
				//h.healthBar = healthBar;
				Health oldH = oldChar.GetComponent<Health>();
				if (oldH) {
					h.FillPercent(oldH.GetHealthAsPercent());
				}
			}


			//Attack a = newChar.GetComponent<Attack>();			
			//Attack oldA = oldChar.GetComponent<Attack>();
			incap = newChar.GetComponent<Incapacitate>();
			//newChar.transform.parent = oldChar.transform.parent;
			/*if (a && oldA) 
			{
				a.enemies = oldA.enemies;
			}*/


			DestroyImmediate (oldChar.gameObject);
			Destroy (oldChar.gameObject);
		}
	}

	#region IPausable implementation

	public void Pause ()
	{
		canInput = false;
		m_PauseCount++;
	}

	public void Unpause ()
	{
		m_PauseCount--;
		if (m_PauseCount <= 0) {
			canInput = true;
			m_PauseCount = 0;
		}
	}

	#endregion
}
