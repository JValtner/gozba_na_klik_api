using AutoMapper;
using Gozba_na_klik.DTOs.DeliveryPersonSchedule;
using Gozba_na_klik.Exceptions;
using Gozba_na_klik.Models;
using Gozba_na_klik.Repositories;
using Gozba_na_klik.Utils;
using Microsoft.Extensions.Logging;

namespace Gozba_na_klik.Services
{
    public class DeliveryPersonScheduleService : IDeliveryPersonScheduleService
    {
        private readonly IDeliveryPersonScheduleRepository _scheduleRepo;
        private readonly IUsersRepository _userRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<DeliveryPersonScheduleService> _logger;

        public DeliveryPersonScheduleService(
            IDeliveryPersonScheduleRepository scheduleRepo,
            IUsersRepository userRepo,
            IMapper mapper,
            ILogger<DeliveryPersonScheduleService> logger)
        {
            _scheduleRepo = scheduleRepo;
            _userRepo = userRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<WeeklyScheduleDto> GetWeeklyScheduleAsync(int deliveryPersonId)
        {
            _logger.LogInformation("Fetching weekly schedule for delivery person {DeliveryPersonId}", deliveryPersonId);

            var user = await _userRepo.GetByIdAsync(deliveryPersonId);
            if (user == null)
            {
                _logger.LogWarning("Delivery person {DeliveryPersonId} not found", deliveryPersonId);
                throw new NotFoundException($"Kurir sa ID {deliveryPersonId} nije pronađen.");
            }

            var schedules = await _scheduleRepo.GetByDeliveryPersonAsync(deliveryPersonId);
            var dtos = _mapper.Map<List<DeliveryScheduleDto>>(schedules);
            var totalHours = dtos.Sum(d => d.Hours);

            _logger.LogInformation("Successfully fetched {Count} schedules for delivery person {DeliveryPersonId}, total hours: {TotalHours}",
                schedules.Count, deliveryPersonId, totalHours);

            return new WeeklyScheduleDto
            {
                Schedule = dtos,
                TotalWeeklyHours = Math.Round(totalHours, 2),
                RemainingHours = Math.Round(40 - totalHours, 2)
            };
        }

        public async Task<DeliveryScheduleDto> CreateScheduleAsync(int deliveryPersonId, CreateDeliveryScheduleDto dto)
        {
            _logger.LogInformation("Creating schedule for delivery person {DeliveryPersonId} on day {DayOfWeek}",
                deliveryPersonId, dto.DayOfWeek);

            var user = await _userRepo.GetByIdAsync(deliveryPersonId);
            if (user == null)
            {
                _logger.LogWarning("Delivery person {DeliveryPersonId} not found", deliveryPersonId);
                throw new NotFoundException($"Kurir sa ID {deliveryPersonId} nije pronađen.");
            }

            var startTime = TimeSpan.Parse(dto.StartTime);
            var endTime = TimeSpan.Parse(dto.EndTime);

            if (startTime >= endTime)
            {
                _logger.LogWarning("Invalid time range: start {StartTime} >= end {EndTime}", startTime, endTime);
                throw new BadRequestException("Početno vreme mora biti pre krajnjeg vremena.");
            }

            var dailyHours = (endTime - startTime).TotalHours;
            if (dailyHours > 10)
            {
                _logger.LogWarning("Daily hours {DailyHours} exceed limit of 10h", dailyHours);
                throw new BadRequestException($"Maksimalno radno vreme je 10 sati dnevno. Pokušali ste da unesete {dailyHours:F2}h.");
            }

            var weeklySchedule = await _scheduleRepo.GetByDeliveryPersonAsync(deliveryPersonId);
            var totalWeeklyHours = weeklySchedule.Sum(s => (s.EndTime - s.StartTime).TotalHours);

            if (totalWeeklyHours + dailyHours > 40)
            {
                var remaining = 40 - totalWeeklyHours;
                _logger.LogWarning("Weekly hours limit exceeded. Current: {Current}h, Attempting to add: {Adding}h, Limit: 40h",
                    totalWeeklyHours, dailyHours);
                throw new BadRequestException(
                    $"Prekoračen nedeljni limit od 40 sati. " +
                    $"Trenutno imate {totalWeeklyHours:F2}h, " +
                    $"pokušavate dodati {dailyHours:F2}h, " +
                    $"preostalo vam je {remaining:F2}h.");
            }

            var dayOfWeek = (DayOfWeek)dto.DayOfWeek;
            var existing = await _scheduleRepo.GetByDeliveryPersonAndDayAsync(deliveryPersonId, dayOfWeek);
            if (existing != null)
            {
                _logger.LogWarning("Schedule already exists for delivery person {DeliveryPersonId} on {DayOfWeek}",
                    deliveryPersonId, dayOfWeek);
                throw new BadRequestException($"Radno vreme za {DateTimeHelper.GetDayName(dayOfWeek)} već postoji.");
            }

            var schedule = new DeliveryPersonSchedule
            {
                DeliveryPersonId = deliveryPersonId,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                IsActive = true
            };

            var created = await _scheduleRepo.AddAsync(schedule);
            _logger.LogInformation("Schedule created successfully with ID {ScheduleId} for delivery person {DeliveryPersonId}",
                created.Id, deliveryPersonId);

            return _mapper.Map<DeliveryScheduleDto>(created);
        }

        public async Task<DeliveryScheduleDto> UpdateScheduleAsync(int deliveryPersonId, int scheduleId, CreateDeliveryScheduleDto dto)
        {
            _logger.LogInformation("Updating schedule {ScheduleId} for delivery person {DeliveryPersonId}",
                scheduleId, deliveryPersonId);

            var schedule = await _scheduleRepo.GetByIdAsync(scheduleId);
            if (schedule == null)
            {
                _logger.LogWarning("Schedule {ScheduleId} not found", scheduleId);
                throw new NotFoundException($"Raspored sa ID {scheduleId} nije pronađen.");
            }

            if (schedule.DeliveryPersonId != deliveryPersonId)
            {
                _logger.LogWarning("Forbidden: Schedule {ScheduleId} does not belong to delivery person {DeliveryPersonId}",
                    scheduleId, deliveryPersonId);
                throw new ForbiddenException("Nemate pristup ovom rasporedu.");
            }

            var startTime = TimeSpan.Parse(dto.StartTime);
            var endTime = TimeSpan.Parse(dto.EndTime);

            if (startTime >= endTime)
            {
                _logger.LogWarning("Invalid time range: start {StartTime} >= end {EndTime}", startTime, endTime);
                throw new BadRequestException("Početno vreme mora biti pre krajnjeg vremena.");
            }

            var dailyHours = (endTime - startTime).TotalHours;
            if (dailyHours > 10)
            {
                _logger.LogWarning("Daily hours {DailyHours} exceed limit of 10h", dailyHours);
                throw new BadRequestException($"Maksimalno radno vreme je 10 sati dnevno. Pokušali ste da unesete {dailyHours:F2}h.");
            }

            var weeklySchedule = await _scheduleRepo.GetByDeliveryPersonAsync(deliveryPersonId);
            var totalWeeklyHours = weeklySchedule
                .Where(s => s.Id != scheduleId)
                .Sum(s => (s.EndTime - s.StartTime).TotalHours);

            if (totalWeeklyHours + dailyHours > 40)
            {
                var remaining = 40 - totalWeeklyHours;
                _logger.LogWarning("Weekly hours limit exceeded. Current (excluding this): {Current}h, Attempting to add: {Adding}h",
                    totalWeeklyHours, dailyHours);
                throw new BadRequestException(
                    $"Prekoračen nedeljni limit od 40 sati. " +
                    $"Trenutno imate {totalWeeklyHours:F2}h (bez ovog rasporeda), " +
                    $"pokušavate dodati {dailyHours:F2}h, " +
                    $"preostalo vam je {remaining:F2}h.");
            }

            var newDayOfWeek = (DayOfWeek)dto.DayOfWeek;
            if (schedule.DayOfWeek != newDayOfWeek)
            {
                var existingOnNewDay = await _scheduleRepo.GetByDeliveryPersonAndDayAsync(deliveryPersonId, newDayOfWeek);
                if (existingOnNewDay != null)
                {
                    _logger.LogWarning("Schedule already exists for delivery person {DeliveryPersonId} on {DayOfWeek}",
                        deliveryPersonId, newDayOfWeek);
                    throw new BadRequestException($"Radno vreme za {DateTimeHelper.GetDayName(newDayOfWeek)} već postoji.");
                }
            }

            schedule.DayOfWeek = newDayOfWeek;
            schedule.StartTime = startTime;
            schedule.EndTime = endTime;

            var updated = await _scheduleRepo.UpdateAsync(schedule);
            _logger.LogInformation("Schedule {ScheduleId} updated successfully", scheduleId);

            return _mapper.Map<DeliveryScheduleDto>(updated);
        }

        public async Task DeleteScheduleAsync(int deliveryPersonId, int scheduleId)
        {
            _logger.LogInformation("Deleting schedule {ScheduleId} for delivery person {DeliveryPersonId}",
                scheduleId, deliveryPersonId);

            var schedule = await _scheduleRepo.GetByIdAsync(scheduleId);
            if (schedule == null)
            {
                _logger.LogWarning("Schedule {ScheduleId} not found", scheduleId);
                throw new NotFoundException($"Raspored sa ID {scheduleId} nije pronađen.");
            }

            if (schedule.DeliveryPersonId != deliveryPersonId)
            {
                _logger.LogWarning("Forbidden: Schedule {ScheduleId} does not belong to delivery person {DeliveryPersonId}",
                    scheduleId, deliveryPersonId);
                throw new ForbiddenException("Nemate pristup ovom rasporedu.");
            }

            await _scheduleRepo.DeleteAsync(scheduleId);
            _logger.LogInformation("Schedule {ScheduleId} deleted successfully", scheduleId);
        }
    }
}