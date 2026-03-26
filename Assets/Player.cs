using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class Player : NetworkBehaviour
{
    //Variable sincronitzada
    private NetworkVariable<Color> _color = new NetworkVariable<Color>();
    private InputSystem_Actions _actions;
    //Variables locals
    [SerializeField]
    private InputActionReference _ActionClickRef;
    private InputAction _ActionClick;
    [SerializeField]
    private InputAction _ActionMovement;
    private InputAction _ActionPointer;
    [SerializeField]
    private InputActionReference _ActionColorRef;

    private Rigidbody2D _rb;
    private SpriteRenderer _sprite;

    //Aquests existeixen com sempre, independentment de si hi ha xarxa.
    //Per tant, no hi feu lgica especfica de client.
    private void Awake()
    {
        _actions = new InputSystem_Actions();
        _rb = GetComponent<Rigidbody2D>();
        _sprite = GetComponent<SpriteRenderer>();
        _color.OnValueChanged += OnColorChanged;
    }

    //Aqu arribem un cop ens connectem i ja existim online
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        //Disposeu de diferents comprovacions. Owner, Client, Servidor, Host, etc
        //aprofiteu-les per tal que cada un s'encarregui de la seva lgica corresponent.
        //Tingueu en compte que els scripts s'executaran a tots els clients.
        if (IsOwner)
        {
            //Canviem a una posici aleatria.
            //Com el control de la posici el fa el servidor, no el podem modificar nosaltres.
            //Li ho demanem amb un RPC
            Vector2 randomSpawn = new Vector2(Random.Range(-6f, 6f), Random.Range(0f, 5f));
            SetSpawnRPC(randomSpawn);

            //noms volem capturar els events del nostre client, no dels altres.
            InicialitzarInput();
        }
    }
    private void FixedUpdate()
    {
        if (IsOwner)
        {
            OnMove();
        }
    }
    //Aquest codi noms s'executar al servidor i el pot cridar l'owner de l'objecte
    [Rpc(SendTo.Server)]
    private void PeticioMoveRPC(Vector2 input)
    {
        _rb.MoveRotation(_rb.rotation + input.x*5);

        Vector2 force = transform.up * input.y * 4f;
        _rb.AddForce(force);
    }

    private void InicialitzarInput()
    {
        _ActionMovement = _actions.Player.Move;
        _actions.Player.Enable();
    }

    //Codi local de client, per ens hem assegurat que noms passa a l'owner
    private void OnMove()
    {
        Vector2 input = _ActionMovement.ReadValue<Vector2>();

        //Demanem al servidor que ens canvi de posici
        PeticioMoveRPC(input);
    }
    [Rpc(SendTo.Server)]
    private void SetSpawnRPC(Vector2 pos)
    {
        transform.position = pos;
        _rb.linearVelocity = Vector2.zero;
    }
    private void OnCanviarColor(InputAction.CallbackContext context)
    {
        //Ja ens hem assegurat que noms l'owner fa aquesta acci
        //per recordar que els Rpc els fa l'owner
        CanviarColorRpc();
    }

    [Rpc(SendTo.Server)]
    private void CanviarColorRpc()
    {
        _color.Value = new Color(Random.Range(-0f, 1f), Random.Range(-0f, 1f), Random.Range(-0f, 1f));
    }

    //Hook que es cridar a cada client un cop el valor hagi estat canviat
    //Per defecte, noms el pot canviar el servidor.
    private void OnColorChanged(Color previousValue, Color newValue)
    {
        Debug.Log($"{NetworkObjectId}: De {previousValue} a {newValue}");
        _sprite.color = newValue;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!IsServer) return;

        if (collision.contactCount > 0)
        {
            Vector2 direction = collision.contacts[0].normal;

            // Rebote tipo impulso
            if (collision.transform.TryGetComponent<Player>(out Player p))
            {
                _rb.AddForce(direction * 2f, ForceMode2D.Impulse);                
              //  p._rb.AddForce(-direction * 2f, ForceMode2D.Impulse);
            }
            else
            {
                if (collision.transform.GetComponent<SpriteRenderer>().color == Color.red)
                    NetworkObject.Despawn();
                else
                    _rb.AddForce(direction * 4f, ForceMode2D.Impulse);
            }
            // Si chocas con otro player, empujar también al otro
            Rigidbody2D otherRb = collision.rigidbody;
        }
    }
}

