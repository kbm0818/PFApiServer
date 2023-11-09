using Amazon;
using Amazon.Runtime.CredentialManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Options;

namespace SDKStore
{
    public static class ProfileGenerator
    {
        public static void Create(string[] args)
        {
            var option = Parse(args);
            if (null == option)
            {
                Console.WriteLine($"Args parsing error...");
                return;
            }

            if (option.ProfileName is null || option.ProfileName.Length == 0)
            {
                Console.WriteLine($"ProfileName is empty...");
                return;
            }

            if (option.AccessKey is null || option.AccessKey.Length == 0)
            {
                Console.WriteLine($"AccessKey is empty...");
                return;
            }

            if (option.SecretKey is null || option.SecretKey.Length == 0)
            {
                Console.WriteLine($"SecretKey is empty...");
                return;
            }

            if(false == WriteProfile(option.ProfileName, option.AccessKey, option.SecretKey))
            {
                Console.WriteLine($"Create profile error...");
                return;
            }

            if(option.Region is not null)
            {
                if(false == AddRegion(option.ProfileName, option.Region))
                {
                    Console.WriteLine($"Add region fail");
                    return;
                }
            }

            Console.WriteLine($"Finish Create profile...");
        }


        private class GenerateOptions
        {
            public string? ProfileName { get; set; }
            public string? AccessKey { get; set; }
            public string? SecretKey { get; set; }
            public RegionEndpoint? Region { get; set; }
        }

        private static GenerateOptions Parse(string[] args)
        {
            Console.WriteLine($"Parsing args...");

            var p = new GenerateOptions();
            var options = new Mono.Options.OptionSet();

            options.Add("p|profilename=", "ProfileName", s => p.ProfileName = s);
            options.Add("a|accesskey=", "AccessKey", s => p.AccessKey = s);
            options.Add("s|secretkey=", "SecretKey", s => p.SecretKey = s);
            options.Add("r|region=", "Region (option)", s => p.Region = RegionEndpoint.GetBySystemName(s));

            options.Parse(args);
            return p;
        }

        private static bool WriteProfile(string profileName, string keyId, string secret)
        {
            Console.WriteLine($"Create the [{profileName}] profile...");

            try
            {
                var options = new CredentialProfileOptions
                {
                    AccessKey = keyId,
                    SecretKey = secret,
                };
                var profile = new CredentialProfile(profileName, options);
                var netSdkStore = new NetSDKCredentialsFile();
                netSdkStore.RegisterProfile(profile);
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message, e.InnerException);
                return false;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message, e.InnerException);
                return false;
            }

            return true;
        }

        private static bool AddRegion(string profileName, RegionEndpoint region)
        {
            Console.WriteLine($"Add [{region.SystemName}] region the [{profileName}] profile...");

            try
            {
                var netSdkStore = new NetSDKCredentialsFile();
                CredentialProfile profile;
                if (netSdkStore.TryGetProfile(profileName, out profile))
                {
                    profile.Region = region;
                    netSdkStore.RegisterProfile(profile);
                }
                else
                {
                    Console.WriteLine($"[{profileName}] profile not exist");
                    return false;
                }
            }
            catch(ArgumentException e)
            {
                Console.WriteLine(e.Message, e.InnerException);
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message, e.InnerException);
                return false;
            }

            return true;
        }
    }
}
