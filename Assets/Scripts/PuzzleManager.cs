using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class PuzzleManager : MonoBehaviour
{
    public List<GameObject> puzzles;

    public Transform spawnPoint;

    public TextMeshProUGUI menuText;

    [SerializeField]
    private GameObject currentPuzzle;

    // Singleton;
    public static PuzzleManager instance;

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
            
            menuText.text = "Loaded " + currentPuzzle.GetComponent<PuzzleSystem>().puzzleName;
            yield return new WaitForSecondsRealtime(1);
            menuText.DOFade(0.0f, 1f).SetEase(Ease.InOutQuad);
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
        menuText.text = "";
        yield return null;
    }


    public void PuzzleSolved()
    {
        // current Puzzle is solved;
        menuText.text = "Puzzle Solved!";
        menuText.DOFade(1f, 0.1f).SetEase(Ease.InOutQuad);
        //menuText.DOFade(0, 3f).SetEase(Ease.InOutQuad);
        // solved animation
        // show finish menu

    }
}
