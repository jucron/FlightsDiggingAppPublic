using FlightsDiggingApp.Models;
using Microsoft.AspNetCore.Http;
using static FlightsDiggingApp.Models.GetAirportsResponseDTO;
using static FlightsDiggingApp.Models.SearchIncompleteResponse;

namespace FlightsDiggingApp.Mappers
{
    public class GetRoundtripsMapper
    {
        private static readonly int _maxItineraries = 200;
        public static GetRoundtripDTO MapGetRoundtripsResponseToDTO(SearchIncompleteResponse searchIncompleteResponse)
        {
           //todo
            return new GetRoundtripDTO();

        }

        public static GetRoundtripsRequest CreateCopyOfGetRoundtripsRequest(GetRoundtripsRequest request, DateTime departDate, DateTime returnDate)
        {
            return new GetRoundtripsRequest
            {
                from = request.from,
                to = request.to,
                currency = request.currency,
                adults = request.adults,
                children = request.children,
                infants = request.infants,
                cabinclass = request.cabinclass,
                initDepartDateString = request.initDepartDateString,
                endDepartDateString = request.endDepartDateString,
                initReturnDateString = request.initReturnDateString,
                endReturnDateString = request.endReturnDateString,
                departDate = departDate.ToString("yyyy-MM-dd"),
                returnDate = returnDate.ToString("yyyy-MM-dd"),
                sessionId = request.sessionId
            };
        }

        internal static GetRoundtripsResponse MapSearchIncompleteResponseToGetRoundtripsResponse(SearchIncompleteResponse result, GetRoundtripsRequest request)
        {
            var response = new GetRoundtripsResponse();

            // Same data as request
            response.from = request.from;
            response.to = request.to;
            response.departDate = request.departDate;
            response.returnDate = request.returnDate;
            response.currency = request.currency;
            response.adults = request.adults;
            response.children = request.children;
            response.infants = request.infants;
            response.cabinclass = request.cabinclass;
            response.flights = new List<GetRoundtripsResponse.Flight>();

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

                    // Data to be fetched
                    var price = itinerary.price.raw;
                    List<GetRoundtripsResponse.Company> companies = new List<GetRoundtripsResponse.Company>();
                    int durationHours = 0;

                    foreach (var leg in itinerary.legs)
                    {
                        // Sum duration of all legs
                        durationHours =+ (leg.durationInMinutes / 60);

                        // Add companies to this flight
                        foreach (var marketingCarrier in leg.carriers.marketing)
                        {
                            // Add if there is no company with the same name
                            if (!companies.Any(c => c.name == marketingCarrier.name))
                            {
                                companies.Add(new GetRoundtripsResponse.Company
                                {
                                    name = marketingCarrier.name,
                                    logoUrl = marketingCarrier.logoUrl
                                });
                            }
                        }
                    }


                    flight.hours = durationHours;
                    flight.rawPrice = price;
                    flight.companies = companies;

                    
                    response.flights.Add(flight);
                    response.link = GenerateLinkByResponse(response);
                }
            }
            else
            {
                var errorDescription = $"SearchIncompleteResponse has no proper data.";
                return new GetRoundtripsResponse() { status = { hasError = true, errorDescription = errorDescription } };
            }

            return response;
        }

        private static string GenerateLinkByResponse(GetRoundtripsResponse response)
        {
            string baseUrl = "https://www.skyscanner.com/transport/flights";

            // Separate infants (age 0-1) from children (age 2+)
            int infants = response.children.Count(c => c.age <= 1);
            List<int> validChildrenAges = response.children
                .Where(c => c.age >= 2)
                .Select(c => c.age)
                .ToList();

            // Validate infants: Each adult can have at most 1 infant
            if (infants > response.adults)
            {
                throw new ArgumentException("Each adult can only have one infant.");
            }

            string childrenAgesParam = validChildrenAges.Any()
                ? $"&childrenv2={string.Join("|", validChildrenAges)}"
            : "";

            string formattedDateTo = response.to.Replace("-", "");
            string formattedDateFrom = response.from.Replace("-", "");

            string url = $"{baseUrl}/{formattedDateFrom}/{formattedDateTo}/{response.departDate}/{response.returnDate}/" +
                         $"?adultsv2={response.adults}{childrenAgesParam}&infants={infants}&cabinclass={response.cabinclass}&currency={response.currency}";

            return url;
        }
    }
}
