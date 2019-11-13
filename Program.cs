using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace test_tls
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var server = FindArgument(args, "server", "s");

                foreach (var protocolType in Enum.GetValues(typeof(SecurityProtocolType)).Cast<SecurityProtocolType>())
                {
                    await RequestAsync(server, protocolType);
                }
            }
            catch (ArgumentException ex)
            {
                await Console.Out.WriteLineAsync();
                await Console.Error.WriteLineAsync(ex.Message);
                await Console.Out.WriteLineAsync();
                await Console.Out.WriteLineAsync($"Usage: {System.AppDomain.CurrentDomain.FriendlyName} [options]");
                await Console.Out.WriteLineAsync("-s, --server\tServer to test.");
            }
        }

        private static async Task RequestAsync(string server, SecurityProtocolType type)
        {
            await Console.Out.WriteAsync($"Using protocol {type}... ");
            try
            {
                ServicePointManager.SecurityProtocol = type;

                var request = (HttpWebRequest)WebRequest.Create(server);

                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    await Console.Out.WriteLineAsync($"StatusCode: {response.StatusCode} {(int)response.StatusCode}");

                    using (var sr = new StreamReader(response.GetResponseStream()))
                    {
                        await sr.ReadToEndAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                await Console.Error.WriteAsync(ex.Message);
                while ((ex = ex.InnerException) != null)
                {
                    await Console.Error.WriteAsync(" Inner: " + ex.Message);
                }
                await Console.Error.WriteLineAsync();
            }
        }

        private static string FindArgument(string[] args, string key, string shortKey, bool optional = false)
        {
            for (int i = 0; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg == $"--{key}" || arg == $"-{shortKey}")
                {
                    return args[i + 1];
                }
            }

            if (optional)
            {
                return null;
            }
            else
            {
                throw new ArgumentException($"-{shortKey} --{key} must be specified.");
            }
        }
    }
}
