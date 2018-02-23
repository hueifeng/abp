﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Authorization.Permissions;

namespace Volo.Abp.Permissions
{
    [Authorize(PermissionPermissions.Permissions.Default)]
    public class PermissionAppService : ApplicationService, IPermissionAppService
    {
        private readonly IPermissionManager _permissionManager;
        private readonly IPermissionDefinitionManager _permissionDefinitionManager;

        public PermissionAppService(
            IPermissionManager permissionManager, 
            IPermissionDefinitionManager permissionDefinitionManager)
        {
            _permissionManager = permissionManager;
            _permissionDefinitionManager = permissionDefinitionManager;
        }

        public async Task<GetPermissionListResultDto> GetAsync(string providerName, string providerKey)
        {
            var result = new GetPermissionListResultDto
            {
                Groups = new List<PermissionGroupDto>()
            };

            foreach (var group in _permissionDefinitionManager.GetGroups())
            {
                var groupDto = new PermissionGroupDto
                {
                    Name = group.Name,
                    DisplayName = group.Name, //TODO: DisplayName
                    Permissions = new List<PermissionGrantInfoDto>()
                };

                foreach (var permission in group.GetPermissionsWithChildren())
                {
                    var grantInfoDto = new PermissionGrantInfoDto
                    {
                        Name = permission.Name,
                        DisplayName = permission.Name, //TODO: Add DisplayName to permission definition
                        ParentName = permission.Parent?.Name,
                        Providers = new List<ProviderInfoDto>()
                    };

                    var grantInfo = await _permissionManager.GetAsync(permission.Name, providerName, providerKey);

                    grantInfoDto.IsGranted = grantInfo.IsGranted;

                    foreach (var provider in grantInfo.Providers)
                    {
                        grantInfoDto.Providers.Add(new ProviderInfoDto
                        {
                            ProviderName = provider.Name,
                            ProviderKey = provider.Key,
                        });
                    }

                    groupDto.Permissions.Add(grantInfoDto);
                }

                result.Groups.Add(groupDto);
            }

            return result;
        }

        [Authorize(PermissionPermissions.Permissions.Update)]
        public async Task UpdateAsync(string providerName, string providerKey, UpdatePermissionsDto input)
        {
            foreach (var permission in input.Permissions)
            {
                await _permissionManager.SetAsync(permission.Name, providerName, providerKey, permission.IsGranted);
            }
        }
    }
}
