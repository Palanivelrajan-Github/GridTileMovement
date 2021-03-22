using UnityEngine;

public class DragTestOn3DGameObject : MonoBehaviour
{
    private RaycastHit2D _raycastHit2D;
    private Touch _touch;

    private void Update()
    {
        if (Input.touchCount <= 0) return;
        _touch = Input.GetTouch(0);

        if (_touch.phase != TouchPhase.Began && _touch.phase != TouchPhase.Moved) return;
        _raycastHit2D =
            Physics2D.Raycast(Camera.main.ScreenToWorldPoint(_touch.position), Vector2.zero);
        var ray = Camera.main.ScreenPointToRay(_touch.position);

        if (_raycastHit2D.collider != null && _raycastHit2D.collider.transform.name == "Cube")
            transform.position = new Vector3(ray.origin.x, ray.origin.y, 0);
    }


    //the script must attach with game object and it should have collider
    private void OnMouseDrag()
    {
        Debug.Log("On Mouse Drag");
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        transform.position = new Vector3(ray.origin.x, ray.origin.y, 0);
    }
}