using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace HighlightingSystem
{
	public partial class Highlighter : MonoBehaviour
	{
		#region Editable Parameters
		// Only these types of Renderers will be highlighted
		static public readonly List<System.Type> types = new List<System.Type>()
		{
			typeof(MeshRenderer), 
			typeof(SkinnedMeshRenderer), 
			typeof(SpriteRenderer), 
			//typeof(ParticleRenderer), 
			//typeof(ParticleSystemRenderer), 
		};
		#endregion

		#region Public Methods
		/// <summary>
		/// Renderers reinitialization. 
		/// Call this method if your highlighted object has changed it's materials, renderers or child objects.
		/// Can be called multiple times per update - renderers reinitialization will occur only once.
		/// </summary>
		public void ReinitMaterials()
		{
			renderersDirty = true;
		}
		
		/// <summary>
		/// Set color for one-frame highlighting mode.
		/// </summary>
		/// <param name='color'>
		/// Highlighting color.
		/// </param>
		public void OnParams(Color color)
		{
			onceColor = color;
		}
		
		/// <summary>
		/// Turn on one-frame highlighting.
		/// </summary>
		public void On()
		{
			once = true;
		}
		
		/// <summary>
		/// Turn on one-frame highlighting with specified color.
		/// Can be called multiple times per update, color only from the latest call will be used.
		/// </summary>
		/// <param name='color'>
		/// Highlighting color.
		/// </param>
		public void On(Color color)
		{
			onceColor = color;
			once = true;
		}
		
		/// <summary>
		/// Set flashing parameters.
		/// </summary>
		/// <param name='color1'>
		/// Starting color.
		/// </param>
		/// <param name='color2'>
		/// Ending color.
		/// </param>
		/// <param name='freq'>
		/// Flashing frequency (times per second).
		/// </param>
		public void FlashingParams(Color color1, Color color2, float freq)
		{
			flashingColorMin = color1;
			flashingColorMax = color2;
			flashingFreq = freq;
		}
		
		/// <summary>
		/// Turn on flashing.
		/// </summary>
		public void FlashingOn()
		{
			flashing = true;
		}
		
		/// <summary>
		/// Turn on flashing from color1 to color2.
		/// </summary>
		/// <param name='color1'>
		/// Starting color.
		/// </param>
		/// <param name='color2'>
		/// Ending color.
		/// </param>
		public void FlashingOn(Color color1, Color color2)
		{
			flashingColorMin = color1;
			flashingColorMax = color2;
			flashing = true;
		}
		
		/// <summary>
		/// Turn on flashing from color1 to color2 and specified frequency.
		/// </summary>
		/// <param name='color1'>
		/// Starting color.
		/// </param>
		/// <param name='color2'>
		/// Ending color.
		/// </param>
		/// <param name='freq'>
		/// Flashing frequency (times per second).
		/// </param>
		public void FlashingOn(Color color1, Color color2, float freq)
		{
			flashingColorMin = color1;
			flashingColorMax = color2;
			flashingFreq = freq;
			flashing = true;
		}
		
		/// <summary>
		/// Turn on flashing with specified frequency.
		/// </summary>
		/// <param name='f'>
		/// Flashing frequency (times per second).
		/// </param>
		public void FlashingOn(float freq)
		{
			flashingFreq = freq;
			flashing = true;
		}
		
		/// <summary>
		/// Turn off flashing.
		/// </summary>
		public void FlashingOff()
		{
			flashing = false;
		}
		
		/// <summary>
		/// Switch flashing mode.
		/// </summary>
		public void FlashingSwitch()
		{
			flashing = !flashing;
		}

		/// <summary>
		/// Set constant highlighting color.
		/// </summary>
		/// <param name='color'>
		/// Constant highlighting color.
		/// </param>
		public void ConstantParams(Color color)
		{
			constantColor = color;
		}
		
		/// <summary>
		/// Fade in constant highlighting using specified transition duration.
		/// </summary>
		/// <param name="time">
		/// Transition time.
		/// </param>
		public void ConstantOn(float time = 0.25f)
		{
			transitionTime = (time >= 0f ? time : 0f);
			transitionTarget = 1f;
		}
		
		/// <summary>
		/// Fade in constant highlighting using specified color and transition duration.
		/// </summary>
		/// <param name="color">
		/// Constant highlighting color.
		/// </param>
		/// <param name="time">
		/// Transition duration.
		/// </param>
		public void ConstantOn(Color color, float time = 0.25f)
		{
			constantColor = color;
			transitionTime = (time >= 0f ? time : 0f);
			transitionTarget = 1f;
		}

		/// <summary>
		/// Fade out constant highlighting using specified transition duration.
		/// </summary>
		/// <param name="time">
		/// Transition time.
		/// </param>
		public void ConstantOff(float time = 0.25f)
		{
			transitionTime = (time >= 0f ? time : 0f);
			transitionTarget = 0f;
		}
		
		/// <summary>
		/// Switch constant highlighting using specified transition duration.
		/// </summary>
		/// <param name="time">
		/// Transition time.
		/// </param>
		public void ConstantSwitch(float time = 0.25f)
		{
			transitionTime = (time >= 0f ? time : 0f);
			transitionTarget = (transitionTarget > 0f ? 0f : 1f);
		}

		/// <summary>
		/// Turn on constant highlighting immediately (without fading in).
		/// </summary>
		public void ConstantOnImmediate()
		{
			transitionValue = transitionTarget = 1f;
		}

        /// <summary>
        /// Turn on constant highlighting using specified color immediately (without fading in).
        /// 立即使用指定的颜色打开常量高亮(不褪色)。
        /// </summary>
        /// <param name='color'>
        /// Constant highlighting color.
        /// </param>
        public void ConstantOnImmediate(Color color)
		{
			constantColor = color;
			transitionValue = transitionTarget = 1f;
		}
		
		/// <summary>
		/// Turn off constant highlighting immediately (without fading out).
		/// </summary>
		public void ConstantOffImmediate()
		{
			transitionValue = transitionTarget = 0f;
		}
		
		/// <summary>
		/// Switch constant highlighting immediately (without fading in/out).
		/// </summary>
		public void ConstantSwitchImmediate()
		{
			transitionValue = transitionTarget = (transitionTarget > 0f ? 0f : 1f);
		}

		/// <summary>
		/// Turn off all types of highlighting (occlusion mode remains intact). 
		/// </summary>
		public void Off()
		{
			once = false;
			flashing = false;
			transitionValue = transitionTarget = 0f;
		}
		
		/// <summary>
		/// Destroy this Highlighter component.
		/// </summary>
		public void Die()
		{
			Destroy(this);
		}
		#endregion

		#region Deprecated Methods
		/// <summary>
		/// DEPRECATED. Use seeThrough property directly. Set see-through mode
		/// </summary>
		public void SeeThrough(bool state)
		{
			seeThrough = state;
		}
		
		/// <summary>
		/// DEPRECATED. Use seeThrough property directly. Enable see-through mode
		/// </summary>
		public void SeeThroughOn()
		{
			seeThrough = true;
		}
		
		/// <summary>
		/// DEPRECATED. Use seeThrough property directly. Disable see-through mode
		/// </summary>
		public void SeeThroughOff()
		{
			seeThrough = false;
		}
		
		/// <summary>
		/// DEPRECATED. Use seeThrough property directly. Switch see-through mode
		/// </summary>
		public void SeeThroughSwitch()
		{
			seeThrough = !seeThrough;
		}

		/// <summary>
		/// DEPRECATED. Use occluder property directly. Enable occluder mode. Non-see-through occluders will be used only in case frame depth buffer is not accessible.
		/// </summary>
		public void OccluderOn()
		{
			occluder = true;
		}
		
		/// <summary>
		/// DEPRECATED. Use occluder property directly. Disable occluder mode. Non-see-through occluders will be used only in case frame depth buffer is not accessible.
		/// </summary>
		public void OccluderOff()
		{
			occluder = false;
		}
		
		/// <summary>
		/// DEPRECATED. Use occluder property directly. Switch occluder mode. Non-see-through occluders will be used only in case frame depth buffer is not accessible.
		/// </summary>
		public void OccluderSwitch()
		{
			occluder = !occluder;
		}
		#endregion
	}
}