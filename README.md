# EpicAuth-CSHARP-Example : Please star 🌟

EpicAuth C# example SDK for https://EpicAuth.cc license key API auth.

## **Security practices**

* Utilize obfuscation provided by companies such as VMProtect or Themida (utilize their SDKs too for greater protection)
* Preform frequent integrity checks to ensure the memory of the program has not been modified
* Don't write the bytes of a file you've downloaded to disk if you don't want that file to be retrieved by the user. Rather, execute the file in memory and erase it from memory the moment execution finishes

While our API ensures licenses validation, it's crucial to implement robust client-side protection like obfuscation and integrity checks to prevent software tampering, as vulnerabilities often stem from insufficient client security.



## What is EpicAuth?

EpicAuth is a **source-available authentication platform** with optional cloud hosting plans.

Official client SDKs are available for:
C#, C++, Python, Java, JavaScript, VB.NET, PHP, Rust, Go, Lua, Ruby, and Perl.

EpicAuth provides features such as:
- Secure authentication
- Webhook-based API requests
- Discord webhook notifications
- Secure ban enforcement
- Client-side integrity checks

Community & updates:
https://t.me/EpicAuth

---

## License

This project is licensed under the **Elastic License 2.0**.  
See the `LICENSE` file for full terms.

Key limitations include:
- You may not offer this software as a hosted or managed service.
- You may not bypass, remove, or alter license enforcement mechanisms.
- You must retain all copyright and license notices.

---

## Disclaimer

This project is an independent implementation.  
It is **not affiliated with, endorsed by, or connected to Elastic or KeyAuth**.

---

## `EpicAuthApp` instance definition

Visit https://EpicAuth.cc/app/ and select your application, then click on the **C#** tab

It'll provide you with the code which you should replace with in the `Program.cs` file (or `Login.cs` file if using Form example)

```cs
public static api EpicAuthApp = new api(
    name: "example",
    ownerid: "JjadBVawds",
    secret: "dbwadwagesrdojgoiurejh98gju8f7fapoi0a9waw1995adc05236948d1762bc",
    version: "1.0"
);
```

## Initialize application

You must call this function prior to using any other EpicAuth function. Otherwise the other EpicAuth function won't work.

```cs
EpicAuthApp.Initialize();
if (!EpicAuthApp.response.success)
{
    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
    Thread.Sleep(2500);
    Environment.Exit(0);
}
```

## Display application information

```cs
EpicAuthApp.fetchStats();
Console.WriteLine("\n Application Version: " + EpicAuthApp.app_data.version);
Console.WriteLine(" Customer panel link: " + EpicAuthApp.app_data.customerPanelLink);
Console.WriteLine(" Number of users: " + EpicAuthApp.app_data.numUsers);
Console.WriteLine(" Number of online users: " + EpicAuthApp.app_data.numOnlineUsers);
Console.WriteLine(" Number of keys: " + EpicAuthApp.app_data.numKeys);
```

## Check session validation

Use this to see if the user is logged in or not.

```cs
EpicAuthApp.check();
Console.WriteLine($" Current Session Validation Status: {EpicAuthApp.response.message}");
```

## Check blacklist status

Check if HWID or IP Address is blacklisted. You can add this if you want, just to make sure nobody can open your program for less than a second if they're blacklisted. Though, if you don't mind a blacklisted user having the program for a few seconds until they try to login and register, and you care about having the quickest program for your users, you shouldn't use this function then. If a blacklisted user tries to login/register, the EpicAuth server will check if they're blacklisted and deny entry if so. So the check blacklist function is just auxiliary function that's optional.

```cs
if(EpicAuthApp.checkblack()) {
    Environment.Exit(0);  // terminate program if user blacklisted.
}
```

## Login with username/password

```cs
string username;
string password;
Console.WriteLine("\n\n Enter username: ");
username = Console.ReadLine();
Console.WriteLine("\n\n Enter password: ");
password = Console.ReadLine();
EpicAuthApp.login(username, password);
if (!EpicAuthApp.response.success)
{
    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
    Thread.Sleep(2500);
    Environment.Exit(0);
}
```

## Register with username/password/key

```cs
string username, password, key, email;
Console.Write("\n\n Enter username: ");
username = Console.ReadLine();
Console.Write("\n\n Enter password: ");
password = Console.ReadLine();
Console.Write("\n\n Enter license: ");
key = Console.ReadLine();
Console.Write("\n\n Enter email (just press enter if none): ");
email = Console.ReadLine();
EpicAuthApp.register(username, password, key, email);
if (!EpicAuthApp.response.success)
{
    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
    Thread.Sleep(2500);
    Environment.Exit(0);
}
```

## Upgrade user username/key

Used so the user can add extra time to their account by claiming new key.

> [!WARNING]
> No password is needed to upgrade account. So, unlike login, register, and license functions - you should **not** log user in after successful upgrade.

```
string username;
string password;
string key;
Console.WriteLine("\n\n Enter username: ");
username = Console.ReadLine();
Console.WriteLine("\n\n Enter license: ");
key = Console.ReadLine();
EpicAuthApp.upgrade(username, key);
// don't proceed to app, user hasn't authenticated yet.
Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
Thread.Sleep(2500);
Environment.Exit(0);
```

## Login with just license key

Users can use this function if their license key has never been used before, and if it has been used before. So if you plan to just allow users to use keys, you can remove the login and register functions from your code.

```cs
string key;
Console.WriteLine("\n\n Enter license: ");
key = Console.ReadLine();
EpicAuthApp.license(key);
if (!EpicAuthApp.response.success)
{
    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
    Thread.Sleep(2500);
    Environment.Exit(0);
}
```

##  Login with web loader

Have your users login through website.

```cs
EpicAuthApp.web_login();

Console.WriteLine("\n Waiting for button to be clicked");
EpicAuthApp.button("close");
```

## Forgot password

Allow users to enter their account information and recieve an email to reset their password.

```cs
string username, email;
Console.Write("\n\n Enter username: ");
username = Console.ReadLine();
Console.Write("\n\n Enter email: ");
email = Console.ReadLine();
EpicAuthApp.forgot(username, email);
// don't proceed to app, user hasn't authenticated yet.
Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
Thread.Sleep(2500);
Environment.Exit(0);
```

## User Data

Show information for current logged-in user.

```cs
Console.WriteLine("\n User data:");
Console.WriteLine(" Username: " + EpicAuthApp.user_data.username);
Console.WriteLine(" IP address: " + EpicAuthApp.user_data.ip);
Console.WriteLine(" Hardware-Id: " + EpicAuthApp.user_data.hwid);
Console.WriteLine(" Created at: " + UnixTimeToDateTime(long.Parse(EpicAuthApp.user_data.createdate)));
if (!String.IsNullOrEmpty(EpicAuthApp.user_data.lastlogin)) // don't show last login on register since there is no last login at that point
    Console.WriteLine(" Last login at: " + UnixTimeToDateTime(long.Parse(EpicAuthApp.user_data.lastlogin)));
Console.WriteLine(" Your subscription(s):");
for (var i = 0; i < EpicAuthApp.user_data.subscriptions.Count; i++)
{
    Console.WriteLine(" Subscription name: " + EpicAuthApp.user_data.subscriptions[i].subscription + " - Expires at: " + UnixTimeToDateTime(long.Parse(EpicAuthApp.user_data.subscriptions[i].expiry)) + " - Time left in seconds: " + EpicAuthApp.user_data.subscriptions[i].timeleft);
}
```

## Check subscription name of user

If you want to wall off parts of your app to only certain users, you can have multiple subscriptions with different names. Then, when you create licenses that correspond to the level of that subscription, users who use those licenses will get a subscription with the name of the subscription that corresponds to the level of the license key they used. The `SubExist` function is in the `Program.cs` file

```cs
if (SubExist("default"))
{
    Console.WriteLine(" Default Subscription Exists");
}
// See if another sub exists with name 
if (SubExist("premium"))
{
    Console.WriteLine(" Premium Subscription Exists");
}
```

## Show list of online users

```cs
var onlineUsers = EpicAuthApp.fetchOnline();
if (onlineUsers != null)
{
    Console.Write("\n Online users: ");
    foreach (var user in onlineUsers)
    {
        Console.Write(user.credential + ", ");
    }
    Console.WriteLine("\n");
}
```

## Application variables

A string that is kept on the server-side of EpicAuth. On the dashboard you can choose for each variable to be authenticated (only logged in users can access), or not authenticated (any user can access before login). These are global and static for all users, unlike User Variables which will be dicussed below this section.

```cs
string appvar = EpicAuthApp.var("variableNameHere");
if (!EpicAuthApp.response.success)
{
    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
    Thread.Sleep(2500);
    Environment.Exit(0);
}
else
    Console.WriteLine("\n App variable data: " + appvar);
```

## User Variables

User variables are strings kept on the server-side of EpicAuth. They are specific to users. They can be set on Dashboard in the Users tab, via SellerAPI, or via your loader using the code below. `discord` is the user variable name you fetch the user variable by. `test#0001` is the variable data you get when fetching the user variable.

```cs
EpicAuthApp.setvar("discord", "test#0001");
if (!EpicAuthApp.response.success)
{
    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
    Thread.Sleep(2500);
    Environment.Exit(0);
}
else
    Console.WriteLine("\n Successfully set user variable");
```

And here's how you fetch the user variable:

```cs
string uservar = EpicAuthApp.getvar("discord");
if (!EpicAuthApp.response.success)
{
    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
    Thread.Sleep(2500);
    Environment.Exit(0);
}
else
    Console.WriteLine("\n User variable value: " + uservar);
```

## Application Logs

Can be used to log data. Good for anti-debug alerts and maybe error debugging. If you set Discord webhook in the app settings of the Dashboard, it will send log messages to your Discord webhook rather than store them on site. It's recommended that you set Discord webhook, as logs on site are deleted 1 month after being sent.

You can use the log function before login & after login.

```cs
EpicAuthApp.log("hello I wanted to log this");
```

## Ban the user

Ban the user and blacklist their HWID and IP Address. Good function to call upon if you use anti-debug and have detected an intrusion attempt.

Function only works after login.

```cs
EpicAuthApp.ban();
```

## Ban the user (with reason)

Ban the user and blacklist their HWID and IP Address. Good function to call upon if you use anti-debug and have detected an intrusion attempt.

Function only works after login.

The reason paramater will be the ban reason displayed to the user if they try to login, and visible on the EpicAuth dashboard.

```cs
EpicAuthApp.ban("You've been banned for a reason.");
```

## Server-sided webhooks


> [!NOTE]
> Read documentation for EpicAuth webhooks here https://EpicAuth.readme.io/reference/webhooks-1

Send HTTP requests to URLs securely without leaking the URL in your application. You should definitely use if you want to send requests to SellerAPI from your application, otherwise if you don't use you'll be leaking your seller key to everyone. And then someone can mess up your application.

1st example is how to send request with no POST data. just a GET request to the URL. `7kR0UedlVI` is the webhook ID, `https://EpicAuth.win/api/seller/?sellerkey=sellerkeyhere&type=black` is what you should put as the webhook endpoint on the dashboard. This is the part you don't want users to see. And then you have `&ip=1.1.1.1&hwid=abc` in your program code which will be added to the webhook endpoint on the EpicAuth server and then the request will be sent.

2nd example includes post data. it is form data. it is an example request to the EpicAuth API. `7kR0UedlVI` is the webhook ID, `https://EpicAuth.win/api/1.2/` is the webhook endpoint.

3rd examples included post data though it's JSON. It's an example reques to Discord webhook `7kR0UedlVI` is the webhook ID, `https://discord.com/api/webhooks/...` is the webhook endpoint.

```cs
// example to send normal request with no POST data
string resp = EpicAuthApp.webhook("7kR0UedlVI", "&ip=1.1.1.1&hwid=abc");

// example to send form data
resp = EpicAuthApp.webhook("7kR0UedlVI", "", "type=init&name=test&ownerid=j9Gj0FTemM", "application/x-www-form-urlencoded");

// example to send JSON
resp = EpicAuthApp.webhook("7kR0UedlVI", "", "{\"content\": \"webhook message here\",\"embeds\": null}", "application/json"); // if Discord webhook message successful, response will be empty

if (!EpicAuthApp.response.success)
{
    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
    Thread.Sleep(2500);
    Environment.Exit(0);
}
else
    Console.WriteLine("\n Response received from webhook request: " + resp);
```

## Download file

> [!NOTE]
> Read documentation for EpicAuth files here https://docs.EpicAuth.cc/website/dashboard/files

Keep files secure by providing EpicAuth your file download link on the EpicAuth dashboard. Make sure this is a direct download link (as soon as you go to the link, it starts downloading without you clicking anything). The EpicAuth download function provides the bytes, and then you get to decide what to do with those. This example shows how to write it to a file named `text.txt` in the same folder as the program, though you could execute with RunPE or whatever you want.

`385624` is the file ID you get from the dashboard after adding file.

```cs
byte[] result = EpicAuthApp.download("621644");
if (!EpicAuthApp.response.success)
{
    Console.WriteLine("\n Status: " + EpicAuthApp.response.message);
    Thread.Sleep(2500);
    Environment.Exit(0);
}
else
    File.WriteAllBytes(Directory.GetCurrentDirectory() + "\\test.txt", result);
```

## Chat channels

Allow users to communicate amongst themselves in your program.

Example from the form example on how to fetch the chat messages.

```cs
var messages = Login.EpicAuthApp.chatget("chatChannelNameHere");
if (messages == null)
{
    dataGridView1.Rows.Insert(0, "EpicAuth", "No Chat Messages", UnixTimeToDateTime(DateTimeOffset.Now.ToUnixTimeSeconds()));
}
else
{
    foreach (var message in messages)
    {
        dataGridView1.Rows.Insert(0, message.author, message.message, UnixTimeToDateTime(long.Parse(message.timestamp)));
    }
}
```

Example from the form example on how to send chat message.

```cs
if (Login.EpicAuthApp.chatsend(chatmsg.Text, chatchannel))
{
    dataGridView1.Rows.Insert(0, Login.EpicAuthApp.user_data.username, chatmsg.Text, UnixTimeToDateTime(DateTimeOffset.Now.ToUnixTimeSeconds()));
}
else
    chatmsg.Text = "Status: " + Login.EpicAuthApp.response.message;
```
