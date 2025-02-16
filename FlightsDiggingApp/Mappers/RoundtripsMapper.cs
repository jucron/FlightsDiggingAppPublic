using System.Net;
using System.Text.Json;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Models.Amadeus;
using FlightsDiggingApp.Models.RapidApi;
using Microsoft.AspNetCore.Http;
using static FlightsDiggingApp.Models.AirportsResponseDTO;
using static FlightsDiggingApp.Models.RapidApi.SearchIncompleteResponse;

namespace FlightsDiggingApp.Mappers
{
    public class RoundtripsMapper
    {
        private static readonly int _maxItineraries = 200;
        public static RoundtripsResponseDTO MapGetRoundtripsResponseToDTO(RapidApiRoundtripsResponse getRoundtripsResponse)
        {
            return new RoundtripsResponseDTO() { data = getRoundtripsResponse, status = getRoundtripsResponse.status };
        }
        public static RoundtripsRequest CreateCopyOfGetRoundtripsRequest(RoundtripsRequest request, DateTime departDate, DateTime returnDate)
        {
            return new RoundtripsRequest
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
                sessionId = request.sessionId,
                filter = request.filter
                
            };
        }

        public static RoundtripsResponseDTO CreateCopyOfGetRoundtripsResponseDTO(RoundtripsResponseDTO responseDTO)
        {
            // Deep copy using JSON serialization
            var responseCopy = JsonSerializer.Deserialize<RoundtripsResponseDTO>(JsonSerializer.Serialize(responseDTO));

            return responseCopy ?? new RoundtripsResponseDTO
            {
                status = OperationStatus.CreateStatusFailure(responseDTO.status.status.httpStatus,"Deep copy resulted in null in RoundtripsMapper.CreateCopyOfGetRoundtripsResponseDTO()")
            };
        }

        private static string GenerateScannerLinkByResponse(RapidApiRoundtripsResponse response)
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
