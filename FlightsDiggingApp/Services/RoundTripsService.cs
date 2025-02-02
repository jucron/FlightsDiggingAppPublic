using System.Net.WebSockets;
using System.Text;
using FlightsDiggingApp.Models;
using System.Text.Json;
using FlightsDiggingApp.Mappers;

namespace FlightsDiggingApp.Services
{
    public class RoundTripsService
    {
        private readonly ILogger _logger;
        private readonly ApiService _apiService;
        public RoundTripsService(ILogger logger, ApiService apiService)
        {
            _logger = logger;
            _apiService = apiService;
        }

        internal async Task HandleRoundTripsAsync(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result;
            string receivedMessage = string.Empty;

            // Receive the request
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            receivedMessage = Encoding.UTF8.GetString(buffer, 0, result.Count);

            _logger.LogInformation($"Received message before deserializing: {receivedMessage}");

            try
            {
                var request = JsonSerializer.Deserialize<GetRoundtripsRequest>(receivedMessage, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });


                if (request != null)
                {
                    _logger.LogInformation($"Received request for RoundTrips in request: {request}");

                    DateTime startDepartDate = DateTime.Parse(request.initDepartDateString);
                    DateTime endDepartDate = DateTime.Parse(request.endDepartDateString);

                    DateTime startReturnDate = DateTime.Parse(request.initReturnDateString);
                    DateTime endReturnDate = DateTime.Parse(request.endReturnDateString);


                    var tasks = new List<Task>();  // Store tasks for concurrent execution

                    for (DateTime departDate = startDepartDate; departDate <= endDepartDate; departDate = departDate.AddDays(1))
                    {

                        for (DateTime returnDate = startReturnDate; returnDate <= endReturnDate; returnDate = returnDate.AddDays(1))
                        {

                            // Create a copy of the request for each iteration
                            var requestCopy = GetRoundtripsMapper.CreateCopyOfGetRoundtripsRequest(request, departDate, returnDate);

                            // Add the request processing as a task to the list
                            tasks.Add(ProcessRoundtripAsync(requestCopy, webSocket));
                        }
                    }
                    // Wait for all tasks to complete before finishing
                    await Task.WhenAll(tasks);
                }
            }
            catch (JsonException)
            {
                _logger.LogError("Invalid JSON format received.");
                await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("Invalid request format")),
                                          WebSocketMessageType.Text,
                                          true,
                                          CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error processing WebSocket request");
            }
            finally
            {
                // Close the WebSocket connection after all data has been sent
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Finished sending data", CancellationToken.None);
            }
        }

        private async Task ProcessRoundtripAsync(GetRoundtripsRequest requestCopy, WebSocket webSocket)
        {
            try
            {
                GetRoundtripsResponse roundtripsResponse = await _apiService.GetRoundtripAsync(requestCopy);

                // Serialize response
                var responseJson = JsonSerializer.Serialize(roundtripsResponse);
                var responseBytes = Encoding.UTF8.GetBytes(responseJson);

                // Send response back
                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes),
                                          WebSocketMessageType.Text,
                                          true, // Marks the message as completed
                                          CancellationToken.None);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing roundtrip request");

                var errorMessage = Encoding.UTF8.GetBytes("Error processing roundtrip request.");
                await webSocket.SendAsync(new ArraySegment<byte>(errorMessage),
                                          WebSocketMessageType.Text,
                                          true,
                                          CancellationToken.None);
            }
        }
    }

}
