using FlightsDiggingApp.Models;
using FlightsDiggingApp.Models.Amadeus;
using FlightsDiggingApp.Models.RapidApi;
using static FlightsDiggingApp.Models.AirportsResponseDTO;

namespace FlightsDiggingApp.Mappers
{
    public class AirportsMapper
    {
        public static AirportsResponseDTO MapAirportsResponseToDTO(IApiServiceResponse airportResponse)
        {
            return airportResponse switch
            {
                AmadeusAirportResponse amadeusAirportResponse => MapAmadeusAirportResponseToDTO(amadeusAirportResponse),

                RapidApiAirportsResponse rapidApiAirportsResponse => MapRapidApiAirportResponseToDTO(rapidApiAirportsResponse),

                _ => throw new ArgumentException("Unknown response type")
            };

        }

        private static AirportsResponseDTO MapRapidApiAirportResponseToDTO(RapidApiAirportsResponse rapidApiAirportsResponse)
        {
            var response = new AirportsResponseDTO();
            response.AirportOptions = new List<AirportOption>();
            if (rapidApiAirportsResponse.data == null)
            {
                return response;
            }

            foreach (var airportResponse in rapidApiAirportsResponse.data)
            {
                response.AirportOptions.Add(new AirportOption
                {
                    city = airportResponse.presentation.suggestionTitle,
                    iataCode = airportResponse.navigation.relevantFlightParams.skyId
                });
            }
            response.status = rapidApiAirportsResponse.operationStatus;
            return response;
        }

        private static AirportsResponseDTO MapAmadeusAirportResponseToDTO(AmadeusAirportResponse amadeusAirportResponse)
        {
            var response = new AirportsResponseDTO { AirportOptions = [] };
            response.status = amadeusAirportResponse.operationStatus;

            if (amadeusAirportResponse.data != null)
            {

                response.AirportOptions = amadeusAirportResponse.data
                    .GroupBy(airport => airport.iataCode)
                    .Select(group => new AirportOption
                    {
                        iataCode = group.Key,
                        city = group.First().address.cityName,
                        country = group.First().address.countryName,
                        airport = string.Join(", ", group.Where(a => a.subType == "AIRPORT").Select(a => a.name))
                    })
                    .ToList();
            }
            return response;
        }
    }
}
