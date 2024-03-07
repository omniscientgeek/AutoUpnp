// See https://aka.ms/new-console-template for more information
using AutoUpnp;
using Mono.Nat;

Console.WriteLine("Hello, World!");

new UpnpAutoUpdate().Run();

while(true)
{
    Thread.Sleep(1000);
}