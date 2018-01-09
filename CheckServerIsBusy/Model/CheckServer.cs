using Cassia;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CheckServerIsBusy.Model
{
    public static class CheckServer
    {
        public static string ClientName { get; set; }
        public static string ClientIPAddress { get; set; }
        public static TimeSpan IdleTime { get; set; }
        private static int Counter { get; set; }
        private static string ServerIPAddress { get; set; }
        private static string UserNameOnServer { get; set; }

        static CheckServer()
        {
            // TODO: Replace to server's IP
            ServerIPAddress = "127.0.0.1";
            // TODO: Replace to server's user name
            UserNameOnServer = Environment.UserName;
        }

        public static async Task<bool> ServerIsBusyAsync()
        {
            return await ServerIsBusyAsyncPrivate();
        }

        private static Task<bool> ServerIsBusyAsyncPrivate()
        {
            return Task.Run(() => ServerIsBusy());
        }

        public static bool ServerIsBusy()
        {
            ITerminalServicesManager manager = new TerminalServicesManager();
            using (ITerminalServer server = manager.GetRemoteServer(ServerIPAddress))
            {
                server.Open();
                var session = server.GetSessions().Where(x => x.UserName == UserNameOnServer).FirstOrDefault();
                if (session != null)
                {
                    ClientName = session.ClientName;
                    ClientIPAddress = session.ClientIPAddress?.ToString();
                    IdleTime = session.IdleTime;

                    ClearMemory();

                    return session.ConnectionState == ConnectionState.Active;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool SendMessage(string message)
        {
            ITerminalServicesManager manager = new TerminalServicesManager();
            using (ITerminalServer server = manager.GetRemoteServer(ServerIPAddress))
            {
                server.Open();
                var session = server.GetSessions().Where(x => x.UserName == UserNameOnServer).FirstOrDefault();
                if (session != null)
                {
                    var response = session.MessageBox(message, "Message for user", RemoteMessageBoxButtons.YesNo, 
                        RemoteMessageBoxIcon.Question, RemoteMessageBoxDefaultButton.Button2, RemoteMessageBoxOptions.None, TimeSpan.Zero, true);
                    return response == RemoteMessageBoxResult.Yes;
                }
            }
            return false;
        }

        private static void ClearMemory()
        {
            Counter++;
            if (Counter == 10)
            {
                GC.Collect();
            }
            Counter = 0;
        }
    }
}
