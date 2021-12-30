using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : NetworkBehaviour
{
    [SerializeField] private int resourceCost = 10;
    [SerializeField] private UnitMovement unitMovement = null;
    [SerializeField] private Health health = null;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private UnityEvent onSelected = null;
    [SerializeField] private UnityEvent onDeselected = null;
    public static event Action<Enemy> ServerOnUnitSpawned;
    public static event Action<Enemy> ServerOnUnitDespawned;

    public static event Action<Enemy> AuthorityOnUnitSpawned;
    public static event Action<Enemy> AuthorityOnUnitDespawned;

    public bool endReached = false;

    public int GetResourceCost()
    {
        return resourceCost;
    }

    public UnitMovement GetUnitMovement()
    {
        return unitMovement;
    }

    public Targeter GetTargeter()
    {
        return targeter;
    }

    #region Server

    public override void OnStartServer()
    {
        health.ServerOnDie += ServerHandleDie;
        unitMovement.ServerOnDie += ServerHandleDie;
        ServerOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        health.ServerOnDie -= ServerHandleDie;
        unitMovement.ServerOnDie -= ServerHandleDie;
        ServerOnUnitDespawned?.Invoke(this);
    }

    [Server]
    private void ServerHandleDie()
    {
        if(endReached)
        {
            Debug.Log("Point Scored for the enemy!");
            endReached = false;
        }
        NetworkServer.Destroy(gameObject);
    }
    
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        AuthorityOnUnitSpawned?.Invoke(this);
    }

    public override void OnStopClient()
    {
        if(!hasAuthority) {return;}
        
        AuthorityOnUnitDespawned?.Invoke(this);
    }

    [Client]
    public void Select()
    {
        if(!hasAuthority) {return;}
        onSelected?.Invoke();
    }

    [Client]
    public void Deselect()
    {
        if(!hasAuthority) {return;}
        onDeselected?.Invoke();
    }
    #endregion
}
