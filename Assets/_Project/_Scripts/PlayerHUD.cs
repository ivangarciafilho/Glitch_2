using UnityEngine;
using System.Collections.Generic;

public class PlayerHUD : SingleInstance<PlayerHUD>
{
	public RuneUI runeUIPrefab;
	public Transform runeUIParent;
	public List<RuneUI> runeUIs = new List<RuneUI>();

	void Start()
	{

	}

	void Update()
	{

	}

	public void AddRune(Rune rune)
	{
		RuneUI runeUI = Instantiate(runeUIPrefab, runeUIParent);
		runeUI.rune = rune;
		runeUI.image.sprite = rune.icon;
		runeUIs.Add(runeUI);
	}
	
	public void RemoveRune(Rune rune)
	{
		for (int i = 0; i < runeUIs.Count; i++)
		{
			if (runeUIs[i].rune == rune)
			{
				Destroy(runeUIs[i].gameObject);
				runeUIs.RemoveAt(i);
				break;
			}
		}
	}

}
