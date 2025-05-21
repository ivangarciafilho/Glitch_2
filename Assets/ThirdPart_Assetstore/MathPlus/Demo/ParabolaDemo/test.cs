using UnityEngine;
using System.Collections;
using MathPlus;

public class test : MonoBehaviour {

	public Vector3 a; //Positions of the guide points
	public Vector3 b; //-
	public Vector3 c; //-

	[Range(0, 1000)]
	public int quality = 10; //Quality of the parabola.

	public GameObject sphere; //Get a prefab to use to show the parabola

	GameObject[] spheres = new GameObject[1000]; //This is set to 1000 so you can set your quality up to 3000 without an error

	void Update () {
		
		for (int i = 0; i < spheres.Length; i++) { //Delete the old parabola!
			Destroy(spheres[i]); //Delete the parabola
		}

		Vector3[] ans = mathPlus.Vector3Parabola(a, b, c, quality); // Use math Plus to create a parabola in 1 line of code!

		for (int i = 0; i < ans.Length; i++) { //This is to create the parabola in the game!
			GameObject newSpehere = GameObject.Instantiate (sphere, new Vector3 (ans [i].x, ans [i].y, ans [i].z), Quaternion.identity) as GameObject;
			spheres [i] = newSpehere; //Store the parabolas parts for future deleting
		}
	}
}
