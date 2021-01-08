using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Azure.Core;

// Note : install the following packages
// dotnet add package Azure.Security.KeyVault.Secrets
// dotnet add package Azure.Identity

class Program
{
    static void Main(string[] args)
    {
        try
        {
            test();
        }
        catch (System.Exception e)
        {
            Console.WriteLine("caught an exception: {0}", e);
        }
    }
    static void test()
    {
        const string keyVaultName = "key-vault-test02";
        const string AZURE_TENANT_ID = "d99eee11-8587-4c34-9201-xxxxxxxxxxxx";
        const string AZURE_CLIENT_ID = "80f16cbd-e549-4c93-9e7b-xxxxxxxxxxxx";
        const string AZURE_CLIENT_SECRET = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx";

        ClientSecretCredential credential = new ClientSecretCredential(AZURE_TENANT_ID, AZURE_CLIENT_ID, AZURE_CLIENT_SECRET);

        var kvUri = String.Format("https://{0}.vault.azure.net", keyVaultName);
        SecretClientOptions options = new SecretClientOptions()
        {
            Retry =
                {
                    Delay= TimeSpan.FromSeconds(5),
                    MaxDelay = TimeSpan.FromSeconds(15),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                 }
        };

        var client = new SecretClient(new Uri(kvUri), credential, options); //new DefaultAzureCredential() for authenticating VM        

        string secretName = "secret003";
        string secretValue = "123456"; //Console.ReadLine();         

        Console.Write("Creating a secret in " + keyVaultName + " called '" + secretName + "' with the value '" + secretValue + "` ...");

        client.SetSecret(secretName, secretValue);

        Console.WriteLine(" done.");

        Console.WriteLine("Forgetting your secret.");
        secretValue = "";
        Console.WriteLine("Your secret is '" + secretValue + "'.");

        Console.WriteLine("Retrieving your secret from " + keyVaultName + ".");

        KeyVaultSecret secret = client.GetSecret(secretName);

        Console.WriteLine("Your secret is '" + secret.Value + "'.");

        Console.Write("Deleting your secret from " + keyVaultName + " ...");

        client.StartDeleteSecret(secretName);

        System.Threading.Thread.Sleep(5000);
        Console.WriteLine(" done.");

    }
}
