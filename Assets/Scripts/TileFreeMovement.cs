using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

//[ExecuteInEditMode]
public class TileFreeMovement : MonoBehaviour
{
    private const float TimeToMove = 0.17f;
    private const int TimeLeftForTake10 = 900; //old 31;
    private const int RatioOfPlusIncrease = 10;
    public Text pointText;
    public Text timerText;
    public Text moveText;
    public Button exitBtn;
    public Button plusBtn;
    public LayerMask borderLayerMask;

    public Sprite[] sprites;

    public Camera mainCamera;

    public GameObject numberPrefab;
    public float currentTime;

    private readonly Queue<IEnumerator> _coroutineQueueOfAppearRandomNumber = new Queue<IEnumerator>();

    private readonly Queue<IEnumerator> _coroutineQueueOfTarget10 = new Queue<IEnumerator>();

    private readonly List<Collider2D> _target10 = new List<Collider2D>();
    private readonly int[] SelectedRandomNumbers = {1, 2, 3, 3, 2, 1};
    private readonly int targetPoint = 20;

    private int _coroutineQueueOfAppearRandomNumberCount;
    private int _currentMoved;
    private RaycastHit2D _endHitRaycastHit2D;
    private bool _isMoving;
    private int _listTarget10Collider2D = -1;
    private GameObject _numberGameObject;

    private bool _numberGenerating;
    private Vector2 _oriPos, _tarPos;

    private int _plusRandomNumbers;

    private int _point;
    private int _randomNumGenSeed;
    private IEnumerator _runningCoroutineOfAppearRandomNumber;
    private IEnumerator _runningCoroutineOfTarget10;

    private RaycastHit2D _startHitRaycastHit2D;

    private Coroutine _startTimerCoroutine;
    private int _totalMoves = 50;
    private int _totalRandomNumberGenerated;

    private Touch _touch;
    private Vector2 _touchStartPosition, _touchEndPosition;

    private void Awake()
    {
        _startTimerCoroutine = null;
        _randomNumGenSeed = 1;
        _totalRandomNumberGenerated = 0;
        moveText.text = $"<{_totalMoves:00}>";
        currentTime = TimeLeftForTake10;
    }

    private void Start()
    {
        SystematicRandomizedNumber(_randomNumGenSeed);
    }


    private void Update()
    {
        if (!_numberGenerating)
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

            pointText.text = $"{_point}/{targetPoint}";

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
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("LoadScene", LoadSceneMode.Single);
    }

    private int SystematicRandomizedNumber(int seed)
    {
        _numberGenerating = true;
        _totalRandomNumberGenerated = 0;

        if (!_isMoving)

            //Random.InitState(seed);

            for (var i = 0; i < 6; i++)
            for (var x = 0; x < 8; x++)
            for (var y = 0; y < 12; y++)
            {
                //old
                var posX = Random.Range(-3, 5);
                var posY = Random.Range(-5, 7);

                //new
                // var posX = Random.Range(-1, 3);
                // var posY = Random.Range(-2, 2);

                if (_runningCoroutineOfAppearRandomNumber == null)
                {
                    _runningCoroutineOfAppearRandomNumber = AppearRandomNumbers(posX, posY);
                    StartCoroutine(_runningCoroutineOfAppearRandomNumber);
                }
                else
                {
                    _coroutineQueueOfAppearRandomNumber.Enqueue(AppearRandomNumbers(posX, posY));
                }
            }


        return _totalRandomNumberGenerated;
    }


    private IEnumerator AppearRandomNumbers(int posX, int posY)
    {
        _runningCoroutineOfAppearRandomNumber = null;


        if (_coroutineQueueOfAppearRandomNumber.Count > 0)
        {
            _runningCoroutineOfAppearRandomNumber = _coroutineQueueOfAppearRandomNumber.Dequeue();
            StartCoroutine(_runningCoroutineOfAppearRandomNumber);
        }

        yield return new WaitForSeconds(Random.Range(1.0f, 2.0f));
        _coroutineQueueOfAppearRandomNumberCount++;

        if (!Physics2D.OverlapBox(new Vector2(posX, posY), new Vector2(0.2f, 0.2f), borderLayerMask))
        {
            var spriteArray = SelectedRandomNumbers[Random.Range(0, SelectedRandomNumbers.Length)];
            _numberGameObject = Instantiate(numberPrefab, new Vector2(posX, posY), quaternion.identity);
            _numberGameObject.name = $"Number ({spriteArray})";
            _numberGameObject.GetComponent<SpriteRenderer>().sprite = sprites[spriteArray];
            _totalRandomNumberGenerated++;
        }

        if (_coroutineQueueOfAppearRandomNumberCount >= 576)
        {
            _coroutineQueueOfAppearRandomNumberCount = 0;

            _startTimerCoroutine = StartCoroutine(TimerStart());

            if (_plusRandomNumbers > 0 && _totalRandomNumberGenerated > 0)
            {
                _plusRandomNumbers--;
                plusBtn.GetComponentInChildren<Text>().text = _plusRandomNumbers.ToString();

                plusBtn.GetComponent<Button>().interactable = _plusRandomNumbers > 0;
            }
            else
            {
                plusBtn.GetComponent<Button>().interactable = _plusRandomNumbers > 0;
            }

            _numberGenerating = false;
        }


        //button

        /*if (SystematicRandomizedNumber(_randomNumGenSeed) > 0)
          {
              _plusRandomNumbers--;
           
              plusBtn.GetComponentInChildren<Text>().text = _plusRandomNumbers.ToString();
              if (_plusRandomNumbers == 0) plusBtn.GetComponent<Button>().interactable = false;
          }*/
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
        _runningCoroutineOfTarget10 = null;

        if (_coroutineQueueOfTarget10.Count > 0)
        {
            _runningCoroutineOfTarget10 = _coroutineQueueOfTarget10.Dequeue();
            StartCoroutine(_runningCoroutineOfTarget10);
        }

        yield return new WaitForSeconds(1.0f);
        _listTarget10Collider2D++;

        /* Method1 create random tiles here
         
        var spriteArray = Random.Range(1, 4);
        _target10[_listTarget10Collider2D].transform.name = $"Number ({spriteArray})";
        _target10[_listTarget10Collider2D].transform.gameObject
            .GetComponent<SpriteRenderer>().sprite = sprites[spriteArray];
            */


        // Method2 Destroy the taken 10
        DestroyImmediate(_target10[_listTarget10Collider2D].transform.gameObject);

        _point++;
        _totalMoves++;
        moveText.text = $"<>:{_totalMoves:00}";

        if (_point % RatioOfPlusIncrease == 0)
        {
            _plusRandomNumbers++;
            plusBtn.GetComponentInChildren<Text>().text = _plusRandomNumbers.ToString();
            plusBtn.GetComponent<Button>().interactable = true;
        }

        if (_point >= 20)
        {
            moveText.text = $"<You Win>";
            _numberGenerating = true;
            plusBtn.GetComponent<Button>().interactable = false;
            exitBtn.GetComponent<Button>().interactable = true;
            exitBtn.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            exitBtn.GetComponentInChildren<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    private IEnumerator TimerStart()
    {
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
            exitBtn.interactable = true;
            exitBtn.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            exitBtn.GetComponentInChildren<Text>().color = new Color(0.0f, 0.0f, 0.0f, 1.0f);
        }
    }

    public void GenerateNumbers()
    {
        if (_plusRandomNumbers > 0)
        {
            StopCoroutine(_startTimerCoroutine);
            plusBtn.GetComponent<Button>().interactable = false;
            SystematicRandomizedNumber(_randomNumGenSeed);
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
                        if (_runningCoroutineOfTarget10 == null)
                        {
                            _runningCoroutineOfTarget10 = DisappearTarget10();
                            StartCoroutine(_runningCoroutineOfTarget10);
                        }
                        else
                        {
                            _coroutineQueueOfTarget10.Enqueue(DisappearTarget10());
                        }
                    }

                    _isMoving = false;


                    if (_totalMoves > 0)
                    {
                        _totalMoves--;
                        moveText.text = $"<>:{_totalMoves:00}";
                    }
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