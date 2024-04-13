using System;

namespace TcpIpLibrary.Users
{
    [Serializable]
    public class UserDatas
    {
        public string _name;
        public string _type;
        public string _address;
        public int _port;

        public override string ToString()
        {
            return $"{_name} - {_type} - {_address} - {_port}";
        }
    }
}