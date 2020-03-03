﻿using System.Collections;
using System.Linq;
using PixelCrushers.DialogueSystem;
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

	private bool hasShownSubtitle;
	private int lastSpeakerId;
	private StandardUISubtitlePanel lastSubtitlePanel;
	private StandardUIMenuPanel lastMenuPanel;
	private Coroutine typeWriterDelay;
	private float typeWriterSpeed;
	private DialogueFillingButton lastSelectedDialogueButton;

	public override void Start()
	{
		var actor = DialogueManager.masterDatabase.GetActor("Player");
		playerImage.sprite = actor.spritePortrait;
		playerName.text = actor.fields.First(f => f.title == "Display Name").value;
		playerPortrait.SetActive(true);
		base.Start();
	}

	public override void Update()
	{
		base.Update();
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
	}

	public override void Open()
	{
		conversationUIElements.defaultPCSubtitlePanel.gameObject.SetActive(false);
		conversationUIElements.defaultNPCSubtitlePanel.gameObject.SetActive(false);
		conversationUIElements.defaultMenuPanel.gameObject.SetActive(false);
		base.Open();
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
		var isSameSpeakerSamePanel = panel == lastSubtitlePanel && subtitle.speakerInfo.id == lastSpeakerId &&
		                             lastMenuPanel == null;
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
		conversationUIElements.defaultMenuPanel.pcImage = playerImage;
		base.ShowResponses(subtitle, responses, timeout);
		if (lastSubtitlePanel)
		{
			lastSubtitlePanel.Close();
		}

		lastSpeakerId = -1;
		lastSubtitlePanel = null;
		lastMenuPanel = conversationUIElements.defaultMenuPanel;
		foreach (var responseButton in lastMenuPanel.instantiatedButtons.Select(b =>
			b.GetComponent<HeartRhythmResponseButton>()))
		{
			responseButton.fillingButton.ResetFill();
			responseButton.fillingButton.Deselect();
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
	}
}