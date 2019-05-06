﻿using FamUnion.Core.Interface;
using FamUnion.Core.Model;
using FamUnion.Core.Utility;
using FamUnion.Core.Validation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FamUnion.Infrastructure.Services
{
    public class ReunionService : IReunionService
    {
        private readonly IReunionRepository _reunionRepository;
        private readonly IAddressRepository _addressRepository;

        public ReunionService(IReunionRepository reunionRepository, IAddressRepository addressRepository)
        {
            _reunionRepository = Validator.ThrowIfNull(reunionRepository, nameof(reunionRepository));
            _addressRepository = Validator.ThrowIfNull(addressRepository, nameof(addressRepository));
        }

        public async Task<Reunion> GetReunionAsync(Guid id)
        {
            var reunion = await _reunionRepository.GetReunionAsync(id).
                ConfigureAwait(continueOnCapturedContext: false);

            await PopulateAddresses(reunion.Yield())
                .ConfigureAwait(continueOnCapturedContext: false);

            return reunion;
        }

        public async Task<IEnumerable<Reunion>> GetReunionsAsync()
        {
            var reunions = await _reunionRepository.GetReunionsAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            await PopulateAddresses(reunions)
                .ConfigureAwait(continueOnCapturedContext: false);

            return reunions;
        }

        public async Task<Reunion> SaveReunionAsync(Reunion reunion)
        {
            var savedReunion = await _reunionRepository.SaveReunionAsync(reunion)
                .ConfigureAwait(continueOnCapturedContext: false);

            if (!(reunion.Location is null))
            {
                _ = await _addressRepository.SaveReunionAddressAsync(savedReunion.Id.Value, reunion.Location)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }

            return await GetReunionAsync(savedReunion.Id.Value).
                ConfigureAwait(continueOnCapturedContext: false);
        }

        public async Task DeleteReunionAsync(Guid id)
        {
            await _reunionRepository.DeleteReunionAsync(id)
                .ConfigureAwait(continueOnCapturedContext: false);
        }

        private Task PopulateAddresses(IEnumerable<Reunion> reunions)
        {
            Parallel.ForEach(reunions, async reunion =>
            {
                if(reunion.Id.HasValue)
                    reunion.Location = await _addressRepository.GetReunionAddressAsync(reunion.Id.Value)
                    .ConfigureAwait(continueOnCapturedContext: false);
            });

            return Task.CompletedTask;
        }
    }
}
