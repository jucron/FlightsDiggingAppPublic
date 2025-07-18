using FlightsDiggingApp.Models;

namespace FlightsDiggingApp.Services.Filters
{
    public class FilterRule
    {
        public FilterType Type { get; set; }
        public Func<Filter, bool> Condition { get; set; }
        public Action<Filter, RoundtripResponseDTO> ApplyFilter { get; set; }
        public Action<Filter, RoundtripResponseDTO> FixFilterRange { get; set; }
        public Action<RoundtripResponseDTO> OrderBySelectedType { get; set; }
        public int Priority { get; set; }

        private static bool IsMinMaxRelevant(MinMax<int> minMax) => minMax.min > 0 || minMax.max > 0;
        public static List<FilterRule> BuildRules()
        {
            return [
                new()
                {
                    Type = FilterType.MaxPrice,
                    Priority = 1,
                    Condition = f => f.maxPrice > 0,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByMaxPrice(f.maxPrice, dto),
                    FixFilterRange = (f, dto) => FilterOperator.FixFilterRangeMaxPrice(f, dto)
                    //todo: implements order by
                },
                new()
                {
                    Type = FilterType.MinPrice,
                    Priority = 5,
                    Condition = f => f.minPrice > 0,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByMinPrice(f.minPrice, dto),
                    FixFilterRange = (f, dto) => FilterOperator.FixFilterRangeMinPrice(f, dto)
                },
                new()
                {
                    Type = FilterType.Duration,
                    Priority = 2,
                    Condition = f => f.maxDurationMinutes > 0,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByMaxDuration(f.maxDurationMinutes, dto),
                    FixFilterRange = (f, dto) => FilterOperator.FixFilterRangeMaxDuration(f, dto)
                },
                new()
                {
                    Type = FilterType.Stops,
                    Priority = 3,
                    Condition = f => f.maxStops > 0,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByMaxStops(f.maxStops, dto),
                    FixFilterRange = (f, dto) => FilterOperator.FixFilterRangeMaxStops(f, dto)
                },
                new()
                {
                    Type = FilterType.DepartureTimeOriginMinutes,
                    Priority = 4,
                    Condition = f => IsMinMaxRelevant(f.departureTimeOriginMinutes),
                    ApplyFilter = (f, dto) => FilterOperator.FilterByDepHourOrigin(f.departureTimeOriginMinutes, dto),
                    FixFilterRange = (f, dto) => FilterOperator.FixFilterRangeDepTimeOrigin(f, dto)
                },
                new()
                {
                    Type = FilterType.DepartureTimeReturnMinutes,
                    Priority = 4,
                    Condition = f => IsMinMaxRelevant(f.departureTimeReturnMinutes),
                    ApplyFilter = (f, dto) => FilterOperator.FilterByDepHourReturn(f.departureTimeReturnMinutes, dto),
                    FixFilterRange = (f, dto) => FilterOperator.FixFilterRangeDepTimeReturn(f, dto)
                }
            ];
        }
    }
}
