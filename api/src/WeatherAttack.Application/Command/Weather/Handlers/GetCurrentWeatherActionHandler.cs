﻿using WeatherAttack.Contracts.Command;
using WeatherAttack.Contracts.Dtos.Weather.Request;
using WeatherAttack.Contracts.Interfaces;
using WeatherAttack.Contracts.Mapper;
using WeatherAttack.Domain.Entities;

namespace WeatherAttack.Application.Command.Weather.Handlers
{
    public class GetCurrentWeatherActionHandler : IActionHandler<GetCurrentWeatherCommand>
    {
        private IOpenWeatherMapService OpenWeatherMap { get; }

        private IMapper<CurrentWeather, CurrentWeatherRequestDto, CurrentWeatherRequestDto> Mapper { get; }

        public GetCurrentWeatherActionHandler(IOpenWeatherMapService openWeatherMap, 
            IMapper<CurrentWeather, CurrentWeatherRequestDto, CurrentWeatherRequestDto> mapper)
        {
            OpenWeatherMap = openWeatherMap;
            Mapper = mapper;
        }

        public GetCurrentWeatherCommand ExecuteAction(GetCurrentWeatherCommand command)
        {
            if (!command.IsValid)
            {
                return command;
            }

            var result = OpenWeatherMap.GetCurrentWeatherByCoordinates(command.Latitude, command.Longitude);

            command.Result = Mapper.ToEntity(result);

            return command;
        }
    }
}