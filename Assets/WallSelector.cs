using Unity.Netcode;
using UnityEngine;

public class WallSelector : NetworkBehaviour
{
    [SerializeField] private SpriteRenderer[] _walls;
    [SerializeField] private float _changeTime = 3f;

    private NetworkVariable<int> _currentWall = new NetworkVariable<int>(-1);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            InvokeRepeating(nameof(ChooseWall), 1f, _changeTime);
        }

        _currentWall.OnValueChanged += OnWallChanged;
    }

    private void ChooseWall()
    {
        int newWall = Random.Range(0, _walls.Length);

        while (newWall == _currentWall.Value)
            newWall = Random.Range(0, _walls.Length);

        _currentWall.Value = newWall;
    }

    private void OnWallChanged(int oldWall, int newWall)
    {
        if (oldWall != -1)
            _walls[oldWall].color = Color.white;

        _walls[newWall].color = Color.red;
    }
}