using UnityEngine;
using System.Collections;
using XboxCtrlrInput;

[RequireComponent(typeof(GeneralPlayerController))]
public class SuperAbility : ActionBehaviour {
	
	[HideInInspector]
	public int character = -1;
    [HideInInspector]
    public bool markerSet;


	public GameObject marker;
	public float energyCost = 50f;

	private SuperAbilityController markerManager;
	private GeneralPlayerController gc;
	private float set;
	private float unset;
	private Energy energy;
	private bool specialEnabled = false;
	
	// Use this for initialization
	void Start () {
		gc = gameObject.GetComponent<GeneralPlayerController>();
		character = gc.character;
		marker = GameObject.Find("SuperAbility Markers");
		markerManager = marker.GetComponent<SuperAbilityController>();
		marker.GetComponent<SuperAbilityController>().markers[gc.character] = this;
		marker = marker.transform.GetChild(gc.character).gameObject;
		energy = GetComponent<Energy>();
		markerSet = false;
		if(marker.activeSelf)
		{
			markerSet = true;
			energy.ReserveEnergy(energyCost);
		}
	}
	
	override public void GetInput() 
	{
        if (gc.player == 0 && StaticParemeters.useKeyboard)
        {

            set = Input.GetAxis ("SetMarker0");
            if (set == 0)
            {
                specialEnabled = false;
            }
            unset = Input.GetAxis ("UnSetMarker0");
        }
        else
        {
            set = XCI.GetAxis(XboxAxis.RightTrigger, gc.controller);
            //set = Input.GetAxis ("SetMarker" + gc.player);
            if (set == 0)
            {
                specialEnabled = false;
            }
            unset = XCI.GetAxis(XboxAxis.LeftTrigger, gc.controller);
        }
		//unset = Input.GetAxis ("UnSetMarker" + gc.player);
	}
	
	override public void ActionUpdate()
	{
		if(set > 0 && !specialEnabled)
		{
			
			specialEnabled = true;
			if(!markerSet)
			{
				if(energy.ReserveEnergy(energyCost))
				{
					markerSet = true;
					marker.SetActive(true);
					marker.transform.position = transform.position - new Vector3(0f,transform.localScale.y,0f);
					markerManager.setMarker(gc.character, true, marker.transform);
					switch(character)
					{
					case 0://Salt
						if( markerManager.IsMarkerActive(1))
						{
							markerManager.SetConnector(0,true);
						}
						if( markerManager.IsMarkerActive(2))
						{
							markerManager.SetConnector(1,true);
						}
						break;
					case 1://Sulphur
						if( markerManager.IsMarkerActive(0))
						{
							markerManager.SetConnector(0,true);
						}
						if( markerManager.IsMarkerActive(2))
						{
							markerManager.SetConnector(2,true);
						}
						break;
					case 2://Mercury
						if( markerManager.IsMarkerActive(0))
						{
							markerManager.SetConnector(1,true);
						}
						if( markerManager.IsMarkerActive(1))
						{
							markerManager.SetConnector(2,true);
						}
						break;
					}
				}
			}else{
                if (!SuperAbilityStatic.m_active)
                {
                    marker.transform.position = transform.position - new Vector3(0f, transform.localScale.y, 0f);
                    markerManager.setMarker(gc.character, true, marker.transform);
                    switch (character)
                    {
                        case 0://Salt
                            if (markerManager.IsMarkerActive(1))
                            {
                                markerManager.SetConnector(0, true);
                            }
                            if (markerManager.IsMarkerActive(2))
                            {
                                markerManager.SetConnector(1, true);
                            }
                            break;
                        case 1://Sulphur
                            if (markerManager.IsMarkerActive(0))
                            {
                                markerManager.SetConnector(0, true);
                            }
                            if (markerManager.IsMarkerActive(2))
                            {
                                markerManager.SetConnector(2, true);
                            }
                            break;
                        case 2://Mercury
                            if (markerManager.IsMarkerActive(0))
                            {
                                markerManager.SetConnector(1, true);
                            }
                            if (markerManager.IsMarkerActive(1))
                            {
                                markerManager.SetConnector(2, true);
                            }
                            break;
                    }
                } 
			}
		}

		if(unset > 0 && markerSet && !SuperAbilityStatic.m_active)
		{
			marker.SetActive(false);
			energy.ReleaseReservedEnergy(energyCost);
			markerSet = false;
			markerManager.setMarker(gc.character, false, null);
			switch(character)
			{
			case 0://Salt
				markerManager.SetConnector(0, false);
				markerManager.SetConnector(1, false);
				break;
			case 1://Sulphur
				markerManager.SetConnector(0, false);
				markerManager.SetConnector(2, false);
				break;
			case 2://Mercury
				markerManager.SetConnector(1, false);
				markerManager.SetConnector(2, false);
				break;
			}
		}
	}

	public void useEnergy()
	{
		markerSet = false;
		marker.SetActive(false);
		markerManager.SetConnector(0,false);
		markerManager.SetConnector(1,false);
		markerManager.SetConnector(2,false);
		energy.ConsumeReservedEnergy(energyCost);
	}
	
	
}
