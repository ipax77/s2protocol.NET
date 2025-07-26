namespace s2protocol.NET.Mpq;

public sealed partial class MPQArchive
{
    /// <summary>
    /// Extracts the contents of the archive into a dictionary where the keys are file names and the values are the
    /// corresponding file data as byte arrays.
    /// </summary>
    /// <remarks>This method requires a valid listfile to be present in order to extract the archive. If the
    /// listfile is missing or empty, an exception is thrown. Each file name in the listfile is used to retrieve the
    /// corresponding file data. If a file cannot be read, an empty byte array is returned for that file.</remarks>
    /// <returns>A dictionary containing the file names as keys and their corresponding file data as byte arrays. If a file
    /// cannot be read, its value in the dictionary will be an empty byte array.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the archive does not have a valid listfile or if the listfile is empty.</exception>
    public Dictionary<string, byte[]> Extract()
    {
        if (_files == null || _files.Length == 0)
            throw new InvalidOperationException("Cannot extract archive without a listfile.");

        var result = new Dictionary<string, byte[]>();
        var text = System.Text.Encoding.UTF8.GetString(_files);
        var fileNames = text.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        foreach (var file in fileNames)
        {

            byte[]? data = ReadFile(file);
            result[file] = data ?? Array.Empty<byte>();
        }
        return result;
    }
    private static readonly string[] separatorArray1 = new[] { "\r\n", "\n" };
    private static readonly string[] separatorArray0 = new[] { "\r\n", "\n" };
    private static readonly string[] separator = new[] { "\r\n", "\n" };

    /// <summary>
    /// Extracts the contents of the archive to a directory on disk.
    /// </summary>
    /// <remarks>The method creates a directory named after the archive (excluding its extension) in the
    /// current working directory and extracts all files listed in the archive's listfile to this directory.
    /// Subdirectories are created as needed to preserve the original file structure.</remarks>
    /// <exception cref="InvalidOperationException">Thrown if the archive does not contain a valid listfile or if the listfile is empty.</exception>
    public void ExtractToDisk()
    {
        if (_files == null || _files.Length == 0)
            throw new InvalidOperationException("Cannot extract archive without a listfile.");

        string archiveName = Path.GetFileNameWithoutExtension(_archivePath);
        string outputDir = Path.Combine(Directory.GetCurrentDirectory(), archiveName);

        Directory.CreateDirectory(outputDir);
        var text = System.Text.Encoding.UTF8.GetString(_files);
        var fileNames = text.Split(separatorArray0, StringSplitOptions.RemoveEmptyEntries);
        foreach (var file in fileNames)
        {
            byte[]? data = ReadFile(file);
            if (data == null) continue;

            string fullPath = Path.Combine(outputDir, file.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);
            File.WriteAllBytes(fullPath, data);
        }
    }

    /// <summary>
    /// Extracts the specified files to the current working directory.
    /// </summary>
    /// <remarks>This method reads the specified files and writes their contents to the current working
    /// directory,  preserving the directory structure indicated by the file paths. If a file cannot be read, it is
    /// skipped.</remarks>
    /// <param name="filenames">An array of file names to extract. Each file name should be a valid relative path.</param>
    public void ExtractFilesToDisc(params string[] filenames)
    {
        ArgumentNullException.ThrowIfNull(filenames);
        foreach (var file in filenames)
        {
            byte[]? data = ReadFile(file);
            if (data == null) continue;

            string outputPath = Path.Combine(Directory.GetCurrentDirectory(), file.Replace('/', Path.DirectorySeparatorChar));
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            File.WriteAllBytes(outputPath, data);
        }
    }

    /// <summary>
    /// Extracts the specified files from the archive and returns their contents as a dictionary.
    /// </summary>
    /// <remarks>The method requires the archive to have a valid listfile that contains the names of all
    /// available files. If a file is not found in the listfile, the method will throw a <see
    /// cref="FileNotFoundException"/>.</remarks>
    /// <param name="filenames">An array of file names to extract from the archive. Each file name must match an entry in the archive's
    /// listfile.</param>
    /// <returns>A dictionary where the keys are the file names and the values are the file contents as byte arrays.  If a file
    /// has no content, its value will be an empty byte array.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the archive does not have a listfile or the listfile is empty.</exception>
    /// <exception cref="FileNotFoundException">Thrown if any of the specified <paramref name="filenames"/> do not exist in the archive.</exception>
    public Dictionary<string, byte[]> ExtractFiles(params string[] filenames)
    {
        ArgumentNullException.ThrowIfNull(filenames);
        if (_files == null || _files.Length == 0)
            throw new InvalidOperationException("Cannot extract archive without a listfile.");

        var result = new Dictionary<string, byte[]>();
        var text = System.Text.Encoding.UTF8.GetString(_files);
        var availableFileNames = text.Split(separatorArray1, StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        foreach (var file in filenames)
        {
            if (!availableFileNames.Contains(file))
            {
                throw new FileNotFoundException(file);
            }
            byte[]? data = ReadFile(file, true);
            result[file] = data ?? Array.Empty<byte>();
        }
        return result;
    }

    /// <summary>
    /// Retrieves the content of the user data header as a byte array.
    /// </summary>
    /// <returns>A byte array containing the content of the user data header.  Returns an empty array if the user data header is
    /// not set.</returns>
    public byte[] GetUserDataHeaderContent()
    {
        if (_userDataHeader is null)
        {
            return [];
        }
        return _userDataHeader.Value.Content;
    }
}
