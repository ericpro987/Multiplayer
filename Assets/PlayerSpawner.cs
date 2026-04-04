using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class PlayerSpawner : NetworkBehaviour
{
    [SerializeField]
    private InputActionReference _spawnActionRef;

    [SerializeField]
    private GameObject _spawnObject;
    private List<GameObject> _spawnedObjects = new List<GameObject>();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            _spawnActionRef.action.performed += OnSpawn;
        }
    }

    private void OnSpawn(InputAction.CallbackContext context)
    {
        //Noms el servidor pot fer-ho, li ho demanem.
        SpawnObjectRPC();
    }

    [Rpc(SendTo.Server)]
    private void SpawnObjectRPC()
    {
        GameObject go = Instantiate(_spawnObject);
        Color color = new Color(Random.value, Random.value, Random.value);
        go.transform.position = new Vector2(Random.Range(-8f, 8f), Random.Range(-5f, 5f));

        //aquesta llista s local i noms l'usa el servidor
        //a la resta de llocs est buida
        _spawnedObjects.Add(go);

        

      

        //Qualsevol d'aquestes dues fa que aparegui a tots els clients.
        //D'altra banda, s una instanciaci local.
        //Fixeu-vos! Primer hem assignat els valors per a propagar-los.

        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        //networkObject.SpawnAsPlayerObject(OwnerClientId);
        networkObject.Spawn();

        // després DEL SPAWN BRO
        go.GetComponent<SpawnedObject>().SetColor(color);

        //Avisem a tots els clients, no s pas necessari, per
        //hem de fer algun exemple de RPC de servidor a clients
        AvisarClientsRPC(OwnerClientId, networkObject.NetworkObjectId);

       /* //Canviem el color de tots els nostres spawns, noms els nostres
        Color color = new Color(Random.Range(-0f, 1f), UnityEngine.Random.Range(-0f, 1f), Random.Range(-0f, 1f));
        foreach (GameObject spawned in _spawnedObjects)
            spawned.GetComponent<SpawnedObject>().ColorAleatori(color);*/
    }

    [Rpc(SendTo.ClientsAndHost)]
    void AvisarClientsRPC(ulong sourceNetworkObjectId, ulong spawnedNetworkObjectId)
    {
        Debug.Log($"{sourceNetworkObjectId} ha spawnejat {spawnedNetworkObjectId}");
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer) return;

        //Aix s per a no rebre errors.
        //Quan el host es tanca, es tanca tot igualment
        if (IsHost) return;

        //Eliminem els objectes relacionats amb el nostre objecte.
        //Recordem que noms el servidor pot fer el Despawn i s
        //qui t la llista.
        //Hi ha un check que ho fa automticament al NetworkObject.
        foreach (GameObject go in _spawnedObjects)
            go.GetComponent<NetworkObject>().Despawn();
        base.OnNetworkDespawn();
    }
}
