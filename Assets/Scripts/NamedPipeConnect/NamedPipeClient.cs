using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NamedPipeConnection
{
    public static class NamedPipeClient
    {
        private const string OkAnswer = "OK";
        private const string CreateObjectCommand = "CreateObject";
        private const string ExecuteCommand = "Execute";
        private const string GetPropertyCommand = "GetProperty";
        private const string SetPropertyCommand = "SetProperty";
        private const string ReturnObjectIDCommand = "ReturnObjectID";
        private const string ReturnBoxedObjectCommand = "ReturnBoxedObject";
        private const string ValueIsRegisteredObjectCommand = "ValueIsRegisteredObject";
        private const string ValueIsBoxedObjectCommand = "ValueIsBoxedObject";
        private const string DestroyCommand = "Destroy";
        private const string NullString = "null";
        private static string _serverName = "."; //192.168.0.119
        private static string _pipeName = "TVCPComObjectsProviderPipe";
        private const int TimeOutMs = 30000;
        private static JsonSerializerSettings _serializerSettings = new JsonSerializerSettings
        {
            Converters =
            {
                new BoxedObjectNETJSONConverter()
            }
        };

        public static async Task CreateObjectOnServerAsync(string typeName, Guid objectID)
        {
            await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    streamString.WriteString($"{CreateObjectCommand} {typeName} {objectID}");

                    var response = streamString.ReadString();
                    ThrowIfNotOkResponse(response);
                }
            });
        }

        public static async Task ExecuteObjectMethodOnServerAsync(Guid objectID, string methodName, ParametersDataContainer parametersContainer)
        {
            await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    var serializedParameters = SerializeOrNullString(parametersContainer);
                    streamString.WriteString($"{ExecuteCommand} {objectID} {methodName} {serializedParameters}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);
                }
            });
        }

        public static async Task<T> ExecuteObjectMethodOnServerAsync<T>(Guid objectID, string methodName, ParametersDataContainer parametersContainer)
        {
            return await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    var serializedParameters = SerializeOrNullString(parametersContainer);
                    streamString.WriteString($"{ExecuteCommand} {objectID} {methodName} {serializedParameters}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);

                    return (T)DeserializeObject(response.Substring(parts[0].Length + 1), typeof(T));
                }
            });
        }

        public static async Task<object> ExecuteObjectMethodOnServerAndReturnBoxedObjectAsync(Guid objectID, string methodName, ParametersDataContainer parametersContainer)
        {
            return await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    var serializedParameters = SerializeOrNullString(parametersContainer);
                    streamString.WriteString($"{ExecuteCommand} {objectID} {methodName} {serializedParameters}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);

                    var boxedObjectWrapper = (BoxedObjectWrapper)DeserializeObject(response.Substring(parts[0].Length + 1), typeof(BoxedObjectWrapper));
                    return boxedObjectWrapper.BoxedObject;
                }
            });
        }

        public static async Task<Guid> ExecuteObjectMethodOnServerAndReturnIDAsync(Guid objectID, string methodName, ParametersDataContainer parametersContainer)
        {
            return await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    var serializedParameters = SerializeOrNullString(parametersContainer);
                    streamString.WriteString($"{ExecuteCommand} {objectID} {methodName} {serializedParameters}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);

                    return Guid.Parse(response.Substring(parts[0].Length + 1));
                }
            });
        }

        public static async Task<T> GetPropertyFromObjectOnServerAsync<T>(Guid objectID, string propertyName)
        {
            return await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    streamString.WriteString($"{GetPropertyCommand} {objectID} {propertyName}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);

                    return (T)DeserializeObject(response.Substring(parts[0].Length + 1), typeof(T));
                }
            });
        }

        public static async Task<object> GetBoxedObjectFromPropertyFromObjectOnServerAsync(Guid objectID, string propertyName)
        {
            return await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    streamString.WriteString($"{GetPropertyCommand} {ReturnBoxedObjectCommand} {objectID} {propertyName}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);

                    var boxedObjectWrapper = (BoxedObjectWrapper)DeserializeObject(response.Substring(parts[0].Length + 1), typeof(BoxedObjectWrapper));
                    return boxedObjectWrapper.BoxedObject;
                }
            });
        }

        public static async Task<Guid> GetPropertyFromObjectOnServerAndReturnIDAsync(Guid objectID, string propertyName)
        {
            return await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    streamString.WriteString($"{GetPropertyCommand} {ReturnObjectIDCommand} {objectID} {propertyName}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);

                    return Guid.Parse(response.Substring(parts[0].Length + 1));
                }
            });
        }

        public static async Task SetPropertyToObjectOnServerAsync(Guid objectID, string propertyName, object value)
        {
            await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    var serializedValue = SerializeOrNullString(value);
                    streamString.WriteString($"{SetPropertyCommand} {objectID} {propertyName} {serializedValue}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);
                }
            });
        }

        public static async Task SetBoxedObjectToPropertyToObjectOnServerAsync(Guid objectID, string propertyName, object boxedObject)
        {
            await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    var serializedValue = SerializeOrNullString(new BoxedObjectWrapper(boxedObject));
                    streamString.WriteString($"{SetPropertyCommand} {ValueIsBoxedObjectCommand} {objectID} {propertyName} {serializedValue}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);
                }
            });
        }

        public static async Task SetPropertyToObjectOnServerAsync(Guid objectID, string propertyName, Guid valueID)
        {
            await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    streamString.WriteString($"{SetPropertyCommand} {ValueIsRegisteredObjectCommand} {objectID} {propertyName} {valueID}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);
                }
            });
        }

        public static async Task DestroyObjectOnServerAsync(Guid objectID)
        {
            await Task.Run(() =>
            {
                using (NamedPipeClientStream pipeClient = new NamedPipeClientStream(_serverName, _pipeName, PipeDirection.InOut))
                {
                    TryConnectBeforeTimeOut(pipeClient, TimeOutMs);

                    var streamString = new StreamString(pipeClient);

                    streamString.WriteString($"{DestroyCommand} {objectID}");

                    var response = streamString.ReadString();
                    var parts = response.Split(' ');
                    ThrowIfNotOkResponse(parts[0], response);
                }
            });
        }

        private static void TryConnectBeforeTimeOut(NamedPipeClientStream stream, int msTimeout)
        {
            if (msTimeout < 0)
                throw new ArgumentException("Timeout can't be negative.", nameof(msTimeout));

            var timeOut = DateTime.Now.AddMilliseconds(msTimeout);
            while (timeOut > DateTime.Now)
            {
                try
                {
                    stream.Connect((int)timeOut.Subtract(DateTime.Now).TotalMilliseconds);

                    return;
                }
                catch
                {
                    if (timeOut < DateTime.Now)
                        throw;
                }
            }

            throw new IOException("NamedPipeClientStream connection timeout.");
        }

        private static void ThrowIfNotOkResponse(string stateResponse, string fullResponse)
        {
            if (stateResponse != OkAnswer)
                throw new IOException($"COM-module communication error, response: {fullResponse}");
        }

        private static void ThrowIfNotOkResponse(string response)
            => ThrowIfNotOkResponse(response, response);

        private static string SerializeObject(object obj)
            => JsonConvert.SerializeObject(obj, Formatting.Indented, _serializerSettings);

        private static object DeserializeObject(string serializedObject, Type objectType)
            => JsonConvert.DeserializeObject(serializedObject, objectType, _serializerSettings);

        private static object SerializeOrNullString(object obj)
            => obj == null ? NullString : SerializeObject(obj);
    }
}