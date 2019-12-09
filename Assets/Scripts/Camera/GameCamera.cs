using System;
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;

public class GameCamera : MonoBehaviour
{
	[SerializeField]
	private Vector3 offset;

	private Vector3 targetPosition;
	[HideInNormalInspector]
	public new Camera camera;
	private PixelPerfectCamera pixelPerfectCamera;

	[Header("Zoom")]
	public AnimationCurve defaultZoomCurve;

	public AnimationCurve danceMoveZoonInCurve;
	private float danceMoveZoomIn;
	private float defaultZoom;
	private Coroutine zoomCoroutine;

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
		camera = GetComponent<Camera>();
		pixelPerfectCamera = GetComponent<PixelPerfectCamera>();
	}

	private IEnumerator Start()
	{
		yield return null;
		yield return null;
		defaultZoom = camera.orthographicSize;
		danceMoveZoomIn = defaultZoom / 3;
	}

	private void LateUpdate()
	{
		var newPosition = Vector3.Lerp(transform.position, targetPosition,
			20 * Time.deltaTime);
		newPosition.z = offset.z;
		transform.position = newPosition;
	}

	public void ChangeTargetPosition(Vector3 targetPosition, bool force = false)
	{
		this.targetPosition = targetPosition + offset;
		if (force)
		{
			transform.position = this.targetPosition;
		}
	}

	public void DanceMoveZoomIn()
	{
		pixelPerfectCamera.enabled = false;
		camera.orthographicSize = defaultZoom;
		ChangeZoom(danceMoveZoomIn, 0.5f, danceMoveZoonInCurve);
	}

	public void ZoomOut()
	{
		ChangeZoom(defaultZoom, 0.5f, null, () => pixelPerfectCamera.enabled = true);
	}

	public void ChangeZoom(float newZoom, float time, AnimationCurve animationCurve = null, Action callback = null)
	{
		if (zoomCoroutine != null)
		{
			StopCoroutine(zoomCoroutine);
		}

		zoomCoroutine = StartCoroutine(ZoomTo());

		IEnumerator ZoomTo()
		{
			if (animationCurve == null)
			{
				animationCurve = defaultZoomCurve;
			}

			var start = camera.orthographicSize;
			float t = 0;
			while (t < 1)
			{
				yield return null;
				t += Time.deltaTime / time;
				if (t > 1)
				{
					t = 1;
				}

				camera.orthographicSize = Mathf.Lerp(start, newZoom, animationCurve.Evaluate(t));
			}

			callback?.Invoke();
		}
	}
	
	public static GameCamera Instance { get; private set; }
}