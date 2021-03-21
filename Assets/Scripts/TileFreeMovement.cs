using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TileFreeMovement : MonoBehaviour
{
    private const float TimeToMove = 0.17f;
    public Text text;

    public LayerMask borderLayerMask;

    public Sprite[] Sprites;
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

            if (_hit.collider != null && _hit.collider.CompareTag("Number")) StartCoroutine(MoveTile(Vector2.left));
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


        if (!Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask))
        {
            while (elapsedTime < TimeToMove)
            {
                _hit.transform.position = Vector2.Lerp(_oriPos, _tarPos, elapsedTime / TimeToMove);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _hit.transform.position = _tarPos;
            _isMoving = false;
        }
        else
        {
            Debug.Log(_hit.transform.name);

            Debug.Log(Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name);


            if (_hit.transform.name ==
                Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name)
            {
                while (elapsedTime < TimeToMove)
                {
                    _hit.transform.position = Vector2.Lerp(_oriPos, _tarPos, elapsedTime / TimeToMove);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                _hit.transform.position = _tarPos;
                _hit.transform.gameObject.SetActive(false);

                if (Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name ==
                    "Number (1)")
                {
                    Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name =
                        "Number (2)";
                    Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.gameObject
                        .GetComponent<SpriteRenderer>().sprite = Sprites[2];
                }
                else if (Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name ==
                         "Number (2)")
                {
                    Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name =
                        "Number (4)";
                    Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.gameObject
                        .GetComponent<SpriteRenderer>().sprite = Sprites[3];
                }
                else if (Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name ==
                         "Number (4)")
                {
                    Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name =
                        "Number (8)";
                    Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.gameObject
                        .GetComponent<SpriteRenderer>().sprite = Sprites[4];
                }
                else if (Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name ==
                         "Number (8)")
                {
                    Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name =
                        "Number (16)";
                    Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.gameObject
                        .GetComponent<SpriteRenderer>().sprite = Sprites[5];
                }
            }


            _isMoving = false;
        }
    }
}