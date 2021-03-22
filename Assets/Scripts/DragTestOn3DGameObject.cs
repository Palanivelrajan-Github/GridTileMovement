using UnityEngine;

public class DragTestOn3DGameObject : MonoBehaviour
{
    private Touch _touch;

    private void Update()
    {
        if (Input.touchCount <= 0) return;
        _touch = Input.GetTouch(0);

        if (_touch.phase != TouchPhase.Began && _touch.phase != TouchPhase.Moved) return;
        var ray = Camera.main.ScreenPointToRay(_touch.position);
        transform.position = new Vector3(ray.origin.x, ray.origin.y, 0);
    }


    //the script must attach with game object and it should have collider
    /*private void OnMouseDrag()
    {
        Debug.Log("On Mouse Drag");
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        transform.position = new Vector3(ray.origin.x, ray.origin.y, 0);
    }*/
}