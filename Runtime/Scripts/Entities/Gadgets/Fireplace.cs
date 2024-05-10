using System;
using System.Collections;
using System.Collections.Generic;

public class Fireplace : Gadget
{

    public CellularSensor cellularSensor { get { if (cellularSensor_ == null) { cellularSensor_ = GetComponent<CellularSensor>(); }; return cellularSensor_; } }
    private CellularSensor cellularSensor_;

    void Start()
    {
        cellularSensor.OnCellChanged += OnCellChanged;
    }

    private void OnCellChanged(Cell cell)
    {
        if (state == GadgetState.On && cell.IsLiquid())
        {
            ChangeState(GadgetState.Off);
        }
        if (state == GadgetState.Off && cell.IsHotterThan(300))
        {
            ChangeState(GadgetState.On);
        }
    }

}
