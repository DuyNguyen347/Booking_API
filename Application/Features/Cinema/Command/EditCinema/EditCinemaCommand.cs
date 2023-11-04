using Application.Interfaces.Customer;
using Application.Interfaces.Repositories;
using Application.Interfaces;
using AutoMapper;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Application.Shared.ApplicationConstants;
using Application.Interfaces.Cinema;

namespace Application.Features.Cinema.Command.EditCinema
{
    public class EditCinemaCommand : IRequest<Result<EditCinemaCommand>>
    {
        public long Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Description { get; set; }
        public string? City { get; set; }
    }

    internal class EditCinemaCommandHandler : IRequestHandler<EditCinemaCommand, Result<EditCinemaCommand>>
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork<long> _unitOfWork;
        private readonly ICinemaRepository _cinemaRepository;

        public EditCinemaCommandHandler(IMapper mapper, ICinemaRepository cinemaRepository, IUnitOfWork<long> unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _cinemaRepository = cinemaRepository;
        }

        public async Task<Result<EditCinemaCommand>> Handle(EditCinemaCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == 0)
            {
                return await Result<EditCinemaCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            }

            var editCinema = await _cinemaRepository.FindAsync(x => x.Id == request.Id && !x.IsDeleted);
            if (editCinema == null) return await Result<EditCinemaCommand>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            
            _mapper.Map(request, editCinema);
            await _cinemaRepository.UpdateAsync(editCinema);
            await _unitOfWork.Commit(cancellationToken);
            request.Id = editCinema.Id;
            return await Result<EditCinemaCommand>.SuccessAsync(request);
        }
    }
}
