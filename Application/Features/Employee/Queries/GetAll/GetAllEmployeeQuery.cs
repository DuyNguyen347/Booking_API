using Application.Interfaces;
using Application.Interfaces.Employee;
using Application.Parameters;
using Domain.Entities;
using Domain.Entities.Employee;
using Domain.Helpers;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp.Formats.Gif;
using System.Linq.Dynamic.Core;
using System.Security.AccessControl;

namespace Application.Features.Employee.Queries.GetAll
{
    public class GetAllEmployeeQuery : RequestParameter, IRequest<PaginatedResult<GetAllEmployeeResponse>>
    {
        public long? WorkShiftId { get; set; }
        public bool? Gender { get; set; }
    }

    internal class GetAllEmployeeHandler : IRequestHandler<GetAllEmployeeQuery, PaginatedResult<GetAllEmployeeResponse>>
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUploadService _uploadService;
        private readonly UserManager<AppUser> _userManager;

        public GetAllEmployeeHandler(IEmployeeRepository employeeRepository, IUploadService uploadService, UserManager<AppUser> userManager)
        {
            _employeeRepository = employeeRepository;
            _uploadService = uploadService;
            _userManager = userManager;
        }

        public async Task<PaginatedResult<GetAllEmployeeResponse>> Handle(GetAllEmployeeQuery request, CancellationToken cancellationToken)
        {
            if (request.Keyword != null)
                request.Keyword = request.Keyword.Trim();

            var query = _employeeRepository.Entities.AsEnumerable()
                        .Where(x => !x.IsDeleted && (string.IsNullOrEmpty(request.Keyword)
                                                || StringHelper.Contains(x.Name, request.Keyword) || x.Id.ToString().Contains(request.Keyword))
                                                && (!request.Gender.HasValue || x.Gender == request.Gender)
                                                && (!request.WorkShiftId.HasValue || x.WorkShiftId == request.WorkShiftId))
                        .AsQueryable()
                        .Select(x => new GetAllEmployeeResponse
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Gender = x.Gender,
                            PhoneNumber = x.PhoneNumber,
                            CreatedOn = x.CreatedOn,
                            LastModifiedOn = x.LastModifiedOn,
                            WorkShiftId = x.WorkShiftId,
                            Email = x.Email,
                            ImageFile = x.Image,
                            ImageLink = _uploadService.GetFullUrl(x.Image),
                            IsAdmin = GetRole(x.Id).Result
                        });
            var data = query.OrderBy(request.OrderBy);
            var totalRecord = data.Count();
            List<GetAllEmployeeResponse> result;

            //Pagination
            if (!request.IsExport)
                result = data.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            else
                result = data.ToList();
            return PaginatedResult<GetAllEmployeeResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }

        public async Task<bool> GetRole(long userid)
        {
            var account = await _userManager.Users.Where(x => !x.IsDeleted && x.UserId == userid && x.TypeFlag == Domain.Constants.Enum.TypeFlagEnum.Employee).FirstOrDefaultAsync();
            var role = await _userManager.GetRolesAsync(account);
           return (role[0] == "Superadmin") ? true : false;
        }
    }
}