using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TileFreeMovement : MonoBehaviour
{
    private const float TimeToMove = 5.17f;
    public Text text;

    public LayerMask borderLayerMask;

    public Sprite[] sprites;

    public Camera mainCamera;
    private RaycastHit2D _endHitRaycastHit2D;
    private bool _isMoving;
    private Vector2 _oriPos, _tarPos;
    private RaycastHit2D _startHitRaycastHit2D;

    private Touch _touch;
    private Vector2 _touchStartPosition, _touchEndPosition;

    private void Update()
    {
#if UNITY_EDITOR


        if (Input.GetMouseButtonDown(0) && !_isMoving)
        {
            _startHitRaycastHit2D =
                Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);


            if (_startHitRaycastHit2D.collider != null && _startHitRaycastHit2D.collider.CompareTag("Number"))
                StartCoroutine(MoveTile(Vector2.left));
        }

#endif


        if (Input.touchCount <= 0 || _isMoving) return;
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

                _startHitRaycastHit2D =
                    Physics2D.Raycast(mainCamera.ScreenToWorldPoint(_touchStartPosition), Vector2.zero);

                _endHitRaycastHit2D =
                    Physics2D.Raycast(mainCamera.ScreenToWorldPoint(_touchEndPosition), Vector2.zero);

                if (_startHitRaycastHit2D.collider != null &&
                    _startHitRaycastHit2D.collider != _endHitRaycastHit2D.collider)
                    if (_startHitRaycastHit2D.collider.CompareTag("Number"))
                    {
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


    private void OnEnable()
    {
        /*var str1 = "Number (011)";
        var str2 = "Number (10)";
        int final1;

        var Num1 = str1.Split("("[0]);
        var Num2 = str2.Split("("[0]);

        final1 = Convert.ToInt32(Num1[1].Substring(0, Num1[1].Length - 1)) +
                 Convert.ToInt32(Num2[1].Substring(0, Num2[1].Length - 1));
        // Debug.Log(final1);

        Debug.Log($"Number ({final1})");*/
    }


    private IEnumerator MoveTile(Vector2 direction)
    {
        _isMoving = true;
        float elapsedTime = 0;
        _oriPos = _startHitRaycastHit2D.transform.position;
        _tarPos = _oriPos + direction;

        if (!Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask))
        {
            yield return null;
            _isMoving = false;

            /*while (elapsedTime < TimeToMove)
            {
                _startHitRaycastHit2D.transform.position = Vector2.Lerp(_oriPos, _tarPos, elapsedTime / TimeToMove);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _startHitRaycastHit2D.transform.position = _tarPos;
            _isMoving = false;*/
        }
        else
        {
            var transformGameObjectName = _startHitRaycastHit2D.transform.name;
            var colliderGameObjectName = Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask)
                .transform.name;


            var transformGameObjectNameSplit = transformGameObjectName.Split("("[0]);
            var colliderGameObjectNameSplit = colliderGameObjectName.Split("("[0]);


            var transformGameObjectNumber = Convert.ToInt32(transformGameObjectNameSplit[1]
                .Substring(0, transformGameObjectNameSplit[1].Length - 1));
            var colliderGameObjectNumber = Convert.ToInt32(colliderGameObjectNameSplit[1]
                .Substring(0, colliderGameObjectNameSplit[1].Length - 1));

            var finalNumberOfCollider = transformGameObjectNumber + colliderGameObjectNumber;

            if (transformGameObjectNumber != 10 && colliderGameObjectNumber != 10 && finalNumberOfCollider <= 10)
            {
                var finalNameOfCollider = $"Number({finalNumberOfCollider})";

                //Debug.Log(finalNameOfCollider);


                Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name =
                    finalNameOfCollider;

                while (elapsedTime < TimeToMove)
                {
                    _startHitRaycastHit2D.transform.position = Vector2.Lerp(_oriPos, _tarPos, elapsedTime / TimeToMove);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                _startHitRaycastHit2D.transform.position = _tarPos;

                _startHitRaycastHit2D.transform.gameObject.SetActive(false);
                Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.gameObject
                    .GetComponent<SpriteRenderer>().sprite = sprites[finalNumberOfCollider];
                _isMoving = false;
            }
            else
            {
                yield return null;
                _isMoving = false;
            }
        }
    }
}