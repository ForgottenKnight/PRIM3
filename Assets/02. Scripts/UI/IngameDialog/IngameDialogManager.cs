using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class IngameDialogManager : MonoBehaviour {
	//public TextAsset fileXML;
    public static IngameDialogManager instance;
	public string eventsFileInAssets = "IngameDialogs.xml";
	struct IngameDialogEvent {
		public string name;
		public string soundEvent;
		public int character;
		public string text;
	}
	private Dictionary<string, List<IngameDialogEvent>> dialogByTrigger;
	private Dictionary<string, IngameDialogEvent> dialogByEvent;

	private List<IngameDialog> playerDialogs;

	// Use this for initialization
	void Start () {
        IngameDialogManager.instance = this;
		dialogByTrigger = new Dictionary<string, List<IngameDialogEvent>> ();
		dialogByEvent = new Dictionary<string, IngameDialogEvent> ();
		Initialize ("Assets/" + eventsFileInAssets);
		playerDialogs = new List<IngameDialog> (3);
	}

	void Initialize(string filename) {
		XmlDocument document = new XmlDocument ();
		//document.LoadXml (fileXML.text);
		document.Load (filename);
		XmlNodeList l_Nodes = document.ChildNodes[1].ChildNodes;
		for (int i = 0; i < l_Nodes.Count; ++i) {
			XmlNode l_Node = l_Nodes[i];
			string l_EventName = l_Node.Attributes.GetNamedItem("name").Value;
			string l_SoundEvent = l_Node.Attributes.GetNamedItem ("sound_event").Value;
			int l_Character = int.Parse(l_Node.Attributes.GetNamedItem ("character").Value);
			string l_Text = l_Node.InnerText.Trim();

			IngameDialogEvent ingameDialogEvent;
			ingameDialogEvent.name = l_EventName;
			ingameDialogEvent.soundEvent = l_SoundEvent;
			ingameDialogEvent.character = l_Character;
			ingameDialogEvent.text = l_Text;

			string l_TriggerName = l_Node.Attributes.GetNamedItem ("triggers").Value;
			if (l_TriggerName == "EVENT") {
				dialogByEvent.Add (l_EventName, ingameDialogEvent);
			} else {
				if (!dialogByTrigger.ContainsKey (l_TriggerName)) {
					dialogByTrigger.Add (l_TriggerName, new List<IngameDialogEvent> ());
				} else if (dialogByTrigger [l_TriggerName] == null) {
					dialogByTrigger [l_TriggerName] = new List<IngameDialogEvent> ();
				}
				dialogByTrigger [l_TriggerName].Add (ingameDialogEvent);
			}

		}
	}
	
	public void ShowEvent(string eventName) {
		if (dialogByEvent.ContainsKey(eventName)) {
			IngameDialogEvent ev = dialogByEvent [eventName];
			IngameDialog ingameDialog = GetIngameDialog (ev.character);
			if (ingameDialog != null) {
				ingameDialog.ShowDialog (ev.text);
			}
		}
	}

	public void ShowTrigger(string triggerName) {
		if (dialogByTrigger.ContainsKey(triggerName)) {
			List<IngameDialogEvent> evs = dialogByTrigger [triggerName];
			if (evs.Count > 0) {
				IngameDialogEvent ev = evs[Random.Range(0, evs.Count)];
				IngameDialog ingameDialog = GetIngameDialog (ev.character);
				if (ingameDialog) {
					ingameDialog.ShowDialog (ev.text);
				}
			}
		}
	}

    public void ShowTriggerByCharacter(string triggerName, int character)
    {
        if (dialogByTrigger.ContainsKey(triggerName))
        {
            List<IngameDialogEvent> evs = dialogByTrigger[triggerName];
            if (evs.Count > 0)
            {
                List<IngameDialogEvent> characterEvs = new List<IngameDialogEvent>();
                for (int i = 0; i < evs.Count; ++i)
                {
                    if (evs[i].character == character)
                    {
                        characterEvs.Add(evs[i]);
                    }
                }
                if (characterEvs.Count > 0)
                {
                    IngameDialogEvent ev = characterEvs[Random.Range(0, characterEvs.Count)];
                    IngameDialog ingameDialog = GetIngameDialog(ev.character);
                    if (ingameDialog)
                    {
                        ingameDialog.ShowDialog(ev.text);
                    }
                }
            }
        }
    }

	private IngameDialog GetIngameDialog(int character) {
		/*if (playerDialogs.Count >= character) {
			if (playerDialogs [character] == null) {
				List<GameObject> players =  CustomTagManager.GetObjectsByTag ("Player");
				foreach (GameObject player in players) {
					if (player.GetComponent<GeneralPlayerController> ().character == character) {
						playerDialogs [character] = player.GetComponent<IngameDialog> ();
					}
				}
			}
			return playerDialogs [character];
		}
		return null;*/
		List<GameObject> players =  CustomTagManager.GetObjectsByTag ("Player");
		foreach (GameObject player in players) {
			if (player.GetComponent<GeneralPlayerController> ().character == character) {
				return player.GetComponent<IngameDialog> ();
			}
		}
		return null;
	}

    void OnDestroy()
    {
        IngameDialogManager.instance = null;
    }
}
