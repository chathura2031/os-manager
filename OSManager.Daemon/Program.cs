using OSManager.Communications.Proto;
using OSManager.Daemon;
using OSManager.Plugins.Intercommunication;

// TODO: Use dependency injection
IIntercommServer server = new ProtoServer();
Handler handler = new Handler(server);
await server.StartServer();
