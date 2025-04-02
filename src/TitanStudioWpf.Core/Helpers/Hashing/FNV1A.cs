namespace TitanStudioWpf.Core.Helpers.Hashing;

public enum HashType
{
    FNV1A32,
    FNV1A64,
}

public class FNV1A
{
    public static ulong Compute(string input, HashType hashType)
    {
        if (hashType == HashType.FNV1A32)
        {
            const uint FNV_PRIME = 16777619;
            const uint FNV_OFFSET = 2166136261;
            uint hash32 = FNV_OFFSET;

            foreach (char c in input)
            {
                hash32 ^= (uint)c;
                hash32 *= FNV_PRIME;
            }

            return hash32;
        }
        else if (hashType == HashType.FNV1A64)
        {
            const ulong FNV_PRIME = 1099511628211;
            const ulong FNV_OFFSET = 14695981039346656037;
            ulong hash64 = FNV_OFFSET;

            foreach (char c in input)
            {
                hash64 ^= (ulong)c;
                hash64 *= FNV_PRIME;
            }

            return hash64;
        }
        return 0;
    }
}
