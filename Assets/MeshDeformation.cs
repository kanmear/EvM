using System;
using UnityEngine;

public class MeshDeformation : MonoBehaviour
{
    public float forceConst = 20f;
    private int _size;

    private Transform _transform;
    
    private Mesh _deformingMesh;
    private Vector3[] _originalVertices, _displacedVertices;
    private Vector3[] _vertexVelocities;
    
    private float _springForce = 20f;
    private float _damping = 6f;

    private Vector3 _upperLeftCorner,
                    _upperPoint,
                    _upperRightCorner,
                    _leftPoint,
                    _rightPoint,
                    _lowerLeftCorner,
                    _lowerPoint,
                    _lowerRightCorner;
    
    void Start()
    {
        var c = GetComponent<Cell>();
        _size = c.size;
        
        _transform = GetComponent<Transform>();
        _deformingMesh = GetComponent<MeshFilter>().mesh;
        _originalVertices = _deformingMesh.vertices;
        _displacedVertices = new Vector3[_originalVertices.Length];
        for (int i = 0; i < _originalVertices.Length; i++)
            _displacedVertices[i] = _originalVertices[i];
        _vertexVelocities = new Vector3[_originalVertices.Length];
        InitializeCorners();
    }
    
    private void InitializeCorners()
    {
        float x = _size;
        float y = x;
        var position = _transform.position;
        _upperLeftCorner = _transform.InverseTransformPoint(position + new Vector3(-x, y)); // 7
        _upperPoint = _transform.InverseTransformPoint(position + new Vector3(0, y)); // 8
        _upperRightCorner = _transform.InverseTransformPoint(position + new Vector3(x, y)); // 9
        _leftPoint = _transform.InverseTransformPoint(position + new Vector3(-x, 0)); // 4
        _rightPoint = _transform.InverseTransformPoint(position + new Vector3(x, 0)); // 6
        _lowerLeftCorner = _transform.InverseTransformPoint(position + new Vector3(-x, -y)); // 1
        _lowerPoint = _transform.InverseTransformPoint(position + new Vector3(0, -y)); // 2
        _lowerRightCorner = _transform.InverseTransformPoint(position + new Vector3(x, -y)); // 3
    }

    private void Update()
    {
        for (int i = 0; i < _displacedVertices.Length; i++)
            UpdateVertex(i);
        _deformingMesh.vertices = _displacedVertices;
        _deformingMesh.RecalculateNormals();
    }
    
    private void UpdateVertex(int i)
    {
        Vector3 velocity = _vertexVelocities[i];
        Vector3 displacement = _displacedVertices[i] - _originalVertices[i];
        velocity -= displacement * (_springForce * Time.deltaTime);
        velocity *= 1f - _damping * Time.deltaTime;
        _vertexVelocities[i] = velocity;
        _displacedVertices[i] += velocity * Time.deltaTime;
        // this doesn't allow vertices to escape square boundaries,
        // but there is no need in that
        // if (Math.Abs(_displacedVertices[i].x) - _size / 2f > 0.01
        //     || Math.Abs(_displacedVertices[i].y) - _size / 2f > 0.01)
        //     _displacedVertices[i] -= velocity * Time.deltaTime;
    }

    public void AddForce(Vector3[] positions)
    {
        for (int i = 0; i < _displacedVertices.Length; i++)
        {
            foreach (Vector3 v in positions)
            {
                var temp = CalculateForce(Vector3.Distance(_originalVertices[i], v));
                _vertexVelocities[i] += new Vector3(0, 1) * (v.y / temp * -1f);
                _vertexVelocities[i] += new Vector3(1, 0) * (v.x / temp * -1f);
            }
        }
    }

    public Vector3 GetLocalPosition(int point)
    {
        return point switch
        {
            3 => _lowerRightCorner,
            5 => _lowerPoint,
            8 => _lowerLeftCorner,
            2 => _rightPoint,
            7 => _leftPoint,
            1 => _upperRightCorner,
            4 => _upperPoint,
            6 => _upperLeftCorner,
            _ => new Vector3()
        };
    }

    private float CalculateForce(float distance)
    {
        return (distance * distance) / forceConst;
    }

}