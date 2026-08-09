using syc_pm_client.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace syc_pm_client.Services.Interfaces
{
    public interface IRequestService
    {
        Task<bool> CreateRequest(RequestDto request);
        Task<List<RequestResponseDto>> GetRequests();
        Task<bool> DeleteRequest(Guid id);
        Task<string> GetAdminPublicKey();
    }
}