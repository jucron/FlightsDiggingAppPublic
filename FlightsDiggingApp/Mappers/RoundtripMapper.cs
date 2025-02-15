using FlightsDiggingApp.Models.Amadeus;
using FlightsDiggingApp.Models.RapidApi;
using FlightsDiggingApp.Models;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Globalization;

namespace FlightsDiggingApp.Mappers
{
    public class RoundtripMapper
    {
        public static RoundtripResponseDTO MapGetRoundtripResponseToDTO(IApiServiceResponse response, RoundtripRequest roundtripRequest)
        {
            return response switch
            {
                AmadeusSearchFlightsResponse amadeusSearchFlightsResponse => MapAmadeusSearchFlightsResponseResponseToRoundtripDTO(amadeusSearchFlightsResponse, roundtripRequest),

                RapidApiRoundtripsResponse rapidApiAirportsResponse => new RoundtripResponseDTO(),

                _ => throw new ArgumentException("MapGetRoundtripResponseToDTO -> Unknown response type")
            };
        }

        public static RoundtripResponseDTO CreateCopyOfRoundtripResponseDTO(RoundtripResponseDTO responseDTO)
        {
            // Deep copy using JSON serialization
            var responseCopy = JsonSerializer.Deserialize<RoundtripResponseDTO>(JsonSerializer.Serialize(responseDTO));

            return responseCopy ?? new RoundtripResponseDTO
            {
                status = OperationStatus.CreateStatusFailure(responseDTO.status.status.httpStatus, "Deep copy resulted in null in RoundtripMapper.CreateCopyOfRoundtripResponseDTO()")
            };
        }

        private static RoundtripResponseDTO MapAmadeusSearchFlightsResponseResponseToRoundtripDTO(AmadeusSearchFlightsResponse amadeusSearchFlightsResponse, RoundtripRequest roundtripRequest)
        {
            if (amadeusSearchFlightsResponse == null || amadeusSearchFlightsResponse.data == null)
            {
                return new RoundtripResponseDTO() { status = amadeusSearchFlightsResponse?.operationStatus ?? OperationStatus.CreateStatusFailure(HttpStatusCode.ExpectationFailed, "Unknown error") };
            }
            var response =  new RoundtripResponseDTO()
            {
                request = roundtripRequest,
                status = amadeusSearchFlightsResponse.operationStatus,
                dictionaries = amadeusSearchFlightsResponse.dictionaries,
                data = []
            };
            foreach (var flight in amadeusSearchFlightsResponse.data)
            {
                var flightDTO = new FlightDTO()
                {
                    Id = flight.id,
                    price = new() { currency = flight.price.currency, total =  double.Parse(flight.price.total, CultureInfo.InvariantCulture) },
                    duration = ParseDuration(flight.itineraries[1].duration),
                    stops = flight.itineraries[1].segments.Count - 1,
                    isOneWay = flight.oneWay,
                    numberOfBookableSeats = flight.numberOfBookableSeats,
                    segments = flight.itineraries[1].segments,
                    link = "todo",
                };
                response.data.Add(flightDTO);
            }
            return response;
        }
        private static FlightDTO.Duration ParseDuration(string durationString)
        {
            if (string.IsNullOrWhiteSpace(durationString))
                // Invalid duration string
                return new FlightDTO.Duration { hours = 0, Minutes = 0 };

            var match = Regex.Match(durationString, @"PT(?:(\d+)H)?(?:(\d+)M)?");

            if (!match.Success)
                // Invalid duration format
                return new FlightDTO.Duration { hours = 0, Minutes = 0 };
            
            return new FlightDTO.Duration
            {
                hours = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 0,
                Minutes = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0
            };
        }
    }
}
