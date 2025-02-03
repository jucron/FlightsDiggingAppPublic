using FlightsDiggingApp.Models;
using static FlightsDiggingApp.Models.GetAirportsResponseDTO;

namespace FlightsDiggingApp.Mappers
{
    public class GetAirportsMapper
    {
        public static GetAirportsResponseDTO MapGetAirportsResponseToDTO(GetAirportsResponse getAirportsResponse)
        {
            var response = new GetAirportsResponseDTO();
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
