using Application.Features.SchoolClassManagement.ClassManagement.DTOs;
using MediatR;

namespace Application.Features.SchoolClassManagement.ClassManagement.Queries.GetAllClasses;

public record GetAllClassesQuery : IRequest<List<ClassResponseDto>>;