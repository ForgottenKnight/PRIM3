using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class CharactersUIManager : MonoBehaviour {
    [Header("Portraits")]
    public Image[] sprites;

	[Header("Referencias")]
	public UICharacterRelationship[] CharacterUI;

	[Header("Estados del personaje")]
	public State[] States;

	[System.Serializable]
	public struct UICharacterRelationship
	{
		//Personaje
		[Header("Personaje")]
		public GameObject character;
		public Health healthScript;
		public Energy energyScript;
		public Image portrait;
		public Text name;

		[Header("UI")]
		public GameObject UI;
		public Image portraitUI;
		public Image healthUI; 
		public Image energyUI; 
		public Image reservedEnergyUI;
		public Text nameUI;

		[Header("Estados")]
		public List<UIState> statesUI;

	}

    private bool assigned = false;


	[System.Serializable]
	public struct State
	{
		public string name;
		public GameObject prefab;
	}

	// Use this for initialization
	void Awake () {

	}

	// Use this for initialization
	void Start () {
		
		AssignUIElements ();
		CreateStates ();
	}
	
	// Update is called once per frame
	void Update () {
		UpdateCharacterUI ();
		
	}

	private void AssignUIElements() {

        if (!assigned)
        {
            assigned = true;
            for (int i = 0; i < CharacterUI.Length; ++i)
            {
                CharacterUI[i].healthUI = CharacterUI[i].UI.transform.Find("Panel HealthBar/HealthBar/Health").gameObject.GetComponent<Image>();
                CharacterUI[i].energyUI = CharacterUI[i].UI.transform.Find("Panel EnergyBar/EnergyBar/Energy").gameObject.GetComponent<Image>();
                CharacterUI[i].reservedEnergyUI = CharacterUI[i].UI.transform.Find("Panel EnergyBar/EnergyBar/ReservedEnergy").gameObject.GetComponent<Image>();
                CharacterUI[i].nameUI = CharacterUI[i].UI.transform.Find("Panel Name/Text").gameObject.GetComponent<Text>();
                CharacterUI[i].portraitUI = CharacterUI[i].UI.transform.Find("Panel Portrait/Portrait").gameObject.GetComponent<Image>();

                CharacterUI[i].healthScript = CharacterUI[i].character.GetComponent<Health>();
                CharacterUI[i].energyScript = CharacterUI[i].character.GetComponent<Energy>();

                CharacterUI[i].nameUI.text = CharacterUI[i].character.GetComponent<GeneralPlayerController>().name;
                CharacterUI[i].portraitUI.sprite = CharacterUI[i].portrait.sprite;
            }
        }
	}

	private void CreateStates() {
		for (int i = 0; i < CharacterUI.Length; ++i) {
			CharacterUI[i].statesUI = new List<UIState>();
			for (int n = 0; n < States.Length; ++n) {
				GameObject state = Instantiate(States[n].prefab);	
				state.transform.SetParent(CharacterUI[i].portraitUI.transform);
				state.transform.localPosition = Vector3.zero;
				state.GetComponent<UIState>().SetCharacter(CharacterUI[i].character);
				CharacterUI[i].statesUI.Add(state.GetComponent<UIState>());
//				Debug.Log("Instantiated");
			}
		}
	}



	private void UpdateCharacterUI() {
		for (int i = 0; i < CharacterUI.Length; ++i) {
			if(CharacterUI[i].UI.activeInHierarchy) {
				CharacterUI[i].healthUI.fillAmount = CharacterUI[i].healthScript.GetHealthAsUnit();
				CharacterUI[i].energyUI.fillAmount = CharacterUI[i].energyScript.GetEnergyAsUnit() - CharacterUI[i].energyScript.GetReservedEnergyAsUnit();
				CharacterUI[i].reservedEnergyUI.fillAmount = CharacterUI[i].energyScript.GetEnergyAsUnit();
			}
		}
	}

	public void changeCharacterUI(int indexUI, GameObject character) {
        if (!assigned)
        {
            AssignUIElements();
        }
		if (indexUI >= 0 && indexUI < CharacterUI.Length) {
	
				int UIIndex = character.GetComponent<GeneralPlayerController>().UI;

				if(CharacterUI[indexUI].character.name != character.name)
				{
                    GeneralPlayerController l_GPC = character.GetComponent<GeneralPlayerController>();
                    CharacterUI[indexUI].portraitUI.sprite = sprites[l_GPC.character].sprite;//CharacterUI[UIIndex].portrait.sprite;
					CharacterUI[indexUI].nameUI.text = CharacterUI[UIIndex].name.text;
				}
				CharacterUI[indexUI].character = character;
				CharacterUI[indexUI].healthScript = CharacterUI[indexUI].character.GetComponent<Health>();
				CharacterUI[indexUI].energyScript = CharacterUI[indexUI].character.GetComponent<Energy>();

				for(int i = 0; i < CharacterUI[indexUI].statesUI.Count; ++i) {
					CharacterUI[indexUI].statesUI[i].SetCharacter(character);
				}
			
		}
	}
	
	public void setActiveCharacterUI(int indexUI, bool active) {
        if (indexUI >= 0 && indexUI < CharacterUI.Length && StaticParemeters.activeCharacters[indexUI])
        {
            CharacterUI[StaticParemeters.characters[indexUI]].UI.SetActive(active);
		}
	}

}
