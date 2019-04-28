﻿using FamUnion.Core.Interface;
using FamUnion.Core.Model;
using FamUnion.Core.Utility;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FamUnion.Infrastructure.Repository
{
    public class AddressRepository : DbAccess<Address>, IAddressRepository
    {
        public AddressRepository(string connection)
            : base(connection)
        {

        }

        public async Task<Address> GetAddressAsync(Guid id)
        {
            ParameterDictionary parameters = new ParameterDictionary(new string[]
            {
                "id", id.ToString()
            });
            return (await ExecuteStoredProc("[dbo].[spGetAddressById]", parameters)
                .ConfigureAwait(continueOnCapturedContext: false)).SingleOrDefault();
        }

        public Task<Address> GetEventAddressAsync(Guid eventId)
        {
            throw new NotImplementedException();
        }

        public Task<Address> GetFamilyAddressAsync(Guid familyId)
        {
            throw new NotImplementedException();
        }

        public async Task<Address> GetReunionAddressAsync(Guid reunionId)
        {
            ParameterDictionary parameters = new ParameterDictionary(new string[]
            {
                "reunionId", reunionId.ToString()
            });
            return (await ExecuteStoredProc("[dbo].[spGetAddressByReunionId]", parameters)
                .ConfigureAwait(continueOnCapturedContext: false)).SingleOrDefault();
        }

        public Task<Address> SaveEventAddressAsync(Guid eventId, Address address)
        {
            throw new NotImplementedException();
        }

        public Task<Address> SaveFamilyAddressAsync(Guid familyId, Address address)
        {
            throw new NotImplementedException();
        }

        public async Task<Address> SaveReunionAddressAsync(Guid reunionId, Address address)
        {
            Address currentAddress = await GetReunionAddressAsync(reunionId).
                ConfigureAwait(continueOnCapturedContext: false);

            if(currentAddress.Equals(address))
            {
                return currentAddress;
            }

            ParameterDictionary parameters = new ParameterDictionary(new string[]
            {
                "reunionId", reunionId.ToString(),
                "addressId", address.Id.GetDbGuidString(),
                "description", address.Description,
                "line1", address.Line1,
                "line2", address.Line2,
                "city", address.City,
                "state", address.State,
                "zipcode", address.ZipCode
            });
            return (await ExecuteStoredProc("[dbo].[spSaveReunionAddress]", parameters)
                .ConfigureAwait(continueOnCapturedContext: false)).SingleOrDefault();
        }
    }
}