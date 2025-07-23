using UnityEngine;
using UnityEditor;
using System.IO;

namespace CafeConnect3D.Editor
{
    /// <summary>
    /// Automated build script for CafeConnect3D
    /// Supports building both client and server versions
    /// </summary>
    public class BuildScript
    {
        private static readonly string BUILD_PATH = "Build";
        private static readonly string CLIENT_PATH = Path.Combine(BUILD_PATH, "Client");
        private static readonly string SERVER_PATH = Path.Combine(BUILD_PATH, "Server");
        
        [MenuItem("CafeConnect3D/Build/Build Client")]
        public static void BuildClient()
        {
            Debug.Log("[BuildScript] Starting client build...");
            
            // Ensure build directory exists
            Directory.CreateDirectory(CLIENT_PATH);
            
            // Build settings
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenePaths(),
                locationPathName = Path.Combine(CLIENT_PATH, "CafeConnect3D.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            };
            
            // Execute build
            var report = BuildPipeline.BuildPlayer(buildOptions);
            
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"[BuildScript] Client build succeeded: {report.summary.totalSize} bytes");
            }
            else
            {
                Debug.LogError($"[BuildScript] Client build failed: {report.summary.result}");
            }
        }
        
        [MenuItem("CafeConnect3D/Build/Build Server")]
        public static void BuildServer()
        {
            Debug.Log("[BuildScript] Starting server build...");
            
            // Ensure build directory exists
            Directory.CreateDirectory(SERVER_PATH);
            
            // Build settings for dedicated server
            BuildPlayerOptions buildOptions = new BuildPlayerOptions
            {
                scenes = GetScenePaths(),
                locationPathName = Path.Combine(SERVER_PATH, "CafeConnect3D_Server.exe"),
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.EnableHeadlessMode
            };
            
            // Execute build
            var report = BuildPipeline.BuildPlayer(buildOptions);
            
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Succeeded)
            {
                Debug.Log($"[BuildScript] Server build succeeded: {report.summary.totalSize} bytes");
                CreateServerStartScript();
            }
            else
            {
                Debug.LogError($"[BuildScript] Server build failed: {report.summary.result}");
            }
        }
        
        [MenuItem("CafeConnect3D/Build/Build All")]
        public static void BuildAll()
        {
            BuildClient();
            BuildServer();
        }
        
        private static string[] GetScenePaths()
        {
            // Get all scenes in build settings
            string[] scenes = new string[EditorBuildSettings.scenes.Length];
            
            for (int i = 0; i < scenes.Length; i++)
            {
                scenes[i] = EditorBuildSettings.scenes[i].path;
            }
            
            return scenes;
        }
        
        private static void CreateServerStartScript()
        {
            string scriptContent = @"@echo off
echo Starting CafeConnect3D Server...
CafeConnect3D_Server.exe -batchmode -nographics -server
pause";
            
            string scriptPath = Path.Combine(SERVER_PATH, "StartServer.bat");
            File.WriteAllText(scriptPath, scriptContent);
            
            Debug.Log($"[BuildScript] Created server start script: {scriptPath}");
        }
    }
}