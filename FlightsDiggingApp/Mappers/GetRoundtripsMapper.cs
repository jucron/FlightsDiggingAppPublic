using FlightsDiggingApp.Models;
using static FlightsDiggingApp.Models.GetAirportsResponseDTO;

namespace FlightsDiggingApp.Mappers
{
    public class GetRoundtripsMapper
    {
        private static readonly int _maxItineraries = 5;
        public static GetRoundtripDTO MapGetRoundtripsResponseToDTO(SearchIncompleteResponse searchIncompleteResponse)
        {
           //todo
            return new GetRoundtripDTO();

        }

        internal static GetRoundtripsResponse MapSearchIncompleteResponseToGetRoundtripsResponse(SearchIncompleteResponse result, GetRoundtripsRequest request)
        {
            var response = new GetRoundtripsResponse();

            response.from = request.from;
            response.to = request.to;
            response.currency = request.currency;

            if (result != null && result.data != null && result.data.itineraries.Count > 0)
            {
                // For each itineraty:
                int count = 0;
                foreach (var itinerary in result.data.itineraries)
                {

                    var flight = new GetRoundtripsResponse.Flight();

                    if (count > _maxItineraries)
                    {
                        break;
                    }
                    // Add count
                    count++;

                    var company = "not defined";

                    // Sum duration of all legs
                    int durationHours = 0;
                    itinerary.legs.ForEach(l => durationHours = +l.durationInMinutes / 60);

                    var price = itinerary.price.raw;
                    if (itinerary.legs.Count > 0 && itinerary.legs[0].carriers != null && itinerary.legs[0].carriers.marketing.Count > 0)
                    {
                        company = itinerary.legs[0].carriers.marketing[0].name;
                    }

                    flight.hours = durationHours;
                    flight.rawPrice = price;
                    flight.company = company;

                    response.flights.Add(flight);
                }
            }
            else
            {
                var errorDescription = $"SearchIncompleteResponse has no proper data.";
                return new GetRoundtripsResponse() { status = { hasError = true, errorDescription = errorDescription } };
            }

            return response;
        }
    }
}
