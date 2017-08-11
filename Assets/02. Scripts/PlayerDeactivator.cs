using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeactivator : MonoBehaviour {

    public GameObject Salt;
    public GameObject Mercury;
    public GameObject Sulphur;

	// Use this for initialization
	void Start () {
          /*  for (int i = 0; i< StaticParemeters.activeCharacters.Length; ++i)
            {
                if(!StaticParemeters.activeCharacters[i])
                {
                    switch(i)
                    {
                        case 0:
                            Salt.transform.parent = null;
                            Salt.SetActive(false);
                            break;
                        case 1:
                            Sulphur.transform.parent = null;
                            Sulphur.SetActive(false);
                            break;
                        case 2:
                            Mercury.transform.parent = null;
                            Mercury.SetActive(false);
                            break;
                    }
                }
            }
        Destroy(gameObject);*/
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DeactivatePlayers()
    {
        for (int i = 0; i < StaticParemeters.activeCharacters.Length; ++i)
        {
            if (!StaticParemeters.activeCharacters[i])
            {
                switch (i)
                {
                    case 0:
                        Salt.transform.parent = null;
                        Salt.SetActive(false);
                        break;
                    case 1:
                        Sulphur.transform.parent = null;
                        Sulphur.SetActive(false);
                        break;
                    case 2:
                        Mercury.transform.parent = null;
                        Mercury.SetActive(false);
                        break;
                }
            }
        }
        Destroy(gameObject);
    }
}
