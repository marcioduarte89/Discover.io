﻿using Grpc.Core;
using System.Collections.Generic;

namespace Discoverio.Server.Services.LoadBalancing.RoundRobin
{
    internal class HostDistributor
    {
        private readonly object _indexLock = new object();
        private int CurrentIndex { get; set; }
        private List<string> Hosts { get; set; }

        public HostDistributor()
        {
            CurrentIndex = 0;
            Hosts = new List<string>();
        }

        public void AddHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new RpcException(new Status(StatusCode.InvalidArgument, $"Host cannot be null or empty"));
            }

            if (Hosts.Contains(host))
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, $"Host {host} already exists"));
            }

            Hosts.Add(host);
        }

        public void RemoveHost(string host)
        {
            if (string.IsNullOrWhiteSpace(host))
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, $"Host cannot be null or empty"));
            }

            if (!HasHost(host))
            {
                throw new RpcException(new Status(StatusCode.AlreadyExists, $"Host {host} does not exist"));
            }

            Hosts.Remove(host);
        }

        public bool HasHost(string host)
        {
            return Hosts.Contains(host);
        }

        public string NextHost()
        {
            lock (_indexLock)
            {
                if (CurrentIndex == Hosts.Count)
                {
                    CurrentIndex = 0;
                }

                return Hosts[CurrentIndex++];
            }
        }

        public bool HasHosts()
        {
            return Hosts.Count > 0;
        }
    }
}
