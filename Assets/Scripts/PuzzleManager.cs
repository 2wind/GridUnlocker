using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public List<GameObject> puzzles;

    public Transform spawnPoint;

    [SerializeField]
    private GameObject currentPuzzle;

    // Singleton;
    public static PuzzleManager instance;

    private void Awake()
    {
        instance = this;
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
        if (!puzzles.Exists(puzzle => name.Equals(puzzle.GetComponent<PuzzleSystem>().puzzleName)))
        {
            Debug.LogError("puzzle " + name + " not found");
            return;
        }
        RemovePuzzle();
        // load puzzle by name
        currentPuzzle = Instantiate(puzzles.Find(puzzle => name.Equals(puzzle.GetComponent<PuzzleSystem>().puzzleName)));
        currentPuzzle.transform.position = spawnPoint.transform.position;

        
    }


    public void RemovePuzzle()
    {
        // remove currently loaded puzzle
        if (currentPuzzle != null)
        {
            Destroy(currentPuzzle);
            currentPuzzle = null;
        }
        
    }


    public void PuzzleSolved()
    {
        // current Puzzle is solved;
        // solved animation
        // show finish menu

    }
}
