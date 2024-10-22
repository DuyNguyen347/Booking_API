using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Account;
using AutoMapper;
using Domain.Constants.Enum;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Shared.ApplicationConstants;
using Application.Interfaces.Category;
using System.Diagnostics.CodeAnalysis;

namespace Application.Features.Category.Command.AddCategory
{
    public class AddCategoryCommand : IRequest<Result<AddCategoryCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
    }

    internal class AddCategoryCommandHandler : IRequestHandler<AddCategoryCommand, Result<AddCategoryCommand>>
    {
        private readonly IMapper _mapper;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork<long> _unitOfWork;


        public AddCategoryCommandHandler(IMapper mapper, ICategoryRepository categoryRepository, IUnitOfWork<long> unitOfWork, IAccountService accountService)
        {
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddCategoryCommand>> Handle(AddCategoryCommand request, CancellationToken cancellationToken)
        {
            var addCategory = _mapper.Map<Domain.Entities.Category.Category>(request);
            await _categoryRepository.AddAsync(addCategory);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addCategory.Id;
            return await Result<AddCategoryCommand>.SuccessAsync(request);
        }
    }
}
