using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

// Send data between client and server. Network variable can be synced between server and client. If server changes a value client gets notified and can act accordingly. 

public class TransformState : INetworkSerializable, IEquatable<TransformState>
{
    public int Tick;
    public Vector3 Position;
    public Quaternion Rotation;
    public bool HasStartedMoving;

   
    
    // This is called whenver you send data and want to receieve data. 
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter // TO-DO look up the format of this implementation <T> anonymous parameter?
    {
        if (serializer.IsReader)
        {
            var reader = serializer.GetFastBufferReader();
            reader.ReadValueSafe(out Tick); // The order you read data is the same order you must write the data.
            reader.ReadValueSafe(out Position);
            reader.ReadValueSafe(out Rotation);
            reader.ReadValueSafe(out HasStartedMoving);
        }
        else // If you are not reading you are writing data.
        {
            var writer = serializer.GetFastBufferWriter();
            writer.WriteValueSafe(Tick);
            writer.WriteValueSafe(Position);
            writer.WriteValueSafe(Rotation);
            writer.WriteValueSafe(HasStartedMoving);
        }
    }

    // Implement IEquatable<TransformState>
    public bool Equals(TransformState other)
    {
        if (other == null) return false;
        return Tick == other.Tick &&
               Position.Equals(other.Position) &&
               Rotation.Equals(other.Rotation) &&
               HasStartedMoving == other.HasStartedMoving;
    }

    // Override Equals(object obj)
    public override bool Equals(object obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != this.GetType()) return false;
        return Equals(obj as TransformState);
    }

    // Override GetHashCode
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + Tick.GetHashCode();
            hash = hash * 23 + Position.GetHashCode();
            hash = hash * 23 + Rotation.GetHashCode();
            hash = hash * 23 + HasStartedMoving.GetHashCode();
            return hash;
        }
    }
}
