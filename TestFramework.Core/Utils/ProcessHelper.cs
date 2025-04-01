using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestFramework.Core.Utils
{
    /// <summary>
    /// Helper class for managing external processes, particularly for C++ application testing
    /// </summary>
    public class ProcessHelper
    {
        private Process? _process;
        private readonly StringBuilder _standardOutput = new StringBuilder();
        private readonly StringBuilder _standardError = new StringBuilder();
        private readonly List<string> _outputLines = new List<string>();
        private readonly List<string> _errorLines = new List<string>();

        /// <summary>
        /// Gets the standard output from the process
        /// </summary>
        public string StandardOutput => _standardOutput.ToString();

        /// <summary>
        /// Gets the standard error from the process
        /// </summary>
        public string StandardError => _standardError.ToString();

        /// <summary>
        /// Gets the output lines from the process
        /// </summary>
        public IReadOnlyList<string> OutputLines => _outputLines;

        /// <summary>
        /// Gets the error lines from the process
        /// </summary>
        public IReadOnlyList<string> ErrorLines => _errorLines;

        /// <summary>
        /// Gets the exit code from the process
        /// </summary>
        public int ExitCode => _process?.ExitCode ?? -1;

        /// <summary>
        /// Gets a value indicating whether the process is running
        /// </summary>
        public bool IsRunning => _process != null && !_process.HasExited;

        /// <summary>
        /// Starts a process with the specified executable and arguments
        /// </summary>
        /// <param name="executable">The path to the executable</param>
        /// <param name="arguments">Command line arguments</param>
        /// <param name="workingDirectory">Working directory for the process</param>
        /// <returns>True if the process started successfully</returns>
        public bool Start(string executable, string arguments = "", string? workingDirectory = null)
        {
            try
            {
                _standardOutput.Clear();
                _standardError.Clear();
                _outputLines.Clear();
                _errorLines.Clear();

                var startInfo = new ProcessStartInfo
                {
                    FileName = executable,
                    Arguments = arguments,
                    WorkingDirectory = workingDirectory ?? Directory.GetCurrentDirectory(),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                _process = new Process { StartInfo = startInfo };
                
                _process.OutputDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        _standardOutput.AppendLine(e.Data);
                        _outputLines.Add(e.Data);
                    }
                };
                
                _process.ErrorDataReceived += (sender, e) =>
                {
                    if (e.Data != null)
                    {
                        _standardError.AppendLine(e.Data);
                        _errorLines.Add(e.Data);
                    }
                };

                _process.Start();
                _process.BeginOutputReadLine();
                _process.BeginErrorReadLine();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Sends input to the process
        /// </summary>
        /// <param name="input">Input to send</param>
        public void SendInput(string input)
        {
            if (IsRunning && _process!.StartInfo.RedirectStandardInput)
            {
                _process.StandardInput.WriteLine(input);
            }
        }

        /// <summary>
        /// Waits for the process to exit
        /// </summary>
        /// <param name="timeoutMs">Timeout in milliseconds</param>
        /// <returns>True if the process exited within the specified timeout</returns>
        public bool WaitForExit(int timeoutMs = 30000)
        {
            if (_process == null)
            {
                return false;
            }

            return _process.WaitForExit(timeoutMs);
        }

        /// <summary>
        /// Waits for a specific output string from the process
        /// </summary>
        /// <param name="expectedOutput">Expected output string</param>
        /// <param name="timeoutMs">Timeout in milliseconds</param>
        /// <returns>True if the expected output was received within the timeout</returns>
        public async Task<bool> WaitForOutputAsync(string expectedOutput, int timeoutMs = 30000)
        {
            if (_process == null)
            {
                return false;
            }

            using var cancellationTokenSource = new CancellationTokenSource(timeoutMs);
            var token = cancellationTokenSource.Token;

            while (!token.IsCancellationRequested)
            {
                if (StandardOutput.Contains(expectedOutput))
                {
                    return true;
                }

                await Task.Delay(100, token).ConfigureAwait(false);
            }

            return false;
        }

        /// <summary>
        /// Kills the process if it is running
        /// </summary>
        public void Kill()
        {
            if (IsRunning)
            {
                try
                {
                    _process!.Kill(true);
                }
                catch (Exception)
                {
                    // Process might have exited between the check and the kill call
                }
            }
        }

        /// <summary>
        /// Disposes the process
        /// </summary>
        public void Dispose()
        {
            Kill();
            _process?.Dispose();
            _process = null;
        }
    }
} 