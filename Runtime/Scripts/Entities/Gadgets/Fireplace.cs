using System;
using System.Collections;
using System.Collections.Generic;

public class Fireplace : Gadget
{

    public CellularSensor cellularSensor { get { if (cellularSensor_ == null) { cellularSensor_ = GetComponent<CellularSensor>(); }; return cellularSensor_; } }
    private CellularSensor cellularSensor_;


    public PixelLight pixelLight { get { if (pixelLight_ == null) { pixelLight_ = GetComponentInChildren<PixelLight>(); }; return pixelLight_; } }
    private PixelLight pixelLight_;


    void Start()
    {
        cellularSensor.OnCellChanged += OnCellChanged;
    }

    private void OnCellChanged(Cell cell)
    {
        if (state == GadgetState.On && cell.IsLiquid() && !cell.IsHotterThan(300))
        {
            pixelLight.gameObject.SetActive(false);
            ChangeState(GadgetState.Off);
        }
        if (state == GadgetState.Off && cell.IsHotterThan(300))
        {
            pixelLight.gameObject.SetActive(true);
            ChangeState(GadgetState.On);
        }
    }

}
