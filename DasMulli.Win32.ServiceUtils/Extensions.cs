
namespace DasMulli.Win32.ServiceUtils
{
    internal static class Extensions
    {
        internal static T[] MarshalUnmananagedArrayToStruct<T>(this System.IntPtr unmanagedArray, int length)
        {
            int size = System.Runtime.InteropServices.Marshal.SizeOf<T>();
            T[] mangagedArray = new T[length];

            for (int i = 0; i < length; i++)
            {
                System.IntPtr ins = new System.IntPtr(unmanagedArray.ToInt64() + i * size);
                mangagedArray[i] = System.Runtime.InteropServices.Marshal.PtrToStructure<T>(ins);
            }

            return mangagedArray;
        }
    }
}