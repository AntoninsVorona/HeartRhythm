using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.UnityGUI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HeartRhythmDialogueUI : StandardDialogueUI
{
	private const float DELAY = 0.55f;

	[SerializeField]
	private GameObject playerPortrait;

	[SerializeField]
	private Image playerImage;

	[SerializeField]
	private TextMeshProUGUI playerName;

	[SerializeField]
	private ScrollRect responseRect;

	[SerializeField]
	private AudioClip playerSound;

	[SerializeField]
	private AudioClip headsetSound;

	private bool hasShownSubtitle;
	private int lastSpeakerId;
	private StandardUISubtitlePanel lastSubtitlePanel;
	private StandardUIMenuPanel lastMenuPanel;
	private Coroutine typeWriterDelay;
	private float typeWriterSpeed;
	private DialogueFillingButton lastSelectedDialogueButton;
	private bool scan;
	private Actor playerActor;
	private AbstractTypewriterEffect playerTypeWriter;
	private AbstractTypewriterEffect headsetTypeWriter;
	private List<DialogueFillingButton> currentButtons;

	public override void Awake()
	{
		playerActor = DialogueManager.masterDatabase.GetActor("Player");
		playerName.text = playerActor.fields.First(f => f.title == "Display Name").value;
		playerPortrait.SetActive(true);
		base.Awake();
		playerTypeWriter = conversationUIElements.defaultPCSubtitlePanel.GetTypewriter();
		headsetTypeWriter = conversationUIElements.defaultNPCSubtitlePanel.GetTypewriter();
	}

	public override void Update()
	{
		base.Update();
		if (scan)
		{
			var hit = AbstractMainMenu.Instance.CurrentUIHit();
			if (hit)
			{
				var dialogueFillingButton = hit.GetComponentInParent<DialogueFillingButton>();
				if (dialogueFillingButton)
				{
					if (dialogueFillingButton != lastSelectedDialogueButton)
					{
						if (lastSelectedDialogueButton)
						{
							lastSelectedDialogueButton.Deselect();
						}

						lastSelectedDialogueButton = dialogueFillingButton;
						lastSelectedDialogueButton.Select();
					}

					if (Input.GetMouseButtonDown(0))
					{
						dialogueFillingButton.Click();
					}
				}
			}

			for (var i = 1; i <= currentButtons.Count; i++)
			{
				if (Input.GetKeyDown(i.ToString()))
				{
					if (lastSelectedDialogueButton)
					{
						lastSelectedDialogueButton.Deselect();
					}

					lastSelectedDialogueButton = currentButtons[i - 1];
					lastSelectedDialogueButton.Select();
					break;
				}
			}
		}
	}

	public override void Open()
	{
		conversationUIElements.defaultPCSubtitlePanel.gameObject.SetActive(false);
		conversationUIElements.defaultNPCSubtitlePanel.gameObject.SetActive(false);
		conversationUIElements.defaultMenuPanel.gameObject.SetActive(false);
		base.Open();
		UpdateHeadset();
		hasShownSubtitle = false;
		lastSpeakerId = -1;
		lastSubtitlePanel = null;
		lastMenuPanel = null;
	}

	public override void ShowSubtitle(Subtitle subtitle)
	{
		conversationUIElements.defaultPCSubtitlePanel.portraitImage = playerImage;
		var panel = subtitle.speakerInfo.isNPC
			? conversationUIElements.defaultNPCSubtitlePanel
			: conversationUIElements.defaultPCSubtitlePanel;
		var isSameSpeakerSamePanel = panel == lastSubtitlePanel && lastMenuPanel == null;
		if (hasShownSubtitle && !isSameSpeakerSamePanel)
		{
			typeWriterSpeed = panel.GetTypewriterSpeed();
			panel.SetTypewriterSpeed(0);
			typeWriterDelay = StartCoroutine(StartTypingAfterDelay(panel));
		}

		base.ShowSubtitle(subtitle);
		hasShownSubtitle = true;
		lastSpeakerId = subtitle.speakerInfo.id;
		lastSubtitlePanel = panel;
		lastMenuPanel = null;
	}

	private IEnumerator StartTypingAfterDelay(StandardUISubtitlePanel panel)
	{
		yield return new WaitForSeconds(DELAY);
		panel.SetTypewriterSpeed(typeWriterSpeed);
		panel.GetTypewriter().Start();
		typeWriterDelay = null;
	}

	public override void HideSubtitle(Subtitle subtitle)
	{
		conversationUIElements.defaultPCSubtitlePanel.portraitImage = null;
		var nextLine = DialogueManager.currentConversationState.firstNPCResponse ??
		               DialogueManager.currentConversationState.pcAutoResponse;
		var endOfConversation = !DialogueManager.currentConversationState.hasAnyResponses;
		if (hasShownSubtitle &&
		    (nextLine == null || nextLine.destinationEntry.ActorID != lastSpeakerId || endOfConversation))
		{
			base.HideSubtitle(subtitle);
		}
	}

	public override void ShowResponses(Subtitle subtitle, Response[] responses, float timeout)
	{
		scan = true;
		conversationUIElements.defaultMenuPanel.pcImage = playerImage;
		base.ShowResponses(subtitle, responses, timeout);
		UpdateHeadset();
		if (lastSubtitlePanel)
		{
			lastSubtitlePanel.Close();
		}

		lastSpeakerId = -1;
		lastSubtitlePanel = null;
		lastMenuPanel = conversationUIElements.defaultMenuPanel;
		currentButtons = lastMenuPanel.instantiatedButtons.Select(b =>
			b.GetComponent<HeartRhythmResponseButton>().fillingButton).ToList();
		foreach (var responseButton in currentButtons)
		{
			responseButton.ResetFill();
			responseButton.Deselect();
		}

		responseRect.verticalNormalizedPosition = 1;
	}

	public override void HideResponses()
	{
		conversationUIElements.defaultMenuPanel.pcImage = null;
		base.HideResponses();
		if (lastSelectedDialogueButton)
		{
			lastSelectedDialogueButton.Deselect();
			lastSelectedDialogueButton = null;
		}

		scan = false;
	}

	public void FastForward()
	{
		if (lastSubtitlePanel)
		{
			var typewriter = lastSubtitlePanel.GetTypewriter();
			if (typeWriterDelay == null)
			{
				if (typewriter.isPlaying)
				{
					typewriter.Stop();
				}
				else
				{
					OnContinue();
				}
			}
		}
		else if (lastSelectedDialogueButton)
		{
			lastSelectedDialogueButton.Click();
		}
	}

	private void UpdateHeadset()
	{
		playerImage.sprite =
			SaveSystem.currentGameSave.globalVariables.wearsHeadset
				? playerActor.spritePortrait
				: playerActor.GetPortraitSprite(2);
	}

	public void AssignAudioClips()
	{
		playerTypeWriter.audioClip = playerSound;
		playerTypeWriter.audioSource.clip = playerSound;
		headsetTypeWriter.audioClip = headsetSound;
		headsetTypeWriter.audioSource.clip = headsetSound;
	}

	public void RemoveAudioClips()
	{
		playerTypeWriter.audioClip = null;
		playerTypeWriter.audioSource.clip = null;
		headsetTypeWriter.audioClip = null;
		headsetTypeWriter.audioSource.clip = null;
	}
}