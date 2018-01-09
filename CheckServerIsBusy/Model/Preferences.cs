using System.Collections.Generic;

namespace CheckServerIsBusy.Model
{
    public sealed class Preferences : Settings<Preferences>
    {
        public IEnumerable<UserAssociations> Users = new List<UserAssociations>();

        public Preferences()
        {
        }
    }

    public sealed class UserAssociations
    {
        public string IPAddress { get; set; }
        public string UserName { get; set; }
    }
}
