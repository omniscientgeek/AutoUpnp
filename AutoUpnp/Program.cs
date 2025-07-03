// See https://aka.ms/new-console-template for more information
using AutoUpnp;
using System;
using System.Threading;

Console.WriteLine("Hello, World!");

new UpnpAutoUpdate().Run();

while(true)
{
    Thread.Sleep(1000);
}