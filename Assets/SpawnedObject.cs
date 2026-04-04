using System.Globalization;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpawnedObject : NetworkBehaviour
{
    private NetworkVariable<Color32> _color = new NetworkVariable<Color32>();
    private SpriteRenderer _sprite;

    private void Awake()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _color.OnValueChanged += OnColorCanviat;
    }

    private void OnColorCanviat(Color32 previousValue, Color32 newValue)
    {
        Debug.Log("COLOR UPDATED");
        ApplyColor(newValue);
    }

    private void ApplyColor(Color32 color)
    {
        _sprite.color = color;
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log("OnNetworkSpawn color: " + _color.Value);

        ApplyColor(_color.Value);
    }

    public void SetColor(Color color)
    {
        Debug.Log("SET COLOR SERVER: " + color);
        _color.Value = (Color32)color;
    }


}

