using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// @kurtdekker - perlin worms...

public class PerlinWorm : MonoBehaviour
{
	[Header( "If this is zero, we'll make random values for all.")]
	public float speed;

	public int frames;
	public bool roll = true;
	public bool pitch = true;


	Vector3[] positions;
	LineRenderer lr;
	public float freq = 0.1f;

	float randA;
	float randB;

	static public float min = 0.0f;
	static public float max = 0.0f;




	public Vector2 GetMinMax()
    {
		return new Vector2(min, max);
	}

	void Awake()
	{
		// defaults
		if (speed == 0)
		{
			speed = Random.Range( 2.0f, 5.0f);
			frames = Random.Range( 10, 100);
		}
	}

	void Start()
	{

		randA = Random.value * 5000.0f;
		randB = 10000 + Random.value * 2500.0f;

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
	protected void Update ()
	{
		PerlinWormLogic();
	}

	void PerlinWormLogic()
	{
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        double noise = ImprovedNoise.noise((x * freq) + randA, (y * freq) + randA, (z * freq) + randA);// Mathf.PerlinNoise( x, y);
        double noise2 = ImprovedNoise.noise((x * freq) + randB, (y * freq) + randB, (z * freq) + randB);// Mathf.PerlinNoise( x, y);

        /*noise *= 2.0f;
		noise2 *= 2.0f;*/

        if (noise < min)
        {
            min = (float)noise;
        }
        if (noise > max)
        {
            max = (float)noise;
        }

        float turn = (float)noise * 360.0f;
        float turn2 = (float)noise2 * 45.0f;

        transform.rotation = Quaternion.identity;
        if (roll)
		{
            //transform.rotation = Quaternion.AngleAxis(turn, transform.forward);
            transform.rotation = Quaternion.Euler(0,turn,0);
        }
           

        if (pitch)
            transform.rotation *= Quaternion.AngleAxis(turn2, transform.right);
        // (0, 0, turn);// = Quaternion.AngleAxis(heading, transform.forward);// Quaternion.Euler (0, 0, heading) * direction;
        //transform.Rotate(heading2, 0, 0);
        transform.position += transform.forward * (speed * Time.deltaTime);

        for (int i = 0; i < positions.Length - 1; i++)
        {
            positions[i] = positions[i + 1];
        }
        positions[positions.Length - 1] = transform.position;

        //CheckOffscreen();

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
