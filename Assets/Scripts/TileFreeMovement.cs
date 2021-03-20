using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TileFreeMovement : MonoBehaviour
{
    private const float TimeToMove = 0.15f;
    public Text text;
    private float _directionXValue;
    private float _directionYValue;

    private RaycastHit2D _hit;
    private bool _isMoving;
    private Vector2 _oriPos, _tarPos;
    private Touch _touch;
    private Vector2 _touchStartPosition, _touchEndPosition;


    private void Update()
    {
#if UNITY_EDITOR

        if (Input.GetMouseButtonDown(0) && !_isMoving)
        {
            _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (_hit.collider != null) StartCoroutine(MoveTile(Vector2.left));
        }

#endif


        if (Input.touchCount > 0 && !_isMoving)
        {
            _touch = Input.GetTouch(0);

            switch (_touch.phase)
            {
                case TouchPhase.Began:
                    _touchStartPosition = _touch.position;
                    break;

                case TouchPhase.Canceled:
                    break;

                case TouchPhase.Ended:


                    _hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(_touchStartPosition), Vector2.zero);

                    if (_hit.collider != null)
                        if (_hit.collider.CompareTag("Number"))
                        {
                            _touchEndPosition = _touch.position;
                            var x = _touchEndPosition.x - _touchStartPosition.x;
                            var y = _touchEndPosition.y - _touchStartPosition.y;
                            _directionXValue = x;
                            _directionYValue = y;

                            if (Mathf.Abs(x) > Mathf.Abs(y))
                            {
                                if (x > 0.0f)
                                    StartCoroutine(MoveTile(Vector2.right));
                                else if (x < 0.0f)
                                    StartCoroutine(MoveTile(Vector2.left));
                            }
                            else if (Mathf.Abs(x) < Mathf.Abs(y))
                            {
                                if (y > 0.0f)
                                    StartCoroutine(MoveTile(Vector2.up));
                                else if (y < 0.0f)
                                    StartCoroutine(MoveTile(Vector2.down));
                            }
                        }

                    break;


                case TouchPhase.Moved:
                    _touchEndPosition = _touch.position;
                    break;

                case TouchPhase.Stationary:
                    break;


                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        text.text = "X:" + _directionXValue + " " + "Y:" + _directionYValue;
    }

    private IEnumerator MoveTile(Vector2 direction)
    {
        _isMoving = true;
        float elapsedTime = 0;
        _oriPos = _hit.transform.position;
        _tarPos = _oriPos + direction;

        while (elapsedTime < TimeToMove)
        {
            _hit.transform.position = Vector2.Lerp(_oriPos, _tarPos, elapsedTime / TimeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _hit.transform.position = _tarPos;
        _isMoving = false;
    }
}