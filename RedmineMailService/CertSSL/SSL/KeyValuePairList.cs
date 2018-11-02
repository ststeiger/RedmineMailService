
namespace AnySqlWebAdmin
{

    public class KeyValuePairList<TKey, TValue>
        : System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<TKey, TValue>>
    {
        public void Add(TKey key, TValue value)
        {
            this.Add(new System.Collections.Generic.KeyValuePair<TKey, TValue>(key, value));
        }

        public System.Collections.Generic.List<TKey> Keys
        {
            get
            {

                System.Collections.Generic.List<TKey> ls
                    = new System.Collections.Generic.List<TKey>();

                for (int i = 0; i < this.Count; ++i)
                {
                    ls.Add(this[i].Key);
                }

                return ls;
            }
        }

        public System.Collections.Generic.List<TValue> Values
        {
            get
            {
                System.Collections.Generic.List<TValue> ls
                    = new System.Collections.Generic.List<TValue>();

                for (int i = 0; i < this.Count; ++i)
                {
                    ls.Add(this[i].Value);
                }

                return ls;
            }
        }

    }


}
