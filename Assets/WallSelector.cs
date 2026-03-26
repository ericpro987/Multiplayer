using Unity.Netcode;
using UnityEngine;

public class WallSelector : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer[] _walls;
    [SerializeField] private float _changeTime = 3f;

    private int _currentWall = -1;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            InvokeRepeating(nameof(ChooseWall), 1f, _changeTime);
        }
    }

    private void ChooseWall()
    {
        int newWall = Random.Range(0, _walls.Length);

        while (newWall == _currentWall)
        {
            newWall = Random.Range(0, _walls.Length);
        }

        if (_currentWall != -1)
        {
            _walls[_currentWall].color = Color.white;
        }

        _walls[newWall].color = Color.red;

        _currentWall = newWall;
    }
}