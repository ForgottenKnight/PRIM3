using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class TextEventsLibrary : MonoBehaviour {
	static Dictionary<string, string> whoDictionary;
	static Dictionary<string, string> textDictionary;
	[System.Serializable]
	public struct ImagesList {
		public string name;
		public Sprite image;
	}
	public ImagesList[] images;
	public string eventsFileInAssets = "TextEvents.xml";
	static Dictionary<string, Sprite> imageDictionaryStatic;
	static Dictionary<string, Sprite> imageDictionary;

	void Start() {
		imageDictionaryStatic = new Dictionary<string, Sprite> ();
		for (int i = 0; i < images.Length; ++i) {
			imageDictionaryStatic.Add(images[i].name, images[i].image);
		}
		TextEventsLibrary.Initialize ("Assets/" + eventsFileInAssets);
	}

	public static void Initialize(string filename) {
		whoDictionary = new Dictionary<string, string> ();
		textDictionary = new Dictionary<string, string> ();
		imageDictionary = new Dictionary<string, Sprite> ();
		XmlDocument document = new XmlDocument ();
		document.Load (filename);
		XmlNodeList nodes = document.ChildNodes[1].ChildNodes;
		for (int i = 0; i < nodes.Count; ++i) {
			XmlNode node = nodes[i];
			string eventName = node.Attributes.GetNamedItem("name").Value;
			string eventType = node.Attributes.GetNamedItem("type").Value;
			if (eventType == "message" || eventType == "dialog") {
				XmlNode text1Node = nodes[i].FirstChild;
				XmlNode text2Node = text1Node.NextSibling;
				string text1 = text1Node.InnerText;
				if (text2Node != null) {
					string text2 = text2Node.InnerText;
					whoDictionary.Add(eventName, text1);
					textDictionary.Add(eventName, text2);
				} else {
					textDictionary.Add(eventName, text1);
				}
			} else if (eventType == "tutorial") {
				XmlNode text1Node = nodes[i].FirstChild;
				XmlNode text2Node = text1Node.NextSibling;
				XmlNode text3Node = text2Node.NextSibling;
				XmlNode text4Node = text3Node.NextSibling;
				string text1 = text1Node.InnerText;
				string text2 = text2Node.InnerText;
				string text3 = text3Node.InnerText;
				string text4 = text4Node.InnerText;
				whoDictionary.Add (eventName, text1);
				imageDictionary.Add (eventName, imageDictionaryStatic[text2]);
				imageDictionary.Add (eventName+"K", imageDictionaryStatic[text3]);
				textDictionary.Add (eventName, text4);
			}
		}
	}

	public static void ShowTextEvent(string eventName) {
		/*if (whoDictionary.ContainsKey (eventName)) {
			string t1 = whoDictionary[eventName];
			string t2 = textDictionary[eventName];
			TutorialManager.instance.ShowMessage(t1, t2);
		} else {
			string t1 = textDictionary[eventName];
			TutorialManager.instance.ShowMessage(t1);
		}*/
	}

	public static void ShowTutorial(string eventName) {
		Debug.Log ("ShowTutorial: " + eventName);
		string t1 = whoDictionary [eventName];
		string t2 = textDictionary [eventName];
		Sprite s1 = imageDictionary [eventName];
		Sprite s2 = imageDictionary [eventName + "K"];
		TutorialManager.instance.ShowTutorial (t1, t2, s1, s2);
	}

	public void ShowTextEv(string eventName) {
		TextEventsLibrary.ShowTextEvent (eventName);
	}

	public void ShowTutorialEv(string eventName) {
		TextEventsLibrary.ShowTutorial (eventName);
	}
}
