using System;

public struct HitHurtPair : IEquatable<HitHurtPair>
{
    public HitSubject Src;
    public HurtSubject Dest;

    public string SrcStamp;
    public string DestStamp;

    public bool Equals(HitHurtPair other)
    {
        return Src == other.Src && Dest == other.Dest;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Src.GetHashCode(), Dest.GetHashCode());
    }
}