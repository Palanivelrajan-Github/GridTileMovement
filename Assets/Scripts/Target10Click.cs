using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target10Click : MonoBehaviour
{
    private const float TimeToMove = 0.2f;
    public LayerMask borderLayerMask;
    private bool _isMoving;
    private Vector2 _oriPos;
    private RaycastHit2D _startHitRaycastHit2D;
    private List<Collider2D> _target10Click = new List<Collider2D>();
    private Vector2 _tarPos;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !_isMoving)
        {
            _startHitRaycastHit2D =
                Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (_startHitRaycastHit2D.collider != null && _startHitRaycastHit2D.collider.CompareTag("Number"))
            {
                
                
                _target10Click = new List<Collider2D>();

                _target10Click.Add(_startHitRaycastHit2D.collider);

                Debug.Log("target 10 selected");
                // StartCoroutine(MoveTile(Vector2.left));
                _startHitRaycastHit2D.collider.transform.GetComponent<BoxCollider2D>().enabled = false;
                _startHitRaycastHit2D.collider.transform.GetComponent<SpriteRenderer>().color =
                    new Color(1.0f, 1.0f, 1.0f, 0.0f);
                foreach (Transform child in _startHitRaycastHit2D.collider.transform)
                {
                    child.GetComponent<BoxCollider2D>().enabled = true;
                    child.GetComponent<SpriteRenderer>().color =
                        new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
                
                
                /*
                if (_target10Click.Count == 1)
                {
                    foreach (Transform child in _startHitRaycastHit2D.collider.transform)
                    {
                        child.GetComponent<BoxCollider2D>().enabled = true;
                        child.GetComponent<SpriteRenderer>().color =
                            new Color(1.0f, 1.0f, 1.0f, 1.0f);
                    }
                    _startHitRaycastHit2D.collider.transform.GetComponent<BoxCollider2D>().enabled = true;
                    _startHitRaycastHit2D.collider.transform.GetComponent<SpriteRenderer>().color =
                        new Color(1.0f, 1.0f, 1.0f, 1.0f);
                }
                else
                {
                    
                }*/
            }
            else if (_startHitRaycastHit2D.collider == null)
            {
                if (_target10Click.Count == 1)
                {
                    _target10Click[0].transform.GetComponent<SpriteRenderer>().color =
                        new Color(1.0f, 1.0f, 1.0f, 1.0f);
                   
                    foreach (Transform child in _target10Click[0].transform)
                    {
                        child.GetComponent<BoxCollider2D>().enabled = false;
                        child.GetComponent<SpriteRenderer>().color =
                            new Color(1.0f, 1.0f, 1.0f, 0.0f);
                    }
                    _target10Click[0].transform.GetComponent<BoxCollider2D>().enabled = true;
                    
                }
            }
        }
    }

    
    private IEnumerator MoveTile(Vector2 direction)
    {
        _isMoving = true;
        float elapsedTime = 0;
        _oriPos = _startHitRaycastHit2D.transform.position;
        _tarPos = _oriPos + direction;

        if (!Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask))
        {
            while (elapsedTime < TimeToMove)
            {
                _startHitRaycastHit2D.transform.GetChild(0).transform.position =
                    Vector2.Lerp(_oriPos, _tarPos, elapsedTime / TimeToMove);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _startHitRaycastHit2D.transform.GetChild(0).transform.position = _tarPos;
            _isMoving = false;
        }
    }
}