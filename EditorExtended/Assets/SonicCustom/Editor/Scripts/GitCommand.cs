using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;

public class GitCommand
{
    static int ExecuteCommand(string command, string arguments = "")
    {
        var process = new Process
        {
            StartInfo = new ProcessStartInfo(command)
            {
                Arguments = arguments,
                UseShellExecute = false,
            }
        };
        process.Start();
        process.WaitForExit();
        return process.ExitCode;
    }

    static void GitSendCommand(string arguments)
    {
        if (ExecuteCommand("git", arguments) != 0)
        {
            UnityEngine.Debug.LogError("git command error");
        }
    }

}
