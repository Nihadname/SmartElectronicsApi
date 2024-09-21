using SmartElectronicsApi.Application.Dtos.SubsCategory;
using SmartElectronicsApi.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Interfaces
{
    public interface ISubCategoryService
    {
        Task<SubCategory> Create(SubCategoryCreateDto subCategoryCreateDto);
        Task<int> Delete(int? id);
    }
}
