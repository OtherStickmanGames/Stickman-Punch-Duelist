using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Language : MonoBehaviour {

	public static bool rus = false;

	// Use this for initialization
	void Awake()
	{
		if (Application.systemLanguage == SystemLanguage.English)
			rus = false;
		if (Application.systemLanguage == SystemLanguage.Russian)
			rus = true;
	}

}
