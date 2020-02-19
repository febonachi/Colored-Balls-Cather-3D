using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manager : MonoBehaviour {
    public enum Status { NotReady, Initialization, Ready };

    #region editor
    [SerializeField] private string managerID = "manager";
    #endregion

    #region public properties
    public string id => managerID;
    public Status status { get; protected set; } = Status.NotReady;
    #endregion

    #region public
    public virtual void initialize() => status = Status.Initialization;
    #endregion
}
