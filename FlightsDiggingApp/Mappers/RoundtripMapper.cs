using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Web;
using FlightsDiggingApp.Models;
using FlightsDiggingApp.Models.Amadeus;
using FlightsDiggingApp.Models.RapidApi;

namespace FlightsDiggingApp.Mappers
{
    public class RoundtripMapper
    {
        public static RoundtripResponseDTO MapGetRoundtripResponseToDTO(IApiServiceResponse response, RoundtripRequest roundtripRequest, Properties.AffiliateProperties affiliateProperties)
        {
            return response switch
            {
                AmadeusSearchFlightsResponse amadeusSearchFlightsResponse => MapAmadeusSearchFlightsResponseResponseToRoundtripDTO(amadeusSearchFlightsResponse, roundtripRequest, affiliateProperties),

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

        private static RoundtripResponseDTO MapAmadeusSearchFlightsResponseResponseToRoundtripDTO(AmadeusSearchFlightsResponse amadeusSearchFlightsResponse, RoundtripRequest roundtripRequest, Properties.AffiliateProperties affiliateProperties)
        {
            if (amadeusSearchFlightsResponse == null || amadeusSearchFlightsResponse.data == null)
            {
                return new RoundtripResponseDTO() { status = amadeusSearchFlightsResponse?.operationStatus ?? OperationStatus.CreateStatusFailure(HttpStatusCode.ExpectationFailed, "Unknown error") };
            }
            var response = new RoundtripResponseDTO()
            {
                request = roundtripRequest,
                status = amadeusSearchFlightsResponse.operationStatus,
                dictionaries = amadeusSearchFlightsResponse.dictionaries,
                data = []
            };
            var link = GenerateLinkByRequest(roundtripRequest, affiliateProperties);
            foreach (var roundtrip in amadeusSearchFlightsResponse.data)
            {
                // If we have two itineraries (departure & return)
                if (roundtrip.itineraries.Count == 2)
                {
                    var roundTripDTO = new RoundTripDTO()
                    {
                        Id = roundtrip.id,
                        price = new() { currency = roundtrip.price.currency, total = double.Parse(roundtrip.price.total, CultureInfo.InvariantCulture) },
                        isOneWay = roundtrip.oneWay,
                        source = roundtrip.source,
                        numberOfBookableSeats = roundtrip.numberOfBookableSeats,
                        link = link,
                        departureFlight = new()
                        {
                            duration = ParseDuration(roundtrip.itineraries[0].duration),
                            stops = roundtrip.itineraries[0].segments.Count - 1,
                            segments = roundtrip.itineraries[0].segments,
                        },
                        returnFlight = new()
                        {
                            duration = ParseDuration(roundtrip.itineraries[1].duration),
                            stops = roundtrip.itineraries[1].segments.Count - 1,
                            segments = roundtrip.itineraries[1].segments,
                        },
                    };
                    roundTripDTO.maxStops = Math.Max(roundTripDTO.departureFlight.stops, roundTripDTO.returnFlight.stops);
                    roundTripDTO.durationStatsMinutes = GenerateDurationStatsMinutes(roundTripDTO);

                    AddFormattedDurationForSegments(roundTripDTO.departureFlight.segments);
                    AddFormattedDurationForSegments(roundTripDTO.returnFlight.segments);

                    response.data.Add(roundTripDTO);
                }
            }
            return response;
        }

        private static void AddFormattedDurationForSegments(List<AmadeusSearchFlightsResponse.Segment> segments)
        {
            foreach (var segment in  segments)
            {
                if (segment.duration != null)
                {
                    segment.durationFormatted = ParseDuration(segment.duration);
                }
            }
        }

        private static MinMax<int> GenerateDurationStatsMinutes(RoundTripDTO roundTripDTO)
        {
            if (roundTripDTO?.departureFlight?.duration == null || roundTripDTO?.returnFlight?.duration == null)
            {
                return new MinMax<int> { min = 0, max = 0 }; 
            }

            int totalMinutesDep = roundTripDTO.departureFlight.duration.hours * 60 + roundTripDTO.departureFlight.duration.minutes;
            int totalMinutesRet = roundTripDTO.returnFlight.duration.hours * 60 + roundTripDTO.returnFlight.duration.minutes;

            int minMinutes = Math.Min(totalMinutesDep, totalMinutesRet);
            int maxMinutes = Math.Max(totalMinutesDep, totalMinutesRet);

            return new MinMax<int>
            {
                min = minMinutes,
                max = maxMinutes
            };
        }

        private static RoundTripDTO.Duration ParseDuration(string durationString)
        {
            if (string.IsNullOrWhiteSpace(durationString))
                // Invalid duration string
                return new RoundTripDTO.Duration { hours = 0, minutes = 0 };

            var match = Regex.Match(durationString, @"PT(?:(\d+)H)?(?:(\d+)M)?");

            if (!match.Success)
                // Invalid duration format
                return new RoundTripDTO.Duration { hours = 0, minutes = 0 };

            return new RoundTripDTO.Duration
            {
                hours = match.Groups[1].Success ? int.Parse(match.Groups[1].Value) : 0,
                minutes = match.Groups[2].Success ? int.Parse(match.Groups[2].Value) : 0
            };
        }

        private static string GenerateLinkByRequest(RoundtripRequest request, Properties.AffiliateProperties affiliateProperties)
        {
            string baseUrl = affiliateProperties.base_url;
            int adults = request.adults;
            int children = request.children;
            int infants = request.infants;
            string travelClassCode = request.travelClass.ToUpper() switch
            {
                "ECONOMY" => "",
                "PREMIUM ECONOMY" => "w",
                "BUSINESS" => "c",
                "FIRST" => "f",
                _ => ""
            };
            string GetDateFormat(string dateStr, string format = "ddMM")
            {
                if (DateTime.TryParse(dateStr, out DateTime parsedDate))
                {
                    return parsedDate.ToString(format);
                }
                return "";
            }

            string searchParams = $"{request.from}{GetDateFormat(request.departDate)}{request.to}{GetDateFormat(request.returnDate)}{travelClassCode}{adults}{children}{infants}]";

            string marker = affiliateProperties.marker;
            string campaignId = affiliateProperties.campaignId;
            string trs = affiliateProperties.trs;
            string p = affiliateProperties.p;
        
            string encodedUrl = HttpUtility.UrlEncode(baseUrl+searchParams);

            return $"https://tp.media/r?marker={marker}&trs={trs}&p={p}&u={encodedUrl}&campaign_id={campaignId}";

        }
    }

}
