using System;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.Design;
using Projekat3;

namespace SistemskoPoslednjiProjekat
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var server = new Server();
            await server.StartAsync();
        }
    }

}
