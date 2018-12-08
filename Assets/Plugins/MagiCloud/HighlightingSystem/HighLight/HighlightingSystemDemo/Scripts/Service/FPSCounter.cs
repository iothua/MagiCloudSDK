using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Text))]
public class FPSCounter : MonoBehaviour
{
	private const int updateFrames = 100;

	private List<float> frameTimes = new List<float>(updateFrames);
	private float sum = 0f;

	private float i = 0f;
	private float avgAcc = 0f;
	private float medAcc = 0f;

	private Text text;

	// 
	void Awake()
	{
		text = GetComponent<Text>();
	}

	// 
	void Update()
	{
		float t = Time.deltaTime;
		sum += t;
		frameTimes.Add(t);

		int l = frameTimes.Count;
		if (l == updateFrames)
		{
			frameTimes.Sort();

			int n = updateFrames / 2;
			
			float medianDeltaTime;
			// even
			if (updateFrames - n * 2 == 0)
			{
				medianDeltaTime = (frameTimes[n - 1] + frameTimes[n]) * 0.5f;
			}
			// odd
			else
			{
				medianDeltaTime = frameTimes[n];
			}
			
			float avg = ((float)l / sum);			// average fps value
			float med = 1f / medianDeltaTime;		// half of the frames were above this fps value

			i++;

			// Accumulated average
			avgAcc = i > 1f ? (i - 1f) / i * avgAcc + avg / i : 0f;

			// Accumulated median
			medAcc = i > 1f ? (i - 1f) / i * medAcc + med / i : 0f;
			
			text.text = string.Format("FPS:\n{0:f2} (avg)\n{1:f2} (med)\n{2:f2} (avg acc)\n{3:f2} (med acc)", avg, med, avgAcc, medAcc);

			frameTimes.Clear();
			sum = 0f;
		}
	}
}
