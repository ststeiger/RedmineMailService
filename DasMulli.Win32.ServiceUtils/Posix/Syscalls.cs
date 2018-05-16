
namespace Posix
{
	
	
	/*
	struct passwd {
		char *pw_name;
		char *pw_passwd;
		uid_t pw_uid;
		gid_t pw_gid;
		// time_t pw_change;
		char *pw_class;
		char *pw_gecos;
		char *pw_dir;
		char *pw_shell;
		// time_t pw_expire;
	};
	*/
	
	
	// https://www.mkssoftware.com/docs/man5/struct_passwd.5.asp
	[System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct passwd_t
    {
        public string pw_name; // User's login name.
        public string pw_passwd; // Encrypted password. This field is not supported.
        
        // [uid_t] 
        public uint pw_uid; // User ID
        // [gid_t] 
        public uint pw_gid; // Group ID
	    
        // https://stackoverflow.com/questions/471248/what-is-ultimately-a-time-t-typedef-to
        // time_t pw_change;
        // public System.IntPtr pw_change; // Password change time. This field is not supported.
	    
        // int t = 1070390676; // value of time_t in an ordinary integer
        // System.DateTime dt = new System.DateTime(1970, 1, 1).AddSeconds(t); 
	    
        // public string pw_class; // User access class. This field is not supported.
        public string pw_gecos; // User's full name
        public string pw_dir; // User's login directory.
        public string pw_shell; // User's login shell.
        
	    // time_t pw_expire;
        // public IntPtr pw_expire; // Password expiration time. This field is not supported. 
    }
	
    public class Syscalls
    {
        // private const string LIBC = "/usr/lib/x86_64-linux-gnu/libc.so";
	    // private const string LIBC ="/lib/x86_64-linux-gnu/libc.so.6";
	    private const string LIBC = "c";
	    
	    [System.Runtime.InteropServices.DllImport (LIBC, SetLastError=true, EntryPoint="free"
		    , CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
	    public static extern void free (System.IntPtr ptr);
	    
	    
	    [System.CLSCompliant (false)]
	    [System.Runtime.InteropServices.DllImport (LIBC, SetLastError=true, EntryPoint="malloc"
		    ,CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
	    public static extern System.IntPtr malloc (ulong size);
	    
	    [System.Runtime.InteropServices.DllImport(LIBC, SetLastError=true, EntryPoint="chown"
		    ,CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
	    public static extern int chown(string fileName, uint owner, uint group);
	    
	    [System.Runtime.InteropServices.DllImport(LIBC, SetLastError=true, EntryPoint="getpwnam"
		    ,CallingConvention = System.Runtime.InteropServices.CallingConvention.Cdecl)]
	    private static extern System.IntPtr posix_getpwnam(string name);
	    
	    
	    public static passwd_t getpwnam(string name)
	    {
		    System.IntPtr pointer = posix_getpwnam(name);
            
		    passwd_t passwd = (passwd_t)System.Runtime.InteropServices.Marshal.PtrToStructure(
			    pointer, typeof(passwd_t));
		    
		    // System.Console.WriteLine(passwd.pw_name);
		    // System.Console.WriteLine(passwd.pw_passwd);
		    // System.Console.WriteLine(passwd.pw_uid);
		    // System.Console.WriteLine(passwd.pw_gid);
		    // System.Console.WriteLine(passwd.pw_gecos);
		    // System.Console.WriteLine(passwd.pw_dir);
		    // System.Console.WriteLine(passwd.pw_shell);
		    
		    return passwd;
	    }
	    
	    
    }
	
	
}
