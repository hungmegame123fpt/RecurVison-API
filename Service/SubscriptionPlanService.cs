using BusinessObject.DTO;
using BusinessObject.DTO.SubscriptionPlan;
using Repository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SubscriptionPlanService
    {
        private IUnitOfWork _unitOfWork;

        public SubscriptionPlanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<APIResponse<SubscriptionPlanDto>> GetByIdAsync(int id)
        {
            try
            {
                var plan = await _unitOfWork.SubscriptionPlanRepository.GetByIdAsync(id);
                if (plan == null)
                {
                    return APIResponse<SubscriptionPlanDto>.ErrorResult("Subscription plan not found");
                }

                var planDto = _mapper.Map<SubscriptionPlanDto>(plan);
                planDto.ActiveSubscriptionCount = await _unitOfWork.SubscriptionPlanRepository.GetActiveSubscriptionCountAsync(id);
                planDto.TotalRevenue = await _unitOfWork.SubscriptionPlanRepository.Get(id);

                return APIResponse<SubscriptionPlanDto>.SuccessResult(planDto);
            }
            catch (Exception ex)
            {
                return APIResponse<SubscriptionPlanDto>.ErrorResult("Failed to retrieve subscription plan");
            }
        }
    }
}
