using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Reflection;

namespace UrlSigningFunction
{
    class Program
    {
        struct CommandLineOptions
        {
            public static string Scheme = "";
            public static string Domain = "";
            public static string Path = "";
            public static string Key = "";
            public static string Expires = "";
            public static string Url = "";
            public static string Ip = "";
        }

        static string SignUrl()
        {
            //  1. Setup Token Key
            //  1.1 Prepend leading slash if missing
            string path = CommandLineOptions.Path;
            if (!path.StartsWith("/"))
                path = "/" + path;

            // 1.2 Extract uri, ignore query string arguments
            if (CommandLineOptions.Domain.IndexOf("?") > -1)
                CommandLineOptions.Domain = CommandLineOptions.Domain.Substring(CommandLineOptions.Domain.IndexOf('?'));

            // 1.3 Formulate the token key
            string token = CommandLineOptions.Expires + path + CommandLineOptions.Key + CommandLineOptions.Ip;

            // 2. Setup URL
            // 2.1 Append argument - secure (compulsory)
            string urlSecures = "?secure=" + Encode(token).Replace("+", "-").Replace("/", "_").Replace("=", "").TrimEnd();

            // 2.2 Append argument - expires
            string urlExpires = "";
            if (CommandLineOptions.Expires.Length > 0)
                urlExpires = "&expires=" + CommandLineOptions.Expires;

            // 2.3 Append argument - ip
            string urlIp = "";
            if (CommandLineOptions.Ip.Length > 0)
               urlIp = "&ip=" + CommandLineOptions.Ip;

            return CommandLineOptions.Scheme + "://" + CommandLineOptions.Domain + path + urlSecures + urlExpires + urlIp;
        }

        public static string Encode(string token)
        {
            // byte array representation of that string
            byte[] encodedToken = new UTF8Encoding().GetBytes(token);

            // need MD5 to calculate the hash
            byte[] hash = ((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(encodedToken);

            // encode in base64
            return System.Convert.ToBase64String(hash);

        }

        static void Usage()
        {
            Console.WriteLine(System.AppDomain.CurrentDomain.FriendlyName + " -s <scheme> -r <CDN domain> -p <file path> -k <URL Signing Key> [-e <expiration time>] [-i <IP>]");
        }

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return;
            }

            for (int i = 0; i < args.Length; i++)
            {
                string opt = args[i];
                if (!opt.StartsWith("-"))
                    continue;
                string val = args[i + 1].Trim(new char[] {'\''}).Trim();
                
                switch (opt.TrimStart(new char[] { '-' }))
                {
                    case "s":
                        CommandLineOptions.Scheme = val;
                        break;
                    case "r":
                        CommandLineOptions.Domain = val;
                        break;
                    case "p":
                        CommandLineOptions.Path = val;
                        break;
                    case "k":
                        CommandLineOptions.Key = val;
                        break;
                    case "e":
                        CommandLineOptions.Expires = val;
                        break;
                    case "i":
                        CommandLineOptions.Ip = val;
                        break;
                    default:
                        {
                            Usage();
                            return;
                        }
                }
            }

            // check if we have recieved all mandatory arguments
            List<string> mandatoryOptions = new List<string>()
            {
                "Scheme",
                "Domain",
                "Path",
                "Key"
            };

            var lst = typeof(CommandLineOptions).GetFields(BindingFlags.Static | BindingFlags.Public).ToList();

            foreach (string option in mandatoryOptions)
            {
                if (lst.Where(fi => fi.Name == option).Single().GetValue(null).ToString().Length == 0)
                {
                    Console.Error.WriteLine("Please provide value for "+ option);
                    Usage();
                    return;
                }
            }

            // call the signing code and print the result
            Console.WriteLine(SignUrl());
        }

    }
}
