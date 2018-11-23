using UnityEngine;
using System.Collections;

static public class ColorTool
{
	private const int period = 1530;
	static private readonly int[] colorComponentOffsets = new int[] { 1020, 0, 510 };
	static private float[] colorComponents = new float[3];
	static private Color colorResult = new Color(0f, 0f, 0f, 1f);

	// 
	static public Color GetColor(float hue)
	{
		hue = Mathf.Clamp01(hue);
		
		int hi = Mathf.CeilToInt(hue * period);
		
		for (int i = 0; i < 3; i++)
		{
			int x = (hi - colorComponentOffsets[i]) % period;
			int o = 0;
			
			if (x < 0) { x += period; }
			
			if (x < 255) { o = x; }
			if (x >= 255 && x < 765) { o = 255; }
			if (x >= 765 && x < 1020) { o = 1020 - x; }
			
			colorComponents[i] = o / 255f;
		}
		colorResult.r = colorComponents[0];
		colorResult.g = colorComponents[1];
		colorResult.b = colorComponents[2];
		return colorResult;
	}
}