## "CommonConfiguration" class library

The `CommonConfiguration` project includes a concrete class for holding the mapping of user secrets so they can be referenced by your console application project(s). This follows the .NET Core configuration provider reference as shown [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2).

As described in the [Microsoft article](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows), the `secrets.json` file maps the configuration key/value pairs to a POCO called `CommonSecrets` in the `CommonConfiguration` project. This file must be created locally on your PC and populated with your configuration/secrets

### Preparing your `secrets.json` file

1. Create a new GUID by opening PowerShell and typing `[guid]::NewGuid()`.

2. Open the Visual Studio solution, click on the `CommonConfiguration` project, locate `<UserSecretsId>XXXXXXXX-XXXXX-XXXXXXXX-XXXXXXXX</UserSecretsId>` and paste in your new GUID value from the previous step.

3. Navigate to the hidden folder on your PC at `C:\Users\<yourId>\AppData\Roaming\Microsoft\UserSecrets` and create a folder with the name of the GUID.

4. Within this newly created directory, create a file called `secrets.json`.

5. Populate the file with the following example text:

```json
{
    "CommonSecrets": {
        "SecretName1": "##INSERT##",
        "SecretName2": "##INSERT##"
    }
}
```

>NOTE: Since the `secrets.json` file is located outside the code repository, it will not be checked and as such, no risk to exposing secrets will exist.

## Using secrets

To use a secret from the existing `CommonSecrets` class, simply add a field for the object to your `Program.cs` file as follows:

```c#
private static CommonSecrets secrets;
```

Then call `GetConfiguration<T>` to populate the field as follows:

```c#
secrets = BootstrapConfig.GetConfiguration<CommonSecrets>(nameof(CommonSecrets));
```

Lastly, you can reference the properties by name to ensure strong typing:

```c#
client = new DocumentClient(new Uri(secrets.CosmosEndpointUri), secrets.CosmosPrimaryKey);
```

## Extending/Adding new secrets

To add a new secret simply extend the `CommonSecrets` class in the `CommonConfiguration` project by adding your new property to the existing set. Then add the same property and your secret to the `secrets.json` file and remember to save both the class and json file.
