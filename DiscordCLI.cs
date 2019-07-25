using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord.Webhook;
using CommandLine;

namespace discord_cli
{
    class Options
    {
        [Option('i', "webhook_id", Required = true, HelpText = "Discord webhook id.")]
        public String WebhookId {get; set;}

        [Option('t', "webhook_token", Required = true, HelpText = "Discord webhook token.")]
        public String WebhookToken {get; set;}

        [Option('u', "user", Required = false, HelpText = "Optional. The userid the message will appear from.")]
        public String User {get; set;}

        [Option('a', "avatar_url", Required = false, HelpText = "Optional. Url for the avatar.")]
        public String AvatarUrl {get; set;}

        [Option('m', "message", Required = true, HelpText = "Message to send to the webhook.")]
        public String Message {get; set;}

        [Option('f', "file", Required = false, HelpText = "Optional. File attachment to send to the webhook.  This is file name that should optionally include the path.")]
        public String Attachment {get; set;}
    }

    class DiscordCLI
    {
        static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                   .WithParsed<Options>(o =>
                   {
                       ProcessWithOptions(o);
                   });
        }

        public static void ProcessWithOptions(Options o) {
            try
            {
                ulong uwebhook_id = 0;
                ulong.TryParse(o.WebhookId, out uwebhook_id);
                SendToDiscord(o.Message, uwebhook_id, o.WebhookToken, o.User, o.AvatarUrl, o.Attachment);
                // if all is ok, the exit with success
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error sending to Discord: {e.Message}");
                Environment.Exit(-1);
            }
        }

        public static void SendToDiscord(String message, ulong discord_webhook_id, String discord_webhook_token, String user, String avatarUrl, String attachment)
        {
            DiscordWebhookClient client = new DiscordWebhookClient(discord_webhook_id, discord_webhook_token);

            Task<ulong> task;
            // if the attachment is null, call sendmessageasync
            if (attachment == null)
            {
                task = client.SendMessageAsync(text: message, username: user, avatarUrl: avatarUrl);
            }
            // else call sendFileAsync
            else
            {
                task = client.SendFileAsync(filePath: attachment, text: message, username: user, avatarUrl: avatarUrl);
            }
                
            Task.WaitAll(task);
            ulong result = task.Result;
        }
    }
}
