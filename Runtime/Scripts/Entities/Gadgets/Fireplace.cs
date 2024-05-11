using System;
using System.Collections;
using System.Collections.Generic;

public class Fireplace : Gadget
{
    public CellularSensor cellularSensor { get { if (cellularSensor_ == null) { cellularSensor_ = GetComponent<CellularSensor>(); }; return cellularSensor_; } }
    private CellularSensor cellularSensor_;

    public PixelLight pixelLight { get { if (pixelLight_ == null) { pixelLight_ = GetComponentInChildren<PixelLight>(); }; return pixelLight_; } }
    private PixelLight pixelLight_;

    public CellularSpawner cellularSpawner { get { if (cellularSpawner_ == null) { cellularSpawner_ = GetComponentInChildren<CellularSpawner>(); }; return cellularSpawner_; } }
    private CellularSpawner cellularSpawner_;

    void Update()
    {
        switch (state)
        {
            case GadgetState.On:
                cellularSpawner.gameObject.SetActive(true);
                pixelLight.gameObject.SetActive(true);
                if (cellularSensor.IsFilledWith((cell => (cell.IsLiquid() || cell.IsSolid()) && !cell.IsHotterThan(300))))
                {
                    ChangeState(GadgetState.Off);
                }
                break;
            case GadgetState.Off:
                cellularSpawner.gameObject.SetActive(false);
                pixelLight.gameObject.SetActive(false);
                if (cellularSensor.ContainsAny((cell => cell.IsHotterThan(300))))
                {
                    ChangeState(GadgetState.On);
                }
                break;
            case GadgetState.Out:
                break;
            default:
                break;
        }

    }

}
