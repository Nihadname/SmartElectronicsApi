using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using SmartElectronicsApi.Application.Dtos.Address;
using SmartElectronicsApi.Application.Dtos.Brand;
using SmartElectronicsApi.Application.Exceptions;
using SmartElectronicsApi.Application.Interfaces;
using SmartElectronicsApi.Core.Entities;
using SmartElectronicsApi.Core.Repositories;
using SmartElectronicsApi.DataAccess.Data.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartElectronicsApi.Application.Implementations
{
    public class AddressService : IAddressService
    {
      private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _contextAccessor;

        public AddressService( IMapper mapper, UserManager<AppUser> userManager, IHttpContextAccessor contextAccessor, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _unitOfWork = unitOfWork;
        }

        public async Task<AddressListItemDto> Create(AddressCreateDto addressCreateDto)
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "Id", "User ID cannot be null");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new CustomException(403, "This user doesn't exist");

            

            if (addressCreateDto.Latitude == null || addressCreateDto.Longitude == null)
            {
                var (latitude, longitude) = await GetCoordinatesFromAddress(addressCreateDto);

                if (latitude == null || longitude == null)
                {
                    throw new CustomException(400, "All", "Unable to get latitude and longitude for the provided address");
                }

                addressCreateDto.Latitude = latitude;
                addressCreateDto.Longitude = longitude;
               
            }

            var mappedAddress = _mapper.Map<Address>(addressCreateDto);
            mappedAddress.AppUserId = user.Id;
            if (await _unitOfWork.addressRepository.isExists(s => s.Country.ToLower().Trim() == addressCreateDto.Country.ToLower().Trim() &&
            s.State.ToLower().Trim() == addressCreateDto.State.ToLower().Trim() &&
                                                                   s.City.ToLower().Trim() == addressCreateDto.City.ToLower().Trim() &&
                                                                   s.Street.ToLower().Trim() == addressCreateDto.Street.ToLower().Trim() &&
                                                                   s.ZipCode.ToLower().Trim() == addressCreateDto.ZipCode.ToLower().Trim() &&
                                                                   s.AppUserId == mappedAddress.AppUserId) is true)
            {
                throw new CustomException(400, "This location already exists in the user's savings");
            }

            if ((await _unitOfWork.addressRepository.GetAll(s => s.AppUserId == user.Id)).Count() >= 6)
            {
                throw new CustomException(400, "All", "You can't have more than 6 addresses saved");
            }
            await _unitOfWork.addressRepository.Create(mappedAddress);
            _unitOfWork.Commit();
            var AddressListItemDto=_mapper.Map<AddressListItemDto>(mappedAddress);
            return AddressListItemDto;
        }

        private async Task<(double? Latitude, double? Longitude)> GetCoordinatesFromAddress(IAddressDto addressDto)
        {
            var addressParts=new List<string>();    
            if (addressDto.Country != null && addressDto.State != null && addressDto.City != null)
            {
                addressParts = new List<string>
    {
        addressDto.Country,
        addressDto.State,
        addressDto.City,
        addressDto.Street
    };
            }
            else
            {
                throw new CustomException("400", "you should enter values like Country,State ,City ");
            }

            var formattedAddress = string.Join(", ", addressParts.Where(part => !string.IsNullOrEmpty(part)));
            // URL encode the address string
            var url = $"https://nominatim.openstreetmap.org/search?format=json&limit=1&q={Uri.EscapeDataString(formattedAddress)}";

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("SmartElectronicsApi/1.0 (nihadcoding@gmail.com)");

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var geocodeResponse = JsonSerializer.Deserialize<List<NominatimResponse>>(content);

                    if (geocodeResponse?.Count > 0)
                    {
                        var location = geocodeResponse[0];
                        if (double.TryParse(location.lat, out double lat) && double.TryParse(location.lon, out double lon))
                        {
                            return (lat, lon);
                        }
                    }
                }
                else
                {
                    // Log the response status code and reason for debugging
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Error: {response.StatusCode}, {response.ReasonPhrase}, Content: {errorContent}");
                }
            }

            return (null, null);
        }

        public class NominatimResponse
        {
            public string lat { get; set; }
            public string lon { get; set; }
        }
        public async Task<AddressListItemDto> Update(int? id , AddressUpdateDto addressUpdateDto)
        {
            if (id == null)
            {
                throw new CustomException(400, "Address ID cannot be null");
            }

            var existingAddress = await _unitOfWork.addressRepository.GetEntity(a => a.Id == id);
            if (existingAddress == null)
            {
                throw new CustomException(404, "You are not authorized to update this address.");
            }

            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId) || userId != existingAddress.AppUserId)
            {
                throw new CustomException(401, "Address not found or does not belong to the user");
            }
            _mapper.Map(addressUpdateDto, existingAddress);
            double? latitude = null;
            double? longitude = null;

            if (addressUpdateDto.City != null || addressUpdateDto.Street != null || addressUpdateDto.State != null
                || addressUpdateDto.Country != null)
            {
                (latitude, longitude) = await GetCoordinatesFromAddress(addressUpdateDto);
                if (latitude == null || longitude == null)
                {
                    throw new CustomException(400, "All", "Unable to get latitude and longitude for the provided address");
                }

                existingAddress.Latitude = latitude;
                existingAddress.Longitude = longitude;
            }
            await _unitOfWork.addressRepository.Update(existingAddress);
            _unitOfWork.Commit();
            var AddressListItemDto = _mapper.Map<AddressListItemDto>(existingAddress);
            return AddressListItemDto;

        }


        public async Task<int> Delete(int? id)
        {
            if (id == null)
            {
                throw new CustomException(400, "Address ID cannot be null");
            }

            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "User ID cannot be null");
            }

            var address = await _unitOfWork.addressRepository.GetEntity(s => s.Id == id && s.AppUserId == userId);
            if (address == null)
            {
                throw new CustomException(404, "Address not found or does not belong to the user");
            }

            await _unitOfWork.addressRepository.Delete(address);
             _unitOfWork.Commit();

            return address.Id;
        }
       public async Task<List<AddressListItemDto>> GetAll()
        {
            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "Id", "User ID cannot be null");
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) throw new CustomException(403, "this user doesnt exist");
            var Addresses =await _unitOfWork.addressRepository.GetAll(s=>s.AppUserId==user.Id);
            var MappedAddresses=_mapper.Map<List<AddressListItemDto>>(Addresses);
            return MappedAddresses;
        }
        public async Task<AddressReturnDto> GetById(int? id)
        {
            if (id == null)
            {
                throw new CustomException(400, "Address ID cannot be null");
            }

            var userId = _contextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new CustomException(400, "User ID cannot be null");
            }

            var address = await _unitOfWork.addressRepository.GetEntity(s => s.Id == id && s.AppUserId == userId);
            if (address == null)
            {
                throw new CustomException(404, "Address not found or does not belong to the user");
            }
            var MappedDto=_mapper.Map<AddressReturnDto>(address);
            return MappedDto;
        }
    }
}
