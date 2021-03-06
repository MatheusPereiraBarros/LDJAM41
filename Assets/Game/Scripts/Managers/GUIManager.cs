using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using System;

namespace MoreMountains.SoccerRacing
{	
	public class GUIManager : MonoBehaviour
	{
		public bool ListenToEvents = true;

		[Header("Essentials")]
		public CanvasGroup Fader;

		[Header("Start")]
		public Text CountdownText;
		public Text CheckpointsText;
		public MMProgressBar EnergyBar;
		public Animator EnergyBarAnimator;

		[Header("Game view")]
		public Text TimerText;
		public Animator TimerAnimator;

		[Header("Start Screen")]
		public CanvasGroup StartScreen;
		public GameObject OptionsScreen;
		public GameObject HowToPlayScreen;

		[Header("Game Over Screen")]
		public CanvasGroup GameOverScreen;
		public Text GameOverStats;
		public Text GameOverScore;

		[Header("Pause Menu")]
		public GameObject PauseButton;
		public CanvasGroup PauseScreen;
		public Text PauseScore;
		public Text PauseScoreHS;
		public Text PauseLevel;
		public Text PauseLevelHS;

	    protected static GUIManager _instance;

		protected Animator _underlootAnimator;
		protected Animator _transitionAnimator;
		protected WaitForSeconds TimerUpdateDelay;

		protected int _totalCheckpoints;

		public static GUIManager Instance
		{
			get
			{
				if(_instance == null)
					_instance = GameObject.FindObjectOfType<GUIManager>();
				return _instance;
			}
		}

		protected virtual void Start()
		{
			Initialization ();
		}

		public virtual void Initialization()
		{
			SetPause (false);
			StartCoroutine (UpdateTimer ());
			TimerUpdateDelay = new WaitForSeconds (1f);
			_totalCheckpoints = GameManager.Instance.Checkpoints.Length;
	    }

		protected virtual IEnumerator UpdateTimer()
		{
			while (true)
			{
				if (GameManager.Instance.GameState.CurrentState == GameStates.GameInProgress)
				{
					var timeSinceStart = GameManager.Instance.TimeSinceStart;
					var seconds = Mathf.Floor(timeSinceStart % 60);
					var minutes = Mathf.Floor(timeSinceStart / 60);
					TimerText.text = String.Format("{0:00}:{1:00}", minutes, seconds);
					TimerAnimator.SetTrigger ("Bump");	
				}

				yield return TimerUpdateDelay;	
			}
		}

		protected virtual void Update()
		{
			EnergyBar.UpdateBar (100 - GameManager.Instance.Energy, 0f, 100f);
			int currentCheckpoint = GameManager.Instance.CurrentCheckpoint;
			currentCheckpoint++;
			if (currentCheckpoint < _totalCheckpoints)
			{
				CheckpointsText.text = "CHECKPOINTS " + currentCheckpoint + "/" + _totalCheckpoints;
			}
			else
			{
				CheckpointsText.text = "ALL CHECKPOINTS CLEAR, GO FOR THE GOAL!";
			}
		}

		public virtual void BumpEnergyBar()
		{
			EnergyBarAnimator.SetTrigger ("Bump");
		}

		public virtual void SetCountdownTextStatus(bool status)
		{
			CountdownText.gameObject.SetActive(status);
		}

		public virtual void SetCountdownText(string newText)
		{
			CountdownText.text = newText;
		}

		public virtual void SetGUILevel()
		{
			PauseLevel.text = GameManager.Instance.CurrentLevel.ToString ();
		}

		public virtual void SetPause(bool state)
		{
			if (PauseScreen == null)
			{
				return;
			}
			PauseScreen.gameObject.SetActive (state);
			PauseScreen.alpha = state ? 1 : 0; 
			PauseScreen.interactable = state;
			PauseScreen.blocksRaycasts = state;
		}

		public virtual void SetHUDActive(bool status)
		{
			PauseButton.gameObject.SetActive (status);
			TimerText.gameObject.SetActive (status);
			CheckpointsText.gameObject.SetActive (status);
			EnergyBar.gameObject.SetActive (status);
		}

		public virtual void SetStartScreen(bool status) 
		{
			OptionsScreen.SetActive (true);
			HowToPlayScreen.SetActive (true);
			StartScreen.gameObject.SetActive (status);
			SetHUDActive (!status);
		}

		public virtual void SetGameOverScreen(bool state)
		{
			GameOverScreen.gameObject.SetActive(state);
			var timeSinceStart = GameManager.Instance.TimeSinceStart;
			var seconds = Mathf.Floor(timeSinceStart % 60);
			var minutes = Mathf.Floor(timeSinceStart / 60);
			TimerText.text = String.Format("{0:00}:{1:00}", minutes, seconds);
			GameOverStats.text = String.Format("CONGRATULATIONS, \nYOU FINISHED THE RACE AND SCORED IN {0:00}:{1:00}", minutes, seconds);
		}
				
		public virtual void FaderOn(bool state,float duration, bool unscaled = true)
		{
	        if (Fader== null)
			{
				return;
	        }
			Fader.gameObject.SetActive(true);
			if (state)
			{
				Fader.alpha = 0f;
				StartCoroutine(MMFade.FadeCanvasGroup(Fader,duration, 1f, unscaled));
			}				
			else
			{
				Fader.alpha = 1f;
				StartCoroutine(MMFade.FadeCanvasGroup(Fader,duration,0f, unscaled));
				StartCoroutine (TurnFaderOff (duration));
			}				
		}		

		protected virtual IEnumerator TurnFaderOff(float duration)
		{
			yield return new WaitForSeconds (duration + 0.2f);
			Fader.gameObject.SetActive (false);
		}

		protected virtual void OnEnable()
		{
			
		}

		protected virtual void OnDisable()
		{
			
		}
	}
}