using Application.Interfaces.Category;
using Application.Interfaces.Cinema;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services.Account;
using AutoMapper;
using Domain.Wrappers;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Cinema.Command.AddCinema
{
    public class AddCinemaCommand : IRequest<Result<AddCinemaCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string City { get; set; } = default!;
    }

    internal class AddCinemaCommandHandler : IRequestHandler<AddCinemaCommand, Result<AddCinemaCommand>>
    {
        private readonly IMapper _mapper;
        private readonly ICinemaRepository _cinemaRepository;
        private readonly IUnitOfWork<long> _unitOfWork;


        public AddCinemaCommandHandler(IMapper mapper, ICinemaRepository cinemaRepository, IUnitOfWork<long> unitOfWork, IAccountService accountService)
        {
            _mapper = mapper;
            _cinemaRepository = cinemaRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<AddCinemaCommand>> Handle(AddCinemaCommand request, CancellationToken cancellationToken)
        {
            var addCategory = _mapper.Map<Domain.Entities.Cinema.Cinema>(request);
            await _cinemaRepository.AddAsync(addCategory);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = addCategory.Id;
            return await Result<AddCinemaCommand>.SuccessAsync(request);
        }
    }
}
