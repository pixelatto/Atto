using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Elendow.SpritedowAnimator;

namespace PowerTools
{
[RequireComponent(typeof(SpriteAnimator))]
[DisallowMultipleComponent]
public class PowerSpriteAnim : PowerSpriteAnimEventHandler 
{	
	#region Definitions

	static readonly string ANIMATION_NAME = "";
	static readonly string CONTROLLER_PATH = "SpriteAnimController";

	#endregion
	#region Vars: Editor

	[SerializeField] SpriteAnimation m_defaultAnim = null;

	#endregion
	#region Vars: Private

	static RuntimeAnimatorController m_sharedAnimatorController = null;

	SpriteAnimator m_animator = null;

	#if UNITY_5_6_OR_NEWER
		List< KeyValuePair<SpriteAnimation, SpriteAnimation> > m_clipPairList = new List< KeyValuePair<SpriteAnimation, SpriteAnimation> >(1);
#else
	SpriteAnimationPair[] m_clipPairArray = null;
#endif

	SpriteAnimation m_currAnim = null;
	int m_fps = 1;

	#endregion
	#region Funcs: Properties


	/// True if an animation is currently playing (even if paused)
	public bool Playing { get { return IsPlaying(); } }

	/// Property for pausing or resuming the currently playing animation
	public bool Paused
	{ 
		get { return IsPaused(); } 
		set 
		{
			if ( value == true ) Pause();
			else Resume();
		}
	}

	/// Property for setting the playback speed
	public int Fps { get { return m_fps; } set { SetSpeed(value); } }

	/// Property for setting/getting the current playback time of the animation
	public float Time { get { return GetTime(); } set { SetTime( value); } }

	/// Property to get or set the the normalized time (between 0.0 to 1.0 from start to end of anim) of the currently playing clip
	public float NormalizedTime { get { return GetNormalisedTime(); } set { SetNormalizedTime( value); } }

	/// The currently playing animation clip
	public SpriteAnimation Clip { get { return m_currAnim; } }

	/// The name of the currently playing animation clip
	public string ClipName { get { return m_currAnim != null ? m_currAnim.name : string.Empty; } }

	#endregion
	#region Funcs: Public 

	/// Plays the specified clip
	public void Play(SpriteAnimation anim, int speed = 15 ) 
	{
		if ( anim == null )
			return;

		if ( m_animator.enabled == false )
			m_animator.enabled = true;

		#if UNITY_5_6_OR_NEWER			
			m_clipPairList[0] = new KeyValuePair<SpriteAnimation, SpriteAnimation>(m_clipPairList[0].Key, anim);
		#else
			m_clipPairArray[0].overrideClip = anim;
			m_controller.clips = m_clipPairArray;
		#endif
		m_animator.SetAnimationTime(0.0f); // Update so that new clip state is reset before hitting play
		m_animator.Play(ANIMATION_NAME);
		m_fps = Mathf.Max(0,speed);
		m_animator.UseAnimatorFPS(m_fps);
		m_currAnim = anim;
	}		 	 

	/// Stops the clip by disabling the animator
	public void Stop()
	{		
		if ( m_animator != null )
			m_animator.enabled = false;
	}

	/// Pauses the animation. Call Resume to start again
	public void Pause()
	{
		if ( m_animator != null )
			m_animator.UseAnimatorFPS(0);
	}

	/// Resumes animation playback at previous speed
	public void Resume()
	{
		if ( m_animator != null )
				m_animator.UseAnimatorFPS(m_fps);
	}

	/// Returns the currently playing clip
	public SpriteAnimation GetCurrentAnimation() { return m_currAnim; }

	///  Returns true if the passed clip is playing. If no clip is passed, returns true if ANY clip is playing
	public bool IsPlaying(SpriteAnimation clip = null) 
	{			
		// Check there's a curr anim, and the animator is enabled
		if ( m_currAnim == null || m_animator.enabled == false )
			return false;

		// Check the specified clip is not the one playing
		if ( clip != null && m_currAnim != clip )
			return false; 

		// If not looping, check the end of the anim hasn't been reached
		//if ( m_currAnim.isLooping == false )
		//	return (float)m_animator.CurrentAnimationTime / (float)(m_animator.PlayingAnimation.AnimationDuration / m_fps) < 1.0f;	
					
		return true;
	} 

	/// Returns true if a clip with the specified name is playing
	public bool IsPlaying(string animName) 
	{ 
		// Check there's a curr anim, and it has the correct name
		if ( m_currAnim == null ||  m_currAnim.name != animName )
			return false;

		// Check if it's currently playing
		return IsPlaying(m_currAnim);
	} 

	public bool IsPaused()
	{
		if ( m_currAnim == null )
			return false;
		return m_fps == 0;		
	}

	/// Sets the current animation playback speed
	public void SetSpeed(int speed)
	{
		m_fps = Mathf.Max(0, speed);
		m_animator.UseAnimatorFPS(m_fps);
	}

	/// Returns the current animation playback speed
	public float GetSpeed() { return m_fps; }

	/// Returns the time of the currently playing clip (or zero if no clip is playing)
	public float GetTime()
	{ 
		if ( m_currAnim != null )
			return m_animator.CurrentAnimationTime;
		return 0;
	}

	/// Property for setting/getting the current playback time of the animation
	public void SetTime( float time )
	{
		if ( m_currAnim == null || m_currAnim.FramesCount <=  0 )
			return;
		SetNormalizedTime(time / ((float)m_currAnim.FramesCount / (float)m_fps));
	}


	/// Returns the normalized time (between 0.0 and 1.0) of the currently playing clip (or zero if no clip is playing)
	public float GetNormalisedTime()
	{
			if (m_currAnim != null)
				return (float)m_animator.CurrentAnimationTime / (float)(m_animator.PlayingAnimation.AnimationDuration / m_fps);
		return 0;
	}

	/// Property to get or set the the normalized time (between 0.0 to 1.0 from start to end of anim) of the currently playing clip
	public void SetNormalizedTime( float ratio )
	{
		if ( m_currAnim == null )
			return;
		m_animator.Play(ANIMATION_NAME);
		m_animator.SetAnimationTime(ratio * m_animator.PlayingAnimation.AnimationDuration);
	}

	#endregion
	#region Funcs: Init

	void Awake()
	{
		if ( m_sharedAnimatorController == null )
		{
			// Lazy load the shared animator controller
			m_sharedAnimatorController = Resources.Load<RuntimeAnimatorController>(CONTROLLER_PATH);
		}

		m_animator = GetComponent<SpriteAnimator>();

		#if UNITY_5_6_OR_NEWER

		#else
			m_clipPairArray = m_controller.clips;
		#endif

		Play(m_defaultAnim);
	}

	// Called when component is first added. Used to add the sprite renderer
	void Reset()
	{		
		// NB: Doing this here rather than using the RequireComponent Attribute means we can add a UI.Image instead if it's a UI Object
		if ( GetComponent<RectTransform>() == null )
		{
			// It's a regular sprite, add the sprite renderer component if it doesn't already exist
			if ( GetComponent<SpriteRenderer>() == null )
			{
				gameObject.AddComponent<SpriteRenderer>();
			}
		}
		else 
		{
			// It's a UI Image, so add the Image component if it doesn't already exist
			if ( GetComponent<UnityEngine.UI.Image>() == null )
			{
				gameObject.AddComponent<UnityEngine.UI.Image>();
			}
		}

	}


	#endregion

}

}