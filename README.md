# CheckServerUsedByUser-Original
This application provides you server status with specific user and if server is busy provides user IP.

**This application will not develop in the future**

In the future will be develop [this project](https://github.com/Diamed/CheckServerUsedByUser)

For working this application correctly you need to replace:

Server's IP at CheckServer class on line 20:
<code>
ServerIPAddress = "127.0.0.1";
</code>

User name on server at CheckServer class on line 22:
<code>
UserNameOnServer = Environment.UserName;
</code>

Path to settings at class Settings line 12:
<code>
private const string DefaultFileName = @"C:\settings.json";
</code>
