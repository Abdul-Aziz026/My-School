using Application.Features.SchoolClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.Queries.GetAllClasses;

public record GetAllClassesQuery : IRequest<List<ClassResponseDto>>;