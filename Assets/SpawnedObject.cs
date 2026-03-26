using System.Globalization;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpawnedObject : NetworkBehaviour
{
    private NetworkVariable<Color> _color = new NetworkVariable<Color>();
    private SpriteRenderer _sprite;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _color.OnValueChanged += OnColorCanviat;
    }

    private void OnColorCanviat(Color previousValue, Color newValue)
    {
        _sprite.color = newValue;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        //quan iniciem, no es cridar el value changed, ja vindr donat
        _sprite.color = _color.Value;
    }

    public void ColorAleatori(Color color)
    {
        if (IsOwner)
        {
            CanviarColorRpc(color);
        }
    }

    [Rpc(SendTo.Server)]
    private void CanviarColorRpc(Color color)
    {
        _color.Value = color;
    }
}

