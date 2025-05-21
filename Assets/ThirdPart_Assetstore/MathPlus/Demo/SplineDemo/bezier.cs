using UnityEngine;
using System.Collections;
using MathPlus;

public class bezier : MonoBehaviour {

	public Vector3[] points;


	[Range(0, 100)]
	public int quality = 10; //Quality of the parabola.

	public GameObject sphere; //Get a prefab to use to show the parabola

	GameObject[] spheres = new GameObject[1000]; //This is set to 1000 so you can set your quality up to 3000 without an error
	Vector3[] ans;

	void Update () {
		
		for (int i = 0; i < spheres.Length; i++) { //Delete the old parabola!
			Destroy(spheres[i]); //Delete the parabola
		}

		ans = mathPlus.CreateCatmullSpline (points, quality, true);


		for (int i = 0; i < ans.Length; i++) { //This is to create the parabola in the game!
			GameObject newSpehere = GameObject.Instantiate (sphere, new Vector3 (ans [i].x, ans [i].y, ans [i].z), Quaternion.identity) as GameObject;
			spheres [i] = newSpehere; //Store the parabolas parts for future deleting
		}
	}
}
