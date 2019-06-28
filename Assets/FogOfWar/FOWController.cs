using System.Collections.Generic;
using UnityEngine;

public class FOWController : MonoBehaviour
{
    [SerializeField] Transform _player;
    [SerializeField] Transform _fowPlane;
    [SerializeField] MeshRenderer _fowMeshRenderer;
    
    [Header("FOW")]
    [SerializeField] float _pointUpdateTime;
    // If the distance between the points is this distance, or less, the point won't be added to the fow points list.
    [SerializeField] float _ignoreDistance;

    // Points are represented as x and z.
    float[] _fowPoints = new float[1000];
    int _fowPointIndex = 0;
    Material _fowMaterial;
    float _currentPointTime;
    float _ignoreDistanceSquared;

    private MaterialPropertyBlock _matBlock;

    void Awake() {
        _fowPlane.gameObject.SetActive(true);
        _fowMaterial = _fowMeshRenderer.material;
        _ignoreDistanceSquared = _ignoreDistance * _ignoreDistance;

        _matBlock = new MaterialPropertyBlock();
    }

    void LateUpdate() {
        _currentPointTime += Time.deltaTime;
        if(_currentPointTime >= _pointUpdateTime) {

            UpdateFOWPoints();

            _currentPointTime -= _pointUpdateTime;
        }
    }

    void UpdateFOWPoints() {
        Vector3 currentPlayerPosition = _player.position;
        float playerX = currentPlayerPosition.x, playerZ = currentPlayerPosition.z;

        if(IsNewFOWPoint(playerX, playerZ)) {
            _fowPoints[_fowPointIndex] = playerX;
            _fowPoints[_fowPointIndex + 1] = playerZ;
            _fowPointIndex += 2;

            //Debug.Log("Adding point");
            //_fowMaterial.SetFloatArray("_Points", _fowPoints);

            _matBlock.SetFloatArray("_Points", _fowPoints);
            _matBlock.SetInt("_Index", _fowPointIndex);
            _fowMeshRenderer.SetPropertyBlock(_matBlock);
        }
    }

    bool IsNewFOWPoint(float playerX, float playerZ) {
        for(int i = 0; i < _fowPoints.Length; i += 2) {
            float x = _fowPoints[i], z = _fowPoints[i + 1];

            if (SquarePointDistance(x, z, playerX, playerZ) <= _ignoreDistanceSquared)
                return false;
        }
        return true;
    }

    float SquarePointDistance (float x1, float z1, float x2, float z2) {
        float xdelta = x1 - x2, zdelta = z1 - z2;
        return xdelta * xdelta + zdelta * zdelta;
    }
}
