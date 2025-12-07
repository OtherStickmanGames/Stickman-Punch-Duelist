using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LanguageFitter : MonoBehaviour {

	[SerializeField] string text;

	// Use this for initialization
	void Start()
	{
		if (Language.rus)
		{
			if (GetComponent<TextMeshProUGUI>())
			{
				GetComponent<TextMeshProUGUI>().text = text;
			}
			else if (GetComponent<TextMeshPro>())
			{
				GetComponent<TextMeshPro>().text = text;
			}
			else if (GetComponent<Text>())
			{
				GetComponent<Text>().text = text;
			}
		}
	} 
}
