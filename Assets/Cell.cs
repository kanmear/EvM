using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    // cell variables
    private bool _isAlive;
    private int _x, _y;
    private readonly int _gap = Container.Gap;
    
    // mesh variables
    [NonSerialized] public int[] Points;
    
    public int size; //in grid units
    public int amount; //how many steps to go from original coordinates to end

    private Renderer _renderer;
    private Material _material;
    private Mesh _mesh;
    private Transform _transform;
    private Vector3[] _vertices;
    private Vector2[] _uv;
    private int[] _triangles;
    
    [NonSerialized] public float TimeSinceChange;

    // cell logic
    public void Construct(int x, int y, bool isAlive = false)
    {
        _x = x; _y = y;
        _isAlive = isAlive;
    }
    
    public (int, int) GeTCoordinates() => (this._x, this._y);
    public bool VitalSigns() => this._isAlive;
    public void SetLife(bool alive) => _isAlive = alive;

    public void Reform()
    {
        // cell logic
        int count = CheckNeighbours(this._x, this._y, out Points);
        if (!this._isAlive && count == 3)
            this._isAlive = true;
        else if (this._isAlive)
            if (count != 2 && count != 3)
                this._isAlive = false;
    }
    void Start()
    {
        var t = GetComponent<Transform>();
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
        _material.color = this._isAlive ? Color.green : Color.black;
        
        t.position = new Vector2(this._x * _gap, this._y * _gap);

        GenerateMesh();
    }

    void Update()
    {
        Color[] array = {Color.blue, Color.cyan, Color.green, Color.magenta, Color.red, Color.yellow};
        if (TimeSinceChange >= array.Length - 1)
            TimeSinceChange = 0f;
        
        Color oldColor = array[(int) TimeSinceChange];
        Color newColor = array[(int) (TimeSinceChange + 1f)];
        
        var color = Color.Lerp(oldColor, newColor, Mathf.PingPong(Time.time, 
            Vector3.Distance(_transform.position, Controller.Center) / 40f));
        _material.color = this._isAlive ? color : Color.black;
    }
    
    private void GenerateMesh()
    {
        _transform = GetComponent<Transform>();
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();

        var pos = _transform.InverseTransformPoint(_transform.position);
        float step = size / (float) amount;
        float t = (float) size / 2;
        
        _vertices = new Vector3[(amount + 1) * (amount + 1)];
        _uv = new Vector2[_vertices.Length];
        for (int i = 0, n = 0; i <= amount; i++)
        {
            for (int j = 0; j <= amount; n++, j++)
            {
                float x = pos.x - t + step * j;
                float y = pos.y - t + step * i;
                _vertices[n] = new Vector3(x, y);
                _uv[i] = new Vector2(x / amount, y / amount);
            }
        }
        _mesh.vertices = _vertices;
        _mesh.uv = _uv;

        _triangles = new int[amount * amount * 6];
        for (int ti = 0, vi = 0, y = 0; y < amount; y++, vi++)
        {
            for (int x = 0; x < amount; x++, ti += 6, vi++)
            {
                _triangles[ti] = vi;
                _triangles[ti + 3] = _triangles[ti + 2] = vi + 1;
                _triangles[ti + 4] = _triangles[ti + 1] = vi + amount + 1;
                _triangles[ti + 5] = vi + amount + 2;
            }
        }
        _mesh.triangles = _triangles;
        
        _mesh.RecalculateNormals();
    }
    
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _isAlive = !_isAlive;
            Container.HelperGrid[(_x, _y)] = _isAlive;
        }
        
        if (Input.GetMouseButtonDown(1))
        {
            int t = CheckNeighbours(_x, _y, out _);
            Debug.Log(t);
        }

        if (Input.GetMouseButtonDown(2))
        {
            _ = CheckNeighbours(_x, _y, out int[] t);
            foreach (var t1 in t)
                Debug.Log(t1);
        }
    }

    private int CheckNeighbours(int x, int y, out int[] pts)
    {
        pts = new int[9];
        int v = 1;
        int count = 0;
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
            {
                if ((i == 1 && j == 1) == false)
                {
                    Container.HelperGrid.TryGetValue((x - 1 + i, y - 1 + j), out var temp);
                    if (temp)
                    {
                        pts[v] = 1;
                        count += Container.HelperGrid[(x - 1 + i, y - 1 + j)] ? 1 : 0;
                    }

                    v++;
                }
            }
        
        return count;
    }
}