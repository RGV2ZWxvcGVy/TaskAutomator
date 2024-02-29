// See https://aka.ms/new-console-template for more information
using TaskAutomator;

while (true) // Keep listening to user input
{
	Console.WriteLine("\nSelect an action to perform:");
	Console.WriteLine("1. Move files larger than [x] MB to a target folder.");
	Console.WriteLine("2. Move files from the target folder back to their original folders.");
	Console.WriteLine("Type 'exit' to close the application.");
	Console.Write("Enter your choice (1, 2, or 'exit'): ");

	string? choice = Console.ReadLine();

	if (choice?.ToLower() == "exit") // Check if the user wants to exit
	{
		Console.WriteLine("Exiting application...");
		break; // Exit the loop, thus ending the program
	}

	Action action;
	switch (choice)
	{
		case "1":
			Console.Write("Enter the maximum file size (MB): ");
			int size = int.TryParse(Console.ReadLine(), out size) ? size : default; // Added basic parsing with fallback

			Console.Write("Enter the file extensions you want to move, for example, [.jpeg, .jpg, .png] (leave empty to move all files): ");
			string fileExtensions = Console.ReadLine() ?? string.Empty;

			Console.Write("Enter the source directory name: ");
			string sourceDirectory = Console.ReadLine() ?? string.Empty;

			Console.Write("Enter the target directory name: ");
			string targetDirectory = Console.ReadLine() ?? string.Empty;

			action = () => BulkActions.Media.MoveFilesIntoTargetDirectory(size, fileExtensions, sourceDirectory, targetDirectory);
			break;
		case "2":
			Console.Write("Enter the target directory name: ");
			string targetDir = Console.ReadLine() ?? string.Empty;

			Console.Write("Enter the original directory name: ");
			string originalDir = Console.ReadLine() ?? string.Empty;

			action = () => BulkActions.Media.MoveFilesBackToOriginalDirectory(targetDir, originalDir);
			break;
		default:
			action = () => Console.WriteLine("Invalid choice.");
			break;
	}

	action.Invoke();
}
