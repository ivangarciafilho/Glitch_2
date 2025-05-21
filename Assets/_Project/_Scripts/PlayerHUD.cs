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
		runeUI.image.sprite = rune.icon;
		runeUIs.Add(runeUI);
	}
}
