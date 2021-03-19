using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TileFreeMovement : MonoBehaviour
{
    private const float TimeToMove = 0.2f;
    public Text text;
    private float _directionXValue;
    private float _directionYValue;
    private bool _isMoving;
    private Vector2 _oriPos, _tarPos;
    private Touch _touch;
    private Vector2 _touchStartPosition, _touchEndPosition;

    private void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.W)) MoveTile(Vector3.up);
        else if (Input.GetKeyDown(KeyCode.S)) MoveTile(Vector3.down);
        else if (Input.GetKeyDown(KeyCode.D)) MoveTile(Vector3.right);
        else if (Input.GetKeyDown(KeyCode.A)) MoveTile(Vector3.left);
        */

        if (Input.touchCount > 0)
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

                    _touchEndPosition = _touch.position;
                    var x = _touchEndPosition.x - _touchStartPosition.x;
                    var y = _touchEndPosition.y - _touchStartPosition.y;
                    _directionXValue = x;
                    _directionYValue = y;

                    if (Mathf.Abs(x) > Mathf.Abs(y) && !_isMoving)
                    {
                        if (x > 0.0f)
                            StartCoroutine(MoveTile(Vector2.right));
                        else if (x < 0.0f)
                            StartCoroutine(MoveTile(Vector2.left));
                    }
                    else if (Mathf.Abs(x) < Mathf.Abs(y) && !_isMoving)
                    {
                        if (y > 0.0f)
                            StartCoroutine(MoveTile(Vector2.up));
                        else if (y < 0.0f)
                            StartCoroutine(MoveTile(Vector2.down));
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
        _oriPos = transform.position;
        _tarPos = _oriPos + direction;

        while (elapsedTime < TimeToMove)
        {
            transform.position = Vector2.Lerp(_oriPos, _tarPos, elapsedTime / TimeToMove);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = _tarPos;
        _isMoving = false;

        //transform.Translate(direction);
    }
}