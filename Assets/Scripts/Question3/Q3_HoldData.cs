using UnityEngine;

public struct Q3_HoldData
{
    public Symbol SymbolData { get; set; }
    public Vector3 PositionData { get; set; }

    public Q3_HoldData(Symbol _symbolData, Vector3 _positionData)
    {
        SymbolData = _symbolData;
        PositionData = _positionData;
    }
}
