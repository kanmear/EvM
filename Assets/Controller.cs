using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour
{
    public static Vector3 Center;
    
    public Cell cell;
    
    private Coroutine _coroutine;
    private Camera _camera;
    private bool _automated;
    // private float _timePassed = 0f;
    
    [SerializeField] private float delay;
    void Start()
    {
        Center = new Vector3(Container.Columns * 2 - Container.Gap / 2, Container.Rows * 2 - Container.Gap / 2);
        
        _camera = Camera.main;
        if (_camera is { })
        {
            _camera.transform.position = new Vector3(Center.x, Center.y, -10);
            _camera.orthographicSize = Container.Columns;
            _camera.aspect = 2;
        }

        CreateGrid();
        // screensaver mode
        // SymmetricalGrid();
    }

    private void CreateGrid()
    {
        for (int y = 0; y < Container.Rows; y++)
            for (int x = 0; x < Container.Columns; x++)
            {
                Container.Grid[x, y] = Instantiate(cell);
                
                var c = Container.Grid[x, y].GetComponent<Cell>();
                c.Construct(x, y);

                Container.HelperGrid[(x, y)] = false;
            }
    }

    void Update()
    {
        // screensaver mode
        // if (!_automated)
        //     _timePassed += Time.deltaTime;
        //
        // if (_timePassed >= 1f && !_automated)
        // {
        //     _coroutine = StartCoroutine(Automate());
        //     _automated = !_automated;
        // }
        // ends here
        
        if (Input.GetKeyDown("s"))
        {
            UpdateGrid();
        }
        
        if (Input.GetKeyDown("a"))
        {
            if (!_automated)
                _coroutine = StartCoroutine(Automate());
            else
                StopCoroutine(_coroutine);
            _automated = !_automated;
        }

        if (Input.GetKeyDown("r"))
            RandomizeOrClearGrid(0);

        if (Input.GetKeyDown("c"))
            RandomizeOrClearGrid(1);

        if (Input.GetKeyDown("p"))
            SymmetricalGrid();

        if (Input.GetButtonDown("Cancel"))
            Application.Quit();
    }

    private void SymmetricalGrid()
    {
        for (int y = 0; y < Container.Rows / 2; y++)
            for (int x = 0; x < Container.Columns / 2; x++)
            {
                bool life = Random.Range(0, 2) == 1;

                var c = Container.Grid[x, y].GetComponent<Cell>();
                c.SetLife(life);
                c = Container.Grid[Container.Columns - x - 1, y].GetComponent<Cell>();
                c.SetLife(life);
                c = Container.Grid[x, Container.Rows - y - 1].GetComponent<Cell>();
                c.SetLife(life);
                c = Container.Grid[Container.Columns - x - 1, Container.Rows - y - 1].GetComponent<Cell>();
                c.SetLife(life);
                
                Container.HelperGrid[(x, y)] = life;
                Container.HelperGrid[(Container.Columns - x - 1, y)] = life;
                Container.HelperGrid[(x, Container.Rows - y - 1)] = life;
                Container.HelperGrid[(Container.Columns - x - 1, Container.Rows - y - 1)] = life;
            }
    }

    private void RandomizeOrClearGrid(int i)
    {
        for (int y = 0; y < Container.Rows; y++)
            for (int x = 0; x < Container.Columns; x++)
            {
                bool life = i == 0 && Random.Range(0, 2) == 1;

                var c = Container.Grid[x, y].GetComponent<Cell>();
                c.SetLife(life);

                Container.HelperGrid[(x, y)] = life;
            }
    }

    private void UpdateGrid()
    {
        for (int y = 0; y < Container.Rows; y++)
            for (int x = 0; x < Container.Columns; x++)
            {
                Container.Grid[x, y].GetComponent<Cell>().Reform();
            }
        
        for (int y = 0; y < Container.Rows; y++)
            for (int x = 0; x < Container.Columns; x++)
            {
                Container.HelperGrid[(x, y)] = Container.Grid[x, y].VitalSigns();
            }
        
        for (int y = 0; y < Container.Rows; y++)
            for (int x = 0; x < Container.Columns; x++)
            {
                var c = Container.Grid[x, y].GetComponent<Cell>();
                c.TimeSinceChange++;
                var m = Container.Grid[x, y].GetComponent<MeshDeformation>();
                for (int i = 1; i < c.Points.Length; i++)
                {
                    if (c.VitalSigns())
                    {
                        m.AddForce(new[] {m.GetLocalPosition(c.Points[i] * i)});
                    }
                }
            }
    }

    private IEnumerator Automate()
    {
        while (true)
        {
            UpdateGrid();
            yield return new WaitForSeconds(delay);
        }
        // ReSharper disable once IteratorNeverReturns
    }
}