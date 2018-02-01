using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenSocialMediaSite : MonoBehaviour {

	public void LoadByString(string socialMediaStr)
	{
		if (socialMediaStr == "facebook") {
			Application.OpenURL ("http://facebook.com/");
		} else if (socialMediaStr == "twitter") {
			Application.OpenURL ("http://twitter.com/");
		}
	}
}
