using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

public class ConfigurePlayers : MonoBehaviour {

	public GameObject Salt;
	public GameObject Mercury;
	public GameObject Sulphur;
    public PlayerDeactivator playerDeactiv;

	// Use this for initialization
	void Start () {

        GeneralPlayerController l_SaltGPC = Salt.GetComponent<GeneralPlayerController>();
        GeneralPlayerController l_SulphurGPC = Sulphur.GetComponent<GeneralPlayerController>();
        GeneralPlayerController l_MercuryGPC = Mercury.GetComponent<GeneralPlayerController>();

        if (!StaticParemeters.savedConfiguration)
        {
            l_SaltGPC.player = 0;
            l_SaltGPC.UI = 0;
            if (StaticParemeters.useKeyboard)
            {
                l_SulphurGPC.controller = XboxController.First;
                l_MercuryGPC.controller = XboxController.Second;
            }
            else
            {
                l_SaltGPC.controller = XboxController.First;
                l_SulphurGPC.controller = XboxController.Second;
                l_MercuryGPC.controller = XboxController.Third;
            }
            l_SulphurGPC.player = 1;
            l_SulphurGPC.UI = 1;
            l_MercuryGPC.player = 2;
            l_MercuryGPC.UI = 2;

         //   Salt.SetActive(true);
          //  Sulphur.SetActive(true);
           // Mercury.SetActive(true);


            switch (StaticParemeters.numPlayers)
            {
                case 1:
                    StaticParemeters.activeCharacters[0] = true;
                    StaticParemeters.characters[0] = 0;
                    l_SaltGPC.charactersUIManager.setActiveCharacterUI(0, true);
                    //Salt.SetActive(true);
                    break;
                case 2:
                    StaticParemeters.activeCharacters[0] = true;
                    StaticParemeters.characters[0] = 0;
                    l_SaltGPC.charactersUIManager.setActiveCharacterUI(0, true);

                    StaticParemeters.activeCharacters[1] = true;
                    StaticParemeters.characters[1] = 1;
                    l_SulphurGPC.charactersUIManager.setActiveCharacterUI(1, true);


                   // Salt.SetActive(true);
                  //  Sulphur.SetActive(true);
                    break;
                case 3:
                    StaticParemeters.activeCharacters[0] = true;   
                    StaticParemeters.characters[0] = 0;
                    l_SaltGPC.charactersUIManager.setActiveCharacterUI(0, true);

                    StaticParemeters.activeCharacters[1] = true;  
                    StaticParemeters.characters[1] = 1;
                    l_SulphurGPC.charactersUIManager.setActiveCharacterUI(1, true);

                    StaticParemeters.activeCharacters[2] = true;
                    StaticParemeters.characters[2] = 2;
                    l_MercuryGPC.charactersUIManager.setActiveCharacterUI(2, true);

                  //  Salt.SetActive(true);
                  //  Sulphur.SetActive(true);
                  //  Mercury.SetActive(true);
                    break;
            }
        }
        else
        {

          //  Salt.SetActive(true);
           // Sulphur.SetActive(true);
            //Mercury.SetActive(true);


            for (int i = 0; i < StaticParemeters.activeCharacters.Length; ++i)
            {
                if(StaticParemeters.activeCharacters[i])
                {
                    switch(i)
                    {
                        case 0:
                            CharactersUIManager l_SaltcharactersUIManager = l_SaltGPC.charactersUIManager;
                            l_SaltGPC.player = StaticParemeters.characters[0];
                            l_SaltGPC.UI = StaticParemeters.characters[0];
                            l_SaltGPC.controller = StaticParemeters.playerControllers[0];
                           // Salt.SetActive(true);
                            l_SaltcharactersUIManager.changeCharacterUI(l_SaltGPC.UI, Salt);

                            break;
                        case 1:
                            CharactersUIManager l_SulphurcharactersUIManager = l_SulphurGPC.charactersUIManager;
                            l_SulphurGPC.player = StaticParemeters.characters[1];
                            l_SulphurGPC.UI = StaticParemeters.characters[1];
                            l_SulphurGPC.controller = StaticParemeters.playerControllers[1];
                          //  Sulphur.SetActive(true);
                            l_SulphurcharactersUIManager.changeCharacterUI(l_SulphurGPC.UI, Sulphur);

                            break;
                        case 2:
                            CharactersUIManager l_MercurycharactersUIManager = l_MercuryGPC.charactersUIManager;
                            l_MercuryGPC.player = StaticParemeters.characters[2];
                            l_MercuryGPC.UI = StaticParemeters.characters[2];
                            l_MercuryGPC.controller = StaticParemeters.playerControllers[2];
                           // Mercury.SetActive(true);
                            l_MercurycharactersUIManager.changeCharacterUI(l_MercuryGPC.UI, Mercury);
                            break;
                    }
                }
            }

            /*if (StaticParemeters.useKeyboard)
            {
                Sulphur.GetComponent<GeneralPlayerController>().controller = XboxController.First;
                Mercury.GetComponent<GeneralPlayerController>().controller = XboxController.Second;
            }
            else
            {
                Salt.GetComponent<GeneralPlayerController>().controller = XboxController.First;
                Sulphur.GetComponent<GeneralPlayerController>().controller = XboxController.Second;
                Mercury.GetComponent<GeneralPlayerController>().controller = XboxController.Third;
            }
            Sulphur.GetComponent<GeneralPlayerController>().player = StaticParemeters.characters[1];
            Sulphur.GetComponent<GeneralPlayerController>().UI = StaticParemeters.characters[1];
            Mercury.GetComponent<GeneralPlayerController>().player = StaticParemeters.characters[2];
            Mercury.GetComponent<GeneralPlayerController>().UI = StaticParemeters.characters[2];

            for(int i = 0; i< StaticParemeters.activeCharacters.Length; ++i)
            {
                if(StaticParemeters.activeCharacters[i])
                {
                    switch(i)
                    {
                        case 0:
                            Salt.SetActive(true);
                            break;
                        case 1:
                            Sulphur.SetActive(true);
                            break;
                        case 2:
                            Mercury.SetActive(true);
                            break;                         
                    }
                }
            }*/
        }

        playerDeactiv.DeactivatePlayers();	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SaveParameters()
    {
        StaticParemeters.SaveConfiguration(Salt, Sulphur,Mercury);
    }
}
