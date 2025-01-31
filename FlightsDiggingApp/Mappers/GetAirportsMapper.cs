using FlightsDiggingApp.Models;
using static FlightsDiggingApp.Models.GetAirportsResponseDTO;

namespace FlightsDiggingApp.Mappers
{
    public class GetAirportsMapper
    {
        public static GetAirportsResponseDTO MapGetAirportsResponseToDTO(GetAirportsResponse GetAirportsResponse)
        {
            var response = new GetAirportsResponseDTO();
            response.AirportOptions = new List<AirportOption>();
            if (GetAirportsResponse.data == null)
            {
                return response;
            }

            foreach (var airportResponse in GetAirportsResponse.data)
            {
                response.AirportOptions.Add(new AirportOption
                {
                    name = airportResponse.presentation.suggestionTitle,
                    id = airportResponse.navigation.relevantFlightParams.skyId
                });
            }
            return response;

        }
    }
}
