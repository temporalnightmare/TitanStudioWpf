using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using TitanStudioWpf.Core.Models;

namespace TitanStudioWpf.Core.Helpers.Formatting;

public class StringDataHelper
{
    public static async Task<StringDataModel> ReadStringAsync(string databasePath)
    {
        StringDataModel sDB = new();

        // Read each byte from the file and put it into an array named 'contents'.
        byte[] contents = await File.ReadAllBytesAsync(databasePath);

        // Set the backing store to memory and read from a file into said memory.

        using (MemoryStream ms = new(contents))
        using (BinaryReader br = new(ms))
        {

            // Move the index pointer 4 bytes, we don't care about the first 4.
            br.ReadBytes(4);

            // Get the total number of string entries as a UInt32.
            sDB.fileCount = br.ReadInt32();

            // Read each string one by one, and add it to the DatabaseStrings array.
            sDB.DatabaseStrings = ReadStrings(br, sDB.fileCount);

            // Dispose the BinaryReader object for garbage collection.
            br.Dispose();
        }
        return sDB;

    }

    public static DatabaseString[] ReadStrings(BinaryReader r, int filecount)
    {
        // Read header to check for mangling
        r.BaseStream.Position = 0;
        byte[] header = r.ReadBytes(4);
        bool isMangled = IsMangled(header);

        // Reset position back to where it was.
        r.BaseStream.Position = 8;

        DatabaseString[] array = new DatabaseString[filecount];
        for (int i = 0; i < array.Length; i++)
        {
            DatabaseString databaseString = new DatabaseString();
            databaseString.Index = i;
            databaseString.Offset = r.ReadInt32();
            databaseString.Length = r.ReadInt32();
            databaseString.Id = r.ReadUInt32();
            long position = r.BaseStream.Position;
            r.BaseStream.Position = databaseString.Offset;

            // Read raw bytes
            byte[] stringBytes = r.ReadBytes((int)databaseString.Length);

            // If managled, demangle the string
            if (isMangled)
            {
                stringBytes = DemangleString(stringBytes, (int)databaseString.Offset);
            }

            databaseString.StringText = Encoding.UTF8.GetString(stringBytes).TrimEnd('\0');
            //.StringText = Encoding.UTF8.GetString(r.ReadBytes(databaseString.Length)).TrimEnd(default(char));
            r.BaseStream.Position = position;
            array[i] = databaseString;
        }
        return array;
    }

    public static bool IsMangled(byte[] header)
    {
        // Check if the first 4 bytes (header tag) indicate a mangled file
        if (header.Length < 4)
            return false;

        return BitConverter.ToInt32(header, 0) == 0x100;
    }

    public static byte[] DemangleString(byte[] data, int offset)
    {
        if (data == null || data.Length == 0)
            return data;

        byte key = (byte)((offset & 0xFF) ^ 0xCD);
        byte[] decrypted = new byte[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            decrypted[i] = (byte)(data[i] ^ key);
            key = data[i]; // Update key with the original character value
        }

        return decrypted;
    }

    public static byte[] MangleString(byte[] data, int offset)
    {
        if (data == null || data.Length == 0)
            return data;

        byte key = (byte)((offset & 0xFF) ^ 0xCD);
        byte[] encrypted = new byte[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            encrypted[i] = (byte)(data[i] ^ key);
            key = encrypted[i]; // Update key with the encrypted character value
        }

        return encrypted;
    }

    public static string DetectAndDecodeString(byte[] bytes, int offset)
    {
        // Try different encodings and return the one that produces readable text
        // This is a heuristic approach - you might need to adjust based on your specific files

        // Try UTF-16 (Unicode) first, as it seems most likely based on your screenshot
        string unicodeText = Encoding.Unicode.GetString(bytes).TrimEnd('\0');
        if (IsReadableText(unicodeText))
            return unicodeText;

        // Try UTF-8 next
        string utf8Text = Encoding.UTF8.GetString(bytes).TrimEnd('\0');
        if (IsReadableText(utf8Text))
            return utf8Text;

        // Try other encodings
        string asciiText = Encoding.ASCII.GetString(bytes).TrimEnd('\0');
        if (IsReadableText(asciiText))
            return asciiText;

        string utf16BEText = Encoding.BigEndianUnicode.GetString(bytes).TrimEnd('\0');
        if (IsReadableText(utf16BEText))
            return utf16BEText;

        // Default to Unicode if nothing else works
        return unicodeText;
    }

    private static bool IsReadableText(string text)
    {
        // A simple heuristic to check if text is readable
        // You might want to adjust this based on your specific needs
        if (string.IsNullOrEmpty(text)) return false;

        // Check if the text contains mostly printable ASCII characters
        int printableCount = 0;
        foreach (char c in text)
        {
            if ((c >= 32 && c <= 126) || c == '\n' || c == '\r' || c == '\t')
                printableCount++;
        }

        // If at least 80% of characters are printable, consider it readable
        return (double)printableCount / text.Length > 0.8;
    }

    public static ObservableCollection<StringGridItem> SerializeDataForGrid(StringDataModel sDB)
    {
        var gridItems = new ObservableCollection<StringGridItem>();

        try
        {
            if (sDB.DatabaseStrings != null)
            {
                for (int i = 0; i < sDB.DatabaseStrings.Length; i++)
                {
                    DatabaseString databaseString = sDB.DatabaseStrings[i];
                    gridItems.Add(new StringGridItem
                    {
                        Index = i,
                        ID = databaseString.Id.ToString(),
                        Offset = databaseString.Offset.ToString("X2"),
                        String = databaseString.StringText
                    });
                }
            }

            return gridItems;
        }
        catch (Exception ex)
        {
            // Handle this via a messager service or ilogger later.
            throw new Exception($"Error serializing data: {ex.Message}", ex);
        }
    }

    // Debug method to examine the binary structure of the file
    public static string ExamineBinaryFile(string filePath, int bytesToExamine = 100)
    {
        StringBuilder result = new StringBuilder();

        try
        {
            using (FileStream fs = new(filePath, FileMode.Open, FileAccess.Read))
            using (BinaryReader br = new(fs))
            {
                // Read header information
                byte[] header = br.ReadBytes(4);
                result.AppendLine($"Header: {BitConverter.ToString(header)}");

                // Read file count
                int fileCount = br.ReadInt32();
                result.AppendLine($"File Count: {fileCount}");

                // Read the first few string entries to examine structure
                result.AppendLine("\nFirst few string entries:");
                for (int i = 0; i < Math.Min(5, fileCount); i++)
                {
                    int offset = br.ReadInt32();
                    int length = br.ReadInt32();
                    int id = br.ReadInt32();

                    result.AppendLine($"Entry {i}:");
                    result.AppendLine($"  Offset: 0x{offset:X}");
                    result.AppendLine($"  Length: {length}");
                    result.AppendLine($"  ID: {id}");

                    // Save current position
                    long position = br.BaseStream.Position;

                    // Jump to string offset
                    br.BaseStream.Position = offset;

                    // Read string bytes
                    byte[] stringBytes = br.ReadBytes(length);

                    // Show raw bytes
                    result.AppendLine($"  Raw bytes: {BitConverter.ToString(stringBytes)}");

                    // Try different encodings
                    result.AppendLine($"  UTF-8: {Encoding.UTF8.GetString(stringBytes)}");
                    result.AppendLine($"  ASCII: {Encoding.ASCII.GetString(stringBytes)}");
                    result.AppendLine($"  UTF-16: {Encoding.Unicode.GetString(stringBytes)}");
                    result.AppendLine($"  UTF-16BE: {Encoding.BigEndianUnicode.GetString(stringBytes)}");

                    // Return to saved position
                    br.BaseStream.Position = position;
                }
            }

            return result.ToString();
        }
        catch (Exception ex)
        {
            return $"Error examining file: {ex.Message}";
        }
    }

}
