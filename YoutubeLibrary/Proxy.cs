﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Proxy.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   Defines the Proxy type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibrary
{
    /// <summary>
    ///     The proxy.
    /// </summary>
    public class Proxy
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Proxy" /> class.
        /// </summary>
        public Proxy()
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Proxy" /> class.
        /// </summary>
        /// <param name="ip">
        ///     The IP.
        /// </param>
        /// <param name="port">
        ///     The port.
        /// </param>
        public Proxy(string ip, string port)
        {
            this.Ip = ip;
            this.Port = port;
            this.HasProxy = true;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Proxy" /> class.
        /// </summary>
        /// <param name="ip">
        ///     The IP.
        /// </param>
        /// <param name="port">
        ///     The port.
        /// </param>
        /// <param name="credentials">
        ///     The credentials.
        /// </param>
        public Proxy(string ip, string port, Credentials credentials)
        {
            this.Ip = ip;
            this.Port = port;
            this.Credentials = credentials;
            this.HasCredentials = true;
            this.HasProxy = true;
        }

        /// <summary>
        ///     Gets the credentials.
        /// </summary>
        public Credentials Credentials { get; }

        /// <summary>
        ///     Gets a value indicating whether the proxy has credentials.
        /// </summary>
        public bool HasCredentials { get; }

        /// <summary>
        ///     Gets a value indicating whether there is a proxy available.
        /// </summary>
        public bool HasProxy { get; }

        /// <summary>
        ///     Gets the proxy formatted.
        /// </summary>
        public string ProxyFormated => !this.HasProxy ? "N/A" : $"{this.Ip}:{this.Port}";

        /// <summary>
        ///     Gets the IP.
        /// </summary>
        private string Ip { get; }

        /// <summary>
        ///     Gets the port.
        /// </summary>
        private string Port { get; }
    }
}