
namespace RedmineMailService.CertSSL
{


    public class cb64
    {
        private const int base64LineBreakPosition = 64;

        internal static readonly char[] base64Table = {'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O',
                                                       'P','Q','R','S','T','U','V','W','X','Y','Z','a','b','c','d',
                                                       'e','f','g','h','i','j','k','l','m','n','o','p','q','r','s',
                                                       't','u','v','w','x','y','z','0','1','2','3','4','5','6','7',
                                                       '8','9','+','/','=' };



        // public static int ConvertToBase64Array(System.IO.StreamWriter sw, byte[] inData, int offset, int length, int totalLength, bool insertLineBreaks)
        // https://www.codeproject.com/Articles/5483/Base-Encoder-Decoder-in-C
        // https://github.com/mono/mono/blob/master/mcs/class/referencesource/mscorlib/system/convert.cs
        public static int ConvertToBase64Array(System.IO.StreamWriter sw, byte[] inData, int offset, int length, bool insertLineBreaks, bool isFinal)
        {
            int lengthmod3 = length % 3;
            int calcLength = offset + (length - lengthmod3);
            int j = 0;
            int charcount = 0;
            //Convert three bytes at a time to base64 notation.  This will consume 4 chars.
            int i;

            // get a pointer to the base64Table to avoid unnecessary range checking
            {
                for (i = offset; i < calcLength; i += 3)
                {
                    if (insertLineBreaks)
                    {
                        if (charcount == base64LineBreakPosition)
                        {
                            sw.Write("\r\n");
                            j += 2;
                            charcount = 0;
                        }
                        charcount += 4;
                    }
                    sw.Write(base64Table[(inData[i] & 0xfc) >> 2]);
                    sw.Write(base64Table[((inData[i] & 0x03) << 4) | ((inData[i + 1] & 0xf0) >> 4)]);
                    sw.Write(base64Table[((inData[i + 1] & 0x0f) << 2) | ((inData[i + 2] & 0xc0) >> 6)]);
                    sw.Write(base64Table[(inData[i + 2] & 0x3f)]);
                    j += 4;
                }

                //Where we left off before
                i = calcLength;

                if (insertLineBreaks && (lengthmod3 != 0 || !isFinal) && (charcount == base64LineBreakPosition))
                {
                    sw.Write("\r\n");
                    j += 2;
                }

                switch (lengthmod3)
                {
                    case 2: //One character padding needed
                        // char char1 = base64Table[(inData[i] & 0xfc) >> 2];
                        // char char2 = base64Table[((inData[i] & 0x03) << 4) | ((inData[i + 1] & 0xf0) >> 4)];
                        // char char3 = base64Table[(inData[i + 1] & 0x0f) << 2];
                        // char char4 = base64Table[64]; //Pad

                        // string e = new string(new char[] { char1, char2, char3, char4 });
                        // System.Console.WriteLine(e);


                        sw.Write(base64Table[(inData[i] & 0xfc) >> 2]);
                        sw.Write(base64Table[((inData[i] & 0x03) << 4) | ((inData[i + 1] & 0xf0) >> 4)]);
                        sw.Write(base64Table[(inData[i + 1] & 0x0f) << 2]);
                        sw.Write(base64Table[64]); //Pad
                        j += 4;
                        break;
                    case 1: // Two character padding needed

                        // char char11 = base64Table[(inData[i] & 0xfc) >> 2];
                        // char char12 = base64Table[(inData[i] & 0x03) << 4];
                        // char char13 = base64Table[64]; //Pad
                        // char char14 = base64Table[64]; //Pad

                        // string ee = new string(new char[] { char11, char12, char13, char14 });
                        // System.Console.WriteLine(ee);

                        sw.Write(base64Table[(inData[i] & 0xfc) >> 2]);
                        sw.Write(base64Table[(inData[i] & 0x03) << 4]);
                        sw.Write(base64Table[64]); //Pad
                        sw.Write(base64Table[64]); //Pad
                        j += 4;
                        break;
                }
            }

            return j;
        }
        

    }


}
