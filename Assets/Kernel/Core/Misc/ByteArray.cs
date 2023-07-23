using System.Text;

public class ByteArray {
	
	#region Variables
    // DEFAULT_SIZE
    public const byte DEFAULT_SIZE = 16;
    // BOOLEAN_SIZE
    public const byte BOOLEAN_SIZE = 1;
    // BYTE_SIZE
    public const byte BYTE_SIZE = 1;
    // CHAR_SIZE
    public const byte CHAR_SIZE = 2;
    // SHORT_SIZE
    public const byte SHORT_SIZE = 2;
    // INT_SIZE
    public const byte INT_SIZE = 4;
    // LONG_SIZE
    public const byte LONG_SIZE = 8;
    
	// Current index of the array
    public int currentPos = 0;
	
	// The byte array
	public byte[] data;
	#endregion
	
	#region Constructor
	/**
	 * Default constructor
	 */
	public ByteArray() {
		data = new byte[DEFAULT_SIZE];
		currentPos = 0;
	}
	
	/**
	 * Constructor take a size of the data
	 * 
	 * @param size 	The size of the data.
	 */
	public ByteArray(uint size) {
		data = new byte[size];
		currentPos = 0;
	}
	
	/**
	 * Consturctor take a specified data source
	 * 
	 * @param src  The specified data source
	 */
	public ByteArray(byte[] src) {
		data = src;
		currentPos = 0;
	}
	#endregion
	
	#region Writing functions
	/**
     * WriteBoolean
     * 
     * @param val 	The value to be writed
     */
    public void writeBoolean(bool val) {
        ensureCapacity(BOOLEAN_SIZE);
        data[currentPos++] = (byte) (val ? 1 : 0);
    }
	
	/**
     * WriteByte takes an byte parameter
     * 
     * @param val	The value to be writed
     */
    public void writeByte(byte val) {
        ensureCapacity(BYTE_SIZE);
        data[currentPos++] = val;
    }
	
	/**
     * WriteByte takes an integer parameter
     * 
     * @param val	The value to be writed
     */
	public void writeByte(int val) {
        writeByte((byte) val);
    }
	
	/**
     * WriteChar
     * 
     * @param c		The char to be writed
     */
    public void writeChar(char c) {
        ensureCapacity(CHAR_SIZE);
        data[currentPos + 1] = (byte) (c >> 0);
        data[currentPos + 0] = (byte) (c >> 8);
        currentPos += 2;
    }
	
	/**
     * WwriteShort takes a short parameter
     * 
     * @param val	The value to be writed
     */
    public void writeShort(short val) {
        ensureCapacity(SHORT_SIZE);
        data[currentPos + 1] = (byte) (val >> 0);
        data[currentPos + 0] = (byte) (val >> 8);
        currentPos += 2;
    }
	
	/**
     * WriteShort takes an integer parameter
     * 
     * @param val	The value to be writed
     */
    public void writeShort(int val) {
        writeShort((short) val);
    }
	
    /**
     * WriteInt
     * 
     * @param val	The value to be writed
     */
    public void writeInt(int val) {
        ensureCapacity(INT_SIZE);
        data[currentPos + 3] = (byte) (val >> 0);
        data[currentPos + 2] = (byte) (val >> 8);
        data[currentPos + 1] = (byte) (val >> 16);
        data[currentPos + 0] = (byte) (val >> 24);
        currentPos += INT_SIZE;
    }
	
    /**
     * WriteLong
     * 
     * @param val	The value to be writed
     */
    public void writeLong(long val) {
        ensureCapacity(LONG_SIZE);
        data[currentPos + 7] = (byte) (val >> 0);
        data[currentPos + 6] = (byte) (val >> 8);
        data[currentPos + 5] = (byte) (val >> 16);
        data[currentPos + 4] = (byte) (val >> 24);
        data[currentPos + 3] = (byte) (val >> 32);
        data[currentPos + 2] = (byte) (val >> 40);
        data[currentPos + 1] = (byte) (val >> 48);
        data[currentPos + 0] = (byte) (val >> 56);
        currentPos += LONG_SIZE;
    }
    /**
     * WriteByteArray
     * 
     * @param src	The byte-array to be writed
     */
    public void writeByteArray(byte[] src) {
        if (src == null) {
            return;
        }
        ensureCapacity(src.Length);
        System.Array.Copy(src, 0, data, currentPos, src.Length);
        currentPos += src.Length;
    }
	
	/**
     * WriteUTF
     * 
     * @param str	The string to be writed
     */
    public void writeUTF(string str) {
        writeByteArray(getByteArrFromUTF(str));
    }
	#endregion
	
	#region Reading functions
	/**
     * ReadBoolean
     * 
     * @return bool
     */
    public bool readBoolean() {
        return data[currentPos++] != 0;
    }
	
	/**
     * ReadByte
     * 
     * @return byte
     */
    public byte readByte() {
        return data[currentPos++];
    }
	
    /**
     * ReadChar
     * 
     * @return char
     */
    public char readChar() {
        char c = (char) (((data[currentPos + 1] & 0xFF) << 0) +
                         ((data[currentPos + 0] & 0xFF) << 8));
        currentPos += CHAR_SIZE;
        return c;
    }
	
    /**
     * ReadShort
     * 
     * @return short
     */
    public short readShort() {
        short s = (short) (((data[currentPos + 1] & 0xFF) << 0) +
                           ((data[currentPos + 0] & 0xFF) << 8));
        currentPos += SHORT_SIZE;
        return s;
    }
	
    /**
     * ReadInt
     * 
     * @return int
     */
    public int readInt() {
        int i = ((data[currentPos + 3] & 0xFF) << 0) +
                ((data[currentPos + 2] & 0xFF) << 8) +
                ((data[currentPos + 1] & 0xFF) << 16) +
                ((data[currentPos + 0] & 0xFF) << 24);
        currentPos += INT_SIZE;
        return i;
    }
	
    /**
     * ReadLong
     * 
     * @return long
     */
    public long readLong() {
        long l = ((data[currentPos + 7] & 0xFFL) << 0) +
                 ((data[currentPos + 6] & 0xFFL) << 8) +
                 ((data[currentPos + 5] & 0xFFL) << 16) +
                 ((data[currentPos + 4] & 0xFFL) << 24) +
                 ((data[currentPos + 3] & 0xFFL) << 32) +
                 ((data[currentPos + 2] & 0xFFL) << 40) +
                 ((data[currentPos + 1] & 0xFFL) << 48) +
                 ((data[currentPos + 0] & 0xFFL) << 56);
        currentPos += LONG_SIZE;
        return l;
    }

	/**
     * Read a byte array that specified length from current index
     * 
     * @param length	The length to be readed
     * 
     * @return byte[]
     */
    public byte[] readByteArray(int length) {
        if (length == -1 || currentPos + length > data.Length)
        {
            length = data.Length - currentPos;
        }
        byte[] temp = new byte[length];
        System.Array.Copy(data, currentPos, temp, 0, length);
        currentPos += length;
        return temp;
    }
	
    /**
     * Read a byte array that specified the range.
     * 
     * @param off		Start index
     * @param length	Length to be readed
     * 
     * @return byte[]
     */
    public byte[] readByteArray(int off, int length) {
        if (length == -1 || off + length > data.Length) {
            length = data.Length - off;
        }
        byte[] temp = new byte[length];
        System.Array.Copy(data, off, temp, 0, length);
        return temp;
    }
	
	/**
     * Read an UTF-8 string
     * 
     * @return string
     */
    public string readUTF() {
        int utflen = readUnsignedShort();
        if (utflen == -1) {
            return "ERROR";
        }

        //float readUTF = UnityEngine.Time.realtimeSinceStartup;
        byte[] bytearr = readByteArray(utflen);
        //LocalData.test_Time_OnByteArray_ReadBytes += UnityEngine.Time.realtimeSinceStartup - readUTF;

        //float convertUTF = UnityEngine.Time.realtimeSinceStartup;
        string content = System.Text.Encoding.UTF8.GetString(bytearr);
        //LocalData.test_Time_OnByteArray_convertUTF += UnityEngine.Time.realtimeSinceStartup - convertUTF;
        return content;
    }
	
	/**
     * ReadUnsignedByte
     * 
     * @return int
     */
    public int readUnsignedByte() {
        return data[currentPos++] & 0x00FF;
    }
	
    /**
     * ReadUnsignedShort
     * 
     * @return int
     */
    public int readUnsignedShort() {
        int ch1 = readUnsignedByte();
        int ch2 = readUnsignedByte();
        if ((ch1 | ch2) < 0) {
            return -1;
        }
        return (ch1 << 8) + (ch2 << 0);
    }
	#endregion
	
	#region Functionality methods
	/**
	 * Check if the length of the data is enough.Data array will reallocate if it length is not enough.
	 * 
	 * @param length	The length we want it be.
	 */
    public void ensureCapacity(int length) {
        if (currentPos + length >= data.Length) {
            byte[] temp = new byte[data.Length + 2 * length];
            System.Array.Copy(data, temp, data.Length);
            data = temp;
        }
    }
	
	/**
	 * Skip the current index 
	 */
	public void skip(int length){
    	currentPos += length;
    }
    
	/**
	 * Get Position
	 * 
	 * @return integer 		Return the current index.
	 */
    public int getPos(){
    	return currentPos;
    }
	
	/**
     * Get the string's byte array
     * @param str	The string to read
     * 
     * @return byte[]	The byte array of the specified string
     */
    public static byte[] getByteArrFromUTF(string str) {
		
		Encoding encoding = new UTF8Encoding();
		byte[] barr = encoding.GetBytes(str);
		int utflen = barr.Length;
		
		// save length
		byte[] bytearr = new byte[utflen + 2];
		bytearr[0] = (byte) ((utflen >> 8) & 0xFF);
        bytearr[1] = (byte) ((utflen >> 0) & 0xFF);
		
		// save bytes
		System.Array.Copy(barr, 0, bytearr, 2, utflen);
		
        return bytearr; 
    }
	
	/**
     * ToByteArray
     * 
     * @return byte[]
     */
    public byte[] toByteArray() {
        if (currentPos < data.Length) {
            return readByteArray(0, currentPos);
        }
        return data;
    }
	
    /**
     * ResetPosition
     */
    public void resetPosition(){
        currentPos = 0;
    }
	
    /**
     * close
     */
    public void close() {
        data = null;
    }
	
    /**
     * RytesToInts
     * 
     * @param bytes
     * 
     * @return int[] 
     */
    public static int[] bytesToInts(byte[] bytes){
        if(bytes == null || bytes.Length < 4){
            return null;
        }
        int[] ints = new int[bytes.Length >> 2];
        ByteArray ba = new ByteArray(bytes);
        for(int i=0,kk=ints.Length; i<kk; i++){
            ints[i] = ba.readInt();
        }
        return ints;
    }
	
    /**
     * IntsToBytes
     * 
     * @param ints
     * 
     * @return byte[]
     */
    public static byte[] intsToBytes(int[] ints){
        if(ints == null || ints.Length <= 0){
            return null;
        }
        byte[] bytes = new byte[ints.Length << 2];
        ByteArray ba = new ByteArray(bytes);
        for(int i=0,kk=ints.Length; i<kk; i++){
            ba.writeInt(ints[i]);
        }
        return ba.toByteArray();
    }
    /**
     * GetLength
     * 
     * @return int 
     */
    public int getLength() {
    	return data.Length;
    }
	#endregion
	
}
