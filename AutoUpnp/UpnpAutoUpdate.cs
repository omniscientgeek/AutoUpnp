using Open.Nat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoUpnp
{
    public class UpnpAutoUpdate
    {
        public async void Run()
        {
            var discovery = new NatDiscoverer();

            // 5 senconds or less
            var cts = new CancellationTokenSource(5000);

            while (true)
            {
                try
                {
                    var devices = await discovery.DiscoverDevicesAsync(PortMapper.Upnp, cts);

                    foreach (var d in devices)
                    {
                        await CheckAsync(d);
                    }

                    Thread.Sleep(60000);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
        }

        async Task CheckAsync(NatDevice device)
        {
            while (true)
            {
                await CheckAsync(device, 80, Protocol.Tcp);
                await CheckAsync(device, 443, Protocol.Tcp);
                await CheckAsync(device, 51820, Protocol.Udp);
                await CheckAsync(device, 19132, Protocol.Udp);
                await CheckAsync(device, 19133, Protocol.Udp);


                Thread.Sleep(1000);
            }
        }

        async Task CheckAsync(NatDevice device, int port, Protocol protocol)
        {
            try
            {
                var mapping = await device.GetSpecificMappingAsync(protocol, port);

                if (mapping != null && !IsLocalAddress(mapping.PrivateIP))
                {
                    await device.DeletePortMapAsync(new Mapping(protocol, port, port));
                }

                await device.CreatePortMapAsync(new Mapping(protocol, port, port, 900, "Justin Computer Webserver"));
            }
            catch (Exception ex)
            {
                if (ex.Message != "Unexpected error sending a message to the device")
                {
                    Console.WriteLine(ex);
                }
            }
        }

        bool IsLocalAddress(IPAddress privateIP)
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (privateIP.Equals(ip))
                    return true;
            }

            return false;
        }
    }
}
