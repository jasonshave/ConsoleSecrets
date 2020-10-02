# "ConsoleSecrets" class library for C#

The `ConsoleSecrets` project includes a concrete class for holding the mapping of user secrets so they can be referenced by your console application project(s). This follows the .NET Core configuration provider reference as shown [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2).

As described in the [Microsoft article](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows), the `secrets.json` file maps the configuration key/value pairs to a POCO called `Secrets` in the `ConsoleSecrets` project.

This project is based off [Grant Hair's solution](https://medium.com/@granthair5/how-to-add-and-use-user-secrets-to-a-net-core-console-app-a0f169a8713f).

## Preparing your 'secrets.json' file

1. Create a new GUID by opening PowerShell and typing `[guid]::NewGuid()`.

2. Open the Visual Studio solution, click on the `ConsoleSecrets` project, locate `<UserSecretsId>CREATE_A_GUID_AND_PUT_IT_HERE</UserSecretsId>` and paste in your new GUID value from the previous step. Alternatively you can simply edit the `\ConsoleSecrets\ConsoleSecrets.csproj` file directly.

3. Navigate to the hidden folder on your PC at `C:\Users\<yourname>\AppData\Roaming\Microsoft\UserSecrets` and create a folder with the name of the GUID.

4. Within this newly created directory, create a file called `secrets.json`.

5. Populate the file with the following example text:

```json
{
    "Secrets": {
        "SecretName1": "INSERT_HERE",
        "SecretName2": "INSERT_HERE"
    }
}
```

>NOTE: Since the `secrets.json` file is located outside the code repository, it will not be checked and as such, no risk to exposing secrets will exist.

## Using secrets

To use a secret from the existing `Secrets` class, simply add a reference to the class library and add a field for the object to your `Program.cs` file as follows:

```c#
private static Secrets secrets = BootstrapSecrets.GetSecrets<Secrets>(nameof(Secrets));
```

Then call the static generic method `GetSecrets<T>` outside of your `Main` method to populate the field as follows:

```c#
class Program
{
    private static Secrets secrets = BootstrapSecrets.GetSecrets<Secrets>(nameof(Secrets));

    static void Main(string[] args)
    {
        Console.WriteLine(secrets.SecretName1);
        SomeCustomMethod();
    }

    private async Task SomeCustomMethod()
    {
        Console.WriteLine(secrets.SecretName2);
    }
}
```

## Using secrets in Azure App Services, Functions, or Key Vault

What's great about this approach is that you **don't need to change anything** when it comes to consuming these secrets in Azure. Additionally, since `GetSecrets<T>` is a generic method, you can separate your secrets into different class definitions.

First off, let's cover the use of secrets in an Azure Function App. Navigate to the **Application configuration** section of your Function App and add a new setting. The name of the setting will reflect the name of your class (i.e. Secrets). In the example below, the class name was `CommonSecrets` and might resemble a structure such as:

```c#
class CommonSecrets
{
    public string CognitiveServicesEndpoint { get; set; }
    public string CognitiveServicesSubscriptionKey { get; set; }
}
```

Conversely, your `secrets.json` file would look like:

```json
{
    "CommonSecrets": {
        "CognitiveServicesEndpoint": "https://someurl.example.com/api/",
        "CognitiveServicesSubscriptionKey": "your-subscription-key"
    }
}
```

The following image shows what the configuration for this would look like:

![FunctionAppConfiguration](/Images/Function_App_Configuration.png)

You simply separate the name of your class and property with a '**:**' symbol:

`CommonSecrets:CognitiveServicesEndpoint`

If you have many secrets and you want them split across different classes (i.e. keep the Cosmos DB secrets separate from Cognitive Services for example), simply create a separate class to hold them, then call the generic method shown below:

```c#
class CognitiveSecrets
{
    public string CognitiveServicesEndpoint { get; set; }
    public string CognitiveServicesSubscriptionKey { get; set; }
}

class CosmosSecrets
{
    public string CosmosDbName { get; set; }
    public string CosmosCollectionName { get; set; }
}
```

Also making sure to separate them for local development in your `secrets.json` file:

```json
{
    "CognitiveSecrets": {
        "CognitiveServicesEndpoint": "https://someurl.example.com/api/",
        "CognitiveServicesSubscriptionKey": "your-subscription-key"
    },
    "CosmosSecrets": {
        "CosmosDbName": "db01",
        "CosmosCollectionName": "collection01"
    }
}
```

Then consume them in your respective services as follows:

```c#
public class CognitiveServicesProcessor
{
    private static CognitiveSecrets cognitiveSecrets = BootstrapSecrets.GetSecrets<CognitiveSecrets>(nameof(CognitiveSecrets));

    private static CosmosSecrets cosmosSecrets = BootstrapSecrets.GetSecrets<CosmosSecrets>(nameof(CosmosSecrets));
}

public static void CognitiveServicesProcessor()
{
    var someObject1 = SomeMethod1(cognitiveSecrets.CognitiveServicesEndpoint);
}

public static void CosmosServicesProcessor()
{
    var someObject2 = SomeMethod2(cosmosSecrets.CosmosDbName, cosmosSecrets.CosmosCollectionName);
}
```

## Extending/Adding new secrets

To add a new secret simply extend the `Secrets` class in the `ConsoleSecrets` project by adding your new property to the existing set. Then add the same property and your secret to the `secrets.json` file.
