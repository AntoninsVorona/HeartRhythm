using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraUtility : MonoBehaviour
{
    private new Camera camera;
    private Vector3 crossHairPos = new Vector3((float) Screen.width / 2, (float) Screen.height / 2);
    private CustomStandaloneInputModule inputModule;
    private int tileLayerMask;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        camera = GetComponent<Camera>();
        tileLayerMask = LayerMask.GetMask("Tile");
    }

    private void Start()
    {
        if (inputModule == null)
        {
            inputModule = EventSystem.current.GetComponent<CustomStandaloneInputModule>();
        }
    }

    public RaycastHit GetRaycastHitFromMouse(string layer, float range = Mathf.Infinity)
    {
        if (inputModule.IsPointerOverUI())
        {
            return default;
        }

        RaycastHit hit;
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, range, LayerMask.GetMask(layer));
        return hit;
    }

    public RaycastHit GetTileFromMouse(float range = Mathf.Infinity)
    {
        if (inputModule.IsPointerOverUI())
        {
            return default;
        }

        RaycastHit hit;
        var ray = camera.ScreenPointToRay(Input.mousePosition);
        Physics.Raycast(ray, out hit, range, tileLayerMask);
        return hit;
    }

    public static CameraUtility Instance { get; private set; }
}