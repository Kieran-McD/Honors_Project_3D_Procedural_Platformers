using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - perlin worms...

public class PerlinWorm : MonoBehaviour
{
	[Header( "If this is zero, we'll make random values for all.")]
	public float speed;

	public float heading;
	public float turnyness;

	public int frames;

	Vector3[] positions;

	LineRenderer lr;

	void Awake()
	{
		// defaults
		if (speed == 0)
		{
			speed = Random.Range( 2.0f, 5.0f);
			heading = Random.Range( 0.0f, 360.0f);
			turnyness = Random.Range( 1450.0f, 2600.0f);
			frames = Random.Range( 10, 100);
		}
	}

	void Start()
	{
		lr = GetComponent<LineRenderer>();
		lr.useWorldSpace = true;

		lr.startColor = Random.ColorHSV();
		lr.endColor = Random.ColorHSV();

		positions = new Vector3[frames];

		var position = transform.position;

		for (int i = 0; i < positions.Length; i++)
		{
			positions[i] = position;
		}

		DrivePositions();
	}

	void Update ()
	{
		Vector3 position = transform.position;

		float x = position.x;
		float y = position.y;

		float noise = Mathf.PerlinNoise( x, y);

		noise -= 0.5f;

		float turn = noise * turnyness;

		heading += turn * Time.deltaTime;

		Vector3 direction = Quaternion.Euler (0, 0, heading) * Vector3.up;

		position += direction * (speed * Time.deltaTime);

		transform.position = position;

		for (int i = 0; i < positions.Length - 1; i++)
		{
			positions[i] = positions[ i + 1];
		}
		positions[positions.Length - 1] = position;

		CheckOffscreen();

		DrivePositions();
	}

	void DrivePositions()
	{
		lr.positionCount = positions.Length;
		lr.SetPositions( positions);
	}

	void CheckOffscreen()
	{
		// boundskeeping hack:
		// when the tail goes "too far away" we grab the
		// whole worm and stick it back in the middle
		var position = positions[0];
		if (position.magnitude > 10)
		{
			position = Vector3.zero;

			transform.position = position;

			for (int i = 0; i < positions.Length; i++)
			{
				positions[i] = position;
			}
		}
	}
}
