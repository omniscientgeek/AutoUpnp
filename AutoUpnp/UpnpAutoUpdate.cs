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
                Check(device, 80, Protocol.Tcp);
                Check(device, 443, Protocol.Tcp);
                Check(device, 51820, Protocol.Udp);
                Check(device, 19132, Protocol.Udp);
                Check(device, 19133, Protocol.Udp);

                //Dayz
                Check(device, 2302, Protocol.Udp);
                Check(device, 2303, Protocol.Udp);
                Check(device, 2304, Protocol.Udp);
                Check(device, 2305, Protocol.Udp);
                Check(device, 27016, Protocol.Udp);


                Thread.Sleep(1000);
            }
        }

        void Check(INatDevice device, int port, Protocol protocol)
        {
            try
            {
                var mapping = device.GetAllMappings().Where(p => p.PublicPort == port && p.Protocol == protocol);

                if (mapping != null && mapping.Any())
                    return;

                var newMapping = device.CreatePortMap(new Mapping(protocol, port, port, 900, "Justin Computer Webserver"));
            }
            catch (Exception ex)
            {
                if (ex.Message != "Unexpected error sending a message to the device")
                {
                    Console.WriteLine(ex);
                }
            }
        }
    }
}
