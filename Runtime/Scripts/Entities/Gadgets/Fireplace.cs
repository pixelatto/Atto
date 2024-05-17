using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireplace : Gadget
{
    public float remainingFuel = 100;

    public CellularSensor cellularSensor { get { if (cellularSensor_ == null) { cellularSensor_ = GetComponent<CellularSensor>(); }; return cellularSensor_; } }
    private CellularSensor cellularSensor_;

    public PixelLight pixelLight { get { if (pixelLight_ == null) { pixelLight_ = GetComponentInChildren<PixelLight>(); }; return pixelLight_; } }
    private PixelLight pixelLight_;

    public CellularSpawner cellularSpawner { get { if (cellularSpawner_ == null) { cellularSpawner_ = GetComponentInChildren<CellularSpawner>(); }; return cellularSpawner_; } }
    private CellularSpawner cellularSpawner_;

    public override void On_State()
    {
        remainingFuel -= Time.deltaTime;
        cellularSpawner.gameObject.SetActive(true);
        pixelLight.gameObject.SetActive(true);
        if (cellularSensor.IsFilledWith((cell => (cell.IsFluid() || cell.IsSolid()) && !cell.IsHotterThan(300))))
        {
            ChangeState(GadgetState.Off);
        }
        if (remainingFuel <= 0)
        {
            remainingFuel = 0;
            ChangeState(GadgetState.Out);
        }
    }

    public override void Off_State()
    {
        base.Off_State();
        cellularSpawner.gameObject.SetActive(false);
        pixelLight.gameObject.SetActive(false);
        if (cellularSensor.ContainsAny((cell => cell.IsHotterThan(300))))
        {
            ChangeState(GadgetState.On);
        }
    }

    public override void Out_State()
    {
        base.Out_State();
        cellularSpawner.gameObject.SetActive(false);
        pixelLight.gameObject.SetActive(false);
        if (remainingFuel > 0)
        {
            ChangeState(GadgetState.Off);
        }
    }

}
