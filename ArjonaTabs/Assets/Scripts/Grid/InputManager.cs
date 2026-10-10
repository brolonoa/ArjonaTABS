using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayerMask;

    private Vector3 lastPosition;

    public event Action OnClick;
    public event Action OnExit;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnClick?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnExit?.Invoke();
        }
    }

    public bool IsPointerOverUIObject()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public Vector3 GetSelectedMapPosition()
    {
        Vector3 mousePosition = Input.mousePosition;

        mousePosition.z = sceneCamera.nearClipPlane;

        Ray ray = sceneCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;

        if (Physics.Raycast(
                ray,
                out hit,
                100,
                placementLayerMask))
        {
            lastPosition = hit.point;
        }

        return lastPosition;
    }
    public bool TryGetClickedObject(out GameObject hitObject)
    {
        Ray ray = sceneCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100, placementLayerMask))
        {
            hitObject = hit.collider.gameObject;
            return true;
        }

        hitObject = null;
        return false;
    }
}
