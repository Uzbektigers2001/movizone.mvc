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
            try
            {
                _logger.LogInformation("Fetching all pricing plans");
                var plans = await _pricingRepository.GetAllAsync();
                return _mapper.Map<IEnumerable<PricingPlanDto>>(plans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all pricing plans");
                throw;
            }
        }

        public async Task<PricingPlanDto?> GetPlanByIdAsync(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching pricing plan with ID: {PlanId}", id);
                throw;
            }
        }

        public async Task<PricingPlanDto?> GetPopularPlanAsync()
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching popular pricing plan");
                throw;
            }
        }

        public async Task<PricingPlanDto> CreatePlanAsync(CreatePricingPlanDto createPlanDto)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating pricing plan: {PlanName}", createPlanDto.Name);
                throw;
            }
        }

        public async Task UpdatePlanAsync(UpdatePricingPlanDto updatePlanDto)
        {
            try
            {
                _logger.LogInformation("Updating pricing plan with ID: {PlanId}", updatePlanDto.Id);

                // Check if plan exists
                var existing = await _pricingRepository.GetByIdAsync(updatePlanDto.Id);
                if (existing == null)
                {
                    throw new NotFoundException("PricingPlan", updatePlanDto.Id);
                }

                // Map DTO properties to existing tracked entity
                _mapper.Map(updatePlanDto, existing);

                // Business validation
                if (existing.Price < 0)
                {
                    throw new BadRequestException("Price must be non-negative");
                }

                // Set update time (CreatedAt already preserved in existing entity)
                existing.UpdatedAt = DateTime.UtcNow;

                // Update in repository
                await _pricingRepository.UpdateAsync(existing);
                _logger.LogInformation("Pricing plan updated successfully: {PlanId}", existing.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating pricing plan with ID: {PlanId}", updatePlanDto.Id);
                throw;
            }
        }

        public async Task DeletePlanAsync(int id)
        {
            try
            {
                _logger.LogInformation("Deleting pricing plan with ID: {PlanId}", id);

                var pricingPlan = await _pricingRepository.GetByIdAsync(id);
                if (pricingPlan == null)
                {
                    throw new NotFoundException("PricingPlan", id);
                }

                // Soft delete
                pricingPlan.IsDeleted = true;
                pricingPlan.DeletedAt = DateTime.UtcNow;
                // TODO: Set DeletedBy from current user context when authentication is available
                // entity.DeletedBy = currentUserId;
                pricingPlan.UpdatedAt = DateTime.UtcNow;
                await _pricingRepository.UpdateAsync(pricingPlan);

                _logger.LogInformation("Pricing plan soft-deleted successfully: {PlanId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting pricing plan with ID: {PlanId}", id);
                throw;
            }
        }
    }
}
