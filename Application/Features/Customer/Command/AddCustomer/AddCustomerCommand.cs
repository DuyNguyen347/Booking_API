using Application.Features.Employee.Command.AddEmployee;
using Application.Interfaces.Customer;
using Application.Interfaces.Employee;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Account;
using AutoMapper;
using Domain.Constants;
using Domain.Constants.Enum;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Org.BouncyCastle.Asn1.Ocsp;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Application.Interfaces;
using static QRCoder.PayloadGenerator;
using Application.Interfaces.Services;
using Application.Dtos.Requests.SendEmail;
using Hangfire;

namespace Application.Features.Customer.Command.AddCustomer
{
    public class AddCustomerCommand : IRequest<Result<AddCustomerCommand>>
    {
        public long Id { get; set; }
        public string CustomerName { get; set; } = default!;
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^[0-9]\d{7,9}$", ErrorMessage = "Phone number must be between 8 and 10 digits.")]
        public string PhoneNumber { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public decimal? TotalMoney { get; set; }
        public string? Email { get; set; }
    }

    internal class AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, Result<AddCustomerCommand>>
    {
        private readonly IMapper _mapper;
        private readonly ICustomerRepository _customnerRepository;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly UserManager<AppUser> _usermanager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _mailService;


        public AddCustomerCommandHandler(IMapper mapper, ICustomerRepository customerRepository, IUnitOfWork<long> unitOfWork, IAccountService accountService, UserManager<AppUser> usermanager,IHttpContextAccessor httpContext, ICurrentUserService currentUserService, IEmailService mailService)
        {
            _mapper = mapper;
            _customnerRepository = customerRepository;
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _usermanager = usermanager;
            _httpContextAccessor = httpContext;
            _currentUserService = currentUserService;
            _mailService = mailService;
        }

        public async Task<Result<AddCustomerCommand>> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
        {
            var addCustomer = _mapper.Map<Domain.Entities.Customer.Customer>(request);
            if((request.Username != null && request.Password != null) && (request.Username != "" && request.Password != ""))
            {
                if (request.Password.Length < 8)
                {
                    return await Result<AddCustomerCommand>.FailAsync(StaticVariable.INVALID_PASSWORD);
                }
                var existEmail = await _customnerRepository.Entities.FirstOrDefaultAsync(x => x.Email == request.Email && !x.IsDeleted);
                if (existEmail != null)
                {
                    return await Result<AddCustomerCommand>.FailAsync(StaticVariable.EMAIL_EXISTS_MSG);
                }
                if (_customnerRepository.Entities.Where(x => x.PhoneNumber == request.PhoneNumber && !x.IsDeleted).FirstOrDefault() != null)
                {
                    return await Result<AddCustomerCommand>.FailAsync(StaticVariable.DUPLICATE_PHONENUMBER);
                }
                bool isUsernameExists = await _accountService.IsExistUsername(request.Username);
                if (isUsernameExists)
                {
                    return await Result<AddCustomerCommand>.FailAsync(StaticVariable.IS_EXISTED_USERNAME);
                }
                await _customnerRepository.AddAsync(addCustomer);
                await _unitOfWork.Commit(cancellationToken);
                request.Id = addCustomer.Id;
                var user = new AppUser()
                {
                    FullName = request.CustomerName,
                    UserName = request.Username,
                    Email = request.Email,
                    EmailConfirmed = false,
                    PhoneNumber = request.PhoneNumber,
                    PhoneNumberConfirmed = true,
                    CreatedOn = DateTime.Now,
                    IsActive = true,
                    TypeFlag = TypeFlagEnum.Customer,
                    UserId = request.Id
                };
                bool result = await _accountService.AddAcount(user, request.Password,RoleConstants.CustomerRole);
                if (result == false)
                {
                    return await Result<AddCustomerCommand>.FailAsync(StaticVariable.ERROR_ADD_USER);
                }
                var tokenConfirmEmail = await _usermanager.GenerateEmailConfirmationTokenAsync(user);
                

                //Console.WriteLine("url link: ", _httpContextAccessor.HttpContext.Request);
                var urlConfirmEmail = new UriBuilder(_currentUserService.HostServerName + "/api/account/confirm-email");
                urlConfirmEmail.Query = $"token={tokenConfirmEmail}&email={request.Email}";

                string finalUrlConfirmEmail = urlConfirmEmail.ToString();

                var email = new EmailRequest()
                {
                    Body = "Vui lòng nhấn link sau để xác thực email: " + finalUrlConfirmEmail,
                    Subject = "Xác thực email",
                    To = request.Email,
                };
                BackgroundJob.Enqueue(() => _mailService.SendAsync(email));
                // generate token confirm email
                //var token = awa

                return await Result<AddCustomerCommand>.SuccessAsync(request,finalUrlConfirmEmail);
            }
            await _customnerRepository.AddAsync(addCustomer);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addCustomer.Id;
            return await Result<AddCustomerCommand>.SuccessAsync(request);
        }
    }
}
