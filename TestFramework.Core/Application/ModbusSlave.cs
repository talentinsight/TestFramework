using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TestFramework.Core.Logger;

namespace TestFramework.Core.Application
{
    /// <summary>
    /// Represents a Modbus slave device
    /// </summary>
    public class ModbusSlave : IDisposable
    {
        private TcpListener? _listener;
        private bool _isRunning;
        private bool _disposed;

        /// <summary>
        /// Gets the logger for the Modbus slave
        /// </summary>
        public ILogger Logger { get; }

        /// <summary>
        /// Gets whether the slave is currently running
        /// </summary>
        public bool IsRunning => _isRunning;

        /// <summary>
        /// Initializes a new instance of the ModbusSlave class
        /// </summary>
        public ModbusSlave()
        {
            Logger = new ConsoleLogger();
        }

        /// <summary>
        /// Starts the Modbus slave
        /// </summary>
        /// <param name="host">The host address to listen on</param>
        /// <param name="port">The port to listen on</param>
        public async Task StartAsync(string host, int port)
        {
            if (_isRunning)
            {
                throw new InvalidOperationException("Slave is already running");
            }

            await Task.Run(() =>
            {
                try
                {
                    _listener = new TcpListener(IPAddress.Parse(host), port);
                    _listener.Start();
                    _isRunning = true;
                    Logger.Log($"Modbus slave started on {host}:{port}", LogLevel.Info);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to start Modbus slave: {ex.Message}", LogLevel.Error);
                    throw;
                }
            });
        }

        /// <summary>
        /// Stops the Modbus slave
        /// </summary>
        public async Task StopAsync()
        {
            if (!_isRunning)
            {
                throw new InvalidOperationException("Slave is not running");
            }

            await Task.Run(() =>
            {
                try
                {
                    _listener?.Stop();
                    _isRunning = false;
                    Logger.Log("Modbus slave stopped", LogLevel.Info);
                }
                catch (Exception ex)
                {
                    Logger.Log($"Failed to stop Modbus slave: {ex.Message}", LogLevel.Error);
                    throw;
                }
            });
        }

        /// <summary>
        /// Disposes the Modbus slave
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the Modbus slave
        /// </summary>
        /// <param name="disposing">True if called from Dispose, false if called from finalizer</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_isRunning)
                    {
                        StopAsync().Wait();
                    }
                    _listener = null;
                }

                _disposed = true;
            }
        }
    }
} 