# "ConsoleSecrets" class library

The `ConsoleSecrets` project includes a concrete class for holding the mapping of user secrets so they can be referenced by your console application project(s). This follows the .NET Core configuration provider reference as shown [here](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-2.2).

As described in the [Microsoft article](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows), the `secrets.json` file maps the configuration key/value pairs to a POCO called `Secrets` in the `ConsoleSecrets` project.

This project is based off [Grant Hair's solution](https://medium.com/@granthair5/how-to-add-and-use-user-secrets-to-a-net-core-console-app-a0f169a8713f).

## Preparing your 'secrets.json' file

1. Create a new GUID by opening PowerShell and typing `[guid]::NewGuid()`.

2. Open the Visual Studio solution, click on the `ConsoleSecrets` project, locate `<UserSecretsId>CREATE_A_GUID_AND_PUT_IT_HERE</UserSecretsId>` and paste in your new GUID value from the previous step.

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

## Extending/Adding new secrets

To add a new secret simply extend the `Secrets` class in the `ConsoleSecrets` project by adding your new property to the existing set. Then add the same property and your secret to the `secrets.json` file.
