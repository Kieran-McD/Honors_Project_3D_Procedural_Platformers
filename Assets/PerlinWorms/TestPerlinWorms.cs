using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - let's get wormy

public class TestPerlinWorms : MonoBehaviour
{
	[Header( "Supply a GameObject with a LineRenderer on it.")]
	public LineRenderer OneWorm;

	void Start()
	{
		Application.targetFrameRate = 60;

		OneWorm.gameObject.SetActive(false);

		for (int i = 0; i < 100; i++)
		{
			var dupe = Instantiate<LineRenderer>( OneWorm);
			var worm = dupe.gameObject.AddComponent<PerlinWorm>();

			worm.gameObject.SetActive(true);
		}
	}
}
