using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

class Program
{
    static void Main()
{
    string repoPath = FindGitRepository();

    if (repoPath == null)
    {
        Console.WriteLine("No Git repository found.");
        Console.ReadKey();
        return;
    }

    Console.Title = "Git Helper";

    while (true)
    {
        Console.Clear();

        Console.WriteLine("==============================");
        Console.WriteLine("        Git Helper");
        Console.WriteLine("==============================");
        Console.WriteLine();

        Console.WriteLine("Repository:");
        Console.WriteLine(repoPath);
        Console.WriteLine();

        bool hasChanges = RepositoryHasChanges(repoPath);

        if (hasChanges)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Changes detected.");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("No changes.");
        }

        Console.ResetColor();

        Console.WriteLine();
        Console.WriteLine("[R] Refresh");
        Console.WriteLine("[A] Commit & Push");
        Console.WriteLine("[Q] Exit");
        Console.WriteLine();

        Console.Write("Choice: ");

        ConsoleKey key = Console.ReadKey(true).Key;
        Console.WriteLine($"Pressed: {key}");

        switch (key)
        {
            case ConsoleKey.R:
                continue;

            case ConsoleKey.Q:
                return;

            case ConsoleKey.A:

                if (!hasChanges)
                {
                    Console.WriteLine();
                    Console.WriteLine("Nothing to commit.");
                    Console.ReadKey();
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine("You pressed C");
                Console.Write("Commit Message: ");

                string? message = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(message))
                    continue;

                CommitAndPush(repoPath, message);

                Console.WriteLine();
                Console.WriteLine("Press any key...");
                Console.ReadKey();
                break;
        }
    }
}

    static string FindGitRepository()
    {
        DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);

        while (dir != null)
        {
            if (Directory.Exists(Path.Combine(dir.FullName, ".git")))
                return dir.FullName;

            dir = dir.Parent;
        }

        return null;
    }

    static bool RepositoryHasChanges(string repo)
    {
        string output = RunGit(repo, "status --porcelain");

        return !string.IsNullOrWhiteSpace(output);
    }
       static void CommitAndPush(string repo, string message)
{
    try
    {
        Console.WriteLine();

        Console.WriteLine("Adding files...");
        RunGit(repo, "add .");

        Console.WriteLine("Creating commit...");
        RunGit(repo, $"commit -m \"{message}\"");

        Console.WriteLine("Pushing to GitHub...");
        RunGit(repo, "push");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine("Done! Changes uploaded successfully.");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine();
        Console.WriteLine("Git Error:");
        Console.WriteLine(ex.Message);
        Console.ResetColor();
    }
}

    static string RunGit(string repo, string arguments)
{
    Process process = new Process();

    process.StartInfo.FileName = "git";
    process.StartInfo.Arguments = arguments;
    process.StartInfo.WorkingDirectory = repo;

    process.StartInfo.RedirectStandardOutput = true;
    process.StartInfo.RedirectStandardError = true;

    process.StartInfo.UseShellExecute = false;
    process.StartInfo.CreateNoWindow = true;

    process.Start();

    string output = process.StandardOutput.ReadToEnd();
    string error = process.StandardError.ReadToEnd();

    process.WaitForExit();

    // بعض أوامر git تكتب الرسائل في stdout وليس stderr
    string result = output + error;

    if (arguments.StartsWith("commit") &&
        result.Contains("nothing to commit", StringComparison.OrdinalIgnoreCase))
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("Nothing to commit.");
        Console.ResetColor();

        return result;
    }

    if (process.ExitCode != 0)
    {
        throw new Exception(result);
    }

    return result;
}
}


