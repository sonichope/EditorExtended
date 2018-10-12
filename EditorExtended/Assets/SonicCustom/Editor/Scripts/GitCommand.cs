using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;

public class GitCommand
{
    enum CommandType
    {
        Push,
        Commit,
        Add
    }

    private static void GitLog(int message, CommandType commandType)
    {
        switch (message)
        {
            case 0:
                UnityEngine.Debug.Log("[git] " + commandType + "コマンド成功");
                return;
        }
        
    }

    private static ProcessStartInfo processInfo;
    private static string repositoryPath;

    //gitの初期化
    public static void Init()
    {
        processInfo = new ProcessStartInfo("git", "");
        processInfo.CreateNoWindow = true;
        processInfo.UseShellExecute = false;
        repositoryPath = Environment.CurrentDirectory;
        repositoryPath = repositoryPath.Remove(repositoryPath.LastIndexOf('\\'));
    }

    //gitの初期化確認
    public static bool IsInit()
    {
        return (processInfo != null && repositoryPath != "");
    }

   //gitのコミット
   public static void Commit(string message)
    {
        if(repositoryPath == "") { return; }
        string arguments = "-C " + repositoryPath + " commit -m \"" + message + "\"";
        processInfo.Arguments = arguments;
        Process process = new Process
        {
            StartInfo = processInfo
        };
        process.Start();
        process.WaitForExit();
        GitLog(process.ExitCode,CommandType.Commit);
    }

    //gitのプッシュ
    public static void Push()
    {
        if (repositoryPath == "") { return; }
        string arguments = "-C " + repositoryPath + " push";
        processInfo.Arguments = arguments;
        Process process = new Process
        {
            StartInfo = processInfo
        };
        process.Start();
        process.WaitForExit();
        GitLog(process.ExitCode,CommandType.Push);
    }

   //gitのインデックスの追加
   public static void Add(string path)
    {
        if (repositoryPath == "") { return; }
        string arguments = "-C " + repositoryPath + " add " + path;
        processInfo.Arguments = arguments;
        Process process = new Process
        {
            StartInfo = processInfo
        };
        process.Start();
        process.WaitForExit();
        GitLog(process.ExitCode,CommandType.Add);
    }

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

    public static void GitSendCommand(string arguments)
    {
        if (ExecuteCommand("git", arguments) != 0)
        {

        }
    }

}
