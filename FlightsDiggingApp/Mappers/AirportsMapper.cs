using FlightsDiggingApp.Models;
using static FlightsDiggingApp.Models.AirportsResponseDTO;

namespace FlightsDiggingApp.Mappers
{
    public class AirportsMapper
    {
        public static AirportsResponseDTO MapGetAirportsResponseToDTO(AirportsResponse getAirportsResponse)
        {
            var response = new AirportsResponseDTO();
            response.AirportOptions = new List<AirportOption>();
            if (getAirportsResponse.data == null)
            {
                return response;
            }

            foreach (var airportResponse in getAirportsResponse.data)
            {
                response.AirportOptions.Add(new AirportOption
                {
                    name = airportResponse.presentation.suggestionTitle,
                    id = airportResponse.navigation.relevantFlightParams.skyId
                });
            }
            response.status = getAirportsResponse.operationStatus;
            return response;

        }
    }
}
