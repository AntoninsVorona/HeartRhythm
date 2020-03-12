using System;
using UnityEngine;

public class YesNoDialogue : MonoBehaviour
{
	[SerializeField]
	private Canvas canvas;

	[SerializeField]
	private FillingButton yes;

	[SerializeField]
	private FillingButton no;

	private FillingButton selected;
	private Action callback;
	private bool scan;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(this);
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
			return;
		}
	
		Hide();
	}

	public void Show(Action callback)
	{
		canvas.gameObject.SetActive(true);
		yes.ResetFill();
		no.ResetFill();
		yes.Deselect();
		no.Deselect();
		selected = null;
		this.callback = callback;
		scan = true;
	}

	public void Hide()
	{
		yes.Deselect();
		no.Deselect();
		selected = null;
		canvas.gameObject.SetActive(false);
		scan = false;
	}

	private void Update()
	{
		if (scan)
		{
			var hit = AbstractMainMenu.Instance.CurrentUIHit();
			if (hit)
			{
				var button = hit.GetComponentInParent<FillingButton>();
				if (button)
				{
					if (button != selected)
					{
						if (selected)
						{
							selected.Deselect();
						}

						selected = button;
						selected.Select();
					}

					if (Input.GetMouseButtonDown(0))
					{
						if (selected == yes)
						{
							callback();
						}
						
						Hide();
					}
				}
				else if (selected)
				{
					selected.Deselect();
					selected = null;
				}
			}
			else if (selected)
			{
				selected.Deselect();
				selected = null;
			}
		}
	}
	
	public static YesNoDialogue Instance { get; private set; }
}