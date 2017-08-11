using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Page {
	public List<Window.IMAGE_CONTROLLER> images = new List<Window.IMAGE_CONTROLLER> ();
	public List<Window.VIDEO_PAGE> videos = new List<Window.VIDEO_PAGE> ();

	public Page(string aCharacter,string aText, int aIndex,  List<Tutorial.IMAGE_CONTROLLER> aImageControllerList) {
		this.m_character = aCharacter;
		this.m_text = aText;
		this.m_index = aIndex;
		this.images = aImageControllerList;
	}

	public Page(string aHeader, string aSubheader, string aText, int aIndex,  List<Tutorial.IMAGE_CONTROLLER> aImageControllerList, List<Tutorial.VIDEO_PAGE> aVideoPageList) {
		this.m_header = aHeader;
		this.m_subheader = aSubheader;
		this.m_text = aText;
		this.m_index = aIndex;
		this.images = aImageControllerList;
		this.videos = aVideoPageList;
	}
	
	public string m_header;
	public string m_subheader;
	public string m_character;
	public string m_text;
	public int m_index;

}
