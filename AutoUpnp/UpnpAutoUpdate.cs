using Mono.Nat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpnp
{
    public class UpnpAutoUpdate
    {
        public void Run()
        {
            NatUtility.DeviceFound += DeviceFound;
            NatUtility.StartDiscovery();

            while (true)
            {
                try
                {
                    if (!NatUtility.IsSearching)
                    {
                        NatUtility.StopDiscovery();
                    }
                    Thread.Sleep(60000);
                } catch(Exception e) {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        private void DeviceFound(object? sender, DeviceEventArgs args)
        {
            INatDevice device = args.Device;

            Check(device);
        }

        void Check(INatDevice device)
        {
            while (true)
            {
                Check(device, 80);
                Check(device, 443);
                Thread.Sleep(1000);
            }
        }

        void Check(INatDevice device, int port)
        {
            try
            {
                var mapping = device.GetAllMappings().Where(p => p.PublicPort == port && p.Protocol == Protocol.Tcp);

                if (mapping != null && mapping.Any())
                    return;

                device.CreatePortMap(new Mapping(Protocol.Tcp, port, port, 0, "Justin Computer Webserver"));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
