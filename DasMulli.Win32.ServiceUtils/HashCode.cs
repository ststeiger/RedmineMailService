
namespace DasMulli.Win32.ServiceUtils
{
    /// <summary>
    /// Simplifies the work of hashing.
    /// Taken from https://rehansaeed.com/gethashcode-made-easy/", and modified with Reshaper
    /// </summary>
    internal struct HashCode
    {
        private readonly int value;
        private HashCode(int value)
        {
            this.value = value;
        }
        public static implicit operator int(HashCode hashCode)
        {
            return hashCode.value;
        }
        public static HashCode Of<T>(T item)
        {
            return new HashCode(GetHashCode(item));
        }
        public HashCode And<T>(T item)
        {
            return new HashCode(CombineHashCodes(this.value, GetHashCode(item)));
        }

        public HashCode AndEach<T>(System.Collections.Generic.IEnumerable<T> items)
        {
            // int hashCode = items.Select(GetHashCode).Aggregate(CombineHashCodes);
            int hashCode = 0;
            using (System.Collections.Generic.IEnumerator<T> e = items.GetEnumerator())
            {
                if (!e.MoveNext())
                    throw new System.Exception("NoElements");

                hashCode = GetHashCode(e.Current);

                while (e.MoveNext())
                    hashCode = CombineHashCodes(hashCode, GetHashCode(e.Current) );
            } // End Using e 

            return new HashCode(CombineHashCodes(this.value, hashCode));
        }


        private static int CombineHashCodes(int h1, int h2)
        {
            unchecked
            {
                // Code copied from System.Tuple so it must be the best way to combine hash codes or at least a good one.
                return ((h1 << 5) + h1) ^ h2;
            }
        }


        private static int GetHashCode<T>(T item)
        {
            return item == null ? 0 : item.GetHashCode();
        }


    }


}
