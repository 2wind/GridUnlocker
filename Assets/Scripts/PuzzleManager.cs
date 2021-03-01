using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    public List<GameObject> puzzles;

    public Transform spawnPoint;

    public Canvas canvas;
    public TextMeshProUGUI menuText;

    [SerializeField]
    private GameObject currentPuzzle;

    // Singleton;
    public static PuzzleManager instance;

    const string TITLE = "GridUnlocker";

    private void Awake()
    {
        instance = this;
        //menuText.text = "";
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadPuzzle(string name)
    {
        StartCoroutine(ILoadPuzzle(name));

    }

    private IEnumerator ILoadPuzzle(string name)
    {
        if (!puzzles.Exists(puzzle => name.Equals(puzzle.GetComponent<PuzzleSystem>().puzzleName)))
        {
            Debug.LogError("puzzle " + name + " not found");
            yield return null;
        }
        else
        {
            menuText.DOFade(1, 0.1f).SetEase(Ease.InOutQuad);
            
            RemovePuzzle();
            // load puzzle by name
            currentPuzzle = Instantiate(puzzles.Find(puzzle => name.Equals(puzzle.GetComponent<PuzzleSystem>().puzzleName)));
            currentPuzzle.transform.position = spawnPoint.transform.position;

            yield return new WaitForSeconds(0.1f);
            menuText.text = "Loaded " + currentPuzzle.GetComponent<PuzzleSystem>().puzzleName;
            menuText.DOFade(0.0f, 1f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(1);
            menuText.text = "";
        }
    }


    public void RemovePuzzle()
    {
        StartCoroutine(IRemovePuzzle());
    }

    private IEnumerator IRemovePuzzle()
    {
        // remove currently loaded puzzle
        if (currentPuzzle != null)
        {
            Destroy(currentPuzzle);
            currentPuzzle = null;
        }
        if (menuText.alpha < 1)
        {
            menuText.DOFade(1f, 0.1f).SetEase(Ease.InOutQuad);
            yield return new WaitForSeconds(0.1f);
        }
        menuText.text = TITLE;
        yield return null;
    }


    public void PuzzleSolved()
    {

        StartCoroutine(IPuzzleSolved());

    }

    private IEnumerator IPuzzleSolved()
    {
        // current Puzzle is solved;
        menuText.text = "Puzzle Solved!";
        menuText.DOFade(1f, 0.1f).SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(0.1f);
        menuText.DOFade(0, 3f).SetEase(Ease.InOutQuad);
        // solved animation
        // show finish menu
        
    }
}
