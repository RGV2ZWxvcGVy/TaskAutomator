namespace TaskAutomator
{
    /// <summary>
    /// A class that contains methods that can handle bulk actions.
    /// </summary>
    public static class BulkActions
    {
        /// <summary>
        /// Methods for media bulk actions.
        /// </summary>
        public static class Media
        {
            /// <summary>
            /// Moves files with a file size greater than the max allowed file size into a separate directory.
            /// </summary>
            /// <param name="sourceDirectory">The source directory that contains the files that should be moved.</param>
            /// <param name="targetDirectory">The target directory that contains the files that exceed the max allowed file size.</param>
            /// <param name="fileExtensions">The file extensions that should be moved to the target directory (optional).</param>
            /// <param name="minFileSize">The minimum file size in MB, which should be moved into the target directory.</param>
            /// <param name="maxFileSize">The maximum file size in MB, which should be moved into the target directory.</param>
            public static void MoveFilesIntoTargetDirectory(string sourceDirectory, string targetDirectory, string fileExtensions, int minFileSize, int maxFileSize)
            {
                if (IsFilePathValid(sourceDirectory) && !Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine("Source directory does not exist.");
                    return;
                }

                // Ensure the resize directory exists
                if (IsFilePathValid(targetDirectory) && !Directory.Exists(targetDirectory))
                {
                    PerformFileOperation(targetDirectory, () => Directory.CreateDirectory(targetDirectory));
                }

                int fileCount = 0;
                // Loop through all the folders in the source directory
                foreach (var folder in Directory.GetDirectories(sourceDirectory))
                {
                    string folderName = new DirectoryInfo(folder).Name;

                    // Loop through each file in the folder
                    foreach (var file in Directory.GetFiles(folder))
                    {
                        FileInfo fileInfo = new(file);

                        // Check for file extension, and file size (in MB), if specified
                        if ((string.IsNullOrWhiteSpace(fileExtensions) || fileExtensions.Contains(fileInfo.Extension, StringComparison.OrdinalIgnoreCase)) &&
                            (minFileSize == default || fileInfo.Length >= minFileSize * 1024 * 1024) &&
                            (maxFileSize == default || fileInfo.Length <= maxFileSize * 1024 * 1024))
                        {
                            string newFileName = $"{folderName}_{fileInfo.Name}";
                            string newFilePath = Path.Combine(targetDirectory, newFileName);

                            // Move file to target folder with new name
                            PerformFileOperation(newFilePath, () => File.Move(file, newFilePath));
                            Console.WriteLine($"Moved: {fileInfo.Name} to {newFilePath}.");
                            fileCount++;
                        }
                    }
                }

                Console.WriteLine("TaskAutomator:BulkActions:Media:MoveFilesIntoTargetDirectory - Processing complete.");
                Console.WriteLine($"Total files moved: {fileCount}.");
            }

            /// <summary>
            /// Moves the files back into it's original directory based on the prefix of the file name.
            /// </summary>
            /// <param name="sourceDirectory">The source directory that contains the files that should be moved.</param>
            /// <param name="originalDirectory">The directory from which the files originated.</param>
            public static void MoveFilesBackToOriginalDirectory(string sourceDirectory, string originalDirectory)
            {
                if (IsFilePathValid(sourceDirectory) && IsFilePathValid(originalDirectory) && !Directory.Exists(sourceDirectory))
                {
                    Console.WriteLine("Source directory does not exist.");
                    return;
                }

                int folderCount = 0;
                int fileCount = 0;
                // Loop through all the files in the source directory
                foreach (var file in Directory.GetFiles(sourceDirectory))
                {
                    FileInfo fileInfo = new(file);
                    string fileName = fileInfo.Name;

                    // Extract the original folder name from the file name
                    int underscoreIndex = fileName.IndexOf('_');
                    if (underscoreIndex > 0)
                    {
                        string originalFolderName = fileName[..underscoreIndex];
                        string originalFileName = fileName[(underscoreIndex + 1)..];
                        string originalFolderPath = Path.Combine(originalDirectory, originalFolderName);
                        string originalFilePath = Path.Combine(originalFolderPath, originalFileName);

                        // Ensure the original folder exists
                        if (IsFilePathValid(originalFolderPath) && !Directory.Exists(originalFolderPath))
                        {
                            // Create the folder if it does not exist
                            PerformFileOperation(originalFolderPath, () => Directory.CreateDirectory(originalFolderPath));
                            Console.WriteLine($"Created folder: {originalFolderPath}.");
                            folderCount++;
                        }

                        // Move the file back to its original folder
                        PerformFileOperation(originalFilePath, () => File.Move(file, originalFilePath));
                        Console.WriteLine($"Moved: {fileName} to {originalFilePath}.");
                        fileCount++;
                    }
                }

                Console.WriteLine("TaskAutomator:BulkActions:Media:MoveFilesBackToOriginalDirectory - Processing complete.");
                Console.WriteLine($"Total folders created: {folderCount}.");
                Console.WriteLine($"Total files moved: {fileCount}.");
            }
        }


        public static bool IsPathWithinRootDirectory(string rootDirectory, string targetPath)
        {
            // Get the full path to ensure resolution of relative paths.
            var fullPath = Path.GetFullPath(targetPath);
            var rootFullPath = Path.GetFullPath(rootDirectory);

            // Check if the target path starts with the root directory path
            return fullPath.StartsWith(rootFullPath, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsFilePathValid(string filePath)
        {
            string applicationRoot = AppDomain.CurrentDomain.BaseDirectory;
            if (IsPathWithinRootDirectory(applicationRoot, filePath))
            {
                return true;
            }

            throw new UnauthorizedAccessException("Access to the path is denied.");
        }

        public static void PerformFileOperation(string filePath, Action action)
        {
            string applicationRoot = AppDomain.CurrentDomain.BaseDirectory;
            if (IsPathWithinRootDirectory(applicationRoot, filePath))
            {
                // Safe to perform file operations
                action.Invoke();
            }
            else
            {
                throw new UnauthorizedAccessException("Access to the path is denied.");
            }
        }
    }
}
