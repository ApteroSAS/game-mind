using System;
using Unity.Netcode;
using UnityEngine;

[System.Serializable]
public struct Q3_HoldData : INetworkSerializable, IEquatable<Q3_HoldData>
{
    public Symbol SymbolData;
    public Vector3 PositionData;

    public Q3_HoldData(Symbol _symbolData, Vector3 _positionData)
    {
        SymbolData = _symbolData;
        PositionData = _positionData;
    }

    public bool Equals(Q3_HoldData other)
    {
        Debug.Log("Equals has been used?");
        return SymbolData.Equals(other.SymbolData) && PositionData.Equals(other.PositionData);
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref SymbolData);
        serializer.SerializeValue(ref PositionData);
    }
}
