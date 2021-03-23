using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//[ExecuteInEditMode]
public class TileFreeMovement : MonoBehaviour
{
    private const float TimeToMove = 0.17f;
    private const int TimeLeftForTake10 = 31;
    public Text pointText;
    public Text timerText;
    public LayerMask borderLayerMask;

    public Sprite[] sprites;

    public Camera mainCamera;

    public GameObject numberPrefab;
    public float currentTime;

    private readonly Queue<IEnumerator> _coroutineQueue = new Queue<IEnumerator>();
    private readonly List<Collider2D> _target10 = new List<Collider2D>();
    private RaycastHit2D _endHitRaycastHit2D;
    private bool _isMoving;
    private int _listTarget10Collider2D = -1;
    private GameObject _numberGameObject;
    private Vector2 _oriPos, _tarPos;

    private int _point;
    private int _randomNumGenSeed;

    private IEnumerator _runningCoroutine;
    private RaycastHit2D _startHitRaycastHit2D;

    private Touch _touch;
    private Vector2 _touchStartPosition, _touchEndPosition;

    private void Awake()
    {
        _randomNumGenSeed = 1;
    }

    private void Start()
    {
        SystematicRandomizedNumber(_randomNumGenSeed);
        StartCoroutine(Timer()); // timer should be placed after the SystematicRandomizedNumber only inside
    }


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

        pointText.text = $"{_point}";

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

    private void SystematicRandomizedNumber(int seed)
    {
        if (!_isMoving)
        {
            Random.InitState(seed);

            for (var i = 0; i < 6; i++)
            for (var x = 0; x < 8; x++)
            for (var y = 0; y < 10; y++)
            {
                var posX = Random.Range(-3, 5);
                var posY = Random.Range(-5, 5);
                var spriteArray = Random.Range(1, 10);


                if (!Physics2D.OverlapBox(new Vector2(posX, posY), new Vector2(0.2f, 0.2f), borderLayerMask))
                {
                    _numberGameObject = Instantiate(numberPrefab, new Vector2(posX, posY), quaternion.identity);
                    _numberGameObject.name = $"Number ({spriteArray})";
                    _numberGameObject.GetComponent<SpriteRenderer>().sprite = sprites[spriteArray];
                }
            }
        }
    }


    /*private void OnEnable()
    {
        Debug.Log("random number generation");


        for (var x = 0; x < 10; x++)
        for (var y = 0; y < 12; y++)
        {
            var posX = Random.Range(-4, 6);
            var posY = Random.Range(-4, 5);
            var spriteArray = Random.Range(1, 11);
//

            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(new Vector3(posX,posY));
        
            if (Physics.Raycast(ray, out hit)) {
                Transform objectHit = hit.transform;
            
                // Do something with the object that was hit by the raycast.
                Debug.Log("Not placed: " + posX + " :" + posY+"   "+objectHit.name);
            }
            else
            {
                _numberGameObject = Instantiate(numberPrefab, new Vector2(posX, posY), quaternion.identity);
                _numberGameObject.name = $"Number ({spriteArray})";
                _numberGameObject.GetComponent<SpriteRenderer>().sprite = sprites[spriteArray];
                
            }
            
//
            /*if (Physics2D.Raycast(mainCamera.ScreenToWorldPoint(new Vector3(posX, posY,0.0f)), Vector2.zero).collider ==
                null)
            {
                _numberGameObject = Instantiate(numberPrefab, new Vector2(posX, posY), quaternion.identity);
                _numberGameObject.name = $"Number ({spriteArray})";
                _numberGameObject.GetComponent<SpriteRenderer>().sprite = sprites[spriteArray];
            }
            else
            {
                Debug.Log("Not placed: " + posX + " :" + posY);
            }#1#
        }
    }*/


    private IEnumerator DisappearTarget10()
    {
        _runningCoroutine = null;

        if (_coroutineQueue.Count > 0)
        {
            _runningCoroutine = _coroutineQueue.Dequeue();
            StartCoroutine(_runningCoroutine);
        }

        yield return new WaitForSeconds(1.0f);
        _listTarget10Collider2D++;

        var spriteArray = Random.Range(1, 10);
        _target10[_listTarget10Collider2D].transform.name = $"Number ({spriteArray})";
        _target10[_listTarget10Collider2D].transform.gameObject
            .GetComponent<SpriteRenderer>().sprite = sprites[spriteArray];
        _point++;
    }

    private IEnumerator Timer()
    {
        currentTime = TimeLeftForTake10;

        while (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            TimerDisplay();
            yield return null;
        }
    }

    private void TimerDisplay()
    {
        var timeInMinutes = ((int) currentTime / 60 % 60).ToString("00");
        var timeInSeconds = ((int) currentTime % 60).ToString("00");
        timerText.text = $"{timeInMinutes}:{timeInSeconds}";

        if (currentTime <= 0)
        {
            _isMoving = true;
            timerText.text = "GAMEOVER";
        }
    }

    public void GenerateNumbers()
    {
        //SystematicRandomizedNumber(_point);
        SystematicRandomizedNumber(_randomNumGenSeed);
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
            var colliderGameObjectName = Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask)
                .transform.name;

//            Debug.Log(colliderGameObjectName);

            if (colliderGameObjectName != "TilemapBorder")
            {
                var transformGameObjectName = _startHitRaycastHit2D.transform.name;

                var transformGameObjectNameSplit = transformGameObjectName.Split("("[0]);
                var colliderGameObjectNameSplit = colliderGameObjectName.Split("("[0]);


                var transformGameObjectNumber = Convert.ToInt32(transformGameObjectNameSplit[1]
                    .Substring(0, transformGameObjectNameSplit[1].Length - 1));

                var colliderGameObjectNumber = Convert.ToInt32(colliderGameObjectNameSplit[1]
                    .Substring(0, colliderGameObjectNameSplit[1].Length - 1));


//                Debug.Log(transformGameObjectNumber + " " + colliderGameObjectNumber);

                var finalNumberOfCollider = transformGameObjectNumber + colliderGameObjectNumber;

                if (transformGameObjectNumber != 10 && colliderGameObjectNumber != 10 && finalNumberOfCollider <= 10)
                {
                    var finalNameOfCollider = $"Number({finalNumberOfCollider})";


                    Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.name =
                        finalNameOfCollider;

                    while (elapsedTime < TimeToMove)
                    {
                        _startHitRaycastHit2D.transform.position =
                            Vector2.Lerp(_oriPos, _tarPos, elapsedTime / TimeToMove);
                        elapsedTime += Time.deltaTime;
                        yield return null;
                    }

                    _startHitRaycastHit2D.transform.position = _tarPos;

                    _startHitRaycastHit2D.transform.GetComponent<BoxCollider2D>().enabled = false;
                    DestroyImmediate(_startHitRaycastHit2D.transform.gameObject);

                    Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask).transform.gameObject
                        .GetComponent<SpriteRenderer>().sprite = sprites[finalNumberOfCollider];

                    if (finalNumberOfCollider == 10)
                    {
                        currentTime += 10.0f; // bonus time given for take 10
                        _target10.Add(Physics2D.OverlapBox(_tarPos, new Vector2(0.2f, 0.2f), borderLayerMask));

                        // make some animation here... 
                        if (_runningCoroutine == null)
                        {
                            _runningCoroutine = DisappearTarget10();
                            StartCoroutine(_runningCoroutine);
                        }
                        else
                        {
                            _coroutineQueue.Enqueue(DisappearTarget10());
                        }
                    }


                    _isMoving = false;
                }
                else
                {
                    yield return null;
                    _isMoving = false;
                }
            }
            else
            {
                yield return null;
                _isMoving = false;
            }
        }
    }
}