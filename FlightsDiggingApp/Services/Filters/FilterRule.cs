using FlightsDiggingApp.Models;
using FlightsDiggingApp.Services.Filters.Helpers;

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

        public static List<FilterRule> BuildRules()
        {
            return [
                new()
                {
                    Type = FilterType.MaxPrice,
                    Priority = 1,
                    Condition = f => f.maxPrice > 0,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByMaxPrice(f.maxPrice, dto),
                    FixFilterRange = (f, dto) => FilterFixer.FixFilterRangeMaxPrice(f, dto),
                    OrderBySelectedType = (dto) => FilterOrdenator.OrderByMaxPrice(dto)
                },
                new()
                {
                    Type = FilterType.Duration,
                    Priority = 2,
                    Condition = f => f.maxDurationMinutes > 0,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByMaxDuration(f.maxDurationMinutes, dto),
                    FixFilterRange = (f, dto) => FilterFixer.FixFilterRangeMaxDuration(f, dto),
                    OrderBySelectedType = (dto) => FilterOrdenator.OrderByMaxDuration(dto)
                },
                new()
                {
                    Type = FilterType.Stops,
                    Priority = 3,
                    Condition = f => true,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByMaxStops(f.maxStops, dto),
                    FixFilterRange = (f, dto) => FilterFixer.FixFilterRangeMaxStops(f, dto),
                    OrderBySelectedType = (dto) => FilterOrdenator.OrderByMaxStops(dto)
                },
                new()
                {
                    Type = FilterType.DepartureTimeOriginMinutesMin,
                    Priority = 4,
                    Condition = f => f.departureTimeOriginMinutes.min > 0,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByDepHourOriginMin(f.departureTimeOriginMinutes, dto),
                    FixFilterRange = (f, dto) => FilterFixer.FixFilterRangeDepTimeOriginMin(f, dto),
                    OrderBySelectedType = (dto) => FilterOrdenator.OrderByDepTimeOrigin(dto)
                },
                new()
                {
                Type = FilterType.DepartureTimeOriginMinutesMax,
                    Priority = 5,
                    Condition = f => f.departureTimeOriginMinutes.max > 0,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByDepHourOriginMax(f.departureTimeOriginMinutes, dto),
                    FixFilterRange = (f, dto) => FilterFixer.FixFilterRangeDepTimeOriginMax(f, dto),
                    OrderBySelectedType = (dto) => FilterOrdenator.OrderByDepTimeOrigin(dto)
                },
                new()
                {
                    Type = FilterType.DepartureTimeReturnMinutesMin,
                    Priority = 4,
                    Condition = f => f.departureTimeReturnMinutes.min > 0,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByDepHourReturnMin(f.departureTimeReturnMinutes, dto),
                    FixFilterRange = (f, dto) => FilterFixer.FixFilterRangeDepTimeReturnMin(f, dto),
                    OrderBySelectedType =(dto) => FilterOrdenator.OrderByDepTimeReturn(dto)
                },
                new()
                {
                    Type = FilterType.DepartureTimeReturnMinutesMax,
                    Priority = 5,
                    Condition = f => f.departureTimeReturnMinutes.max > 0,
                    ApplyFilter = (f, dto) => FilterOperator.FilterByDepHourReturnMax(f.departureTimeReturnMinutes, dto),
                    FixFilterRange = (f, dto) => FilterFixer.FixFilterRangeDepTimeReturnMax(f, dto),
                    OrderBySelectedType =(dto) => FilterOrdenator.OrderByDepTimeReturn(dto)
                }
            ];
        }
    }
}
