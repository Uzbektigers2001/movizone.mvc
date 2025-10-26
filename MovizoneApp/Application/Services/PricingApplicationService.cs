using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using MovizoneApp.Application.Interfaces;
using MovizoneApp.Core.Exceptions;
using MovizoneApp.Core.Interfaces;
using MovizoneApp.DTOs;
using MovizoneApp.Models;

namespace MovizoneApp.Application.Services
{
    /// <summary>
    /// Application service for pricing plan business logic
    /// Uses DTOs and AutoMapper for clean separation from database models
    /// </summary>
    public class PricingApplicationService : IPricingApplicationService
    {
        private readonly IPricingPlanRepository _pricingRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<PricingApplicationService> _logger;

        public PricingApplicationService(
            IPricingPlanRepository pricingRepository,
            IMapper mapper,
            ILogger<PricingApplicationService> logger)
        {
            _pricingRepository = pricingRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PricingPlanDto>> GetAllPlansAsync()
        {
            _logger.LogInformation("Fetching all pricing plans");
            var plans = await _pricingRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<PricingPlanDto>>(plans);
        }

        public async Task<PricingPlanDto?> GetPlanByIdAsync(int id)
        {
            _logger.LogInformation("Fetching pricing plan with ID: {PlanId}", id);
            var plan = await _pricingRepository.GetByIdAsync(id);

            if (plan == null)
            {
                _logger.LogWarning("Pricing plan with ID {PlanId} not found", id);
                return null;
            }

            return _mapper.Map<PricingPlanDto>(plan);
        }

        public async Task<PricingPlanDto?> GetPopularPlanAsync()
        {
            _logger.LogInformation("Fetching popular pricing plan");
            var plans = await _pricingRepository.GetAllAsync();
            var popularPlan = plans.FirstOrDefault(p => p.IsPopular);

            if (popularPlan == null)
            {
                _logger.LogWarning("No popular pricing plan found");
                return null;
            }

            return _mapper.Map<PricingPlanDto>(popularPlan);
        }

        public async Task<PricingPlanDto> CreatePlanAsync(CreatePricingPlanDto createPlanDto)
        {
            _logger.LogInformation("Creating new pricing plan: {PlanName}", createPlanDto.Name);

            // Map DTO to Model
            var pricingPlan = _mapper.Map<PricingPlan>(createPlanDto);

            // Business validation (additional to DTO validation)
            if (pricingPlan.Price < 0)
            {
                throw new BadRequestException("Price must be non-negative");
            }

            // Set timestamps
            pricingPlan.CreatedAt = DateTime.UtcNow;

            // Save to repository
            var created = await _pricingRepository.AddAsync(pricingPlan);

            _logger.LogInformation("Pricing plan created successfully with ID: {PlanId}", created.Id);

            // Map back to DTO and return
            return _mapper.Map<PricingPlanDto>(created);
        }

        public async Task UpdatePlanAsync(UpdatePricingPlanDto updatePlanDto)
        {
            _logger.LogInformation("Updating pricing plan with ID: {PlanId}", updatePlanDto.Id);

            // Check if plan exists
            var existing = await _pricingRepository.GetByIdAsync(updatePlanDto.Id);
            if (existing == null)
            {
                throw new NotFoundException("PricingPlan", updatePlanDto.Id);
            }

            // Map DTO to Model
            var pricingPlan = _mapper.Map<PricingPlan>(updatePlanDto);

            // Business validation
            if (pricingPlan.Price < 0)
            {
                throw new BadRequestException("Price must be non-negative");
            }

            // Preserve creation date and set update time
            pricingPlan.CreatedAt = existing.CreatedAt;
            pricingPlan.UpdatedAt = DateTime.UtcNow;

            // Update in repository
            await _pricingRepository.UpdateAsync(pricingPlan);
            _logger.LogInformation("Pricing plan updated successfully: {PlanId}", pricingPlan.Id);
        }

        public async Task DeletePlanAsync(int id)
        {
            _logger.LogInformation("Deleting pricing plan with ID: {PlanId}", id);

            var pricingPlan = await _pricingRepository.GetByIdAsync(id);
            if (pricingPlan == null)
            {
                throw new NotFoundException("PricingPlan", id);
            }

            // Soft delete
            pricingPlan.IsDeleted = true;
            pricingPlan.UpdatedAt = DateTime.UtcNow;
            await _pricingRepository.UpdateAsync(pricingPlan);

            _logger.LogInformation("Pricing plan soft-deleted successfully: {PlanId}", id);
        }
    }
}
