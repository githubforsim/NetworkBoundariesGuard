namespace NTcpIpWrapper
{
    public static class Factory
    {
        public static NDistributeToClients.IConnexionForOwner CreateMultiThreaded(
            NDistributeToClients.IChief chief, 
            SNfoConnexion nfo
            )
        {
            return new NDistributeToClients.MultiThreadWrapper(chief, nfo);
        }

        public static DistributeAllMsgToOneClient.IConnexionForOwner CreateMultiThreaded(
            DistributeAllMsgToOneClient.IClientOfConnexion client, 
            DistributeAllMsgToOneClient.IChief chief, 
            EModeConnexion mode
            )
        {
            return new DistributeAllMsgToOneClient.MultiThreadWrapper(client, chief, mode);
        }
    }
}
