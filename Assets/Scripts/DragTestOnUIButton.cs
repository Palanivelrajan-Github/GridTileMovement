using UnityEngine;
using UnityEngine.EventSystems;

public class DragTestOnUIButton : MonoBehaviour,IDragHandler
{
   
    
    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector2(Input.mousePosition.x,Input.mousePosition.y);
        
    }

   
}
