using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FondsSocial.Application.DTOs;
using FondsSocial.Domain.Entities;
using FondsSocial.Infrastructure.UnitOfWork;

namespace FondsSocial.Application.Services
{
    public class TypeDePretService : ITypeDePretService
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public TypeDePretService(IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
        }

        public async Task<TypeDePretDto> CreateAsync(CreateTypeDePretDto dto)
        {
            var entity = _mapper.Map<TypeDePret>(dto);
            await _uow.TypeDePrets.AddAsync(entity);
            await _uow.SaveChangesAsync();
            return _mapper.Map<TypeDePretDto>(entity);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _uow.TypeDePrets.GetByIdAsync(id);
            if (existing == null) return false;
            _uow.TypeDePrets.Delete(existing);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<PagedResult<TypeDePretDto>> GetAllAsync(int page = 1, int pageSize = 20)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var (items, totalCount) = await _uow.TypeDePrets.GetPagedAsync(page, pageSize);
            return new PagedResult<TypeDePretDto>
            {
                Items = _mapper.Map<IEnumerable<TypeDePretDto>>(items),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public async Task<TypeDePretDto?> GetByIdAsync(int id)
        {
            var e = await _uow.TypeDePrets.GetByIdAsync(id);
            if (e == null) return null;
            return _mapper.Map<TypeDePretDto>(e);
        }

        public async Task<bool> UpdateAsync(int id, UpdateTypeDePretDto dto)
        {
            var existing = await _uow.TypeDePrets.GetByIdAsync(id);
            if (existing == null) return false;
            _mapper.Map(dto, existing);
            _uow.TypeDePrets.Update(existing);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}
