using Application.Features.Customer.Queries.GetById;
using Application.Interfaces.Customer;
using Application.Interfaces;
using Domain.Constants;
using Domain.Entities;
using Domain.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.Cinema;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Cinema.Queries.GetById
{
    public class GetCinemaByIdQuery : IRequest<Result<GetCinemaByIdResponse>>
    {
        public long Id { get; set; }
    }
    internal class GetCinemaByIdQueryHandler : IRequestHandler<GetCinemaByIdQuery, Result<GetCinemaByIdResponse>>
    {
        private readonly ICinemaRepository _cinemaRepository;

        public GetCinemaByIdQueryHandler(ICinemaRepository cinemaRepository)
        {
            _cinemaRepository = cinemaRepository;
        }

        public async Task<Result<GetCinemaByIdResponse>> Handle(GetCinemaByIdQuery request, CancellationToken cancellationToken)
        {
            var cinema = await (from e in _cinemaRepository.Entities
                                  where e.Id == request.Id && !e.IsDeleted
                                  select new GetCinemaByIdResponse()
                                  {
                                      Id = e.Id,
                                      Name = e.Name,
                                      Description = e.Description,
                                      City = e.City
                                  }).FirstOrDefaultAsync(cancellationToken: cancellationToken);
            if (cinema == null) return await Result<GetCinemaByIdResponse>.FailAsync(StaticVariable.NOT_FOUND_MSG);
            return await Result<GetCinemaByIdResponse>.SuccessAsync(cinema);
        }
    }
}
