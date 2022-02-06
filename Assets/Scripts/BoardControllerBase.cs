using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public delegate void BailEventHandler(object sender, BailEventArgs e);

public class BoardControllerBase : MonoBehaviour
{
    public event BailEventHandler PlayerBailed;
    public event EventHandler PlayerRespawned;

    protected virtual void OnPlayerBailed(BailEventArgs e)
    {
        BailEventHandler handler = PlayerBailed;
        handler?.Invoke(this, e);
    }

    protected virtual void OnPlayerRespawned(EventArgs e)
    {
        EventHandler handler = PlayerRespawned;
        handler?.Invoke(this, e);
    }
}

public class BailEventArgs : EventArgs
{
    public GameObject RagdollInstance;
}
