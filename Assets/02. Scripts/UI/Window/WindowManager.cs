using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine.UI;

public class WindowManager : MonoBehaviour {
	//public static WindowManager instance;

	[System.Serializable]
	public struct ImagesList {
		public string name; //Event name
		public List<Window.IMAGE_CONTROLLER> imageControllers;
	}
	
	[System.Serializable]
	public struct VideoList {
		public string name; //Event name
		public List<Window.VIDEO_PAGE> movieTextures ;
	}

	[Header("Window Prefabs")]
	public GameObject dialogNoImage;
	public GameObject dialogImageRight;
	public GameObject dialogImageLeft;
	public GameObject tutorialPrefab;

	[Header("Assets")]
	public List<ImagesList> images = new List<ImagesList>();
	public List<VideoList> videos = new List<VideoList>();
	public string directory = "Assets/";
	public string filename = "WindowsL1.xml";

	[Header("Windows")]
	public Dictionary<string, Window> windows = new Dictionary<string, Window>();
	public List<Window> openWindows = new List<Window>();

	[Header("Configuration")]
	public bool closeAllOnNewWindow;


	// Use this for initialization
	void Start () {
		LoadWindows (directory + filename);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void LoadWindows(string aFilename) {
		XmlDocument l_document = new XmlDocument ();
		l_document.Load (aFilename);
		XmlNodeList l_nodes = l_document.ChildNodes[1].ChildNodes;
		for (int i = 0; i < l_nodes.Count; ++i) {
			XmlNode l_node = l_nodes[i];
			string l_eventName = l_node.Attributes.GetNamedItem("name").Value;
			string l_eventType = l_node.Attributes.GetNamedItem("type").Value;
			if (l_eventType == "dialog") {

			
				XmlNode l_imagePositionNode = l_node.Attributes.GetNamedItem ("imagePosition");
				string l_imagePosition;

				if (l_imagePositionNode != null) {
					l_imagePosition = l_imagePositionNode.Value;
				} else {
					l_imagePosition = "none";
				}

				Dialog l_dialog = CreateDialog(l_imagePosition);
				XmlNodeList l_pages = l_nodes[i].SelectNodes(".//page");



				List<Tutorial.IMAGE_CONTROLLER> l_imageControllerList = new List<Tutorial.IMAGE_CONTROLLER>();
				ImagesList l_imageList = images.Find(vl => vl.name == l_eventName);

				if (l_imageList.imageControllers != null)
				{
					for (int n = 0; n < l_imageList.imageControllers.Count; n++)
					{
						l_imageControllerList.Add(l_imageList.imageControllers[n]);
					}
				}


				for(int n = 0; n < l_pages.Count; n++) {
					XmlNode l_page = l_pages[n];

					XmlNode l_characterNode = l_page.SelectSingleNode(".//character");
					XmlNode l_textNode = l_page.SelectSingleNode(".//text");

					string l_character;
					if (l_characterNode != null) {
						l_character = l_characterNode.InnerText;
					} else {
						l_character = "Null";
					}
					string l_text = l_textNode.InnerText;

					List<Tutorial.IMAGE_CONTROLLER> l_pageImages;
					l_pageImages = l_imageControllerList.FindAll(i1 => i1.pages.Contains(n));
					AddPageToDialog(l_dialog, l_character, l_text, l_pageImages);
				}
				windows.Add(l_eventName, l_dialog);
			}

			if (l_eventType == "tutorial") {		
				XmlNodeList l_pages = l_nodes[i].SelectNodes(".//page");	

				List<Tutorial.IMAGE_CONTROLLER> l_imageControllerList = new List<Tutorial.IMAGE_CONTROLLER>();
				ImagesList l_imageList = images.Find(vl => vl.name == l_eventName);

                if (l_imageList.imageControllers != null)
                {
                    for (int n = 0; n < l_imageList.imageControllers.Count; n++)
                    {
                        l_imageControllerList.Add(l_imageList.imageControllers[n]);
                    }
                }

				List<Tutorial.VIDEO_PAGE> l_movieTextureList = new List<Tutorial.VIDEO_PAGE>();
				VideoList l_videoList = videos.Find(vl => vl.name == l_eventName);
				if (l_videoList.movieTextures != null) {
					for (int n = 0; n < l_videoList.movieTextures.Count; n++) {
						l_movieTextureList.Add (l_videoList.movieTextures [n]);
					}
				}
				Tutorial l_tutorial = CreateTutorial();
				
				for(int n = 0; n < l_pages.Count; n++) {
					XmlNode l_page = l_pages[n];

					XmlNode l_headerNode = l_page.SelectSingleNode(".//header");
					XmlNode l_subheaderNode = l_page.SelectSingleNode(".//subheader");
					XmlNode l_textNode = l_page.SelectSingleNode(".//text");

					string l_header = l_headerNode.InnerText;
					string l_subheader = l_subheaderNode.InnerText;
					string l_text = l_textNode.InnerText;

					List<Tutorial.IMAGE_CONTROLLER> l_pageImages;
					l_pageImages = l_imageControllerList.FindAll(i1 => i1.pages.Contains(n));

					List<Tutorial.VIDEO_PAGE> l_pageVideos;
					l_pageVideos = l_movieTextureList.FindAll(i1 => i1.pages.Contains(n));
					AddPageToTutorial(l_tutorial, l_header, l_subheader, l_text, l_pageImages, l_pageVideos);
				}
				windows.Add(l_eventName, l_tutorial);
			}
		}
	}

	public void ShowWindow(string aEventName) {
		Window windowToShow = windows [aEventName];
		windowToShow.Show ();
		openWindows.Add (windowToShow);
	}

	public void HideWindow(string aEventName) {
		Window windowToHide = windows [aEventName];
		windowToHide.Hide ();
		openWindows.Remove (windowToHide);
	}

	
	public void OpenWindow(string aEventName) {
		if (windows.ContainsKey (aEventName)) {
			Window windowToOpen = windows [aEventName];
            if (windowToOpen != null) {
                for (int i = 0; i < openWindows.Count; i++) {
                    if (windowToOpen != openWindows[i]) {
                        if (!openWindows[i].IsClosed()) {
                            openWindows[i].Close();
                        }
                        openWindows.Remove(openWindows[i]);
                    }
                }
                windowToOpen.Open();
                openWindows.Add(windowToOpen);
            } else
            {
                Debug.LogWarning("El evento de ventana \"" + aEventName + "\" ha sido destruido y no se puede utilizar.");
            }
		} else {
			Debug.LogWarning ("El evento de ventana \"" + aEventName + "\" no existe.");
		}
	}

	public void CloseWindow(string aEventName) {
		Window windowToClose = windows [aEventName];
		windowToClose.Close ();
		openWindows.Remove (windowToClose);
	}
	

	public Dialog CreateDialog(string type) {
		GameObject l_dialog;
		Dialog l_dialogScript;

		if (type == "left") {
			l_dialog = Instantiate (dialogImageLeft) as GameObject;
		
		} else if (type == "right") {
			l_dialog = Instantiate (dialogImageRight) as GameObject;
		} else {
			l_dialog = Instantiate (dialogNoImage) as GameObject;
		}

		l_dialogScript = l_dialog.GetComponent<Dialog> ();
		return l_dialogScript;
	}

	public void AddPageToDialog(Dialog aDialog, string aCharacter, string aTextNode,  List<Tutorial.IMAGE_CONTROLLER> aImageControllerList) {
		aDialog.AddPage (aCharacter, aTextNode, aImageControllerList);
		aDialog.LoadPage (0);
	}

	public Tutorial CreateTutorial() {
		GameObject l_tutorial = Instantiate (tutorialPrefab) as GameObject;
		Tutorial l_tutorialScript = l_tutorial.GetComponent<Tutorial>();
		//l_tutorialScript.SetVideo (aVideo);
		/*
		for (int i = 0; i < aImageControllerList.Count; i++) {
			Tutorial.IMAGE_CONTROLLER l_imageController = aImageControllerList[i];
			l_tutorialScript.AddImage( l_imageController.controller, l_imageController.image, l_imageController.pages);
		}*/
		return l_tutorialScript;
	}

	public void AddPageToTutorial(Tutorial aTutorial, string aHeader, string aSubheader, string aTextNode, List<Tutorial.IMAGE_CONTROLLER> aImageControllerList, List<Tutorial.VIDEO_PAGE> aVideoPageList) {
		aTutorial.AddPage (aHeader, aSubheader, aTextNode, aImageControllerList, aVideoPageList);
		aTutorial.LoadPage (0);
	}
	
}
